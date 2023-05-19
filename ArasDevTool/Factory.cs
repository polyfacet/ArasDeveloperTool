using ArasDevTool.Command.Commands;
using Hille.Aras.DevTool.Interfaces.Logging;
using Hille.Aras.DevTool.Interfaces.Command;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.IO;

namespace ArasDevTool {
    class Factory {
        public static List<ICommand> Implementations = new List<ICommand>();

        public static ICommand GetCommand(string commandName) {
            LoadCommands();
            commandName = commandName.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            List<ICommand> commands = Implementations.FindAll(c => c.Name.StartsWith(commandName, StringComparison.OrdinalIgnoreCase));
            if (commands.Count == 1)return commands[0];
            return new UnknownCommand();
        }

        public static ILogger GetLogger() {
            return new BasicLogger();
        }

        private static void LoadCommands() {
            // Load "all" assemblies (and already loaded)
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();
            var referencedPaths = GetReferenecedDllFiles();
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            // Get Implementations of ICommand
            var type = typeof(ICommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));
            foreach(Type tempType in types) {
                if (tempType.IsClass && !tempType.IsAbstract) {
                    ICommand cmd = (ICommand)Activator.CreateInstance(tempType);
                    Implementations.Add(cmd);
                }
            }
        }

        private static List<string> GetReferenecedDllFiles() {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var referencedPaths = Directory.GetFiles(baseDir, "Hille*.dll");
            var list = referencedPaths.ToList<string>();
            if (Directory.Exists(Path.Combine(baseDir, "plugins"))){
                var customPluginsPaths = Directory.GetFiles(Path.Combine(baseDir, "plugins"), "*.dll");
                list.AddRange(customPluginsPaths.ToList<string>());
            }
            return list;
        }

    }
}
