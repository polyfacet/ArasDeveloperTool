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

        protected int RunArasDevTool(string args) {
            args = Command + " " + args;
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo("ArasDevTool.exe", args)
            };
            p.Start();
            p.WaitForExit();
            return p.ExitCode;
        }

    }
}
