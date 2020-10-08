using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Logging {
    interface ILogger {
        void Log(string message);
        void LogSuccess(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogError(Exception ex);
    }
}
