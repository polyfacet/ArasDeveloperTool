using Hille.Aras.DevTool.Interfaces.Command;
using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client.IOM;
using Hille.Aras.DevTool.Common.Commands.Aras.Action;
using System.Net.NetworkInformation;

namespace Hille.Aras.DevTool.Common.Commands.Command.ArasCommands {
    public class CreatedItemTypesCountCommand : ArasBaseCommand {

        private string _monthInput = string.Empty;
        private string _countThresholdInput = string.Empty;
        private int _year;
        private int _month;
        private int _countThreshold = 5;

        public override void DoRun() {
            if (_year == 0 || _month == 0) {
                LoadPreviousMonthAsDefault();
            }
            var restoreMethod = new CreatedItemTypesCount(Inn, _year, _month, _countThreshold);
            var result = restoreMethod.Run();
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        private void LoadPreviousMonthAsDefault() {
            DateTime now = DateTime.Now;
            int currentMonth = now.Month;
            int currentYear = now.Year;
            if (currentMonth == 1) {
                _month = 12;
                _year = currentYear - 1;
            }
            _month = currentMonth - 1;
            _year = currentYear;
        }

        public override List<string> GetHelp() {
            return new List<string>()
            {

                "List number of instances created for each Item Type in Aras, ordered descending on highest number of instances.",
                " ",
                "Options:   ",
                " -month \t Month to apply count for. Format: yyyy-MM  (default previous month)",
                " -count-display-threshold \t Threshold for the minimum instances created in order to be printed to output {int} #(default 5)", 
                "  "
            };
        }

        public override string GetName() {
            return "CreatedItemTypesCount";
        }

        public override bool GetValidateInput(List<string> inputArgs) {
            
            if (CommandUtils.OptionExistWithValue(inputArgs, "-month", out string monthInput)) {
                _monthInput = monthInput;
                _year = DateTime.Parse(monthInput, null).Year;
                _month = DateTime.Parse(monthInput, null).Month;
            }

            if (CommandUtils.OptionExistWithValue(inputArgs, "-count-display-threshold", out string countThresholdInput)) {
                _countThresholdInput = countThresholdInput;
                _countThreshold = int.Parse(countThresholdInput, null);
            }
            return true;
        }
    }
}
