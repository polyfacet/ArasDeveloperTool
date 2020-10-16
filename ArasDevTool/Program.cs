using ArasDevTool.Command;
using ArasDevTool.Loggers;
using ArasDevTool.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool
{
    public class Program{

        public enum Result {
            OK,
            ERROR,
            HELP
        }


        private static ILogger _logger;

        private static ILogger Logger { 
            get {
                if (_logger == null) _logger = Factory.GetLogger();
                return _logger; 
            } 
            set {
                _logger = value; 
            } 
        }

        public static int Main(string[] args) {
            var argList = args.ToList();
            string commandName = (argList.Count > 0) ? argList[0] : "";
            Logger.Log($"Starting {commandName}");
            ICommand command = Factory.GetCommand(commandName);
            if (command is ILoggable) {
                ((ILoggable)command).Logger = _logger;
            }
            if (!command.ValidateInput(argList) || HelpInInput(argList)) {
                Logger.Log($"Help for: {command.Name}");
                foreach (string line in command.Help()) {
                    Logger.Log("  " + line);
                }
                return (int) Result.HELP;
            }
            else {
                try {
                    command.Run();
                }
                catch (Exception ex) {
                    Logger.LogError(ex);
                    return (int) Result.ERROR;
                }
            }
            return (int) Result.OK;
        }

        private static bool HelpInInput(List<string> inputArgs) {
            string help = inputArgs.SingleOrDefault(s => s.ToLower() == "--help" || s.ToLower() == "-h");
            if (String.IsNullOrEmpty(help)) {
                return false;
            }
            return true;
        }
       
    }
}
