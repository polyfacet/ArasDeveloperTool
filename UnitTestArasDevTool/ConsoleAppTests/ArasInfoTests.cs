using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ArasDevTool;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestArasDevTool.ConsoleAppTests {

    [TestClass]
    public class ArasInfoTests : ConsoleAppTestBase {
        public override string Command => "ArasInfo";

        [TestMethod]
        public void ArasInfoHelp() {
            AssertRun(HELP_FLAG, (int) Program.Result.HELP);
        }

        [TestMethod]
        public void ArasInfoConnectionString() {
            AssertRun(CS_CONNENCTION_STRING, (int)Program.Result.OK);
        }
        
        [TestMethod]
        public void ArasInfoStoredConnection() {
            AssertRun(string.Empty, (int)Program.Result.OK);
        }
    }
}
