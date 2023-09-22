using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Logging;
using Innovator.Client;

namespace Hille.Aras.DevTool.Common.Commands.Command.Commands;
class ListEnvironmentsCommand : ICommand {
    public string Name => "ListEnvironments";
    public ILogger Logger = new BasicLogger();

    public List<string> Help()
    {
        return new List<string>()
        {
            "List configured/setup environments"
        };
    }

    public void Run()
    {
        Logger.Log($"Config file path: {DefaultSetupHandler.ConfigFilePath}" );
        var setupHandler = new DefaultSetupHandler();
        foreach (string envName in setupHandler.GetConfiguredEnvironmentNames())
        {
            var config = setupHandler.GetConfig(envName);
            string message = $@"
            Environment Name: {envName}
            Address: {config.ArasAddress}
            DB: {config.ArasDBName}
            User: {config.ArasUser}
            ";
            Logger.Log(message);
        }
    }

    public bool ValidateInput(List<string> inputArgs)
    {
        return true;
    }
}
