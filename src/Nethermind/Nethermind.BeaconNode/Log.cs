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

using System;
using Microsoft.Extensions.Logging;
using Nethermind.BeaconNode.Containers;
using Nethermind.Core2.Crypto;
using Nethermind.Core2.Types;
using Attestation = Nethermind.BeaconNode.Containers.Attestation;
using BeaconBlock = Nethermind.BeaconNode.Containers.BeaconBlock;
using BeaconState = Nethermind.BeaconNode.Containers.BeaconState;

namespace Nethermind.BeaconNode
{
    internal static class Log
    {
        // 1bxx preliminary

        public static readonly Action<ILogger, string, string, string, int, Exception?> WorkerStarted =
            LoggerMessage.Define<string, string, string, int>(LogLevel.Information,
                new EventId(1000, nameof(WorkerStarted)),
                "{ProductTokenVersion} started; {Environment} environment (config '{Config}') [{ThreadId}]");
        
        public static readonly Action<ILogger, Hash32, ulong, int, Exception?> InitializeBeaconState =
            LoggerMessage.Define<Hash32, ulong, int>(LogLevel.Information,
                new EventId(1101, nameof(InitializeBeaconState)),
                "Initialise beacon state from ETH1 block {Eth1BlockHash}, time {Eth1Timestamp}, with {DepositCount} deposits.");

        // 2bxx completion

        public static readonly Action<ILogger, Hash32, BeaconState, Hash32, BeaconBlock, Exception?> ValidatedStateTransition =
            LoggerMessage.Define<Hash32, BeaconState, Hash32, BeaconBlock>(LogLevel.Information,
                new EventId(2000, nameof(ValidatedStateTransition)),
                "Validated state transition to new state root {StateRoot} ({BeaconState}) by block {BlockSigningRoot} ({BeaconBlock})");
        
        public static readonly Action<ILogger, BeaconBlock, BeaconState, Checkpoint, Hash32, Exception?> CreateGenesisStore =
            LoggerMessage.Define<BeaconBlock, BeaconState, Checkpoint, Hash32>(LogLevel.Information,
                new EventId(2100, nameof(CreateGenesisStore)),
                "Creating genesis store with block {BeaconBlock} for state {BeaconState}, with checkpoint {JustifiedCheckpoint}, with signing root {SigningRoot}");
        
        public static readonly Action<ILogger, ulong, int, Exception?> WorkerStoreAvailableTickStarted =
            LoggerMessage.Define<ulong, int>(LogLevel.Information,
                new EventId(2101, nameof(WorkerStoreAvailableTickStarted)),
                "Store available with genesis time {GenesisTime}, starting clock tick [{ThreadId}]");

        public static readonly Action<ILogger, Attestation, Exception?> OnAttestation =
            LoggerMessage.Define<Attestation>(LogLevel.Information,
                new EventId(2300, nameof(OnAttestation)),
                "Fork choice received attestation {Attestation}");

        public static readonly Action<ILogger, Hash32, BeaconBlock, Exception?> OnBlock =
            LoggerMessage.Define<Hash32, BeaconBlock>(LogLevel.Information,
                new EventId(2301, nameof(OnBlock)),
                "Fork choice received block {BlockSigningRoot} ({BeaconBlock})");

        public static readonly Action<ILogger, Epoch, Slot, ulong, Exception?> OnTickNewEpoch =
            LoggerMessage.Define<Epoch, Slot, ulong>(LogLevel.Information,
                new EventId(2301, nameof(OnTickNewEpoch)),
                "Fork choice new epoch {Epoch} at slot {Slot} time {Time:n0}");

        // 4bxx warning

        public static readonly Action<ILogger, CommitteeIndex, Slot, int, Exception?> InvalidIndexedAttestationBit1 =
            LoggerMessage.Define<CommitteeIndex, Slot, int>(LogLevel.Warning,
                new EventId(4100, nameof(InvalidIndexedAttestationBit1)),
                "Invalid indexed attestation from committee {CommitteeIndex} for slot {Slot}, because it has {BitIndicesCount} bit 1 indices.");
        public static readonly Action<ILogger, CommitteeIndex, Slot, int, ulong, Exception?> InvalidIndexedAttestationTooMany =
            LoggerMessage.Define<CommitteeIndex, Slot, int, ulong>(LogLevel.Warning,
                new EventId(4101, nameof(InvalidIndexedAttestationTooMany)),
                "Invalid indexed attestation from committee {CommitteeIndex} for slot {Slot}, because it has total indices {TotalIndices}, more than the maximum validators per committee {MaximumValidatorsPerCommittee}.");
        public static readonly Action<ILogger, CommitteeIndex, Slot, int, Exception?> InvalidIndexedAttestationIntersection =
            LoggerMessage.Define<CommitteeIndex, Slot, int>(LogLevel.Warning,
                new EventId(4102, nameof(InvalidIndexedAttestationIntersection)),
                "Invalid indexed attestation from committee {CommitteeIndex} for slot {Slot}, because it has {IntersectingValidatorCount} validator indexes in common between custody bit 0 and custody bit 1.");
        public static readonly Action<ILogger, CommitteeIndex, Slot, int, int, Exception?> InvalidIndexedAttestationNotSorted =
            LoggerMessage.Define<CommitteeIndex, Slot, int, int>(LogLevel.Warning,
                new EventId(4103, nameof(InvalidIndexedAttestationNotSorted)),
                "Invalid indexed attestation from committee {CommitteeIndex} for slot {Slot}, because custody bit {CustodyBit} index {IndexNumber} is not sorted.");
        public static readonly Action<ILogger, CommitteeIndex, Slot, Exception?> InvalidIndexedAttestationSignature =
                LoggerMessage.Define<CommitteeIndex, Slot>(LogLevel.Warning,
                    new EventId(4104, nameof(InvalidIndexedAttestationSignature)),
                "Invalid indexed attestation from committee {CommitteeIndex} for slot {Slot}, because the aggregate signature does not match.");

        // 5bxx error

        // 8bxx finalization

        // 9bxx critical



    }
}