using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command.Commands {
    class UnknownCommand : ICommand {

        private string _inputCommandName;
        public string Name => "ArasDevTool";

        public List<string> Help() {
            List<string> messages = new List<string>();
            if (!String.IsNullOrEmpty(_inputCommandName)) {
                messages.Add($"Unknown command: {_inputCommandName}");
            }
            messages.Add("Availible commands:");
            foreach (KeyValuePair<string, ICommand> kvp in Factory.impl) {
                messages.Add(kvp.Key);
            }
            return messages;
        }

        public void Run() {}

        public bool ValidateInput(List<string> inputArgs) {
            if (inputArgs.Count > 0) _inputCommandName = inputArgs[0];
            return false;
        }
    }
}
