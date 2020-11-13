using Hille.Aras.DevTool.Interfaces.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Hille.Aras.DevTool.Common.Configuration.Resources;

namespace Hille.Aras.DevTool.Common.Commands {
    class DefaultSetupHandler : ISetupHandler {

        public enum Setting {
            Env,
            Address,
            ArasDBName,
            ArasUser,
            ArasPassword,
            DataBaseName,
            SqlCmd,
            SqlServer,
            DBBackupDir
        }


        private const string ARAS_CONFIG_FILE = "aras-env.config";
        private const string DEFAULT_ENV = "dev";
        private const string XPATH_URL = "ArasConnection/Url";
        private const string XPATH_ARAS_DB_NAME = "ArasConnection/Db";
        private const string XPATH_ARAS_USER = "ArasConnection/User";
        private const string XPATH_ARAS_PASSWORD = "ArasConnection/Password";
        private const string XPATH_DATABASE_NAME = "Database/DBName";
        private const string XPATH_SQLCMD = "Database/Sqlcmd";
        private const string XPATH_DB_SERVER = "Database/Server";
        private const string XPATH_BACKUP_DIR = "Database/BackupDirectory";
        private const string XPATH_ENV_NAME = "//Environment/Name";
        private XmlDocument XmlDoc;
        private XmlNode EnvNode;

        public IArasConnectionConfig GetArasConnectionConfig(string env) {
            IArasConnectionConfig config = new ArasConnectionConfig();
            if (String.IsNullOrEmpty(env)) {
                env = DEFAULT_ENV;
            }
            config.Name = env;
            XmlDoc = new XmlDocument();
            XmlDoc.Load(ARAS_CONFIG_FILE);
            EnvNode = GetEnvironmentNode(env);
            if (EnvNode == null) throw new ApplicationException($"Environment with name {env} does not exist");
            config.ArasAddress = EnvNode.SelectSingleNode(XPATH_URL).InnerText;
            config.ArasDBName = EnvNode.SelectSingleNode(XPATH_ARAS_DB_NAME).InnerText;
            config.ArasUser = EnvNode.SelectSingleNode(XPATH_ARAS_USER).InnerText;
            config.ArasPassword = EnvNode.SelectSingleNode(XPATH_ARAS_PASSWORD).InnerText;
            return config;
        }

        private XmlNode GetEnvironmentNode(string env) {
            foreach(XmlNode nameNode in XmlDoc.SelectNodes(XPATH_ENV_NAME)) {
                if (nameNode.InnerText.Equals(env,StringComparison.OrdinalIgnoreCase)) {
                    return nameNode.ParentNode;
                }
            }
            return null;
        }

        public IArasSetupConfig GetConfig(string env) {
            IArasConnectionConfig connectionConfig =  GetArasConnectionConfig(env);
            IArasSetupConfig config = new ArasSetupConfig(connectionConfig)
            {
                DatabaseName = EnvNode.SelectSingleNode(XPATH_DATABASE_NAME).InnerText,
                SqlServer = EnvNode.SelectSingleNode(XPATH_DB_SERVER).InnerText,
                SqlCmd = EnvNode.SelectSingleNode(XPATH_SQLCMD).InnerText,
                BackupDir = EnvNode.SelectSingleNode(XPATH_BACKUP_DIR).InnerText
            }; 
            return config;
        }

        public IArasSetupConfig Setup(string env) {
            IArasConnectionConfig connectionConfig = SetupConnection(env);
            IArasSetupConfig config = new ArasSetupConfig(connectionConfig);
            config.DatabaseName = EnvNode.SelectSingleNode(XPATH_DATABASE_NAME).InnerText;
            config.SqlServer = EnvNode.SelectSingleNode(XPATH_DB_SERVER).InnerText;
            config.SqlCmd = EnvNode.SelectSingleNode(XPATH_SQLCMD).InnerText;
            config.BackupDir = EnvNode.SelectSingleNode(XPATH_BACKUP_DIR).InnerText;

            string configValue;
            configValue = GetValueFromUserInput($"Set Database Name ({config.ArasDBName}):", config.ArasDBName);
            SaveToFile(EnvNode, Setting.DataBaseName, configValue);
            configValue = GetValueFromUserInput($"Set sqlcmd ({config.SqlCmd}):", config.SqlCmd);
            SaveToFile(EnvNode, Setting.SqlCmd, configValue);
            configValue = GetValueFromUserInput($"Set SQL Server ({config.SqlServer}):", config.SqlServer);
            SaveToFile(EnvNode, Setting.SqlServer, configValue);
            configValue = GetValueFromUserInput($"Set Backup Dir ({config.BackupDir}):", config.BackupDir);
            SaveToFile(EnvNode, Setting.DBBackupDir, configValue);
            config = this.GetConfig(env);
            return config;
        }

