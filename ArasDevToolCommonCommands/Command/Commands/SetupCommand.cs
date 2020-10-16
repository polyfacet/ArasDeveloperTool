
using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Configuration;

namespace ArasDevTool.Command.Commands {
    public class SetupCommand : ICommand {
        public string Name => "Setup";

        public List<string> Help() {
            return new List<string>
            {
                "Configure Aras connection"
            };
        }

        public void Run() {
            ArasXmlStoredConfig config = new ArasXmlStoredConfig();
            config.Setup();
        }

        public bool ValidateInput(List<string> inputArgs) {
            return true;
        }
           
    }
}
