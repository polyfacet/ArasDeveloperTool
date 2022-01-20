using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hille.Aras.DevTool.Interfaces.Configuration {
    public class ArasSetupConfig :  IArasSetupConfig {
        public ArasSetupConfig(IArasConnectionConfig connectionConfig) {
            Name = connectionConfig.Name;
            ArasAddress = connectionConfig.ArasAddress;
            ArasDBName = connectionConfig.ArasDBName;
            ArasUser = connectionConfig.ArasUser;
            ArasPassword = connectionConfig.ArasPassword;
            TimeoutSeconds = connectionConfig.TimeoutSeconds;
        }

        public string SqlCmd { get; set; }
        public string SqlServer { get; set; }
        public string DatabaseName { get; set; }
        public string BackupDir { get; set; }

        public string Name { get ; set; }
        public string ArasAddress { get; set; }
        public string ArasDBName { get; set; }
        public string ArasUser { get; set; }
        public string ArasPassword { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
