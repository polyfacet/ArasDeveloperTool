using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Configuration;
using System;
using System.Diagnostics;

namespace Hille.Aras.DevTool.Common.Commands.Command.Commands;
public class SetupCommand : ICommand {

    private bool ExtendedSetup;
    private string Env = "dev";
    private IArasConnectionConfig Config;

    public string Name => "Setup";

    public List<string> Help() {
        return new List<string>
        {
            "Configure Aras connection",
            "",
            "Options:",
            "-env deploy \t Specify name of setup: Default dev",
            "-ext \t Extended configuration for Database, Import/Export support"
        };
    }

    public void Run() {
        RunSetup();
        TestSetup();
    }

    private void RunSetup() {
        DefaultSetupHandler setupHandler = new DefaultSetupHandler();
        if (ExtendedSetup) {
            Config = setupHandler.Setup(Env);
        }
        else {
            Config = setupHandler.SetupConnection(Env);
        }

    }

    private void TestSetup() {
        TestConnection();
        if (Config is IArasSetupConfig) {
            TestSqlCmd(((IArasSetupConfig)Config).SqlCmd);
        }
    }

    private void TestSqlCmd(string sqlCmd) {
        using (Process p = new Process
            {
                StartInfo = new ProcessStartInfo(sqlCmd, "-?")
            }
        ) {
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit();
            int exitCode = p.ExitCode;
            Console.WriteLine(exitCode);
        }
    }

    private void TestConnection() {
        var testCommand = new ArasCommands.TestArasConnectionCommand
        {
            Logger = new Hille.Aras.DevTool.Interfaces.Logging.BasicLogger()
        };
        var inputArgs = new List<string>
        {
            testCommand.Name,
            "-env",
            Env
        };
        if (testCommand.ValidateInput(inputArgs))
            testCommand.Run();
    }

    public bool ValidateInput(List<string> inputArgs) {
        if (CommandUtils.HasOption(inputArgs,"-ext")) {
            ExtendedSetup = true;
        }
        if (CommandUtils.OptionExistWithValue(inputArgs,"-env",out string env)) {
            Env = env;
        }
        return true;
    }
        
}
