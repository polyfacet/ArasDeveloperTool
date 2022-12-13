using Hille.Aras.DevTool.Interfaces.Command;
using Innovator.Client.IOM;
using Innovator.Client.QueryModel.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Hille.Aras.DevTool.Common.Commands.Command.ArasCommands
{
    class FindItemsInAml : ArasBaseCommand
    {
        private string _fileName;
        public override void DoRun() {
            if (_fileName == null) { Log.LogError("No filename set."); return; }
            if (!System.IO.File.Exists(_fileName)) { Log.LogError($"File not found: {_fileName} "); return;           }
            FileInfo fi = new FileInfo(_fileName);
            Log.Log($"Parsing file: {fi.FullName}");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fi.FullName);
            XmlNodeList itemNodes = xmlDoc.GetElementsByTagName("Item");
            Log.Log($"{itemNodes.Count} number of Item nodes");
            
            foreach (XmlNode itemNode in itemNodes) {
                string itemType = string.Empty;
                string id = string.Empty;
                XmlNode typeAttribute = itemNode.Attributes.GetNamedItem("type");
                itemType = typeAttribute?.Value;
                XmlNode idAttribute = itemNode.Attributes.GetNamedItem("id");
                id = idAttribute?.Value;
                if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(itemType)) { 
                    string keyedName = string.Empty;

                    string aml = $@"<AML>
                            <Item action='get' select='keyed_name' type='{itemType}' id='{id}' />
                        </AML>";
                    Item result = Inn.applyAML(aml);
                    //Log.Log(result.dom.OuterXml);
                    if (result.isError()) {
                        keyedName = "Not Found:";
                        string message = $"{keyedName} for {itemType} with {id}";
                        Log.LogWarning(message);
                    }
                    else {
                        keyedName = result.getProperty("keyed_name");
                        string message = $"{keyedName} for {itemType} with {id}";
                        Log.LogSuccess(message);
                    }

                    

                }
                else {
                    Log.Log("No type or id attribute.");
                }

            }

            
        }

        public override List<string> GetHelp() {
            return new List<string>()
            {
                "Parses an 'AML-file' (xml) for <Item type='X' id='Y'> '",
                "Tries to find the items in the Aras system",
                "and prints their keyed_name if found",
                "or that it does not exist.",
                "",
                "Parameters:",
                "  -file \t File to parse. Example: \"-file MyAmls.xml\"" ,
                "  "
            };
        }

        public override string GetName() {
            return "FindItemsInAml";
        }

        public override bool GetValidateInput(List<string> inputArgs) {
            if (CommandUtils.OptionExistWithValue(inputArgs, "-file", out string fileName)) {
                _fileName = fileName;
                return true;
            }
            else {
                Log.LogError("Name of method not specified");
            }
            return false;
        }
    }
}
