using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Configuration {
    class ArasConnectionConfig : IArasConnectionConfig {
        public string ArasAddress { get; set; }
        public string ArasDBName { get; set; }
        public string ArasUser { get; set; }
        public string ArasPassword { get; set; }
    }
}
