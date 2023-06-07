using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client.IOM;
using Innovator.Client.Model;

namespace Hille.Aras.DevTool.Common.Commands.Aras.Action {
    internal class CreatedItemTypesCount {

        private readonly Innovator.Client.IOM.Innovator Inn;
        private readonly int Year;
        private readonly int Month;
        private readonly int CountThreshold;

        private HashSet<string> ExplicitExemptions = new HashSet<string>(){
            "Activity Task Value",
            "Workflow Process Activity",
            "Change Controlled Item"
        };
        
        public CreatedItemTypesCount(Innovator.Client.IOM.Innovator inn, int year, int month, int countThreshold) {
            Inn = inn;
            Year = year;
            Month = month;
            CountThreshold = countThreshold;
        }

        public List<string> Run() {
            var result = new List<string>();
            var itemTypeInstancesList = new List<ItemTypeInstancesCount>();

            var itemTypeNames = GetAllNoneCoreItemTypeNames();
            foreach ( var itemType in itemTypeNames ) {
                ItemTypeInstancesCount itemTypeInstanceCount = GetItemTypeInstanceCount(itemType);
                if (itemTypeInstanceCount != null) {
                    itemTypeInstancesList.Add(itemTypeInstanceCount);
                }
            }

            // Sort on highest count desc.
            itemTypeInstancesList.Sort((obj1, obj2) => obj2.Count.CompareTo(obj1.Count));
            foreach ( var itemTypeInst in itemTypeInstancesList ) {
                if (itemTypeInst.Count < CountThreshold) return result;
                result.Add($"{itemTypeInst.Count} \t  {itemTypeInst.Name}");
            }

            return result;
        }

        private ItemTypeInstancesCount GetItemTypeInstanceCount(string itemType) {
            DateTime dateFrom = DateTime.Parse($"{Year}-{Month}", null);
            DateTime toDate = dateFrom.AddMonths(1);
            string dateFromString = dateFrom.ToString("yyyy-MM-ddTHH:mm:ss");
            string toDateString = toDate.ToString("yyyy-MM-ddTHH:mm:ss");
            string aml = $@"<AML>
                    <Item action='get' type='{itemType}' select='id'>
                        <created_on condition='gt'>{dateFromString}</created_on>
                        <created_on condition='lt'>{toDateString}</created_on>
                    </Item>'
                </AML>";
            Item result = Inn.applyAML(aml);
            if (!result.isError()) {
                return new ItemTypeInstancesCount(itemType, result.getItemCount());
            }
            return null;
        }

        private HashSet<string> GetAllNoneCoreItemTypeNames() {
            HashSet<string> coreItemTypeNames = GetCoreItemTypeNamesAndOtherExemptions();

            var itemTypeNames = new HashSet<string>();
            Item itemTypes = Inn.newItem("ItemType", "get");
            itemTypes.setAttribute("select", "name");
            itemTypes = itemTypes.apply();
            for (int i = 0; i < itemTypes.getItemCount(); i++) {
                string name = itemTypes.getItemByIndex(i).getProperty("name", "N/A");
                if (!coreItemTypeNames.Contains(name)) {
                    itemTypeNames.Add(name);
                }
            }
            return itemTypeNames;
        }

        private HashSet<string> GetCoreItemTypeNamesAndOtherExemptions() {
            var list = new HashSet<string>();
            foreach (var item in ExplicitExemptions)
            {
                list.Add(item);
            }
            string aml = @"<AML>
                  <Item action='get' type='PackageDefinition' select='id'>
                    <or>
                      <name>com.aras.innovator.core</name>
                      <name>com.aras.innovator.ssvc</name>
                      <name>com.aras.innovator.history</name>
                    </or>
                   <Relationships>
                      <Item action='get' type='PackageGroup' select='id'>
                        <or>
                          <name>ItemType</name>
                          <name>RelationshipType</name>
                        </or>
                        <Relationships>
                          <Item action='get' type='PackageElement' select='name'>
            
                          </Item>
                        </Relationships>
                      </Item>
                    </Relationships>
                  </Item>
                </AML>";
            Item result = Inn.applyAML(aml);

            Item packageElements = result.getItemsByXPath("//Item[@type='PackageElement']");
            for (int i = 0; i < packageElements.getItemCount(); i++) {
                string name = packageElements.getItemByIndex(i).getProperty("name", "N/A");
                list.Add(name);
            }

            return list;
        }

        private class ItemTypeInstancesCount {

            public readonly string Name;
            public readonly int Count;
            public ItemTypeInstancesCount(string name, int count)
            {
                Name = name;
                Count = count;
            }
        }
        
    }
}
