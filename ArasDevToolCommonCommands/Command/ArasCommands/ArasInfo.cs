using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hille.Aras.DevTool.Common.Commands.Command.ArasCommands {
    class ArasInfo : ArasBaseCommand {

        Aras.ArasInfo _arasInfo;
        private Aras.ArasInfo ArasInformation { 
            get { 
                if (_arasInfo == null) {
                    _arasInfo = new Aras.ArasInfo(Inn);
                }
                return _arasInfo;
            }  
        }

        public override void DoRun() {
            Log.Log("Url: " +Inn.getConnection().MapClientUrl("../../") + " DB: " + Inn.getConnection().Database);
            PrintReleaseAndServicePack();
            Log.Log(String.Empty);
            PrintLatestDatabaseUpgrade();
            Log.Log(String.Empty);
            PrintLatestModifiedAdminType();
            Log.Log(String.Empty);
            PrintLastLoggedInUser();
        }

        public override List<string> GetHelp() {
            return new List<string>()
            {
                "Shows the following information of Aras Environment",
                "* Release and Service Pack",
                "* Latest Database Upgrade",
                "* Latest modified admin type",
                "* Latest logged in user (non admin)"
            };
        }

        public override string GetName() {
            return "ArasInfo";
        }

        public override bool GetValidateInput(List<string> inputArgs) {
            return true;
        }

        private void PrintLastLoggedInUser() {
            Log.Log("Latest logged in user (non admin)");
            KeyValuePair<string,string> loggedInUserInfo = ArasInformation.LastestLoggedInUserNonAdmin;
            Log.Log(loggedInUserInfo.Key + " : " + loggedInUserInfo.Value);
        }

        private void PrintLatestModifiedAdminType() {
            Log.Log("Latest modifed admin type");
            Aras.AdminType adminType = ArasInformation.ModifiedAdminTypesByLastModifcationDate[0];
            string message = adminType.GetArasType() + " : " + adminType.GetKeyedName();
            message += " : " + adminType.ModificationDate.ToString("yyyy-MM-dd HH:mm:ss");
            Log.Log(message);
        }

        private void PrintLatestDatabaseUpgrade() {
            Log.Log("Latest Database Upgrade");
            Log.Log(ArasInformation.LatestDatabaseUpgradeItem.ToString());
        }

        private void PrintReleaseAndServicePack() {
            Log.Log("Release and Service Pack");
            Log.Log(ArasInformation.MajorVersion + " " + ArasInformation.ServicePack);
        }

    }
}
