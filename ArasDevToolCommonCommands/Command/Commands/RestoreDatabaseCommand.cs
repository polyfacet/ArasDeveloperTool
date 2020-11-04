using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hille.Aras.DevTool.Common.Commands.Command.Commands {
    class RestoreDatabaseCommand : ICommand {
        private ArasXmlStoredConfig _config;

        public string Name => "RestoreDB";

        public List<string> Help() {
            var msgs = new List<string>
            {
                "  Restores a database backup from configured directory"
            };
            return msgs;
        }

        public void Run() {
            
            _config = new ArasXmlStoredConfig("dev");
            string dir = _config.BackupDir;
            

        }

        public bool ValidateInput(List<string> inputArgs) {
            return true;
        }
        
    }
}
