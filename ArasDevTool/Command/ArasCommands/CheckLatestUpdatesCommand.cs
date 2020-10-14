using Aras.IOM;
using ArasDatabaseRepair.Resources;
using ArasDevTool.Aras;
using ArasDevTool.Aras.DatabaseUpgrade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command.ArasCommands {
    class CheckLatestUpdatesCommand : ArasBaseCommad {

        private const int DEAFULT_NBR_OF_ITEMS = 10;
        private int NumberOfItemsToShow = DEAFULT_NBR_OF_ITEMS;

        public override void DoRun() {
            List<AdminType> allAdminTypes = GetAllAdminTypes();
            // Sort
            Log.Log("Sorting " + allAdminTypes.Count + " number of Admin Types.");
            allAdminTypes = allAdminTypes.OrderBy(admType => admType.ModificationDate).ToList();
            allAdminTypes.Reverse();

            for (int i=0;i<NumberOfItemsToShow;i++) {
                AdminType adminType = allAdminTypes[i];
                Log.Log($"{adminType.ModificationDate} : {adminType.GetArasType().PadLeft(20)} : {adminType.GetKeyedName()}");
            }

            // Get Database Upgrade Info
            Log.Log("****** Latest db upgrades/imports ******");
            var dbUpgradeInfo = new DatabaseUpgradeInfo(Inn);
            Log.Log($"{"Date".PadRight(20)}: {"Release".PadRight(8)} : {"Name".PadRight(20)} : {"Is Latest"} : {"Description"}");
            foreach (DatabaseUpgrade dbUpgrade in dbUpgradeInfo.GetDatabaseUpgrades()) {
                Log.Log($"{dbUpgrade.AppliedOn.PadLeft(20)}: {dbUpgrade.TargetRelease.PadLeft(8)} : {dbUpgrade.Name.PadLeft(20)} : {dbUpgrade.IsLatest} : {dbUpgrade.Description}");
            }
        }

        private List<AdminType> GetAllAdminTypes() {
            List<AdminType> adminTypes = new List<AdminType>();

            string amlQuery = ArasMetaDataResources.GetArasMetaDataAml(ArasMetaDataResources.ArasVersion.R11);
            Item result = Inn.applyAML(amlQuery);
            if (!result.isError()) {
                Log.Log($"Admin item count: {result.getItemCount()} ");
                int i = 0;
                foreach (System.Xml.XmlNode itemNode in result.nodeList) {
                    Item adminItemType = result.getItemByIndex(i);
                    AdminType adminType = new AdminType(Inn, adminItemType);
                    adminTypes.Add(adminType);
                    i++;
                }
            }
            else {
                Log.LogError(result.getErrorString());
            }
            return adminTypes;
        }

        public override List<string> GetHelp() {
            List<string> helpMessages = new List<string>();
            helpMessages.Add("Flags: -c");
            helpMessages.Add(@"(Number of items to show: ""-c 20""");
            return helpMessages;
        }

        public override string GetName() {
            return "CheckLatestUpdates";
        }

        public override bool GetValidateInput(List<string> inputArgs) {
            NumberOfItemsToShow = DEAFULT_NBR_OF_ITEMS;
            int i = 0;
            foreach (string arg in inputArgs) {
                i++;
                if (arg == "-c") {
                    if (inputArgs.Count > i) {
                        string strValue = inputArgs[i];
                        int value;
                        if (int.TryParse(strValue,out value)) {
                            NumberOfItemsToShow = value;
                        }
                    }
                }
            }
            return true;
        }
    }
}
