using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Configuration;
using Hille.Aras.DevTool.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hille.Aras.DevTool.Common.Commands.Command.Commands {
    class BackupDatabaseCommand : ICommand, ILoggableCommand {
        private ArasXmlStoredConfig _config;

        public string Name => "BackupDB";

        private ILogger Log;
        public ILogger Logger { set => Log = value; }

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
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.CurrentCulture);
            if (dir.EndsWith(@"\",StringComparison.Ordinal)) {
                filePath = $@"{dir}{_config.DatabaseName}";
            }
            else {
                 filePath = $@"{dir}\{_config.DatabaseName}";
            }
            filePath += $"_{timeStamp}.bak";
            BackupDatabase(_config.SqlCmd, _config.SqlServer, _config.DatabaseName, filePath);
            Log.LogSuccess("Backup created: " + filePath);
        }

        public bool ValidateInput(List<string> inputArgs) {
            return true;
        }

        private void BackupDatabase(string sqlCmdExe, string server, string dbName, string filePath) {
            string sqlCmd = $"BACKUP DATABASE {dbName} TO DISK='{filePath}' WITH FORMAT";
            string cmd = $@"-S {server} -E -Q ""{sqlCmd}"" ";

            Console.WriteLine(cmd);
            
            using (Process p = new Process()
                {
                    StartInfo = new ProcessStartInfo(sqlCmdExe, cmd)
                } ) {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
                p.WaitForExit();
                if (p.ExitCode != 0) {
                    throw new ApplicationException("Backup failed with exit code: " + p.ExitCode);
                }
            }
        }
    }
}
