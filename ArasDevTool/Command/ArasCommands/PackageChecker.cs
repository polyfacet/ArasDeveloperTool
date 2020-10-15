using Aras.IOM;
using ArasDatabaseRepair.Resources;
using ArasDevTool.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Command.ArasCommands {
    class PackageChecker : ArasBaseCommand {

        private Dictionary<string, Item> _packageMap;
        public bool DryRun { get; set; }
        public bool AutoPack { get; set; }
        private string Prefix = "HC_";
        public override void DoRun() {
            // TODO: Sätt som parameter och interactive mode
            DryRun = true; //DEBUG mode

            Log.Log($"Executing {Name}");
            Item result = Inn.applyAML(GetMetaAml());
            Log.Log($"Meta data count: {result.getItemCount()}");
            LoadPackageMap(Inn);
            Log.Log($"Package Elements total count: {_packageMap.Count}");

            int itemsCount = 0;
            int unpackagedItemsCount = 0;
            int itemsAddedToPackageCount = 0;
            for (int i = 0; i < result.getItemCount(); i++) {
                Item item = result.getItemByIndex(i);
                string keyedName = item.getPropertyAttribute("id", "keyed_name");
                if (keyedName.ToUpper().StartsWith(Prefix)) {
                    itemsCount++;
                    if (!CheckIfInPackage(item)) {
                        Log.Log($"Not in any package: {item.getType()} named {keyedName}");
                        unpackagedItemsCount++;
                        var pkgMgr = new Aras.PackageManagment.PackageManager(Inn)
                        {
                            DryRun = DryRun,
                            AutoPack = AutoPack
                        };
                        ((ILoggable)pkgMgr).Logger = Log;
                        Item packageElement = pkgMgr.FindAndAddSuitablePackageForItem(item);
                        if (!packageElement.isError()) {
                            string packageDefName = pkgMgr.GetPackageDefinitionNameFromPackageElement(packageElement);
                            Log.LogSuccess($"Added to package {packageDefName} as {packageElement.getProperty("element_type")} ");
                            itemsAddedToPackageCount++;
                        }
                        else {
                            Log.LogWarning("Could not find a suitable package");
                        }
                        

                    }
                }
            }
            Log.Log($"Items checked: {itemsCount}");
            Log.Log($"Items not in any package: {unpackagedItemsCount}");
            Log.Log($"Items added to package: {itemsAddedToPackageCount}");
            //Log.Log($"Duration:{(int)stopwatch.Elapsed.TotalSeconds} seconds");
            
        }

        public override List<string> GetHelp() {
            List<string> helpMessages = new List<string>
            {
                "Flags: -prefix",
                @"Check Items with specific prefix. Example: ""-prefix HC_""",
                "Dont commit any changes: --DryRun ", 
                "Let it automatically select package for you: --Auto"
            };
            return helpMessages;
        }

        public override string GetName() {
            return "PackageChecker";
        }

        public override bool GetValidateInput(List<string> inputArgs) {
            Prefix = GetValueForFlag("-prefix", inputArgs);
            if (String.IsNullOrEmpty(Prefix)) return false;
            if (!String.IsNullOrEmpty(inputArgs.SingleOrDefault(s => s.ToUpper() == "--DRYRUN")) )  {
                DryRun = true;
            }
            else {
                DryRun = false;
            }
            if (!String.IsNullOrEmpty(inputArgs.SingleOrDefault(s => s.ToUpper() == "--AUTO"))) {
                AutoPack = true;
            }
            else {
                AutoPack = false;
            }
            return true;
        }

        private void LoadPackageMap(Innovator inn) {
            _packageMap = new Dictionary<string, Item>();
            string amlQueryAllPackageElements = @"<AML>
                <Item action='get' type='PackageElement'/>
            </AML>";
            Item packageElements = inn.applyAML(amlQueryAllPackageElements);
            for (int i = 0; i < packageElements.getItemCount(); i++) {
                Item packageElement = packageElements.getItemByIndex(i);
                string elementId = packageElement.getProperty("element_id");
                if (String.IsNullOrEmpty(elementId)) {
                    Log.LogError("Missing element_id : " + packageElement.dom.OuterXml);
                }
                else {
                    if (!_packageMap.ContainsKey(elementId)) {
                        _packageMap.Add(elementId, packageElement);
                    }
                    else {
                        Log.LogError("Already in other package element, element_id: " + elementId);
                    }
                }
            }
        }

        private bool CheckIfInPackage(Item item) {
            if (_packageMap.ContainsKey(item.getProperty("config_id"))) {
                return true;
            }
            return false;
        }

        private string GetMetaAml() {
            // TODO: R11/R12
            return ArasMetaDataResources.GetArasMetaDataAml(ArasMetaDataResources.ArasVersion.R11);
        }

    }
}
