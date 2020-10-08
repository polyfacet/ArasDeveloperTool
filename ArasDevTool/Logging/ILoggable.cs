using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Logging {
    interface ILoggable {
        ILogger Logger { set; }
    }
}
