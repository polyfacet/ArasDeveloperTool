using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestArasDevTool.ConsoleAppTests {

    [TestClass]
    public class CheckLastetTests : ConsoleAppTestBase {


        public override string Command => "CheckLatestUpdates";

        [TestMethod]
        public void CheckLatestUpdates() {
            string args = String.Empty;

            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = "-c 1";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = CS_CONNENCTION_STRING + " -c 1";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = CS_CONNENCTION_STRING;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = HELP_FLAG;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);
        }

    }
}
