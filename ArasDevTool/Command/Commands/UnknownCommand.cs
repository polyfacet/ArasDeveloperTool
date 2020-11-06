using System;
using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;

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
            var commandNames = new List<string>() ;
            foreach (KeyValuePair<string, ICommand> kvp in Factory.impl) {
                if (!kvp.Key.Equals(Name,StringComparison.OrdinalIgnoreCase)) { // Dont add self
                    commandNames.Add(kvp.Key);
                }
            }
            commandNames.Sort();
            foreach (string cmdName in commandNames) messages.Add(cmdName);
            messages.Add("");
            messages.Add("Options:");
            messages.Add("  --help    Displays help for a command");
            return messages;
        }

        public void Run() {}

        public bool ValidateInput(List<string> inputArgs) {
            if (inputArgs.Count > 0) _inputCommandName = inputArgs[0];
            return false;
        }
    }
}
