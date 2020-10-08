using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command.Commands {
    class DummyCommand : Command.ICommand {
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
}
