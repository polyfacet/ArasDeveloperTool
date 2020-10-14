using Aras.IOM;
using ArasDevTool.Configuration;
using ArasDevTool.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command.ArasCommands {
    abstract class ArasBaseCommad : IArasCommand, ILoggableCommand {

        private Configuration.IArasConnectionConfig _config;
        protected ILogger Log;
        protected Innovator Inn;
        public ILogger Logger { set => Log = value; }

        public abstract string GetName();
        public abstract List<string> GetHelp();
        public abstract void DoRun();
        public abstract bool GetValidateInput(List<string> inputArgs);

        
        public Innovator Innovator { set => Inn = value; }

        public string Name => this.GetName();

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
            msgs.AddRange(GetHelp());
            return msgs;
        }

        public void Run() {
            DoRun();
        }

        public bool ValidateInput(List<string> inputArgs) {
            bool valid = false;
            if (inputArgs.Count == 1 
                || !String.IsNullOrEmpty(inputArgs.SingleOrDefault(s => s.ToLower().StartsWith("-env")))
                || String.IsNullOrEmpty(inputArgs.SingleOrDefault(s => s.ToLower().StartsWith("-cs")))) {
                _config = new ArasXmlStoredConfig("dev");
                valid = GetValidateInput(inputArgs);
                if (valid) {
                    Inn = new Aras.ArasConnection(_config.ArasAddress, _config.ArasDBName, _config.ArasUser, _config.ArasPassword).GetInnovator();
                }
                return valid;
            }

            string connectionStringArg = inputArgs.SingleOrDefault(s => s.ToLower().StartsWith("-cs"));
            if (!String.IsNullOrEmpty(connectionStringArg)) {
                _config = GetConfigFromConnectionStringArg(connectionStringArg);
                if (_config != null) {
                    valid = GetValidateInput(inputArgs);
                    if (valid) {
                        Inn = new Aras.ArasConnection(_config.ArasAddress, _config.ArasDBName, _config.ArasUser, _config.ArasPassword).GetInnovator();
                    }
                    return valid;
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

