using Innovator.Client.IOM;
using Hille.Aras.DevTool.Interfaces.Logging;
using System;
using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;
using Hille.Aras.DevTool.Common.Commands.Aras.Resources;
using Hille.Aras.DevTool.Common.Commands.Aras;

namespace Hille.Aras.DevTool.Common.Commands.Command.ArasCommands {
    public class PackageChecker : ArasBaseCommand {

        private Dictionary<string, Item> _packageMap;
        public bool DryRun { get; set; }
        public bool AutoPack { get; set; }
        private string Prefix = "HC_";
        public override void DoRun() {
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
                if (keyedName.StartsWith(Prefix,StringComparison.OrdinalIgnoreCase)) {
                    itemsCount++;
                    if (!CheckIfInPackage(item)) {
                        Log.Log($"Not in any package: {item.getType()} named {keyedName}");
                        unpackagedItemsCount++;
                        var pkgMgr = new Hille.Aras.DevTool.Common.Commands.Aras.PackageManagment.PackageManager(Inn)
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
        }

        public override List<string> GetHelp() {
            List<string> helpMessages = new List<string>
            {
                "Parameters:",
                "  -prefix \t Check Items with specific prefix. Example: \"-prefix HC_\"" ,
                "Options:",
                "  --DryRun \t Dont commit any changes.  ", 
                "  --Auto \t Let it automatically select package for you."
            };
            return helpMessages;
        }

        public override string GetName() {
            return "PackageChecker";
        }

        public override bool GetValidateInput(List<string> inputArgs) {
            if (!CommandUtils.OptionExistWithValue(inputArgs, "-prefix", out Prefix)) return false;
            DryRun = (CommandUtils.HasOption(inputArgs, "--DRYRUN")) ? true : false;
            AutoPack = (CommandUtils.HasOption(inputArgs, "--AUTO")) ? true : false;
            return true;
        }

        private void LoadPackageMap(Innovator.Client.IOM.Innovator inn) {
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
            return ArasMetaDataResources.GetArasMetaDataAml(ArasUtils.GetMajorVersion(Inn));
        }

    }
}
