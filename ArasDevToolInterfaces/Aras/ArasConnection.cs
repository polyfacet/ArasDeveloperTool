using Hille.Aras.DevTool.Interfaces.Logging;
using Innovator.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Xml;

namespace Hille.Aras.DevTool.Interfaces.Aras {
    public class ArasConnection {
        private readonly string Url;
        private readonly string DB;
        private readonly string User;
        private readonly string Password;
		private readonly int? TimeOutMilliSecs = null;
		private const string CONNECTION_NAME = "ArasDevTool";
		private int NumberOfLoginRetries = 3;
		private int SecondsToWaitBeforeRetryLogin = 10;

        private Innovator.Client.IOM.Innovator Inn;

        public ArasConnection(string address, string db, string user, string password) {
            Url = address;
            this.DB = db;
            this.User = user;
            this.Password = password;
		}

		public ArasConnection(string address, string db, string user, string password,int timeoutMillisec) : this(address,db,user,password) {
			TimeOutMilliSecs = timeoutMillisec;
        }


        public Innovator.Client.IOM.Innovator GetInnovator() {
            if (this.Inn == null) {
				Login();
            }
            return Inn;
        }

        public override string ToString() {
            return $"Address: {Url} DB: {DB} User: {User}";
        }

        public static string[] GetDBList(string url) {
			if (!url.EndsWith("/")) url += "/";
			url += "Server/DBList.aspx";
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "POST";
			string[] array = new string[0];
			
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			string str = "<?xml version='1.0' encoding='utf-8' ?>";
			byte[] bytes = uTF8Encoding.GetBytes(str + "<GetDB/>");
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentLength = (long)bytes.Length;
			httpWebRequest.ContentType = "text/xml";
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			Stream requestStream = httpWebRequest.GetRequestStream();
			try {
				requestStream.Write(bytes, 0, bytes.Length);
			}
			finally {
				requestStream.Close();
			}

			StreamReader streamReader = null;
			string xml = "";
			HttpWebResponse httpWebResponse;
			httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				
			if (httpWebRequest.HaveResponse) {
				Stream responseStream = httpWebResponse.GetResponseStream();
				try {
					streamReader = new StreamReader(responseStream, uTF8Encoding);
					xml = streamReader.ReadToEnd();
				}
				finally {
					if (streamReader != null) {
						streamReader.Close();
					}
					responseStream.Close();
					httpWebResponse.Close();
				}
				
				// Parse Xml-response
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xml);
				XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//DB");
				if (xmlNodeList.Count > 0) {
					int num = 0;
					array = new string[xmlNodeList.Count];
					foreach(XmlNode xmlNode in xmlNodeList) {
						array[num++] = xmlNode.Attributes["id"].Value;
					}
				}
			}
			return array;
        }

		private void Login() {
			IRemoteConnection connection = SetupConnection();
			int loginTries = 0;
			retryLogin:
			try {
				loginTries++;
                connection.Login(new ExplicitCredentials(DB, User, Password));
            }
			catch (Exception ex) {
				if (loginTries >= NumberOfLoginRetries) {
					ex.Data.Add("Login Failed", $"Failed to to login after trying {loginTries} times ");
					throw;
				}
				var logger = new ConsoleLogger();
				logger.LogWarning($"({DateTime.Now}) Failed to login. Try number {loginTries} of {NumberOfLoginRetries}. Trying again in {SecondsToWaitBeforeRetryLogin} seconds");
				Thread.Sleep(SecondsToWaitBeforeRetryLogin*1000);
				goto retryLogin;
			}
            Inn = new Innovator.Client.IOM.Innovator(connection);
        }

		private IRemoteConnection SetupConnection() {
			IRemoteConnection conn;
            if (TimeOutMilliSecs == null) {
                conn = Innovator.Client.Factory.GetConnection(Url, CONNECTION_NAME);
            }
            else {
                ConnectionPreferences connectionPreferences = new ConnectionPreferences();
                connectionPreferences.Url = Url;
                connectionPreferences.Name = CONNECTION_NAME;
                connectionPreferences.DefaultTimeout = TimeOutMilliSecs;
                conn = Innovator.Client.Factory.GetConnection(connectionPreferences);
            }
			return conn;
        }
    }

}