        public IArasConnectionConfig SetupConnection(string env) {
            
            if (String.IsNullOrEmpty(env)) {
                env = DEFAULT_ENV;
            }
            // Get User Input
            env = GetValueFromUserInput($"Set name of environment ({env}):", env);
            CreateConfigFileIfMissing();
            XmlDoc = new XmlDocument();
            XmlDoc.Load(ARAS_CONFIG_FILE);

            EnvNode = GetEnvironmentNode(env);
            if (EnvNode == null) {
                AddNewEnvNode(env);
                EnvNode = GetEnvironmentNode(env);
            }
            Console.WriteLine(EnvNode.SelectSingleNode("Name").InnerText);
            IArasConnectionConfig config = new DefaultSetupHandler().GetArasConnectionConfig(env);
            string address = GetValueFromUserInput($"Set Aras url ({config.ArasAddress}):", config.ArasAddress);
            SaveToFile(EnvNode, Setting.Address, address);
            string defaultDBName = GetDefaultDBName(config,address);
            string configValue;
            configValue = GetValueFromUserInput($"Set Aras DBName ({defaultDBName}):", defaultDBName);
            SaveToFile(EnvNode, Setting.ArasDBName, configValue);
            configValue = GetValueFromUserInput($"Set Aras user ({config.ArasUser}):", config.ArasUser);
            SaveToFile(EnvNode, Setting.ArasUser, configValue);
            configValue = GetValueFromUserInput($"Set Aras password ({config.ArasPassword}):", config.ArasPassword);
            SaveToFile(EnvNode, Setting.ArasPassword, configValue);

            return new DefaultSetupHandler().GetArasConnectionConfig(env);
        }

        private string GetDefaultDBName(IArasConnectionConfig config,string address) {
            Console.WriteLine("Getting databases");
            string[] dbs = Interfaces.Aras.ArasConnection.GetDBList(address);
            string defaultDBName = config.ArasDBName;
            string extistingDB = dbs.FirstOrDefault(s => s.Equals(defaultDBName, StringComparison.Ordinal));
            if (String.IsNullOrEmpty(extistingDB)) {
                defaultDBName = (dbs.Length > 0) ? dbs[0] : defaultDBName; // Get first db in list
            }
            return defaultDBName;
        }

        private void AddNewEnvNode(string env) {
            XmlNode newEnvironmentNode = GetNewEnvironmentNode(env);
            var configsNode = XmlDoc.SelectSingleNode("//Configs");
            XmlNode impNode = configsNode.OwnerDocument.ImportNode(newEnvironmentNode, true);
            configsNode.AppendChild(impNode);
            XmlDoc.Save(ARAS_CONFIG_FILE);
        }

        private void SaveToFile(XmlNode firstEnvNode, Setting setting, string value) {
            switch (setting) {
                case Setting.Env:
                    firstEnvNode.SelectSingleNode(XPATH_ENV_NAME).InnerText = value;
                    break;
                case Setting.Address:
                    firstEnvNode.SelectSingleNode(XPATH_URL).InnerText = value;
                    break;
                case Setting.ArasDBName:
                    firstEnvNode.SelectSingleNode(XPATH_ARAS_DB_NAME).InnerText = value;
                    break;
                case Setting.ArasUser:
                    firstEnvNode.SelectSingleNode(XPATH_ARAS_USER).InnerText = value;
                    break;
                case Setting.ArasPassword:
                    firstEnvNode.SelectSingleNode(XPATH_ARAS_PASSWORD).InnerText = value;
                    break;
                case Setting.DataBaseName:
                    firstEnvNode.SelectSingleNode(XPATH_DATABASE_NAME).InnerText = value;
                    break;
                case Setting.SqlCmd:
                    firstEnvNode.SelectSingleNode(XPATH_SQLCMD).InnerText = value;
                    break;
                case Setting.SqlServer:
                    firstEnvNode.SelectSingleNode(XPATH_DB_SERVER).InnerText = value;
                    break;
                case Setting.DBBackupDir:
                    firstEnvNode.SelectSingleNode(XPATH_BACKUP_DIR).InnerText = value;
                    break;
                default:
                    break;
            }
            XmlDoc.Save(ARAS_CONFIG_FILE);
        }

        private void CreateConfigFileIfMissing() {
            if (!File.Exists(ARAS_CONFIG_FILE)) {
                string xml = ConfigResources.GetDefaultArasConfigEnvXml();
                using (StreamWriter sw = new StreamWriter(ARAS_CONFIG_FILE, true)) {
                    sw.WriteLine(@"<?xml version=""1.0""?>");
                    sw.WriteLine("<Configs>");
                    sw.Write(xml);
                    sw.WriteLine("</Configs>");
                }
            }
        }

        private XmlNode GetNewEnvironmentNode(string newEnv) {
            string xmlContent = ConfigResources.GetDefaultArasConfigEnvXml();
            xmlContent = xmlContent.Replace(@"<Name>dev</Name>", $@"<Name>{newEnv}</Name>");
            XmlDocument partXmlDoc = new XmlDocument();
            partXmlDoc.LoadXml(xmlContent);
            return partXmlDoc.DocumentElement;
        }

        private string GetValueFromUserInput(string message, string defaultValue) {
            Console.WriteLine(message);
            string value = Console.ReadLine();
            if (String.IsNullOrEmpty(value)) value = defaultValue;
            return value;
        }

    }
}
