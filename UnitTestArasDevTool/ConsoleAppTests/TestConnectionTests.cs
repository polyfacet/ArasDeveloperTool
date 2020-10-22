using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestArasDevTool.ConsoleAppTests {

    [TestClass]
    public class TestConnectionTests : ConsoleAppTestBase {


        public override string Command => "TestConnection";

        [TestMethod]
        public void TestConnection() {
            string args = String.Empty;
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = $"-cs={CONNENCTION_STRING}";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.OK);

            args = $"{HELP_FLAG}";
            Assert.IsTrue(RunArasDevTool(args) == (int)ArasDevTool.Program.Result.HELP);
        }

    }
}
