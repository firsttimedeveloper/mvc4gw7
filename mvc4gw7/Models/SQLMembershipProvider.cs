using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Configuration;
using System.Data.Entity;

using System.Configuration;  


using System.Data;  
using System.Data.SqlClient;  

using System.Text;  

using System.Configuration.Provider;
using System.Security.Cryptography;

using mvc4gw7.Models;

namespace mvc4gw7.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
//            : base("DefaultConnection")
            : base("name=WebHosting")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserInRole> UsersInRoles { get; set; }
        public DbSet<UserProfile> UsersProfiles { get; set; }
    }

    public class MembershipInitializer :DropCreateDatabaseAlways <UsersContext>
    {
        protected override void Seed(UsersContext context)
        {
            List<User> users = new List<User>() 
            {
            new User {UserName="admin", Password = "admin", CreationDate = DateTime.Now, IsApproved = true},
            new User {UserName="other1", Password = "other1", CreationDate = DateTime.Now, IsApproved = true},
            new User {UserName="other2", Password = "other2", CreationDate = DateTime.Now, IsApproved = true},
            };
            users.ForEach(x => context.Users.Add(x));
            context.SaveChanges();

            List<Role> roles = new List<Role>()
            {
            new Role{Id=1, Name="Administrator"},
            new Role{Id=2, Name="Member"}
            };
            roles.ForEach(x => context.Roles.Add(x));
            context.SaveChanges();

            List<UserInRole> usersInRoles = new List<UserInRole>()
            {
            new UserInRole{Id=1, UserName="admin", RoleId=1},
            new UserInRole{Id=2, UserName="other1", RoleId=2},
            new UserInRole{Id=3, UserName="other2", RoleId=2},
            };
            usersInRoles.ForEach(x => context.UsersInRoles.Add(x));
            context.SaveChanges();

            List<UserProfile> usersProfiles = new List<UserProfile>()
            {
                new UserProfile {UserName="admin", Surname="adminSurname", Name= "adminName", Patronymic="adminPatronymic", Email="adminEmail", User= context.Users.Find("admin")},
                new UserProfile {UserName="other1", Surname="other1Surname", Name= "other1Name", Patronymic="other1Patronymic", Email="other1Email", User=context.Users.Find("other1")},
                new UserProfile {UserName="other2", Surname="other2Surname", Name= "other2Name", Patronymic="other2Patronymic", Email="other2Email", User= context.Users.Find("other2")}
            };
            usersProfiles.ForEach(x => context.UsersProfiles.Add(x));
            context.SaveChanges();
        }
    }

          
    public class SQLMembershipProvider: MembershipProvider
    {        
        private MembershipUser CreateMembershipUserFromUser (User user)
        {
            using (UsersContext db = new UsersContext())
            {
                UserProfile userProfile = db.UsersProfiles.Find(user.UserName);
                MembershipUser membershipUser = new MembershipUser("MyMembershipProvider", user.UserName, user.UserName, (userProfile==null ? null:userProfile.Email), null, null, user.IsApproved, false, user.CreationDate, DateTime.Now, DateTime.Now, user.CreationDate, DateTime.MinValue);
                return membershipUser;
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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            using (UsersContext db = new UsersContext())
            {
                User user = db.Users.Find(username);
                if (user != null && user.Password == oldPassword)
                {
                    user.Password = newPassword;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }            
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            using (UsersContext db = new UsersContext())
            {
                try
                {
                    if (db.Users.Find(username) == null)
                    {
                        User user = new User();
                        user.UserName = username;
                        user.Password = password;
                        user.CreationDate = DateTime.Now;
                        user.IsApproved = false;
                        db.Users.Add(user);
                        db.SaveChanges();
                        MembershipUser membershipUser = CreateMembershipUserFromUser(user);
                        status = MembershipCreateStatus.Success;
                        return membershipUser;
                    }
                    else
                    {
                        status = MembershipCreateStatus.DuplicateUserName;
                        return null;
                    }
                }
                catch
                {
                    status = MembershipCreateStatus.ProviderError;
                    return null;
                }
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            using (UsersContext db = new UsersContext())
            {
                bool result = false;
                try
                {
                    User user = db.Users.Find(username);
                    db.Users.Remove(user);
                    db.SaveChanges();
                    result = true;
                }
                catch
                {
                }
                return result;
            }
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection collection = new MembershipUserCollection();
            using (UsersContext db = new UsersContext())
            {
                List<User> users = db.Users.ToList();
                foreach (User user in users)
                    collection.Add(CreateMembershipUserFromUser(user));
                totalRecords = users.Count();
                return collection;
            }

        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            using (UsersContext db = new UsersContext())
            {
                return db.Users.Find(username).Password;
            }
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (UsersContext db = new UsersContext())
            {

                    User user = db.Users.Find(username);
                    if (user!=null)
                    {
                        MembershipUser membershipUser = CreateMembershipUserFromUser(user);
                    return membershipUser;
                    }                
                else
                {
                    return null;
                }

            }

        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            using (UsersContext db = new UsersContext())
            {
                User user = db.Users.Find(userName);
                if (user == null)
                {
                    return false;
                }
                else
                {
                    user.IsApproved = true;
                    db.SaveChanges();
                    return true;
                }
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            using (UsersContext db = new UsersContext())
            {
                    User user = db.Users.Find(username);

                    if (user.IsApproved && user!=null && user.Password==password)
                        isValid = true;
            }
            
            return isValid;

        }
    }
}