using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hille.Aras.DevTool.Common.Commands.Command.Commands {
    class BackupDatabaseCommand : ICommand {
        private ArasXmlStoredConfig _config;

        public string Name => "BackupDB";

        public List<string> Help() {
            var msgs = new List<string>
            {
                "  Creates database backup"
            };
            return msgs;
        }

        public void Run() {
            
            _config = new ArasXmlStoredConfig("dev");
            string dir = _config.BackupDir;
            string filePath;
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            if (dir.EndsWith(@"\")) {
                filePath = $@"{dir}{_config.DatabaseName}";
            }
            else {
                 filePath = $@"{dir}\{_config.DatabaseName}";
            }
            filePath += $"_{timeStamp}.bak";
            BackupDatabase(_config.SqlCmd, _config.SqlServer, _config.DatabaseName, filePath);            

        }

        public bool ValidateInput(List<string> inputArgs) {
            return true;
        }

        private void BackupDatabase(string sqlCmdExe, string server, string dbName, string filePath) {
            string sqlCmd = $"BACKUP DATABASE {dbName} TO DISK='{filePath}' WITH FORMAT";
            string cmd = $@"-S {server} -E -Q ""{sqlCmd}"" ";

            Console.WriteLine(cmd);
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo(sqlCmdExe, cmd)
            };
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit();
            Console.WriteLine(p.ExitCode);
            if (p.ExitCode != 0) {
                throw new ApplicationException("Backup failed with exit code: " + p.ExitCode);
            }
        }
    }
}
