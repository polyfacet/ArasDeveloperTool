﻿
using System;


namespace Hille.Aras.DevTool.Interfaces.Logging;
public class ConsoleLogger : ILogger {

    public void Log(string message) {
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void LogError(string message) {
        Console.ForegroundColor = ConsoleColor.Red;
        Log("ERROR: " + message);
    }

    public void LogError(Exception ex) {
        LogError(ex.ToString());
    }

    public void LogSuccess(string message) {
        Console.ForegroundColor = ConsoleColor.Green;
        Log("SUCCESS: " + message);
    }

    public void LogWarning(string message) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Log("WARNING: " + message);
    }
}
