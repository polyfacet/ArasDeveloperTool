using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command {
    interface ICommand {
        string Name { get; }
        List<string> Help();
        bool ValidateInput(List<string> inputArgs);
        void Run();
    }
}
