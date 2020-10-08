using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Aras {
    public class ArasConnection {
        private readonly string Url;
        private readonly string DB;
        private readonly string User;
        private readonly string Password;

        private Innovator Inn;
        private HttpServerConnection ServerConnection;

        public ArasConnection(string address, string db, string user, string password) {
            Url = address;
            this.DB = db;
            this.User = user;
            this.Password = password;
        }

        public Innovator GetInnovator() {
            if (this.Inn == null)
                Inn = IomFactory.CreateInnovator(GetArasConnection);
            return Inn;
        }

        private HttpServerConnection GetArasConnection {
            get {
                if (ServerConnection == null) {
                    ServerConnection = IomFactory.CreateHttpServerConnection(Url, DB, User, Password);
                    Item login_result = ServerConnection.Login();
                    if (login_result.isError()) {
                        ServerConnection = null;
                        throw new Exception("Login failed:" + login_result.ToString());
                    }
                }
                return ServerConnection;
            }
        }

        public override string ToString() {
            return $"Address: {Url} DB: {DB} User: {User}";
        }
    }

}
