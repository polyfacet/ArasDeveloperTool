
using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Configuration;

namespace Hille.Aras.DevTool.Common.Commands.Command.Commands {
    public class SetupCommand : ICommand {
        public string Name => "Setup";

        public List<string> Help() {
            return new List<string>
            {
                "Configure Aras connection",
                "",
                "Options:",
                "-e   Extended configuration for Database, Import/Export support"
            };
        }

        public void Run() {
            ArasXmlStoredConfig config = new ArasXmlStoredConfig();
            config.Setup();
            // Test Connection
            TestConnection();
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
            return true;
        }
           
    }
}
