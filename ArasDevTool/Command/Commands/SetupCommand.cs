using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command.Commands {
    class SetupCommand : ICommand {
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
