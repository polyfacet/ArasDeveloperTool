using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestArasDevTool {
    [TestClass]
    public class ConsoleAppTests {

        // TODO: Refactor
        
        private const string TEST_CONNECTION_COMMAND = "TestConnection";
        private const string CHECK_LATEST_COMMAND = "CheckLatestUpdates";
        private const string PACKAGE_CHECKER_COMMAND = "PackageChecker";
        private const string HELP_FLAG = "--help";
        private const string CONNENCTION_STRING = "http://localhost/HCAras12;HCAras12;admin;innovator";
        private const string CS_CONNENCTION_STRING = "-cs=http://localhost/HCAras12;HCAras12;admin;innovator";


        private int RunArasDevTool(string args) {
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo("ArasDevTool.exe", args)
            };
            p.Start();
            p.WaitForExit();
            return p.ExitCode;
        }

        [TestMethod]
        public void TestConnectionTest() {
            string args;
            args = TEST_CONNECTION_COMMAND;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = $"{TEST_CONNECTION_COMMAND} -cs={CONNENCTION_STRING}";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = $"{TEST_CONNECTION_COMMAND} {HELP_FLAG}";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);
        }

        [TestMethod]
        public void CheckLatestCommand() {
            string args;

            args = CHECK_LATEST_COMMAND;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = CHECK_LATEST_COMMAND + " -c 1";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = CHECK_LATEST_COMMAND + " " + CS_CONNENCTION_STRING + " -c 1";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = CHECK_LATEST_COMMAND + " " + CS_CONNENCTION_STRING;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = CHECK_LATEST_COMMAND + " " + HELP_FLAG;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);

        }

        [TestMethod]
        public void PackageCheckerCommand() {
            string args;

            args = PACKAGE_CHECKER_COMMAND;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);

            args = PACKAGE_CHECKER_COMMAND + " -prefix HC_ --Auto --DryRun";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = PACKAGE_CHECKER_COMMAND + " " + CS_CONNENCTION_STRING + " -prefix HC_ --Auto --DryRun";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = PACKAGE_CHECKER_COMMAND + " " + CS_CONNENCTION_STRING;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);

            args = PACKAGE_CHECKER_COMMAND + " " + HELP_FLAG;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);

        }


    }
}
