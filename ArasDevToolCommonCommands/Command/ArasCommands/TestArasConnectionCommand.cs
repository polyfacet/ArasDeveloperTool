using Innovator.Client.IOM;
using Hille.Aras.DevTool.Interfaces.Configuration;
using Hille.Aras.DevTool.Interfaces.Logging;
using Hille.Aras.DevTool.Interfaces.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hille.Aras.DevTool.Interfaces.Aras;

namespace Hille.Aras.DevTool.Common.Commands.Command.ArasCommands {
    public class TestArasConnectionCommand : ILoggableCommand {

        private IArasConnectionConfig _config;
        public string Name => "TestConnection";

        ILogger Log;
        public ILogger Logger { set => Log = value; }

        public List<string> Help() {
            List<string> msgs = new List<string>
            {
                "Specify connection string: E.g.",
                @" -cs=""http://localhost/innovator;InnovatorSoluions;admin;innovator""",
                @" -cs=""http://localhost/innovator;InnovatorSoluions;admin""",
                "Or environment: E.g.",
                "-env=dev",
                "Non specified is equivalent with '-env=dev'"
            };
            return msgs;
        }

        public void Run() {
            ArasConnection conn = new ArasConnection(_config.ArasAddress, _config.ArasDBName, _config.ArasUser, _config.ArasPassword);
            Innovator.Client.IOM.Innovator inn = conn.GetInnovator();
            string amlQuery = @"<AML>
                <Item action='get' type='Variable' select='name,value'>
                <or>
                    <name>VersionMajor</name>
                    <name>VersionServicePack</name>
                </or>
                </Item></AML>";

            Item result = inn.applyAML(amlQuery);
            if (!result.isError()) {
                Log.LogSuccess($"Connection OK to {conn.ToString()}");
                string majorVersion = String.Empty;
                string servicePack = String.Empty;
                for (int i = 0; i<result.getItemCount(); i++) {
                    Item item = result.getItemByIndex(i);
                    if (item.getProperty("name") == "VersionMajor")
                        majorVersion = item.getProperty("value", string.Empty);
                    if (item.getProperty("name") == "VersionServicePack")
                        servicePack = item.getProperty("value", string.Empty);
                }
                Log.Log($"Release: {majorVersion}");
                Log.Log($"Service Pack: {servicePack}");
            }
            else {
                Log.LogError($"{result.getErrorString()}");
            }
        }

        public bool ValidateInput(List<string> inputArgs) {
            if (inputArgs.Count==1) {
                _config = new ArasXmlStoredConfig("dev");
                return true;
            }

            string connectionStringArg = inputArgs.SingleOrDefault(s => s.ToLower().StartsWith("-cs"));
            if (!String.IsNullOrEmpty(connectionStringArg)) {
                _config = GetConfigFromConnectionStringArg(connectionStringArg);
                if (_config != null) {
                    return true;
                }
                Log.LogError("Wrong connection string format");
                return false;
            }
            return false;
        }

        private IArasConnectionConfig GetConfigFromConnectionStringArg(string connectionStringArg) {
            if (connectionStringArg.Contains("=") && connectionStringArg.Contains(";")) {
                string connectionString = connectionStringArg.Split('=')[1].Trim();
                string[] parameters = connectionString.Split(';');
                if (parameters.Length >= 3) {
                    IArasConnectionConfig config = new ArasConnectionConfig
                    {
                        ArasAddress = parameters[0].Trim(),
                        ArasDBName = parameters[1].Trim(),
                        ArasUser = parameters[2].Trim()
                    };
                    if (parameters.Length > 3 && !String.IsNullOrEmpty(parameters[3].Trim())) {
                        config.ArasPassword = parameters[3].Trim();
                    }
                    else {
                        Console.WriteLine("Set Aras Password");
                        config.ArasPassword = ConsoleUtils.ReadPassword();
                    }
                    return config;
                }
            }
            return null;
        }
    }
}
