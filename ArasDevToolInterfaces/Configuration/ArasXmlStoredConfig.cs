using System;
using System.Xml;
using System.IO;
using Hille.Aras.DevTool.Interfaces;

namespace Hille.Aras.DevTool.Interfaces.Configuration {
    public class ArasXmlStoredConfig : IArasConnectionConfig {
        private const string ARAS_CONFIG_FILE = "aras-env.config";

        private XmlDocument XmlDoc;
        private XmlNode EnvNode;
        private string _environmentName;
        private string _backupDir = null;
        private string _dbName = null;
        private string _arasAddress;
        private string _arasUser;
        private string _arasPassword;
        public ArasXmlStoredConfig(string env) {
            _environmentName = env;
            XmlDoc = new XmlDocument();
            XmlDoc.Load(ARAS_CONFIG_FILE);
            EnvNode = XmlDoc.SelectSingleNode($"//Environment[@name='{env}']");
            if (EnvNode == null) throw new ApplicationException($"No env node with name {env}");
        }

        public ArasXmlStoredConfig() { }

        public string EnvName { get { return _environmentName; } }

        public string DBName {
            get {
                if (_dbName == null) {
                    _dbName = EnvNode.SelectSingleNode("//DatabaseName").InnerText;
                }
                return _dbName;
            }
        }

        public string BackupDir {
            get {
                if (_backupDir == null) {
                    _backupDir = EnvNode.SelectSingleNode("//DatabaseBackupDir").InnerText;
                }
                return _backupDir;
            }
        }

        
        public string ArasAddress { get { return EnvNode.SelectSingleNode("//Url").InnerText; } set { _arasAddress = value;  } }
        public string ArasDBName { get { return EnvNode.SelectSingleNode("//Db").InnerText; } set { _dbName = value; } }

        public string ArasUser { get { return EnvNode.SelectSingleNode("//User").InnerText; } set { _arasUser = value; } }

        public string ArasPassword { get { return EnvNode.SelectSingleNode("//Password").InnerText; } set { _arasPassword = value; } }
        public string WebAppPath { get { return EnvNode.SelectSingleNode("//WebAppPath").InnerText; } }
        public string ConsoleUpgradePath {
            get {
                if (XmlDoc.SelectSingleNode("//ConsoleUpgradePath") == null) return String.Empty;
                return XmlDoc.SelectSingleNode("//ConsoleUpgradePath").InnerText;
            }
        }

        public void Setup() {
            string configFileName = "aras-env.config";
            string defaultEnvName = "dev";
            string defaultArasAddress = "http://localhost/Innovator";
            string defaultDBName = "InnovatorSolutions";
            string storedDBName = string.Empty;
            string defaultArasUser = "root";
            string defaultArasPassword = "innovator";

            Console.WriteLine("Setup starting");
            if (!File.Exists(configFileName)) {
                File.Copy("aras-env.config.template", configFileName);
            }
            else {
                // Set existing value as default
                XmlDocument existingXml = new XmlDocument();
                existingXml.Load(configFileName);
                XmlNode envNode1 = existingXml.SelectSingleNode("//Environment");
                string existingEnv = envNode1.Attributes.GetNamedItem("name").Value;
                ArasXmlStoredConfig existingConfig = new ArasXmlStoredConfig(existingEnv);
                defaultArasAddress = existingConfig.ArasAddress;
                storedDBName = existingConfig.ArasDBName;
                defaultArasUser = existingConfig.ArasUser;
                defaultArasPassword = existingConfig.ArasPassword;
            }

            // Get User Input
            string env = GetValueFromUserInput($"Set name of environment ({defaultEnvName}):", defaultEnvName);
            string address = GetValueFromUserInput($"Set Aras url ({defaultArasAddress}):", defaultArasAddress);
            if (storedDBName == defaultDBName ) {
                Console.WriteLine("Getting databases");
                string[] dbs = Aras.ArasConnection.GetDBList(address);
                defaultDBName = (dbs.Length > 0) ? dbs[0] : defaultDBName;
            }
            else {
                defaultDBName = storedDBName;
            }
            string dbName = GetValueFromUserInput($"Set Aras DBName ({defaultDBName}):", defaultDBName);
            string arasUser = GetValueFromUserInput($"Set Aras user ({defaultArasUser}):", defaultArasUser);
            string arasPassword = GetValueFromUserInput($"Set Aras password ({defaultArasPassword}):", defaultArasPassword);

            // Update Xml file
            XmlDoc = new XmlDocument();
            XmlDoc.Load(configFileName);
            XmlNode firstEnvNode = XmlDoc.SelectSingleNode("//Environment");
            firstEnvNode.Attributes.GetNamedItem("name").Value = env;
            firstEnvNode.SelectSingleNode("//ArasConnection/Url").InnerText = address;
            firstEnvNode.SelectSingleNode("//ArasConnection/Db").InnerText = dbName;
            firstEnvNode.SelectSingleNode("//ArasConnection/User").InnerText = arasUser;
            firstEnvNode.SelectSingleNode("//ArasConnection/Password").InnerText = arasPassword;
            XmlDoc.Save(configFileName);
        }

        private string GetValueFromUserInput(string message, string defaultValue) {
            Console.WriteLine(message);
            string value = Console.ReadLine();
            if (String.IsNullOrEmpty(value)) value = defaultValue;
            return value;
        }
    }
}
