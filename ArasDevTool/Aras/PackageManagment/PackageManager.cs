using Aras.IOM;
using ArasDevTool.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Aras.PackageManagment {
    class PackageManager : ILoggable {
        ILogger Log;
        public ILogger Logger { set => Log = value; }

        private readonly Innovator Inn;
        private Dictionary<string, Item> _cachedPackageElementIdToPackageDefinitionMap;

        public PackageManager(Innovator inn) {
            Inn = inn;
        }

        public bool DryRun { get; set; }
        public bool AutoPack { get; set; }


        public Item FindAndAddSuitablePackageForItem(Item item) {
            Item packageElement = Inn.newError($"No package element created for: {item.TypeAndName()}");
            Item relatedItemType;
            string errorMessage;
            switch (item.getType()) {
                case "ItemType":
                    // Probably not possbile to figure out which packages it should belong to.
                    errorMessage = $"Could not find a suitable package for: {item.TypeAndName()}";
                    return Inn.newError(errorMessage);

                default:
                    relatedItemType = GetClosestRelatedItemType(item);
                    break;
            }
            if (!relatedItemType.isError()) {
                // Find a "suitable" package
                Item packageDefinition = GetPackageDefinition(relatedItemType);
                if (packageDefinition == null) {
                    errorMessage = $"No package def found for: {relatedItemType.TypeAndName()} , {relatedItemType.getID()}";
                    Log.LogWarning(errorMessage);
                    return Inn.newError(errorMessage);
                }
                packageDefinition = ValidateWithUser(packageDefinition);
                if (packageDefinition == null) return packageElement;
                // Create a new PackageElement from item and put it in the package
                packageElement = AddItemToPackageDefinition(item, packageDefinition);
                if (packageElement != null && !packageElement.isError()) {
                    AddToCache(packageElement, packageDefinition);
                }

            }
            return packageElement;
        }

        public string GetPackageDefinitionNameFromPackageElement(Item packageElement) {
            if (_cachedPackageElementIdToPackageDefinitionMap.ContainsKey(packageElement.getID())) {
                return _cachedPackageElementIdToPackageDefinitionMap[packageElement.getID()].getProperty("name", "N/A");
            }
            return GetPackageDefinitionByPackageElement(packageElement).Name();
        }

        private Item ValidateWithUser(Item packageDefinition) {
            if (AutoPack) return packageDefinition;
            Console.WriteLine($"Add to: {packageDefinition.Name()}? Yes/No/Search (Y/N/S)");
            string answer = Console.ReadLine();
            if (answer.ToUpper() == "N" || answer.ToUpper() == "NO") return null;
            if (answer.ToUpper() == "Y" || answer.ToUpper() == "YES") return packageDefinition;
            if (answer.ToUpper() == "S" || answer.ToUpper() == "SEARCH") {
                Item newPkgDef = GetNewPackageDefinitionFromUserInput();
                if (newPkgDef != null) {
                    return newPkgDef;
                }            
            }
            return null;
        }

        private Item GetNewPackageDefinitionFromUserInput() {
        SearchAgain:
            Console.WriteLine("Search package: Exmample: *com.acme*");
            string searchString = Console.ReadLine();
            // Search it and list them and make user select by number or cancel or new :
            // (1) com.acme.plm
            // (2) com.acme.documents
            // (New) CREATE NEW
            // (c) Cancel
            List<Item> packageDefinitions = FindPackageDefinitions(searchString);
            int i = 0;
            if (packageDefinitions.Count > 0) {
                Console.WriteLine("Select package by entering specific number:");
                foreach (Item packageDef in packageDefinitions) {
                    i++;
                    Console.WriteLine($"({i}) {packageDef.Name()}");
                }
                Console.WriteLine("(New) CREATE NEW");
                Console.WriteLine("(c) Cancel");
                string choice = Console.ReadLine();
                if (int.TryParse(choice, out int intChoice)
                    && intChoice - 1 < packageDefinitions.Count()) {
                    return packageDefinitions[intChoice - 1];
                }
                if (choice.ToUpper() == "C") {
                    return null;
                }
                if (choice.ToUpper() == "NEW") {
                    NewPackageInput:
                    Console.WriteLine("Assign name for new package:");
                    string newName = Console.ReadLine();
                    Item newPackageDefintion = CreateNewPackageDefintion(newName);
                    if (newPackageDefintion.isError()) {
                        Log.LogError($@"Could not create new Package Defintion with name
                                {newName} : {newPackageDefintion.getErrorString()}");
                        goto NewPackageInput;
                    }
                    return newPackageDefintion;
                }
            }
            else {
                Log.LogWarning($"No result for: {searchString}");
                goto SearchAgain;
            }
            return null;
        }

        private Item CreateNewPackageDefintion(string newName) {
            Item packageDefintion = Inn.newItem("PackageDefinition", "add");
            packageDefintion.setProperty("name", newName);
            if (DryRun) return packageDefintion;
            packageDefintion = packageDefintion.apply();
            return packageDefintion;
        }

        private List<Item> FindPackageDefinitions(string searchString) {
            var packageDefinitions = new List<Item>();
            string amlQuery = $@"<AML><Item action='get' type='PackageDefinition' orderBy='modified_on DESC'>
                <name condition='like'>{searchString}</name>                
                </Item></AML>";
            Item pkgsDefs = Inn.applyAML(amlQuery);
            for (int i = 0; i<pkgsDefs.getItemCount();i++) {
                packageDefinitions.Add(pkgsDefs.getItemByIndex(i));
            }
            return packageDefinitions;
        }

        private Item GetClosestRelatedItemType(Item item) {
            Item relatedItem = GetPrimaryRelatedItem(item);
            // "Recurse" until we find an ItemType
            while (!relatedItem.isError() && relatedItem.getType() != "ItemType") {
                relatedItem = GetPrimaryRelatedItem(relatedItem);
            }
            if (!relatedItem.isError() && relatedItem.getProperty("is_relationship") == "1") {
                string aml1 = $@"<AML>
                    <Item action='get' type='RelationshipType'>
                        <name>{relatedItem.getProperty("name")}</name>
                    </Item></AML>";
                relatedItem = Inn.applyAML(aml1);
                relatedItem = GetPrimaryRelatedItem(relatedItem);
            }
            return relatedItem;
        }

        private Item GetPrimaryRelatedItem(Item item) {
            Item resultItem = null;
            string errorMessage;
            string amlQuery = $@"<AML>
                    <Item action='getItemWhereUsed' type='{item.getType()}' id='{item.getID()}'></Item>
                </AML>";
            Item result = Inn.applyAML(amlQuery);
            if (!result.isError()) {
                // Get related ItemType of type ItemType primarily otherwise return first
                Item whereUsedItems = result.getItemsByXPath("//relatedItems/Item");
                for (int i = 0; i < whereUsedItems.getItemCount(); i++) {
                    Item tempItem = whereUsedItems.getItemByIndex(i);
                    if (resultItem == null) resultItem = tempItem; // 1st if not ItemType is found.
                    if (tempItem.getType() == "ItemType") {
                        resultItem = tempItem; // Go with the ItemType
                        break;
                    }
                }
            }
            if (resultItem == null) {
                errorMessage = $"No primary related Item found for {item.TypeAndName()}";
                Log.LogWarning(errorMessage);
                //TODO: Check if it is referenced in Method Code (js)
                resultItem = Inn.newError(errorMessage);
                return resultItem;
            }
            return Inn.getItemById(resultItem.getType(), resultItem.getID());
        }

        private Item GetPackageDefinition(Item relatedItemType) {
            string errroMessage;
            // Find package element
            string amlQuery = $@"<AML>
                <Item action='get' type='PackageElement' select='id'>
                    <element_id>{relatedItemType.getProperty("config_id")}</element_id>
                </Item></AML>";
            Item packageElement = Inn.applyAML(amlQuery);
            if (packageElement.isError()) {
                errroMessage = $"No found Package Element for: {relatedItemType.getType()} , {relatedItemType.getProperty("config_id")} ";
                Log.LogWarning(errroMessage);
                return Inn.newError(errroMessage);
            }
            // Find Package Definition containing the package element 
            return GetPackageDefinitionByPackageElement(packageElement);
        }

        private Item GetPackageDefinitionByPackageElement(Item packageElement) {
            string amlQuery = $@"<AML><Item action='get' type='PackageGroup' select='source_id'>
                <Relationships>
                    <Item action='get' type='PackageElement' select='id'>
                        <id>{packageElement.getID()}</id>
                </Item></Relationships></Item></AML>";
            Item resultItem = Inn.applyAML(amlQuery);
            if (resultItem.isError()) return resultItem;
            Item pkgDef = resultItem.getPropertyItem("source_id");
            return Inn.getItemById(pkgDef.getType(), pkgDef.getID());
        }

        private Item AddItemToPackageDefinition(Item item, Item packageDefinition) {
            // Get PackageGoup
            string itemType = item.getType();
            string aml = $@"<AML><Item action='get' type='PackageGroup' select='id'>
                    <source_id>{packageDefinition.getID()}</source_id>
                    <name>{itemType}</name>
                </Item></AML>";
            Item packageGroup = Inn.applyAML(aml);
            if (packageGroup.isError()) {
                Log.Log($"Package group missing, adding it: {item.getType()}");
                packageGroup = AddPackageGroup(item.getType(), packageDefinition);
            }
            // Add new PackageElement
            Item pkgElement = Inn.newItem("PackageElement", "add");
            pkgElement.setProperty("element_id", item.getProperty("config_id"));
            pkgElement.setProperty("element_type", itemType);
            pkgElement.setProperty("name", item.getProperty("name", item.getID()));
            pkgElement.setProperty("source_id", packageGroup.getID());
            if (DryRun) return pkgElement;
            return pkgElement.apply();
        }

        private Item AddPackageGroup(string typeName, Item packageDefinition) {
            Item pkgGroup = Inn.newItem("PackageGroup", "add");
            pkgGroup.setID(Inn.getNewID());
            pkgGroup.setProperty("source_id", packageDefinition.getID());
            pkgGroup.setProperty("name", typeName);
            if (DryRun) return pkgGroup;
            pkgGroup = pkgGroup.apply();
            return pkgGroup;
        }

        private void AddToCache(Item packageElement, Item packageDefinition) {
            if (_cachedPackageElementIdToPackageDefinitionMap == null) _cachedPackageElementIdToPackageDefinitionMap = new Dictionary<string, Item>();

            if (!_cachedPackageElementIdToPackageDefinitionMap.ContainsKey(packageElement.getID())) {
                _cachedPackageElementIdToPackageDefinitionMap.Add(packageElement.getID(), packageDefinition);
            }
        }
    }
}
