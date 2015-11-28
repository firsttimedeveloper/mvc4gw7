using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Security;
using System.Web.Profile;
using System.Configuration;

namespace mvc4gw7.Models
{
    public class SQLProfileProvider: ProfileProvider
    {
        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(string[] usernames)
        {
            using (UsersContext db = new UsersContext())
            {
                List<UserProfile> profiles = new List<UserProfile>();
                int i;                
                for (i = 0; i < usernames.Count(); i++)
                {
                    string userName=usernames[i];
                    profiles.Add(db.UsersProfiles.Find(userName));
                }
                db.UsersProfiles.RemoveRange(profiles);
                db.SaveChanges();
                return i;
            }

        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
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

        public override System.Configuration.SettingsPropertyValueCollection GetPropertyValues(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyCollection collection)
        {
            using (UsersContext db = new UsersContext())
            {
                SettingsPropertyValueCollection settings = new SettingsPropertyValueCollection();

                string userName = context["UserName"].ToString();
                UserProfile userProfile = db.UsersProfiles.Find(userName);
                
                if (userProfile == null)
                {
                    foreach (SettingsProperty profileProperty in collection)
                    {
                        SettingsPropertyValue value = new SettingsPropertyValue(collection[profileProperty.Name]);
                        value.PropertyValue = null;
                        settings.Add(value);
                    }
                }
                
                else
                
                {

                    foreach (SettingsProperty profileProperty in collection)
                    {
                        SettingsPropertyValue value = new SettingsPropertyValue(collection[profileProperty.Name]);
                        value.PropertyValue = userProfile.GetType().GetProperty(profileProperty.Name).GetValue(userProfile, null);
                        settings.Add(value);
                    }
                }

                return settings;
            }
            
        }

        public override void SetPropertyValues(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyValueCollection collection)
        {
            using (UsersContext db = new UsersContext())
            {
                string userName = context["UserName"].ToString();                
                UserProfile userProfile = db.UsersProfiles.Find(userName);
                if (userProfile == null)
                {
                    userProfile = new UserProfile();

                    foreach (SettingsPropertyValue profilePropertyValue in collection)
                    {
                        userProfile.GetType().GetProperty(profilePropertyValue.Name).SetValue(userProfile, profilePropertyValue.PropertyValue, null);
                    }

                    db.UsersProfiles.Add(userProfile);
                }

                else
                {
                    foreach (SettingsPropertyValue profilePropertyValue in collection)
                    {
                        userProfile.GetType().GetProperty(profilePropertyValue.Name).SetValue(userProfile, profilePropertyValue.PropertyValue, null);
                    }
                
                }

                db.SaveChanges();
            }

        }
    }
}