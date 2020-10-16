using Innovator.Client;

namespace ArasDevTool.Aras {
    public class ArasConnection {
        private readonly string Url;
        private readonly string DB;
        private readonly string User;
        private readonly string Password;

        private Innovator.Client.IOM.Innovator Inn;

        public ArasConnection(string address, string db, string user, string password) {
            Url = address;
            this.DB = db;
            this.User = user;
            this.Password = password;
        }

        public Innovator.Client.IOM.Innovator GetInnovator() {
            if (this.Inn == null) {
                IRemoteConnection conn = Innovator.Client.Factory.GetConnection(Url, "ArasDevTool");
                conn.Login(new ExplicitCredentials(DB, User, Password));
                Inn = new Innovator.Client.IOM.Innovator(conn);
            }
            return Inn;
        }

        public override string ToString() {
            return $"Address: {Url} DB: {DB} User: {User}";
        }
    }

}
