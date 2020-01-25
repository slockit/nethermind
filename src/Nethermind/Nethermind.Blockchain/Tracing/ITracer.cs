﻿//  Copyright (c) 2018 Demerzel Solutions Limited
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

using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Evm.Tracing;
using Nethermind.Store;

namespace Nethermind.Blockchain.Tracing
{
    /// <summary>
    /// A simple and flexible bridge for any tracing operations on blocks and transactions.
    /// </summary>
    public interface ITracer
    {
        /// <summary>
        /// Allows to trace a block from the past.
        /// </summary>
        /// <param name="blockHash">The hash of the block to trace. It has to be a canonical hash.</param>
        /// <param name="tracer">The trace can collect any information from inside EVM or block processing contexts.</param>
        void Trace(Keccak blockHash, IBlockTracer tracer);
        
        /// <summary>
        /// Allows to trace an arbitrarily constructed block.
        /// </summary>
        /// <param name="block">Block to trace.</param>
        /// <param name="tracer">Trace to act on block processing events.</param>
        void Trace(Block block, IBlockTracer tracer);
        
        void Accept(ITreeVisitor visitor, Keccak stateRoot);
    }
}