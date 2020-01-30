//  Copyright (c) 2018 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.

using System.ComponentModel;

namespace Nethermind.Blockchain
{
    public static class Metrics
    {
        [Description("Total MGas processed")]
        public static decimal Mgas { get; set; }
        
        [Description("Total number of transactions processed")]
        public static long Transactions { get; set; }
        
        [Description("Total number of blocks processed")]
        public static long Blocks { get; set; }
        
        [Description("Total number of chain reorganizations")]
        public static long Reorganizations { get; set; }
        
        [Description("Number of blocks awaiting for recovery of public keys from signatures.")]
        public static long RecoveryQueueSize { get; set; }
        
        [Description("Number of blocks awaiting for processing.")]
        public static long ProcessingQueueSize { get; set; }
        
        [Description("Number of sync peers.")]
        public static long SyncPeers { get; set; }
        
        [Description("Number of pending transactions broadcasted to peers.")]
        public static long PendingTransactionsSent { get; set; }
        
        [Description("Number of pending transactions received from peers.")]
        public static long PendingTransactionsReceived { get; set; }
        
        [Description("Number of pending transactions received that were ignored.")]
        public static long PendingTransactionsDiscarded { get; set; }
        
        [Description("Number of known pending transactions.")]
        public static long PendingTransactionsKnown { get; set; }
    }
}