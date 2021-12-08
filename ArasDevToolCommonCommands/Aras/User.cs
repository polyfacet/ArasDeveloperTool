using Innovator.Client.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hille.Aras.DevTool.Common.Commands.Aras {
    class User {
        readonly Innovator.Client.IOM.Innovator Inn;

        private readonly List<string> ExplictAdminOrSystemUserNames = new List<string>() {
            "root",
            "admin",
            "vadmin",
            "esadmin",
            "authadmin"
        };

        public User(Item user) {
            Inn = user.getInnovator();
            LoginName = user.getProperty("login_name");
            LastLogin = user.getProperty("last_login_date");
            KeyedName = user.getProperty("keyed_name");
            IdentityId = user.getProperty("owned_by_id");
        }

        public string LoginName { get; }
        public string LastLogin { get; }
        public string KeyedName { get; }

        public string IdentityId { get; }

        public bool IsAdmin {
            get {
                if (IsExplicitAdminOrSystemUser()) return true;
                //UGLY: This only checks on level. And is not "cached"
                string aml = $@"<AML>
                <Item action='get' type='Member' select='source_id'>
                    <related_id>{IdentityId}</related_id>
                </Item></AML>";
                Item memberItems = Inn.applyAML(aml);
                for (int i = 0; i < memberItems.getItemCount(); i++) {
                    if (memberItems.getItemByIndex(i).getPropertyAttribute("source_id", "keyed_name", string.Empty) == "Administrators") {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool IsExplicitAdminOrSystemUser() {
            if (ExplictAdminOrSystemUserNames.Contains(LoginName, StringComparer.InvariantCultureIgnoreCase)) return true;
            return false;
        }

        public static List<User> LatestLoggedInUsers(Innovator.Client.IOM.Innovator inn) {
            var userList = new List<User>();
            string aml = @"<AML>
                <Item action='get' type='User' orderBy='last_login_date DESC'>
                </Item></AML>";
            Item users = inn.applyAML(aml);
            for (int i = 0; i < users.getItemCount(); i++) {
                userList.Add(new User(users.getItemByIndex(i)));
            }
            return userList;
        }

    }
}
