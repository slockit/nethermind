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

using System;
using System.IO;
using Nethermind.DataMarketplace.Core.Domain;
using Nethermind.Serialization.Rlp;

namespace Nethermind.DataMarketplace.Infrastructure.Rlp
{
    public class EthRequestDecoder : IRlpDecoder<EthRequest>
    {
        public static void Init()
        {
            // here to register with RLP in static constructor
        }

        public EthRequestDecoder()
        {
        }

        static EthRequestDecoder()
        {
            Serialization.Rlp.Rlp.Decoders[typeof(EthRequest)] = new EthRequestDecoder();
        }
        
        public EthRequest Decode(RlpStream rlpStream, RlpBehaviors rlpBehaviors = RlpBehaviors.None)
        {
            var sequenceLength = rlpStream.ReadSequenceLength();
            if (sequenceLength == 0)
            {
                return null;
            }

            var id = rlpStream.DecodeKeccak();
            var host = rlpStream.DecodeString();
            var address = rlpStream.DecodeAddress();
            var value = rlpStream.DecodeUInt256();
            var requestedAt = DateTimeOffset.FromUnixTimeSeconds(rlpStream.DecodeLong()).UtcDateTime;
            var transactionHash = rlpStream.DecodeKeccak();

            return new EthRequest(id, host, address, value, requestedAt, transactionHash);
        }

        public Serialization.Rlp.Rlp Encode(EthRequest item, RlpBehaviors rlpBehaviors = RlpBehaviors.None)
        {
            if (item == null)
            {
                return Serialization.Rlp.Rlp.OfEmptySequence;
            }

            return Serialization.Rlp.Rlp.Encode(
                Serialization.Rlp.Rlp.Encode(item.Id),
                Serialization.Rlp.Rlp.Encode(item.Host),
                Serialization.Rlp.Rlp.Encode(item.Address),
                Serialization.Rlp.Rlp.Encode(item.Value),
                Serialization.Rlp.Rlp.Encode(new DateTimeOffset(item.RequestedAt).ToUnixTimeSeconds()),
                Serialization.Rlp.Rlp.Encode(item.TransactionHash));
        }

        public void Encode(MemoryStream stream, EthRequest item, RlpBehaviors rlpBehaviors = RlpBehaviors.None)
        {
            throw new System.NotImplementedException();
        }

        public int GetLength(EthRequest item, RlpBehaviors rlpBehaviors)
        {
            throw new System.NotImplementedException();
        }
    }
}