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
    public class AmlRunnerCommand : ArasBaseCommand {

        private string _filePath;
        private string _env;

        public override void DoRun() {
            var mfParser = new ManifestParser(_filePath, _env);
            foreach (AMLFile amlFile in mfParser.AMLFilesToExecute) {
                foreach(string aml in amlFile.AMLs) {
                    Log.Log(aml);
                    Item result = Inn.applyAML(aml);
                    if (result.isError()) {
                        Log.LogError($"{result.getErrorString()}");
                        if (amlFile.StopOnError) {
                            Log.LogError("Stopping Executtion");
                            return;
                        }
                    }
                    else {
                        Log.LogSuccess("OK");
                    }
                }
            }
        }

        public override List<string> GetHelp() {
            return new List<string>()
            {
                "Executes 'aml-files' refered in a manifest file",
                "Parameters:",
                "  -mf \t Manifest file for amls Example: \"-mf MyAmlRuns.mf\"" ,
                "  ",
                "Options:   ",
                "  -es dev \t Run environment specifics amls in sub-folder env\\dev"
            };
        }

        public override string GetName() {
            return "AmlRunner";
        }

        public override bool GetValidateInput(List<string> inputArgs) {
            if (CommandUtils.OptionExistWithValue(inputArgs, "-es", out string env)) {
                _env = env;
            }

            if (CommandUtils.OptionExistWithValue(inputArgs,"-mf",out string filePath)) {
                _filePath = filePath;
                if (File.Exists(filePath)) return true;
                Log.LogError($"File not found: {filePath}");
            }
            Log.LogError("No manifest file specified");
            return false;
        }
    }
}
