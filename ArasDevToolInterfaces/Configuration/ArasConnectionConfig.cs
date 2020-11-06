
namespace Hille.Aras.DevTool.Interfaces.Configuration {
    public class ArasConnectionConfig : IArasConnectionConfig {
        public string ArasAddress { get; set; }
        public string ArasDBName { get; set; }
        public string ArasUser { get; set; }
        public string ArasPassword { get; set; }
        public string SqlCmd { get; set; }
        public string SqlServer { get; set; }
        public string DatabaseName { get; set; }
        public string BackupDir { get; set; }
    }
}
