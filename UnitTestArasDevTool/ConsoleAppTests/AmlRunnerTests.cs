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
    public class AmlRunnerTests : ConsoleAppTestBase {


        public override string Command => "AmlRunner";

        [TestMethod]
        public void AmlRunnerStoredConnection() {
            AssertRun(@"-mf ""TestData\AMLRunner\AMLRunnerTest.mf""", (int)Program.Result.OK);
        }

        [TestMethod]
        public void AmlRunnerConnectionString() {
            AssertRun(CS_CONNENCTION_STRING + " -mf \"TestData\\AMLRunner\\AMLRunnerTest.mf\"", (int)Program.Result.OK);
        }

        [TestMethod]
        public void CheckLatestUpdatesHelp() {
            AssertRun(HELP_FLAG, (int)Program.Result.HELP);
        }

    }
}
