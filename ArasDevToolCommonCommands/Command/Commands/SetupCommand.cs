using System.Linq;
using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Configuration;
using System;
using System.Diagnostics;

namespace Hille.Aras.DevTool.Common.Commands.Command.Commands {
    public class SetupCommand : ICommand {

        private bool ExtendedSetup = false;

        public string Name => "Setup";

        public List<string> Help() {
            return new List<string>
            {
                "Configure Aras connection",
                "",
                "Options:",
                "-ext   Extended configuration for Database, Import/Export support"
            };
        }

        public void Run() {
            ArasXmlStoredConfig config = new ArasXmlStoredConfig();
            config.Setup(ExtendedSetup);
            // Test Connection
            TestConnection();
            // Test SqlCmd
            if (ExtendedSetup) {
                TestSqlCmd(config.SqlCmd);
            }
        }

        private void TestSqlCmd(string sqlCmd) {
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo(sqlCmd, "-?")
            };
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit();
            int exitCode = p.ExitCode;
            Console.WriteLine(exitCode);
        }

        private void TestConnection() {
            var testCommand = new ArasCommands.TestArasConnectionCommand
            {
                Logger = new Hille.Aras.DevTool.Interfaces.Logging.BasicLogger()
            };
            var inputArgs = new List<string>
            {
                testCommand.Name
            };
            if (testCommand.ValidateInput(inputArgs))
                testCommand.Run();
        }

        public bool ValidateInput(List<string> inputArgs) {
            if (CommandUtils.HasOption(inputArgs,"-ext")) {
                ExtendedSetup = true;
            }
            return true;
        }
           
    }
}
