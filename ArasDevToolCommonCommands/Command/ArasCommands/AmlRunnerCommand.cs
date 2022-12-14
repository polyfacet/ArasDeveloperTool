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
        private bool _singleFile;

        public override void DoRun() {
            if (_singleFile) {
                RunSingleFile(_filePath);
            }
            else {
                RunByManifestFile();
            }
        }

        private void RunSingleFile(string amlFilePath) {
            AMLFile amlFile = new AMLFile(amlFilePath);
            RunSingleFile(amlFile);
        }

        private void RunSingleFile(AMLFile amlFile) {
            foreach (string aml in amlFile.AMLs) {
                Log.Log(aml);
                Item result = Inn.applyAML(aml);
                if (result.isError()) {
                    Log.LogError($"{result.getErrorString()}");
                    if (amlFile.StopOnError) {
                        Log.LogError("Stopping Execution");
                        return;
                    }
                }
                else {
                    Log.LogSuccess("OK");
                }
            }
        }

        private void RunByManifestFile() {
            var mfParser = new ManifestParser(_filePath, _env);
            foreach (AMLFile amlFile in mfParser.AMLFilesToExecute) {
                RunSingleFile(amlFile);
            }
        }

        public override List<string> GetHelp() {
            return new List<string>()
            {
                "Executes 'aml-files' refered in a manifest file",
                "Parameters:",
                "  -mf \t Manifest file for amls Example: \"-mf MyAmlRuns.mf\"" ,
                "  OR ",
                "  -file \t Single file for AML(s): \"-file MyAmls.xml\"" ,
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

            if (CommandUtils.OptionExistWithValue(inputArgs,"-mf",out string mfFilePath)) {
                _filePath = mfFilePath;
                if (File.Exists(mfFilePath)) return true;
                Log.LogError($"File not found: {mfFilePath}");
            }
            if (CommandUtils.OptionExistWithValue(inputArgs, "-file", out string filePath)) {
                _filePath = filePath;
                _singleFile = true;
                if (File.Exists(filePath)) return true;
                Log.LogError($"File not found: {filePath}");
            }
            Log.LogError("No manifest or file file specified");
            return false;
        }
    }
}
