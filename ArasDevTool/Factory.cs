using ArasDevTool.Command.Commands;
using Hille.Aras.DevTool.Interfaces.Logging;
using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Common.Commands.Command.ArasCommands;

namespace ArasDevTool {
    class Factory {
        // TODO: Make a better factory, without implmenentation dependencies
        public static Dictionary<string, ICommand> impl = new Dictionary<string, ICommand>() {
            {"Dummy".ToLower(),new DummyCommand()},
            {"Setup".ToLower(),new SetupCommand()},
            {"TestConnection".ToLower(),new TestArasConnectionCommand()},
            {"CheckLatestUpdates".ToLower(),new CheckLatestUpdatesCommand()},
            {"PackageChecker".ToLower(), new PackageChecker() }
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

        public static ILogger GetLogger() {
            return new BasicLogger();
        }
    }
}
