using System;

namespace Hille.Aras.DevTool.Interfaces.Logging {
    public interface ILogger {
        void Log(string message);
        void LogSuccess(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogError(Exception ex);
    }
}
