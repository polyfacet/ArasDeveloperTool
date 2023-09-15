using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Configuration;
using Hille.Aras.DevTool.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace Hille.Aras.DevTool.Common.Commands.Command.Commands;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "generic/late bound/reflection")]
class RestoreDatabaseCommand : ICommand, ILoggableCommand {
    private IArasSetupConfig _config;
    private int DBDisplayCount = 5;
    private string Env = string.Empty;
    public string Name => "RestoreDB";

    private ILogger Log;
    public ILogger Logger { set => Log = value; }

    private bool IsTest;        

    public List<string> Help() {
        var msgs = new List<string>
        {
            "  Restores a database backup from configured directory",
            " -env deploy \t For specific environment. Default dev",
            "  -dc \t Number of backups to select from: -dc 10 \t (default 5)"
        };
        return msgs;
    }

    public void Run() {
        _config = new DefaultSetupHandler().GetConfig(Env); 
        string filePath = RestoreDatabase(_config);
        if (!String.IsNullOrEmpty(filePath)) {
            Log.LogSuccess($"Restored backup to {_config.DatabaseName} from file: {filePath}");
        }
    }

    public bool ValidateInput(List<string> inputArgs) {
        if (CommandUtils.OptionExistWithValue(inputArgs, "-env", out string env)) {
            Env = env;
        }
        if (CommandUtils.OptionExistWithValue(inputArgs, "-dc", out string dcValue)) {
            if (int.TryParse(dcValue, out int value)) {
                DBDisplayCount = value;
            }
        }
        if (CommandUtils.HasOption(inputArgs,"--test")) {
            IsTest = true;
        }
        return true;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    private string RestoreDatabase(IArasSetupConfig config) {
        //"Kill processes"
        KillConnections(config);

        // Get files by newest first
        var files = Directory.GetFiles(config.BackupDir)
            .Where(f => f.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(f => new FileInfo(f).CreationTime);
        ;

        int selectCount = 0;
        var filePathMap = new Dictionary<int, string>();
        foreach(string file in files) {
            selectCount++;
            if (selectCount <= DBDisplayCount) {
                filePathMap.Add(selectCount, file);
            }
            else {
                break;
            }
        }

        if (selectCount==0) Log.LogWarning("No backup files found");
        
        foreach(KeyValuePair<int,string> kvp in filePathMap) {
            Console.WriteLine($"({kvp.Key}) {Path.GetFileName(kvp.Value)}");
        }
        Console.WriteLine("Select database to restore");
        string input = Console.ReadLine();
        if (int.TryParse(input, out int selectedIndex)
            && filePathMap.ContainsKey(selectedIndex)) {
            string fileToRestore = filePathMap[selectedIndex];
            Log.Log($"Restoring: {fileToRestore}");
            if (IsTest) {Log.Log("Only test!"); return fileToRestore;}
            int result = RestoreDataBase(_config, fileToRestore);
            if (result == 0) {
                return fileToRestore;
            }
            else {
                Log.LogError("Failed to restore database: Return Code: " + result);
            }
        }
        else {
            Log.LogWarning("Not a valid selection, restore aborted");
        }
        return string.Empty;
    }

    private static void KillConnections(IArasSetupConfig config) {
        string query = $@"USE master;
            GO
            ALTER DATABASE {config.DatabaseName}
            SET SINGLE_USER
            WITH ROLLBACK IMMEDIATE;
            ALTER DATABASE {config.DatabaseName}
            SET MULTI_USER;
            GO";
        string cmd = $@"-S {config.SqlServer} -E -Q ""{query}"" ";

        int result = ExecCommand(config.SqlCmd, cmd);
        if (result != 0) {
            throw new ApplicationException("Kill db process failed with exit code: " + result);
        }
    }

    private static int RestoreDataBase(IArasSetupConfig config, string filePath) {
        string query = $@"RESTORE DATABASE {config.DatabaseName}
            FROM DISK='{filePath}' WITH REPLACE";
        string cmd = $@"-S {config.SqlServer} -E -Q ""{query}"" ";
        return ExecCommand(config.SqlCmd, cmd);
    }

    private static int ExecCommand(string exec, string parameters) {
        using (Process p = new Process() { StartInfo = new ProcessStartInfo(exec, parameters) }) {
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit();
            return p.ExitCode;
        }
    }

}
