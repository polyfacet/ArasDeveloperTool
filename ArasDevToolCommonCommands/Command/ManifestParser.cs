using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Hille.Aras.DevTool.Common.Commands.Command {
    class ManifestParser {

        private readonly string _filePath;
        private readonly string _envParameter;
        
        private System.Xml.XmlDocument XMLDoc { get; set; }
        private List<AMLFile> AMLFilesToExecuteField;
        

        public ManifestParser(string manifestFile) : this(manifestFile,string.Empty) {}

        public ManifestParser(string manifestFile, string envParameter) {
            _filePath = manifestFile;
            _envParameter = envParameter;
        }

        public List<AMLFile> AMLFilesToExecute {
            get {
                if (AMLFilesToExecuteField == null) {
                    LoadFileList();
                }
                return AMLFilesToExecuteField;
            }
            set {
                AMLFilesToExecuteField = value;
            }
        }


        private void LoadFileList() {
            AMLFilesToExecuteField = new List<AMLFile>();
            if (XMLDoc == null) {
                XMLDoc = new XmlDocument();
                XMLDoc.Load(_filePath);
            }

            string basePath;
            basePath = Path.GetDirectoryName(_filePath);


            XmlNodeList packageElements = XMLDoc.GetElementsByTagName("package");
            foreach (XmlNode packageElement in packageElements) {
                if (packageElement is XmlElement) {
                    XmlElement element = (XmlElement)packageElement;
                    XmlAttribute xmlAttr = element.GetAttributeNode("path");

                    // Add additional options
                    XmlAttribute stopOnErrorAttr = element.GetAttributeNode("stopOnError");
                    bool stopOnError = false;
                    if (stopOnErrorAttr != null && stopOnErrorAttr.Value.Equals("TRUE",StringComparison.CurrentCultureIgnoreCase))
                        stopOnError = true;

                    if (xmlAttr != null) {
                        string dirPath = xmlAttr.Value;
                        dirPath = Path.Combine(basePath, dirPath);
                        if (Directory.Exists(dirPath)) {
                            string[] files = Directory.GetFiles(dirPath);
                            List<string> fileList = new List<string>();
                            fileList.AddRange(files);
                            // Sort alfabeticaly
                            fileList.Sort();

                            foreach (string file in fileList) {
                                AMLFile amlFile = new AMLFile(file)
                                {
                                    StopOnError = stopOnError
                                };
                                AMLFilesToExecute.Add(amlFile);
                            }

                            // Environment specifics
                            if (!string.IsNullOrEmpty(_envParameter)) {
                                // Get env specific amls to run
                                string envDirPath = System.IO.Path.Combine(dirPath, @"env\" + _envParameter);
                                if (Directory.Exists(envDirPath)) {
                                    files = Directory.GetFiles(envDirPath);
                                    fileList = new List<string>();
                                    fileList.AddRange(files);
                                    // Sort alfabeticaly
                                    fileList.Sort();

                                    foreach (string file in fileList) {
                                        AMLFile amlFile = new AMLFile(file)
                                        {
                                            StopOnError = stopOnError
                                        };
                                        AMLFilesToExecute.Add(amlFile);
                                    }
                                }
                            }
                        }
                        else
                            Console.WriteLine("Directory:  " + dirPath + " does not exist");
                    }
                }
            }
        }
    }

    public class AMLFile {

        private List<string> _amls;

        public AMLFile(string filePath) {
            FilePath = filePath;
            StopOnError = false;
        }

        public string FilePath { get; }
        public bool StopOnError { get; set; }
        public IEnumerable<string> AMLs {
            get {
                if (_amls == null) {
                    LoadAmls();
                }
                return _amls;
            }
        }
            

        private void LoadAmls() {
            _amls = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            foreach(XmlNode amlNode in xmlDoc.SelectNodes("//AML")) {
                _amls.Add(amlNode.OuterXml);
            }
        }
    }
}
