﻿// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Python.Analysis.Diagnostics;
using Microsoft.Python.Analysis.Tests.FluentAssertions;
using Microsoft.Python.Core;
using Microsoft.Python.Parsing.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;

namespace Microsoft.Python.Analysis.Tests {
    [TestClass]
    public class LintNoMethodArgumentTests : AnalysisTestBase {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
            => TestEnvironmentImpl.TestInitialize($"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}");

        [TestCleanup]
        public void Cleanup() => TestEnvironmentImpl.TestCleanup();

        [TestMethod, Priority(0)]
        public async Task MethodNoArgs() {
            const string code = @"
class Test:
    def test():
        pass
";
            var analysis = await GetAnalysisAsync(code);
            analysis.Diagnostics.Should().HaveCount(1);

            var diagnostic = analysis.Diagnostics.ElementAt(0);
            diagnostic.ErrorCode.Should().Be(ErrorCodes.NoMethodArgument);
            diagnostic.SourceSpan.Should().Be(3, 9, 3, 13);
            diagnostic.Message.Should().Be(Resources.NoMethodArgument.FormatInvariant("test"));
        }

        [TestMethod, Priority(0)]
        public async Task ClassMethodNoArgs() {
            const string code = @"
class Test:
    @classmethod
    def test():
        pass
";
            var analysis = await GetAnalysisAsync(code);
            analysis.Diagnostics.Should().HaveCount(1);

            var diagnostic = analysis.Diagnostics.ElementAt(0);
            diagnostic.ErrorCode.Should().Be(ErrorCodes.NoMethodArgument);
            diagnostic.SourceSpan.Should().Be(4, 9, 4, 13);
            diagnostic.Message.Should().Be(Resources.NoMethodArgument.FormatInvariant("test"));
        }

        [TestMethod, Priority(0)]
        public async Task DefaultMethodNoArgs() {
            const string code = @"
class Test:
    def __init__():
        pass
";
            var analysis = await GetAnalysisAsync(code);
            analysis.Diagnostics.Should().HaveCount(1);

            var diagnostic = analysis.Diagnostics.ElementAt(0);
            diagnostic.ErrorCode.Should().Be(ErrorCodes.NoMethodArgument);
            diagnostic.SourceSpan.Should().Be(3, 9, 3, 17);
            diagnostic.Message.Should().Be(Resources.NoMethodArgument.FormatInvariant("__init__"));
        }

        [TestMethod, Priority(0)]
        public async Task FirstArgumentSpace() {
            const string code = @"
class Test:
    def test( ):
        pass
";
            var analysis = await GetAnalysisAsync(code);
            analysis.Diagnostics.Should().HaveCount(1);

            var diagnostic = analysis.Diagnostics.ElementAt(0);
            diagnostic.ErrorCode.Should().Be(ErrorCodes.NoMethodArgument);
            diagnostic.SourceSpan.Should().Be(3, 9, 3, 13);
            diagnostic.Message.Should().Be(Resources.NoMethodArgument.FormatInvariant("test"));
        }

        [TestMethod, Priority(0)]
        public async Task NoDiagnosticOnStaticMethod() {
            const string code = @"
class Test:
    @staticmethod
    def test():
        pass
";
            var analysis = await GetAnalysisAsync(code);
            analysis.Diagnostics.Should().BeEmpty();
        }

        [TestMethod, Priority(0)]
        public async Task NoDiagnosticInMetaclass() {
            const string code = @"
class Test(type):
    def test():
        pass
";
            var analysis = await GetAnalysisAsync(code);
            analysis.Diagnostics.Should().BeEmpty();
        }
    }
}
