﻿using System;

namespace Cortex.Containers
{
    public class Eth1Data
    {
        public Eth1Data(ulong depositCount, Hash32 eth1BlockHash)
            : this(Hash32.Zero, depositCount, eth1BlockHash)
        {
        }

        public Eth1Data(Hash32 depositRoot, ulong depositCount, Hash32 blockHash)
        {
            DepositRoot = depositRoot;
            DepositCount = depositCount;
            BlockHash = blockHash;
        }

        public Hash32 BlockHash { get; }
        public ulong DepositCount { get; }
        public Hash32 DepositRoot { get; private set; }

        public static Eth1Data Clone(Eth1Data other)
        {
            var clone = new Eth1Data(
                Hash32.Clone(other.DepositRoot),
                other.DepositCount,
                Hash32.Clone(other.BlockHash));
            return clone;
        }

        public void SetDepositRoot(Hash32 depositRoot)
        {
            if (depositRoot == null)
            {
                throw new ArgumentNullException(nameof(depositRoot));
            }
            DepositRoot = depositRoot;
        }
    }
}
