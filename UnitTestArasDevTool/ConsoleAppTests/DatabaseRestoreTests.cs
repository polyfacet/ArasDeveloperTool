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
    public class DatabaseRestoreTests : ConsoleAppTestBase {

        public override string Command => "RestoreDB";

        [TestMethod]
        public void RestoreDBTest() {
            return; // Disabled test, only used in the dev
            AssertRun(String.Empty, (int)Program.Result.OK);
        }

        [TestMethod]
        public void RestoreDBHelp() {
            AssertRun(HELP_FLAG, (int)Program.Result.HELP);
        }
    }
}
