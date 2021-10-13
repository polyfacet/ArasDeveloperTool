using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Hille.Aras.DevTool.Common.Commands.Aras.Resources
{
    class ArasMetaDataResources
    {
        public enum ArasVersion {
            R11 = 11,
            R12 = 12
        }

        private const string RESOURCE_FILE_NAME_R11 = "Metadata export 11.0.xml";
        private const string RESOURCE_FILE_NAME_R12 = "Metadata export 12.0.xml";

        public static string GetArasMetaDataAml(ArasVersion version){
            string resourceFileName;
            switch (version) {
                case ArasVersion.R11:
                    resourceFileName = RESOURCE_FILE_NAME_R11;
                    break;
                case ArasVersion.R12:
                    resourceFileName = RESOURCE_FILE_NAME_R12;
                    break;
                default:
                    resourceFileName = RESOURCE_FILE_NAME_R12;
                    break;
            }

            if (ExternalResourceFileExists(resourceFileName)) {
                return GetArasMetaDataAmlFromExternalResource(resourceFileName);
            }
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(resourceFileName,StringComparison.OrdinalIgnoreCase));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream)){
                    string result = reader.ReadToEnd();
                    return result;
                }
        }
                

        internal static string GetArasMetaDataAml(int majorVersion) {
            ArasVersion arasVersion = ArasVersion.R12;
            switch (majorVersion) {
                case 11:
                    arasVersion = ArasVersion.R11;
                    break;
                default:
                    break;
            }
            return GetArasMetaDataAml(arasVersion);
        }

        private static bool ExternalResourceFileExists(string resourceFileName) {
            string filePath = GetExternalResourceFilePath(resourceFileName);
            if (File.Exists(filePath)) return true;
            return false;
        }
       

        private static string GetArasMetaDataAmlFromExternalResource(string resourceFileName) {
            string filePath = GetExternalResourceFilePath(resourceFileName);
            using (StreamReader reader = new StreamReader(filePath)) {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        private static string GetExternalResourceFilePath(string resourceFileName) {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ExtResources\", resourceFileName);
        }



    }
}
