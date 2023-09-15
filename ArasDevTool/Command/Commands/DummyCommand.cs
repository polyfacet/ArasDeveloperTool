using System;
using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;


namespace ArasDevTool.Command.Commands;
class DummyCommand : ICommand {
    public string Name => "Dummy";

    public List<string> Help() {
        return new List<string>
        {
            "Just a dummy command", "Just add any parameter"
        };
    }

    public void Run() {
        Console.WriteLine($"{Name} run");
    }

    public bool ValidateInput(List<string> inputArgs) {
        if (inputArgs.Count > 1) {
            return true;
        }
        return false;
    }
}
