using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Security;


namespace mvc4gw7.Models
{
    public class SQLRoleProvider:RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (UsersContext db = new UsersContext())
            {
                foreach (string username in usernames)
                {
                    foreach (string rolename in roleNames)
                    {
                        UserInRole userInRole = new UserInRole();
                        userInRole.UserName = (string) Membership.GetUser(username).ProviderUserKey;
                        userInRole.RoleId = db.Roles.FirstOrDefault(x => x.Name == rolename).Id;
                        db.UsersInRoles.Add(userInRole);
                        db.SaveChanges();
                    }

                }
            }
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            using (UsersContext db = new UsersContext())
            {
                User user = db.Users.Find(username);
                return user.UserInRoles.Select(x => x.Role).Select(y => y.Name).ToArray();
            }

        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (UsersContext db = new UsersContext())
            {
                List<UserInRole> usersInRoles = new List<UserInRole>();

                for (int i = 0; i < usernames.Count(); i++)
                {
                    string userName = usernames[i];
                    for (int j = 0; j < roleNames.Count(); j++)
                    {
                        string roleName = roleNames[j];
                        int roleId = (int) db.Roles.FirstOrDefault(x=>x.Name==roleName).Id;
                        UserInRole userInRole = db.UsersInRoles.FirstOrDefault(x => x.UserName == userName && x.RoleId == roleId);
                        usersInRoles.Add(userInRole);
                    }
                }

                db.UsersInRoles.RemoveRange(usersInRoles);
                db.SaveChanges();
            }

        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}