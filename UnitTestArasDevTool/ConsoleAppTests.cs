using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestArasDevTool {
    [TestClass]
    public class ConsoleAppTests {

        // TODO: Refactor
        // TODO: Bryt ut testerna... pga att Main är static

        private const string TEST_CONNECTION_COMMAND = "TestConnection";
        private const string CHECK_LATEST_COMMAND = "CheckLatestUpdates";
        private const string PACKAGE_CHECKER_COMMAND = "PackageChecker";
        private const string HELP_FLAG = "--help";
        private const string CONNENCTION_STRING = "http://localhost/HCAras12;HCAras12;admin;innovator";
        private const string CS_CONNENCTION_STRING = "-cs=http://localhost/HCAras12;HCAras12;admin;innovator";

        [TestMethod]
        public void TestConnectionTest() {

            string[] args;

            args = new[] {
                TEST_CONNECTION_COMMAND
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.OK);

            args = new [] {
                TEST_CONNECTION_COMMAND,
                $"-cs={CONNENCTION_STRING}"
            } ;
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.OK);
           
            args = new string []{
                TEST_CONNECTION_COMMAND,
                HELP_FLAG
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.HELP);
        }

        [TestMethod]
        public void CheckLatestCommand() {
            string[] args;

            args = new[] {
                CHECK_LATEST_COMMAND
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.OK);

            args = new[] {
                CHECK_LATEST_COMMAND,
                "-c", "1"

            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.OK);

            args = new [] {
                CHECK_LATEST_COMMAND,
                CS_CONNENCTION_STRING,
                "-c","1"
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.OK);

            args = new[] {
                CHECK_LATEST_COMMAND,
                CS_CONNENCTION_STRING
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.OK);

            args = new[] {
                CHECK_LATEST_COMMAND,
                HELP_FLAG
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int) ArasDevTool.Program.Result.HELP);

        }

        [TestMethod]
        public void PackageCheckerCommand() {
            string[] args;

            args = new[] {
                PACKAGE_CHECKER_COMMAND
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.HELP);

            args = new[] {
                PACKAGE_CHECKER_COMMAND,
                "-prefix", "HC_",
                "--Auto",
                "--DryRun"

            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.OK);

            args = new[] {
                PACKAGE_CHECKER_COMMAND,
                CS_CONNENCTION_STRING,
                "-prefix","HC_", 
                "--Auto",
                "--DryRun"
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.OK);

            args = new[] {
                PACKAGE_CHECKER_COMMAND,
                CS_CONNENCTION_STRING
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.HELP);

            args = new[] {
                PACKAGE_CHECKER_COMMAND,
                HELP_FLAG
            };
            Assert.IsTrue(ArasDevTool.Program.Main(args) == (int)ArasDevTool.Program.Result.HELP);

        }


    }
}
