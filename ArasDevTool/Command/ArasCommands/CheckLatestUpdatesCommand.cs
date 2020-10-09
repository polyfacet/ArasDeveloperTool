using Aras.IOM;
using ArasDatabaseRepair.Resources;
using ArasDevTool.Aras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command.ArasCommands {
    class CheckLatestUpdatesCommand : ArasBaseCommad {

        private int NumberOfItemsToShow = 10;

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
            int i = 0;
            foreach(string arg in inputArgs) {
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
