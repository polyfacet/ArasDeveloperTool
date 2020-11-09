using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Interfaces.Logging;
using Hille.Aras.DevTool.Interfaces.Configuration;
using Hille.Aras.DevTool.Interfaces.Aras;
using System;
using System.Collections.Generic;

namespace Hille.Aras.DevTool.Common.Commands.Command.ArasCommands {
    public abstract class ArasBaseCommand : IArasCommand, ILoggableCommand {

        private IArasConnectionConfig _config;
        protected ILogger Log { get; set; }
        protected Innovator.Client.IOM.Innovator Inn { get; set; }
        public ILogger Logger { set => Log = value; }

        public abstract string GetName();
        public abstract List<string> GetHelp();
        public abstract void DoRun();
        public abstract bool GetValidateInput(List<string> inputArgs);

        
        public Innovator.Client.IOM.Innovator Innovator { set => Inn = value; }

        public string Name => this.GetName();

        public List<string> Help() {
            List<string> msgs = new List<string>
            {
                "Specify connection string: E.g.",
                @" -cs=""http://localhost/innovator;InnovatorSoluions;admin;innovator""",
                @" -cs=""http://localhost/innovator;InnovatorSoluions;admin""",
                "Or environment: E.g.",
                "-env=dev",
                "Non specified is equivalent with '-env=dev'",
                ""
            };
            msgs.AddRange(GetHelp());
            return msgs;
        }

        public void Run() {
            DoRun();
        }

        public bool ValidateInput(List<string> inputArgs) {
            bool valid;
            if (CommandUtils.HasOption(inputArgs, "--help")) {
                return true;
            }

            if (CommandUtils.HasOptionStartingWith(inputArgs, "-cs", out string connectionStringArg)) {
                _config = GetConfigFromConnectionStringArg(connectionStringArg);
                if (_config != null) {
                    valid = GetValidateInput(inputArgs);
                    if (valid) {
                        Inn = new ArasConnection(_config.ArasAddress, _config.ArasDBName, _config.ArasUser, _config.ArasPassword).GetInnovator();
                    }
                    return valid;
                }
                Log.LogError("Wrong connection string format");
                return false;
            }

            if (inputArgs != null && inputArgs.Count == 1
                || !CommandUtils.HasOptionStartingWith(inputArgs,"-env")
                || CommandUtils.HasOptionStartingWith(inputArgs, "-cs")) {
                _config = new ArasXmlStoredConfig("dev");
                valid = GetValidateInput(inputArgs);
                if (valid) {
                    Log.Log($"New Aras Connection: {_config.ArasAddress}, {_config.ArasDBName}, {_config.ArasUser}");
                    Inn = new ArasConnection(_config.ArasAddress, _config.ArasDBName, _config.ArasUser, _config.ArasPassword).GetInnovator();
                }
                return valid;
            }
     
            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
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

