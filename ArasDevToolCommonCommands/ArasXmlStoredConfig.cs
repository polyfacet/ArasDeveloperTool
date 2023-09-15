using System;
using System.Xml;
using System.IO;
using System.Linq;
using Hille.Aras.DevTool.Interfaces.Configuration;
using System.Xml.Linq;

namespace Hille.Aras.DevTool.Common.Configuration;
public class ArasXmlStoredConfig : IArasSetupConfig {
    private const string ARAS_CONFIG_FILE = "aras-env.config";

    private XmlDocument XmlDoc;
    private readonly XmlNode EnvNode;
    private readonly string _environmentName;
    
    private string _dbName;
    private string _arasAddress;
    private string _arasUser;
    private string _arasPassword;
    private string _sqlCmd;
    private string _sqlServer;
    private string _databaseName;
    private string _backupDir;

    public ArasXmlStoredConfig(string env) {
        _environmentName = env;
        XmlDoc = new XmlDocument();
        if (!File.Exists(ARAS_CONFIG_FILE)) {
            //File.Create(ARAS_CONFIG_FILE);
            string xml = Resources.ConfigResources.GetDefaultArasConfigEnvXml();
            using(StreamWriter sw = new StreamWriter(ARAS_CONFIG_FILE,true)) {
                sw.WriteLine(@"<?xml version=""1.0""?>");
                sw.WriteLine("<Configs>");
                sw.Write(xml);
                sw.WriteLine("</Configs>");
            }
        }
        XmlDoc.Load(ARAS_CONFIG_FILE);
        EnvNode = XmlDoc.SelectSingleNode($"//Environment[@name='{env}']");

        if (EnvNode == null) {
            string xmlContent = Resources.ConfigResources.GetDefaultArasConfigEnvXml();
            xmlContent = xmlContent.Replace(@"name=""dev""", $@"name=""{env}""");
            XmlDocument partXmlDoc = new XmlDocument();
            partXmlDoc.LoadXml(xmlContent);
            XmlNode newNode = partXmlDoc.DocumentElement;
            var configsNode = XmlDoc.SelectSingleNode("//Configs");
            configsNode.OwnerDocument.ImportNode(newNode,true);
            XmlDoc.Save(ARAS_CONFIG_FILE);
        }
    }

    public ArasXmlStoredConfig() { }

    public string EnvName { get { return _environmentName; } }

    
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

    public string SqlCmd { 
        get {
            if (!String.IsNullOrEmpty(_sqlCmd) ) {
                return _sqlCmd;
            }
            return EnvNode.SelectSingleNode("//Sqlcmd").InnerText; 
        } 
        set { _sqlCmd = value; } }
    public string SqlServer { get { return EnvNode.SelectSingleNode("//Server").InnerText; } set { _sqlServer = value; } }
        
    public string DatabaseName { get { return EnvNode.SelectSingleNode("//DBName").InnerText; } set { _databaseName = value; } }
    public string BackupDir { get { return EnvNode.SelectSingleNode("//BackupDirectory").InnerText; } set { _backupDir = value; } }

    public void Setup(bool extendedSetup) {
        string configFileName = "aras-env.config";
        string defaultEnvName = _environmentName;
        string defaultArasAddress = "http://localhost/Innovator";
        string defaultDBName = "InnovatorSolutions";
        string storedDBName = string.Empty;
        string defaultArasUser = "root";
        string defaultArasPassword = "innovator";
        string defaultDatabaseName = "InnovatorSolutions";
        string defaultDBServer = @".\SQLEXPRESS";
        string defaultBackupDir = Path.GetTempPath();
        string defaultSqlCmd = "sqlcmd.exe";


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
            defaultDatabaseName = existingConfig.DatabaseName;
            defaultDBServer = existingConfig.SqlServer;
            defaultSqlCmd = existingConfig.SqlCmd;
            defaultBackupDir = existingConfig.BackupDir;
        }

        // Get User Input
        string env = GetValueFromUserInput($"Set name of environment ({defaultEnvName}):", defaultEnvName);
        string address = GetValueFromUserInput($"Set Aras url ({defaultArasAddress}):", defaultArasAddress);
        if (storedDBName == defaultDBName ) {
            Console.WriteLine("Getting databases");
            string[] dbs = Interfaces.Aras.ArasConnection.GetDBList(address);
            defaultDBName = (dbs.Length > 0) ? dbs[0] : defaultDBName;
        }
        else {
            defaultDBName = storedDBName;
        }
        string dbName = GetValueFromUserInput($"Set Aras DBName ({defaultDBName}):", defaultDBName);
        string arasUser = GetValueFromUserInput($"Set Aras user ({defaultArasUser}):", defaultArasUser);
        string arasPassword = GetValueFromUserInput($"Set Aras password ({defaultArasPassword}):", defaultArasPassword);

        string sqlServerName = string.Empty;
        string databaseName = string.Empty;
        string backupDir = string.Empty;
        string sqlcmd = string.Empty;
        if (extendedSetup) {
            sqlServerName = GetValueFromUserInput($"Set SQL Server ({defaultDBServer}):", defaultDBServer);
            sqlcmd = GetValueFromUserInput($"Set sqlcmd.exe path ({defaultSqlCmd}):", defaultSqlCmd);
            databaseName = GetValueFromUserInput($"Set Database Name ({defaultDatabaseName}):", defaultDatabaseName);
            backupDir = GetValueFromUserInput($"Set Database backup dir ({defaultBackupDir}):", defaultBackupDir);
        }
        

        // Update Xml file
        XmlDoc = new XmlDocument();
        XmlDoc.Load(configFileName);
        XmlNode firstEnvNode = XmlDoc.SelectSingleNode("//Environment");
        firstEnvNode.Attributes.GetNamedItem("name").Value = env;
        firstEnvNode.SelectSingleNode("//ArasConnection/Url").InnerText = address;
        firstEnvNode.SelectSingleNode("//ArasConnection/Db").InnerText = dbName;
        firstEnvNode.SelectSingleNode("//ArasConnection/User").InnerText = arasUser;
        firstEnvNode.SelectSingleNode("//ArasConnection/Password").InnerText = arasPassword;
        if (extendedSetup) {
            firstEnvNode.SelectSingleNode("//Database/DBName").InnerText = databaseName;
            firstEnvNode.SelectSingleNode("//Database/Sqlcmd").InnerText = sqlcmd;
            _sqlCmd = sqlcmd;
            firstEnvNode.SelectSingleNode("//Database/Server").InnerText = sqlServerName;
            firstEnvNode.SelectSingleNode("//Database/BackupDirectory").InnerText = backupDir;
        }
        XmlDoc.Save(configFileName);
    }

    private string GetValueFromUserInput(string message, string defaultValue) {
        Console.WriteLine(message);
        string value = Console.ReadLine();
        if (String.IsNullOrEmpty(value)) value = defaultValue;
        return value;
    }
}
