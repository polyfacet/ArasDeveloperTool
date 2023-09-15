using Hille.Aras.DevTool.Interfaces.Command;
using System.Collections.Generic;
using Innovator.Client.IOM;
using Hille.Aras.DevTool.Common.Commands.Aras.Action;

namespace Hille.Aras.DevTool.Common.Commands.Command.ArasCommands;
public class RestoreMethodCommand : ArasBaseCommand {

    private string _methodName;
    private string _toDateInput = string.Empty;

    public override void DoRun() {
        var restoreMethod = new RestoreMethod(Inn, _methodName, _toDateInput);
        Item result = restoreMethod.Apply();
        if (result.isError()) {
            Log.LogError(result.getErrorString());
            return;
        }
        Log.LogSuccess(result.getResult());
    }

    public override List<string> GetHelp() {
        return new List<string>()
        {
            "'Restore' a specified Method to previous generation or a specified  date",
            "Parameters:",
            "  -name \t Name of Method. Example: \"-name MY_UpdateAllParts\"" ,
            " ",
            "Options:   ",
            "  -toDate \t Date to restore method to. Format: yyyy-MM-ddTHH:mm:ss",
            "  "
        };
    }

    public override string GetName() {
        return "RestoreMethod";
    }

    public override bool GetValidateInput(List<string> inputArgs) {
        if (CommandUtils.OptionExistWithValue(inputArgs, "-name", out string methodName)) {
            _methodName = methodName;
        }
        else {
            Log.LogError("Name of method not specfied");
            return false;
        }

        if (CommandUtils.OptionExistWithValue(inputArgs, "-toDate", out string inputBody)) {
            _toDateInput = inputBody;
        }
        return true;
    }
}
