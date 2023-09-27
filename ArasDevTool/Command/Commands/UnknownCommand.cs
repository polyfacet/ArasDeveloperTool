using System;
using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;
using System.Reflection;

namespace ArasDevTool.Command.Commands;
class UnknownCommand : ICommand {

    private string _inputCommandName;
    public string Name => "ArasDevTool";

    public List<string> Help() {
        List<string> messages = new List<string>();
        if (!String.IsNullOrEmpty(_inputCommandName)) {
            messages.Add($"Unknown or ambiguous command: {_inputCommandName}");
        }
        messages.Add("Available commands:");
        var commandNames = new List<string>() ;
        foreach (ICommand command in Factory.Implementations) {
            if (!command.Name.Equals(Name,StringComparison.OrdinalIgnoreCase)) { // Dont add self
                commandNames.Add(command.Name);
            }
        }
        commandNames.Sort();
        foreach (string cmdName in commandNames) messages.Add(cmdName);
        messages.Add("");
        messages.Add("Options:");
        messages.Add("  --help    Displays help for a command");
        messages.Add($"Version: {GetFileVersion()}");
        return messages;
    }

    public string GetFileVersion() {
        return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
    }

    public void Run() {}

    public bool ValidateInput(List<string> inputArgs) {
        if (inputArgs.Count > 0) _inputCommandName = inputArgs[0];
        return false;
    }
}
