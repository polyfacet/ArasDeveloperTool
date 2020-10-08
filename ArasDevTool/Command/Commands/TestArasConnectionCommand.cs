using Aras.IOM;
using ArasDevTool.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command.Commands {
    class TestArasConnectionCommand : ILoggableCommand {

        private Config _config;
        public string Name => "TestConnection";

        ILogger Log;
        public ILogger Logger { set => Log = value; }

        public List<string> Help() {
            List<string> msgs = new List<string>();
            msgs.Add("Specify connection string: E.g.");
            msgs.Add(@" -cs=http://localhost/innovator;InnovatorSoluions;admin;innovator");
            msgs.Add(@" -cs=http://localhost/innovator;InnovatorSoluions;admin");
            msgs.Add("Or environment: E.g.");
            msgs.Add("'-env=dev'");
            msgs.Add("Non specified is equivalent with '-env=dev'");
            return msgs;
        }

        public void Run() {
            Aras.ArasConnection conn = new Aras.ArasConnection(_config.ArasAddress, _config.ArasDBName, _config.ArasUser, _config.ArasPassword);
            Innovator inn = conn.GetInnovator();
            Item result = inn.applyAML("<AML><Item action='get' type='ItemType'><name>ItemType</name></Item></AML>");
            Log.LogSuccess($"Connection OK to {conn.ToString()}");
        }

        public bool ValidateInput(List<string> inputArgs) {
            if (inputArgs.Count==1) {
                _config = new Config("dev");
                return true;
            }
            // TODO:Implement other configs input
            return false;
        }
    }
}
