using Hille.Aras.DevTool.Interfaces.Command;
using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client.IOM;

namespace Hille.Aras.DevTool.Common.Commands.Command.ArasCommands {
    public class ApplyMethod : ArasBaseCommand {

        private string _methodName;
        private string _inputBody;

        public override void DoRun() {
            Item result = Inn.applyMethod(_methodName,"");
            if (result.isError()) {
                Log.LogError(result.getErrorString());
                return;
            }
            Log.LogSuccess(result.getResult());
        }

        public override List<string> GetHelp() {
            return new List<string>()
            {
                "Executes a specfied Method",
                "Parameters:",
                "  -name \t Name of Method. Example: \"-name MY_UpdateAllParts\"" ,
                " ",
                "Options:   ",
                "  -body \t Input to method",
                "  "
            };
        }

        public override string GetName() {
            return "ApplyMethod";
        }

        public override bool GetValidateInput(List<string> inputArgs) {
            if (CommandUtils.OptionExistWithValue(inputArgs, "-name", out string methodName)) {
                _methodName = methodName;
                return true;
            }
            else {
                Log.LogError("Name of method not specfied");
            }

            if (CommandUtils.OptionExistWithValue(inputArgs, "-body", out string inputBody)) {
                _inputBody = inputBody;
            }
            return false;
        }
    }
}
