﻿using Hille.Aras.DevTool.Interfaces.Configuration;
using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Hille.Aras.DevTool.Common.Commands.Command.Commands;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "generic/late bound/reflection")]
class BackupDatabaseCommand : ICommand, ILoggableCommand {
    private IArasSetupConfig  _config;

    private string Env = string.Empty;
    private string Brand = string.Empty;

    public string Name => "BackupDB";

    private ILogger Log;
    public ILogger Logger { set => Log = value; }

    public List<string> Help() {
        var msgs = new List<string>
        {
            "  Creates database backup",
            " -env deploy \t Backup for specific environment. Default dev",
            " -b  v_1_1_0 \t Brand the backup file instead of timestamp"
        };
        return msgs;
    }

    public void Run() {
        _config = new DefaultSetupHandler().GetConfig(Env); 
        string dir = _config.BackupDir;
        string filePath;
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.CurrentCulture);
        if (dir.EndsWith(@"\",StringComparison.Ordinal)) {
            filePath = $@"{dir}{_config.DatabaseName}";
        }
        else {
                filePath = $@"{dir}\{_config.DatabaseName}";
        }
        if (!String.IsNullOrEmpty(Brand)) {
            filePath += $"_{Brand}.bak";
        }
        else {
            filePath += $"_{timeStamp}.bak";
        }
        BackupDatabase(_config.SqlCmd, _config.SqlServer, _config.DatabaseName, filePath);
        if (!System.IO.File.Exists(filePath)) {
            Log.LogError("Backup was not created: " + filePath);
            return;
        }
        Log.LogSuccess("Backup created: " + filePath);


    }

    public bool ValidateInput(List<string> inputArgs) {
        if (CommandUtils.OptionExistWithValue(inputArgs,"-env", out string env)) {
            Env = env;
        }
        if (CommandUtils.OptionExistWithValue(inputArgs,"-b",out string brand)) {
            Brand = brand;
        }
        return true;
    }

    private void BackupDatabase(string sqlCmdExe, string server, string dbName, string filePath) {
        string query = $"BACKUP DATABASE {dbName} TO DISK='{filePath}' WITH FORMAT";
        string cmd = $@"-S {server} -E -Q ""{query}"" ";

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

