using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ArasDevTool;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestArasDevTool.ConsoleAppTests {

    [TestClass]
    public class PackageCheckerTests : ConsoleAppTestBase {
        
        public override string Command => "PackageChecker";

        [TestMethod]
        public void PackageCheckerHelp() {
            AssertRun(HELP_FLAG, (int)Program.Result.HELP);
        }

        [TestMethod]
        public void PackageCheckerHelp2() {
            AssertRun(String.Empty, (int)Program.Result.HELP);
        }

        [TestMethod]
        public void PackageCheckerHelp3() {
            AssertRun(CS_CONNENCTION_STRING, (int)Program.Result.HELP);
        }

        [TestMethod]
        public void PackageCheckerPrefixStored() {
            AssertRun("-prefix HC_ --Auto --DryRun", (int)Program.Result.OK);
        }

        [TestMethod]
        public void PackageCheckerPrefixConnectionString() {
            string args = CS_CONNENCTION_STRING + " -prefix HC_ --Auto --DryRun";
            AssertRun(args, (int)Program.Result.OK);
        }

    }
}
