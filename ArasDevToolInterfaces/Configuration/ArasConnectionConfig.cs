
namespace Hille.Aras.DevTool.Interfaces.Configuration {
    public class ArasConnectionConfig : IArasConnectionConfig {
        public string ArasAddress { get; set; }
        public string ArasDBName { get; set; }
        public string ArasUser { get; set; }
        public string ArasPassword { get; set; }
        public string Name { get; set; }
    }
}
