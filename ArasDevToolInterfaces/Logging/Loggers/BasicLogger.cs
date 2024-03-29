﻿using System;
using System.IO;

namespace Hille.Aras.DevTool.Interfaces.Logging;
public class BasicLogger : ConsoleLogger, ILogger {

    private enum MessageType {
        NORMAL,
        SUCCESS,
        WARNING,
        ERROR,
        FATAL
    }

    private bool LogToFileDisabled;
    public BasicLogger() {
        LogFilePath = $"logs/Log_{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
    }
    public string LogFilePath { get; set; }

    public new void Log(string message) {
        base.Log(message);
    }

    public new void LogError(string message) {
        base.LogError(message);
        LogToFile(message, MessageType.ERROR);
    }

    public new void LogError(Exception ex) {
        base.LogError(ex);
        LogToFile(ex.ToString(), MessageType.FATAL);
    }

    public new void LogSuccess(string message) {
        base.LogSuccess(message);
    }

    public new void LogWarning(string message) {
        base.LogWarning(message);
    }

    private void LogToFile(string message, MessageType messageType = MessageType.NORMAL) {
        if (!LogToFileDisabled && CanWriteToFile()) {
            try {
                message = $"[{DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss")}]  {message}";
                using (StreamWriter sw = new StreamWriter(LogFilePath, true)) {
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex) {
                base.LogError(ex);
            }
        }
    }

    private bool CanWriteToFile() {
        if (File.Exists(LogFilePath)) return true;
        try {
            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
            File.Create(LogFilePath).Close();
        }
        catch (Exception ex) {
            LogToFileDisabled = true;
            LogWarning("Could not create log file: " + LogFilePath);
            LogWarning(ex.Message);
            return false;
        }
        return true;
    }
}
