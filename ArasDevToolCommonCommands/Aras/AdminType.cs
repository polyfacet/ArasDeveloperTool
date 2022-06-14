using Hille.Aras.DevTool.Common.Commands.Aras.Resources;
using Innovator.Client.IOM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hille.Aras.DevTool.Common.Commands.Aras {
    internal class AdminType : InnovatorBase {

        private const string MODIFIED_ON = "modified_on";
        public AdminType(Innovator.Client.IOM.Innovator inn, Item item):base(inn) {
            Item = item;
        }

        public static List<AdminType> GetAllAdminTypes(Innovator.Client.IOM.Innovator inn,int maxRecords) {
            List<AdminType> adminTypes = new List<AdminType>();
            string amlQuery = ArasMetaDataResources.GetArasMetaDataAml(ArasUtils.GetMajorVersion(inn));
            amlQuery = string.Format(amlQuery, maxRecords);
            Item result = inn.applyAML(amlQuery);
            if (!result.isError()) {
                int i = 0;
                foreach (System.Xml.XmlNode itemNode in result.nodeList) {
                    Item adminItemType = result.getItemByIndex(i);
                    AdminType adminType = new AdminType(inn, adminItemType);
                    adminTypes.Add(adminType);
                    i++;
                }
            }
            return adminTypes;
        }

        public static List<AdminType> GetAllAdminTypesOrderByLatestModfied(Innovator.Client.IOM.Innovator inn, int maxRecords) {
            List<AdminType> allAdminTypes = AdminType.GetAllAdminTypes(inn, maxRecords);
            allAdminTypes = allAdminTypes.OrderBy(admType => admType.ModificationDate).ToList();
            allAdminTypes.Reverse();
            return allAdminTypes;
        }

        public Item Item { get; set; }

        public string GetId() {
            return Item.getID();
        }

        public string CongfigId() {
            string configId =  Item.getProperty("config_id");
            if (!String.IsNullOrEmpty(configId)) return configId;
            Item = Inn.getItemById(GetArasType(),GetId());
            configId = Item.getProperty("config_id", "N/A");
            return configId;
        }

        public string GetArasType() {
            return Item.getType();
        }

        public string GetKeyedName() {
            return Item.getPropertyAttribute("config_id", "keyed_name");
        }

        public string GetConfigId() {
            return Item.getProperty("config_id");
        }

        private bool isRelationShipField = System.Convert.ToBoolean(-1);
        public bool IsRelationShip {
            get {
                if (System.Convert.ToInt32(isRelationShipField) == -1) {
                    isRelationShipField = false;
                    if (this.GetArasType() == "ItemType") {
                        string value = this.RetrieveProperty("is_relationship");
                        if (value == "1")
                            isRelationShipField = true;
                    }
                }
                return isRelationShipField;
            }
        }

        private DateTime modificationDateField = DateTime.MinValue;
        public DateTime ModificationDate {
            get {
                if (modificationDateField == DateTime.MinValue) {
                    
                    string dateString = Item.getProperty(MODIFIED_ON);
                    if (string.IsNullOrEmpty(dateString))
                        dateString = RetrieveProperty(MODIFIED_ON);
                    if (DateTime.TryParse(dateString, out DateTime modDate)) {
                        modificationDateField = modDate;
                    }
                }
                return modificationDateField;
            }
        }

        private string packageNameField;
        public string PackageName {
            get {
                if (string.IsNullOrEmpty(packageNameField)) {
                    LoadPackageName();
                }
                return packageNameField;
            }
        }

        private string packageElementNameField;
        public string PackageElementName {
            get {
                if (string.IsNullOrEmpty(packageElementNameField)) {
                    LoadPackageName();
                }
                return packageElementNameField;
            }
        }

      
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "<Pending>")]
        protected string RetrieveProperty(string propertyName) {
            string amlQuery = string.Format("<AML><Item action='get' type='{0}' id='{1}' select='{2}'></Item></AML>"
                                                 , this.GetArasType(), this.GetId(), propertyName);

            Item item = Inn.applyAML(amlQuery);
            string value = item.getProperty(propertyName);
            return value;
        }
        private void LoadPackageName() {
            string amlQuery = $@"<AML>
                  <Item action='get' type='PackageGroup' select='source_id'> 
                    <Relationships>
                      <Item action='get' type='PackageElement' select='element_type,name'>
                        <element_type>{this.GetArasType()}</element_type>
                        <element_id>{this.GetConfigId()}</element_id>
                      </Item>  
                    </Relationships>
                  </Item>
                </AML>";
            Item result = Inn.applyAML(amlQuery);
            if (result.isError()) return;
            packageNameField =  result.getPropertyAttribute("source_id", "keyed_name");
            packageElementNameField = result.getItemsByXPath("//Item[@type='PackageElement']").getProperty("name");
        }

    }
}
