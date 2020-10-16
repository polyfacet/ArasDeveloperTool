﻿namespace Hille.Aras.DevTool.Interfaces.Configuration {
    public interface IArasConnectionConfig {

        string ArasAddress { get; set; }
        string ArasDBName { get; set; }
        string ArasUser { get; set; }
        string ArasPassword { get; set; }
    }
}
