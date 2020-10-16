using Innovator.Client.IOM;
using ArasDevTool.Configuration;
using ArasDevTool.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command.ArasCommands {
    class TestArasConnectionCommand : ILoggableCommand {

        private Configuration.IArasConnectionConfig _config;
        public string Name => "TestConnection";

        ILogger Log;
        public ILogger Logger { set => Log = value; }

        public List<string> Help() {
            List<string> msgs = new List<string>();
            msgs.Add("Specify connection string: E.g.");
            msgs.Add(@" -cs=""http://localhost/innovator;InnovatorSoluions;admin;innovator""");
            msgs.Add(@" -cs=""http://localhost/innovator;InnovatorSoluions;admin""");
            msgs.Add("Or environment: E.g.");
            msgs.Add("-env=dev");
            msgs.Add("Non specified is equivalent with '-env=dev'");
            return msgs;
        }

        public void Run() {
            Aras.ArasConnection conn = new Aras.ArasConnection(_config.ArasAddress, _config.ArasDBName, _config.ArasUser, _config.ArasPassword);
            Innovator.Client.IOM.Innovator inn = conn.GetInnovator();
            Item result = inn.applyAML("<AML><Item action='get' type='ItemType'><name>ItemType</name></Item></AML>");
            if (!result.isError()) {
                Log.LogSuccess($"Connection OK to {conn.ToString()}");
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
