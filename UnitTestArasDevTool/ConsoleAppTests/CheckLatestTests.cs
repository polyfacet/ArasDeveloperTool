using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ArasDevTool;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestArasDevTool.ConsoleAppTests {

    [TestClass]
    public class CheckLatestTests : ConsoleAppTestBase {


        public override string Command => "CheckLatestUpdates";

        [TestMethod]
        public void CheckLatestUpdatesStored() {
            AssertRun(string.Empty, (int)Program.Result.OK);
        }

        [TestMethod]
        public void CheckLatestUpdatesStoredLimit() {
            AssertRun("-c 1", (int)Program.Result.OK);
        }

        [TestMethod]
        public void CheckLatestUpdatesConnectionString() {
            AssertRun(CS_CONNENCTION_STRING, (int)Program.Result.OK);
        }

        [TestMethod]
        public void CheckLatestUpdatesConnectionStringLimit() {
            AssertRun(CS_CONNENCTION_STRING + " -c 1", (int)Program.Result.OK);
        }

        [TestMethod]
        public void CheckLatestUpdatesHelp() {
            AssertRun(HELP_FLAG, (int)Program.Result.HELP);
        }

    }
}
