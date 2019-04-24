/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Encoding;
using Nethermind.Core.Extensions;
using Nethermind.Core.Logging;
using Nethermind.Store;

namespace Nethermind.Blockchain.Synchronization
{
    public class NodeDataDownloader : INodeDataDownloader
    {
        private readonly IEthSyncPeerPool _syncPeerPool;
        private readonly INodeDataFeed _nodeDataFeed;
        private const int MaxRequestSize = 384;
        private int _pendingRequests;
        private int _consumedNodesCount;
        private ILogger _logger;

        public NodeDataDownloader(IEthSyncPeerPool syncPeerPool, INodeDataFeed nodeDataFeed, ILogManager logManager)
        {
            _syncPeerPool = syncPeerPool ?? throw new ArgumentNullException(nameof(syncPeerPool));
            _nodeDataFeed = nodeDataFeed ?? throw new ArgumentNullException(nameof(nodeDataFeed));
            _logger = logManager.GetClassLogger() ?? throw new ArgumentNullException(nameof(logManager));
        }

        private Semaphore _parallelNodeSyncs = new Semaphore(0, 50);

        private int _lastPeerCount;

        private async Task ExecuteRequest(CancellationToken token, StateSyncBatch batch)
        {
            SyncPeerAllocation nodeSyncAllocation = null;
            if (_parallelNodeSyncs.WaitOne(100))
            {
                nodeSyncAllocation = _syncPeerPool.Borrow(BorrowOptions.DoNotReplace, $"node sync");
//                _logger.Warn($"Sending node data request to {nodeSyncAllocation.Current}");
            }
            
            try
            {   
                ISyncPeer peer = nodeSyncAllocation?.Current?.SyncPeer;
                batch.AssignedPeer = nodeSyncAllocation;
                if (peer != null)
                {
                    var hashes = batch.RequestedNodes.Select(r => r.Hash).ToArray();
                    batch.Responses = await peer.GetNodeData(hashes, token); // handle timeout here
                }
                else
                {
                    await Task.Delay(50);
                }
                
                var handlerResult = _nodeDataFeed.HandleResponse(batch);
                Interlocked.Add(ref _consumedNodesCount, handlerResult.NodesConsumed);
                if (handlerResult.NodesConsumed == 0)
                {
                    _syncPeerPool.ReportNoSyncProgress(nodeSyncAllocation);
                }
            }
            finally
            {
                if (nodeSyncAllocation != null)
                {
//                    _logger.Warn($"Free {nodeSyncAllocation?.Current}");
                    _syncPeerPool.Free(nodeSyncAllocation);
                    _parallelNodeSyncs.Release(1);
                }

                int afterDecrement = Interlocked.Decrement(ref _pendingRequests);
                if (_logger.IsTrace) _logger.Trace($"Decrementing pending requests - now at {afterDecrement}");
            }
        }

        private void UpdateParallelism()
        {   
            int newPeerCount = _syncPeerPool.UsefulPeerCount;
            int difference = newPeerCount - _lastPeerCount;

            if (difference == 0)
            {
                return;
            }
            
            if(_logger.IsInfo) _logger.Info($"Node sync parallelism: {_syncPeerPool.UsefulPeerCount} useful peers out of {_syncPeerPool.PeerCount} in total.");
            
            if (difference > 0)
            {
                _parallelNodeSyncs.Release(difference);
            }
            else
            {
                for (int i = 0; i < -difference; i++)
                {
                    _parallelNodeSyncs.WaitOne(10000); // when failing?    
                }
            }

            _lastPeerCount = newPeerCount;
        }

        private async Task KeepSyncing(CancellationToken token)
        {
            HashSet<Task> tasks = new HashSet<Task>();
            StateSyncBatch[] dataBatches;
            do
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                UpdateParallelism();
                dataBatches = PrepareRequests();
                for (int i = 0; i < dataBatches.Length; i++)
                {
                    StateSyncBatch currentBatch = dataBatches[i];
                    if (_logger.IsTrace) _logger.Trace($"Creating new task with - {dataBatches[i].RequestedNodes.Length}");
                    Task task = ExecuteRequest(token, currentBatch);
                    tasks.Add(task);
                }

                if (tasks.Count != 0)
                {
                    Task firstComplete = await Task.WhenAny(tasks);
                    if (_logger.IsTrace) _logger.Trace($"Removing task from the list of {tasks.Count} node sync tasks");
                    if (!tasks.Remove(firstComplete))
                    {
                        if (_logger.IsError) _logger.Error($"Could not remove node sync task - task count {tasks.Count}");
                    }

                    if (firstComplete.IsFaulted)
                    {
                        // all the missing ones
//                        int consumed = _nodeDataFeed.HandleResponse(t.Result);
//                        Interlocked.Add(ref _consumedNodesCount, consumed);

// disconnect the guy

//                        if (_logger.IsDebug) _logger.Debug($"Node sync task throwing {firstComplete.Exception?.Message}");
//                        throw ((AggregateException) firstComplete.Exception).InnerExceptions[0];
                    }
                }
            } while (dataBatches.Length + _pendingRequests + tasks.Count != 0 || _nodeDataFeed.TotalNodesPending != 0);

            _logger.Debug($"Finished with {dataBatches.Length} {_pendingRequests} {tasks.Count}");
        }

        private StateSyncBatch[] PrepareRequests()
        {
            List<StateSyncBatch> requests = new List<StateSyncBatch>();
            do
            {
                StateSyncBatch currentBatch = _nodeDataFeed.PrepareRequest(MaxRequestSize);
                if (currentBatch.RequestedNodes.Length == 0)
                {
                    break;
                }

                requests.Add(currentBatch);
            } while (_pendingRequests + requests.Count < _lastPeerCount);

            var requestsArray = requests.ToArray();
            Interlocked.Add(ref _pendingRequests, requestsArray.Length);
            if (_logger.IsTrace) _logger.Trace($"Pending requests {_pendingRequests}");
            return requestsArray;
        }

        public async Task<long> SyncNodeData(CancellationToken token, Keccak rootNode)
        {
            _consumedNodesCount = 0;
            _nodeDataFeed.SetNewStateRoot(rootNode);
            await KeepSyncing(token);
            return _consumedNodesCount;
        }

        public bool IsFullySynced(Keccak stateRoot)
        {
            return _nodeDataFeed.IsFullySynced(stateRoot);
        }
    }
}