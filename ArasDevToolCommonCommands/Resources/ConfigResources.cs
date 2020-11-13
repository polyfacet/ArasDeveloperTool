using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Hille.Aras.DevTool.Common.Configuration.Resources {
    class ConfigResources {

        private const string RESOURCE_FILE_NAME = "aras-env.config";

        public static string GetDefaultArasConfigEnvXml(){
            string resourceFileName = RESOURCE_FILE_NAME;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(resourceFileName,StringComparison.OrdinalIgnoreCase));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream)){
                    string result = reader.ReadToEnd();
                    return result;
                }
        }
    }
}
