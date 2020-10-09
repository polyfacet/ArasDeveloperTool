using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Aras {
    internal class AdminType : InnovatorBase {

        private const string MODIFIED_ON = "modified_on";
        public AdminType(Innovator inn, Item item):base(inn) {
            Item = item;
        }

        public Item Item { get; set; }

        public string GetId() {
            return Item.getID();
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
                    // Dim modDate As Date = DateTime.Parse(dateString)
                    DateTime modDate;
                    DateTime.TryParse(dateString, out modDate);
                    modificationDateField = modDate;
                }
                return modificationDateField;
            }
        }

        protected string RetrieveProperty(string propertyName) {
            string amlQuery = string.Format("<AML><Item action='get' type='{0}' id='{1}' select='{2}'></Item></AML>"
                                                 , this.GetArasType(), this.GetId(), propertyName);

            Item item = Inn.applyAML(amlQuery);
            string value = item.getProperty(propertyName);
            return value;
        }

    }
}
