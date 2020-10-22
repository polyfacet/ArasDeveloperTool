using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestArasDevTool.ConsoleAppTests {

    [TestClass]
    public class PackageCheckerTests : ConsoleAppTestBase {

        
        public override string Command => "PackageChecker";

        [TestMethod]
        public void PackageChecker() {
            string args = String.Empty;

            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);

            args = "-prefix HC_ --Auto --DryRun";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = CS_CONNENCTION_STRING + " -prefix HC_ --Auto --DryRun";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = CS_CONNENCTION_STRING;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);

            args = HELP_FLAG;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);
        }

    }
}
