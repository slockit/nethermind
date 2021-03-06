﻿/*
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Ethereum.Test.Base;
using Nethermind.Core;
using Nethermind.Core.Attributes;
using NUnit.Framework;

namespace Ethereum.Blockchain.Block.Test
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class StateTests : LegacyBlockchainTestBase
    {
        [Todo(Improve.TestCoverage, "SuicideStorage tests")]
        [TestCaseSource(nameof(LoadTests))]
        public async Task Test(LegacyBlockchainTest test)
        {
            if (test.Name.Contains("randomStatetest94"))
            {
                // test has unreasonable amount of gas assigned to the block
                // it passes but causes the builds to take half an hour
                return;
            }
            
            if (test.Name.Contains("suicideStorage"))
            {
                return;
            }
            
            await RunTest(test);
        }
        
        public static IEnumerable<LegacyBlockchainTest> LoadTests() { return new DirectoryTestsSource("bcStateTests").LoadLegacyTests(); }
    }
}