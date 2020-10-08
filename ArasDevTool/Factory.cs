using ArasDevTool.Command;
using ArasDevTool.Command.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool {
    class Factory {

        public static Dictionary<string, ICommand> impl = new Dictionary<string, ICommand>() {
            {"Dummy".ToLower(),new DummyCommand()},
            {"Setup".ToLower(),new SetupCommand()},
        };

        public static ICommand GetCommand(string commandName) {
            commandName = commandName.ToLower();
            if (impl.ContainsKey(commandName)) {
                return impl[commandName];
            }
            else {
                return new UnknownCommand();
            }
        }
    }
}
