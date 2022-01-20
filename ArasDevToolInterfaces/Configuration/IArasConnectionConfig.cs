namespace Hille.Aras.DevTool.Interfaces.Configuration {
    public interface IArasConnectionConfig {
        string Name { get; set; }
        string ArasAddress { get; set; }
        string ArasDBName { get; set; }
        string ArasUser { get; set; }
        string ArasPassword { get; set; }
        int TimeoutSeconds { get; set; }
    }
}
