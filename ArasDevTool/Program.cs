using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hille.Aras.DevTool.Interfaces.Logging;
using Hille.Aras.DevTool.Interfaces.Command;

namespace ArasDevTool;
public static class Program{

    public enum Result {
        OK,
        ERROR,
        GENERIC_HELP,
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
        try {
            InitLogger();
            var argList = args.ToList();
            if (argList.Contains("-v")) {
                Logger.Log("Version:");
                Logger.Log("  " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                return (int)Result.HELP;
            }
            string commandName = (argList.Count > 0) ? argList[0] : "";
            ICommand command = Factory.GetCommand(commandName);
            if (command is ILoggable) {
                ((ILoggable)command).Logger = _logger;
            }
            if (!command.ValidateInput(argList) || HelpInInput(argList)) {
                Logger.Log($"Help for: {command.Name}");
                foreach (string line in command.Help()) {
                    Logger.Log("  " + line);
                }
                if (command.Name == "ArasDevTool") {
                    return (int)Result.GENERIC_HELP;
                }
                return (int)Result.HELP;
            }
            else {
                command.Run();
            }
            return (int)Result.OK;
        }
        catch (Exception ex) {
            Logger.LogError(ex);
            return (int) Result.ERROR;
        }
    }

    private static void InitLogger() {
        Logger.Log("");
    }

    private static bool HelpInInput(List<string> inputArgs) {
        string help = inputArgs.SingleOrDefault(s => s.ToLower(System.Globalization.CultureInfo.CurrentCulture) == "--help" || s.ToLower(System.Globalization.CultureInfo.CurrentCulture) == "-h");
        if (String.IsNullOrEmpty(help)) {
            return false;
        }
        return true;
    }
    
}
