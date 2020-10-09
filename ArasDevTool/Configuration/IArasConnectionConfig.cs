using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Configuration {
    interface IArasConnectionConfig {

        string ArasAddress { get; set; }
        string ArasDBName { get; set; }
        string ArasUser { get; set; }
        string ArasPassword { get; set; }
    }
}
