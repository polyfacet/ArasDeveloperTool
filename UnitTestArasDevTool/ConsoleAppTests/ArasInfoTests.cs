using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestArasDevTool.ConsoleAppTests {

    [TestClass]
    public class ArasInfoTests : ConsoleAppTestBase {


        public override string Command => "ArasInfo";

        [TestMethod]
        public void ArasInfo() {
            string args;
            int expectedResult;
            
            args = HELP_FLAG;
            expectedResult = (int)ArasDevTool.Program.Result.HELP;
            Assert.IsTrue(RunArasDevTool(args) ==  expectedResult, GetFailMessage(expectedResult));

            args = CS_CONNENCTION_STRING;
            expectedResult = (int)ArasDevTool.Program.Result.OK;
            Assert.IsTrue(RunArasDevTool(args) == expectedResult, GetFailMessage(expectedResult));

            args = String.Empty;
            expectedResult = (int)ArasDevTool.Program.Result.OK;
            Assert.IsTrue(RunArasDevTool(args) == expectedResult, GetFailMessage(expectedResult));
            
        }

    }
}
