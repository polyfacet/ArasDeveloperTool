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
    public class DatabaseBackupTests : ConsoleAppTestBase {

        public override string Command => "BackupDB";

        [TestMethod]
        public void BackupTest() {
            AssertRun(String.Empty, (int)Program.Result.OK);
        }

        [TestMethod]
        public void BackupHelp() {
            AssertRun(HELP_FLAG, (int)Program.Result.HELP);
        }
    }
}
