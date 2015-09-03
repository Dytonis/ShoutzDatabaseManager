using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoutzDatabaseManager_AdministratorData
{
    public class AdministratorData
    {
        public static class Adminstrators
        {
            public static volatile List<User> Table = new List<User>()
            {
                new User("Steve.Wright", "sandst0ne"),
                new User("Tanner.Wright", "sandst0ne")
            };

            public static User GetUserByUsername(string Username)
            {
                foreach(User u in Table)
                {
                    if(u.GetUsername().ToUpper() == Username.ToUpper())
                    {
                        return u;
                    }
                }

                throw new KeyNotFoundException("The username does not excist");
            }
            public static bool CheckPasswordToUser(User U, string Password)
            {
                if(U.GetPassword() == Password)
                    return true;
                return false;
            }
        }
    }

    public class User
    {
        private string username = "";
        private string password = "";

        public User(string Username, string Password)
        {
            username = Username;
            password = Password;
        }

        public string GetPassword() //encrypt here in the future
        {
            return this.password;
        }

        public string GetUsername()
        {
            return this.username;
        }
    }
}
