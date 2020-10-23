using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestArasDevTool.ConsoleAppTests {

    public abstract class ConsoleAppTestBase {

        public const string CONSOLE_APP_NAME = "ArasDevTool.exe";
        protected const string HELP_FLAG = "--help";
        protected const string CONNENCTION_STRING = "http://localhost/HCAras12;HCAras12;admin;innovator";
        protected const string CS_CONNENCTION_STRING = "-cs=http://localhost/HCAras12;HCAras12;admin;innovator";
        abstract public string Command { get; }
        private string _inputArgs;
        private int _result;

        protected int RunArasDevTool(string args) {
            _inputArgs = args;
            args = Command + " " + args;
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo("ArasDevTool.exe", args)
            };
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit();
            _result = p.ExitCode;
            return _result;
        }

        protected string GetFailMessage() {
            if (String.IsNullOrEmpty(_inputArgs)) {
                return $"Running {Command} with no args failed";
            }
            else {
                return $"Running {Command} with args: {_inputArgs}";
            }
        }

        protected string GetFailMessage(int expectedResult) {
            string msg = GetFailMessage();
            msg += $" Expected/Actual : {expectedResult}/{_result}";
            return msg;
        }

        protected void AssertRun(string args, int expectedResult) {
            int result = RunArasDevTool(args);
            Assert.IsTrue(result == expectedResult, GetFailMessage(expectedResult));
        }
    }
}
