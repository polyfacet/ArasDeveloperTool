using ArasDevTool.Command.Commands;
using Hille.Aras.DevTool.Interfaces.Logging;
using System.Collections.Generic;
using Hille.Aras.DevTool.Interfaces.Command;
using System;
using System.Linq;
using System.Reflection;
using System.IO;
//using Hille.Aras.DevTool.Common.Commands.Command.ArasCommands;

namespace ArasDevTool {
    class Factory {
        public static Dictionary<string, ICommand> impl = new Dictionary<string, ICommand>();

        public static ICommand GetCommand(string commandName) {
            LoadCommands();
            commandName = commandName.ToLower();
            if (impl.ContainsKey(commandName)) {
                return impl[commandName];
            }
            else {
                return new UnknownCommand();
            }
        }

        public static ILogger GetLogger() {
            return new BasicLogger();
        }

        private static void LoadCommands() {
            // Load all assemblies
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            //var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
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
                    //Console.WriteLine(tempType.FullName);
                    ICommand cmd = (ICommand)Activator.CreateInstance(tempType);
                    impl.Add(cmd.Name.ToLower(), cmd);
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
