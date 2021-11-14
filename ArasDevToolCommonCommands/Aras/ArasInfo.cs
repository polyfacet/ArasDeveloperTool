using Hille.Aras.DevTool.Common.Aras.DatabaseUpgrade;
using Innovator.Client.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hille.Aras.DevTool.Common.Commands.Aras {
    internal class ArasInfo : InnovatorBase {
        private DatabaseUpgrade _latestDatabaseUpgradeItem;
        private List<AdminType> _modifiedAdminTypesByLastModifcationDate;
        private KeyValuePair<string, string> _lastestLoggedInUserNonAdmin = new KeyValuePair<string, string>(String.Empty,String.Empty);
        private string _servicePack;
        private string _majorVersion;

        public ArasInfo(Innovator.Client.IOM.Innovator inn) : base(inn) { }


        public string MajorVersion {
            get {
                if (_majorVersion == null) LoadInfo();
                return _majorVersion;
            }
        }

        public string ServicePack {
            get {
                if (_servicePack == null) LoadInfo();
                return _servicePack;
            }
        }

        public DatabaseUpgrade LatestDatabaseUpgradeItem {
            get {
                if (string.IsNullOrEmpty(_latestDatabaseUpgradeItem.Name)) LoadInfo();
                return _latestDatabaseUpgradeItem;
            }
        }

        public List<AdminType> ModifiedAdminTypesByLastModifcationDate {
            get {
                if (_modifiedAdminTypesByLastModifcationDate == null) LoadInfo();
                return _modifiedAdminTypesByLastModifcationDate;
            }
        }

        public KeyValuePair<string, string> LastestLoggedInUserNonAdmin {
            get {
                if (String.IsNullOrEmpty(_lastestLoggedInUserNonAdmin.Key)) LoadInfo();
                return _lastestLoggedInUserNonAdmin;
            }
        }

        private void LoadInfo() {

            // Version and SP
            KeyValuePair<string, string> versionAndSP = GetMajorVersionAndServicPack();
            _majorVersion = versionAndSP.Key;
            _servicePack = versionAndSP.Value;

            // Modified ItemTypes
            _modifiedAdminTypesByLastModifcationDate = AdminType.GetAllAdminTypesOrderByLatestModfied(Inn,1000);

            // DB Upgrade
            var dbUpgradeInfo = new DatabaseUpgradeInfo(Inn);
            _latestDatabaseUpgradeItem = dbUpgradeInfo.GetDatabaseUpgrades()[0];

            // Lastest logged in user
            foreach (User user in User.LatestLoggedInUsers(Inn)) {
                if (!user.IsAdmin) {
                    _lastestLoggedInUserNonAdmin = new KeyValuePair<string, string>(user.KeyedName, user.LastLogin);
                    break;
                }
            }
        }

        private KeyValuePair<string, string> GetMajorVersionAndServicPack() {
            string servicePack = "N/A";
            string majorVersion = "N/A";
            string amlQuery = @"<AML>
                <Item action='get' type='Variable' select='name,value'>
                <or>
                    <name>VersionMajor</name>
                    <name>VersionServicePack</name>
                </or>
                </Item></AML>";

            Item result = Inn.applyAML(amlQuery);
            if (!result.isError()) {
                for (int i = 0; i < result.getItemCount(); i++) {
                    Item item = result.getItemByIndex(i);
                    if (item.getProperty("name") == "VersionMajor")
                        majorVersion = item.getProperty("value", string.Empty);
                    if (item.getProperty("name") == "VersionServicePack")
                        servicePack = item.getProperty("value", string.Empty);
                }
            }
            return new KeyValuePair<string, string>(majorVersion, servicePack);
        }

    }
}
