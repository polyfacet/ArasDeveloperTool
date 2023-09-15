using System.Collections.Generic;

namespace Hille.Aras.DevTool.Interfaces.Command;
public interface ICommand {
    string Name { get; }
    List<string> Help();
    bool ValidateInput(List<string> inputArgs);
    void Run();
}
