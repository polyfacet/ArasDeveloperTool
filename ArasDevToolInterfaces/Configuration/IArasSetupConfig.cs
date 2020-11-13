using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hille.Aras.DevTool.Interfaces.Configuration {
    public interface IArasSetupConfig : IArasConnectionConfig {
        string SqlCmd { get; set; }
        string SqlServer { get; set; }
        string DatabaseName { get; set; }
        string BackupDir { get; set; }

    }
}
