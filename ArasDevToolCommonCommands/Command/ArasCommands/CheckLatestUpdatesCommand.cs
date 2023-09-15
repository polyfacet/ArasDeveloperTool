using System.Collections.Generic;
using System.Linq;
using Hille.Aras.DevTool.Common.Commands.Aras;
using Hille.Aras.DevTool.Common.Aras.DatabaseUpgrade;
using Hille.Aras.DevTool.Interfaces.Command;

namespace Hille.Aras.DevTool.Common.Commands.Command.ArasCommands;
public class CheckLatestUpdatesCommand : ArasBaseCommand {

    private const int DEFAULT_NBR_OF_ITEMS = 10;
    private int NumberOfItemsToShow = DEFAULT_NBR_OF_ITEMS;

    public override void DoRun() {
        List<AdminType> allAdminTypes = AdminType.GetAllAdminTypes(Inn, NumberOfItemsToShow);
        // Sort
        Log.Log("Sorting " + allAdminTypes.Count + " number of Admin Types.");
        allAdminTypes = allAdminTypes.OrderBy(admType => admType.ModificationDate).ToList();
        allAdminTypes.Reverse();
        allAdminTypes.RemoveAll(item => item.GetArasType() == "Team");

        for (int i=0;i<NumberOfItemsToShow;i++) {
            AdminType adminType = allAdminTypes[i];
            string packageElementName = (adminType.GetKeyedName() == adminType.PackageElementName) ? string.Empty : $"({adminType.PackageElementName})"; // Only print if it differs from the name of the item. Because it can have been added to package with another name
            Log.Log($"{adminType.ModificationDate} : {adminType.GetArasType().PadLeft(20)} : {adminType.GetKeyedName().PadRight(30)} : {adminType.PackageName} {packageElementName}");
        }

        // Get Database Upgrade Info
        Log.Log("****** Latest db upgrades/imports ******");
        var dbUpgradeInfo = new DatabaseUpgradeInfo(Inn);
        Log.Log($"{"Date".PadRight(20)}: {"Release".PadRight(8)} : {"Name".PadRight(20)} : {"Is Latest"} : {"Description"}");
        foreach (DatabaseUpgrade dbUpgrade in dbUpgradeInfo.GetDatabaseUpgrades()) {
            Log.Log($"{dbUpgrade.AppliedOn.PadLeft(20)}: {dbUpgrade.TargetRelease.PadLeft(8)} : {dbUpgrade.Name.PadLeft(20)} : {dbUpgrade.IsLatest} : {dbUpgrade.Description}");
        }
    }

    public override List<string> GetHelp() {
        List<string> helpMessages = new List<string>
        {
            "Options:",
            "  -c \t Number of items to show: \"-c 20\""
        };
        return helpMessages;
    }

    public override string GetName() {
        return "CheckLatestUpdates";
    }

    public override bool GetValidateInput(List<string> inputArgs) {
        NumberOfItemsToShow = DEFAULT_NBR_OF_ITEMS;
        if (CommandUtils.OptionExistWithValue(inputArgs, "-c", out string numberString)
            && int.TryParse(numberString, out int value)) {
                NumberOfItemsToShow = value;
        }
        return true;
    }
}
