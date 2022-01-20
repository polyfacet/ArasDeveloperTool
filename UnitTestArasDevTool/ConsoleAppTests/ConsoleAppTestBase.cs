using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArasDevTool;

namespace UnitTestArasDevTool.ConsoleAppTests {

    public abstract class ConsoleAppTestBase {

        public const string CONSOLE_APP_NAME = "ArasDevTool.exe";
        protected const string HELP_FLAG = "--help";
        protected static string CONNECTION_STRING = $"{TestData.URL};{TestData.DB};{TestData.USER};{TestData.PASSWORD}";
        protected static string CS_CONNENCTION_STRING = $"-cs={CONNECTION_STRING}";
        protected static string CONNENCTION_TIMEOUT_0 = "-connectionTimeout 0";
        abstract public string Command { get; }
        private string _inputArgs;
        private int _result;
        private string _resultString;

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
            _resultString = ConvertToProgramResult(_result);
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
            msg += $" Expected/Actual : {ConvertToProgramResult(expectedResult)}/{_resultString}";
            return msg;
        }

        protected void AssertRun(string args, int expectedResult) {
            int result = RunArasDevTool(args);
            Assert.IsTrue(result == expectedResult, GetFailMessage(expectedResult));
        }

        protected string ConvertToProgramResult(int value) {
            if (Enum.IsDefined(typeof(ArasDevTool.Program.Result),value)) {
                Program.Result result = (Program.Result) value;
                return result.ToString();
            }
            else {
                return value.ToString();
            }
        }
    }
}
