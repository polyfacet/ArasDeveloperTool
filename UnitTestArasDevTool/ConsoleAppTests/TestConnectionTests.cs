using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ArasDevTool;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestArasDevTool.ConsoleAppTests {

    [TestClass]
    [TestCategory("ConsoleTests")]
    public class TestConnectionTests : ConsoleAppTestBase {

        public override string Command => "TestConnection";

        [TestMethod]
        public void TestConnectionStored() {
            AssertRun(String.Empty, (int)Program.Result.OK);
        }

        [TestMethod]
        public void TestConnectionConnectionString() {
            AssertRun(CS_CONNENCTION_STRING, (int)Program.Result.OK);
        }

        [TestMethod]
        public void TestConnectionHelp() {
            AssertRun(HELP_FLAG, (int)Program.Result.HELP);
        }

        [TestMethod]
        public void TestConnectionWithZeroTimeOut() {
            AssertRun($"{CS_CONNENCTION_STRING} {CONNENCTION_TIMEOUT_0}", (int)Program.Result.OK);
        }
    }
}
