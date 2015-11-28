using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

using System.Xml;
using System.Web.Security;
using System.Web.Profile;
using System.Data.SqlClient;
using Npgsql;
using NpgsqlTypes;
using mvc4gw7.Models;

namespace mvc3gw5.Controllers
{
    [Authorize(Roles="Administrator")]
    public class CRUDController : Controller
    {
        readonly IAppMediaRepository repository;
        public CRUDController(IAppMediaRepository repository)
        {
            this.repository = repository;
        }

        //readonly string connectionString= @"Data Source=.\SQLEXPRESS; AttachDBFilename=D:\Maksim\Documents\ExampleSite\ASP.NET\Examples\mvc4\groundwork7(site)\mvc4gw7\App_Data\AppDB.mdf; Initial Catalog=AppDB; Integrated Security=true";
        readonly string connectionString = @"workstation id=AppMedia.mssql.somee.com;packet size=4096;user id=firsttime_SQLLogin_1;pwd=fe1zoshmsh;data source=AppMedia.mssql.somee.com;persist security info=False;initial catalog=AppMedia";            

        public ActionResult Index()
        {
            return View();
        }


        //TABLES LIST
        public ActionResult MediaCollectionsCRUD()
        {
            return View(repository.GetAllMediaCollectionsDescriptions().ToList());
        }

        public ActionResult EraseCollection(int CollectionId)
        {
            repository.EraseMediaCollection(CollectionId);
            return RedirectToAction("MediaCollectionsCRUD");
        }

        public ActionResult MembersCRUD()
        {
            PartialView("UserCRUD_partial_table", Membership.GetAllUsers());
            return View();
        }

        public ActionResult EraseMember(string username)
        {
            Roles.RemoveUserFromRole(username, "Member");
            ProfileManager.DeleteProfile(username);
            Membership.DeleteUser(username);
            return RedirectToAction("MembersCRUD");
        }

        [HttpGet]
        public ActionResult UpdateMember(string username)
        {
            RegisterModel updateModel = new RegisterModel();
            
            ProfileBase profile = ProfileBase.Create(username);

            updateModel.UserName = profile.GetPropertyValue("UserName").ToString();
            updateModel.Surname = profile.GetPropertyValue("Surname").ToString(); ;
            updateModel.Name = profile.GetPropertyValue("Name").ToString();
            updateModel.Patronymic=profile.GetPropertyValue("Patronymic").ToString();
            updateModel.Email=profile.GetPropertyValue("Email").ToString();

            MembershipUser user = Membership.GetUser(username);
            updateModel.Password = user.GetPassword(username);
            updateModel.ConfirmPassword = updateModel.Password;

            return View(updateModel);
        }

        [HttpPost]
        public ActionResult UpdateMember(RegisterModel updateModel)
        {
            ProfileBase profile = ProfileBase.Create(updateModel.UserName);

            profile.SetPropertyValue("Name", updateModel.Name);
            profile.SetPropertyValue("Surname", updateModel.Surname);
            profile.SetPropertyValue("Patronymic", updateModel.Patronymic);
            profile.SetPropertyValue("Email", updateModel.Email);
            profile.Save();

            MembershipUser user = Membership.GetUser(updateModel.UserName);
            user.ChangePassword(user.GetPassword(updateModel.UserName), updateModel.Password);

            return RedirectToAction("MembersCRUD");
        }



        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult CreateUser()
        //{
        //    return PartialView("UsersCRUD_partial_createUser");
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CreateUser(RegisterModel user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        MembershipCreateStatus status;
        //        Membership.CreateUser(username: user.UserName, password: user.Password, email: user.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, status: out status);
        //        if (status == MembershipCreateStatus.Success)
        //        {
        //            return PartialView("UsersCRUD_partial_table", Membership.GetAllUsers());
        //        }
        //        else
        //        {
        //            switch (status)
        //            {
        //                case MembershipCreateStatus.DuplicateUserName:
        //                    ModelState.AddModelError("UserName", "Пользователь с таким именем уже существует");
        //                    break;

        //                default:
        //                    ModelState.AddModelError("", "Неизвестная ошибка");
        //                    break;

        //            }
        //            return PartialView("UsersCRUD_partial_createUser", user);
        //        }
        //    }
        //    else
        //    {
        //        return PartialView("UsersCRUD_partial_createUser", user);
        //    }

        //}

        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult DeleteUser(string username)
        //{
        //    Membership.DeleteUser(username, false);
        //    return PartialView("UsersCRUD_partial_table", Membership.GetAllUsers());
        //}

        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult UpdateUser(string username)
        //{
        //    UpdateModel updateuser = new UpdateModel();
        //    MembershipUser membershipuser = Membership.GetUser(username, false);
        //    updateuser.UserId = (Guid)membershipuser.ProviderUserKey;
        //    updateuser.UserName = membershipuser.UserName;
        //    updateuser.Email = membershipuser.Email;
        //    return PartialView("UsersCRUD_partial_updateUser", updateuser);
        //}
        
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult UpdateUser(UpdateModel updateUser)
        //{
        //    return PartialView("UsersCRUD_partial_table", Membership.GetAllUsers());
        //}

        //public ActionResult MakeAdminAccount()
        //{
        //    List<User> users = new List<User>();

        //    using (DBMembershipContext db = new DBMembershipContext())
        //    {

        //        //User newAdminUser = new User();
        //        //newAdminUser = users.First(x => x.UserName == "admin");
        //        //newAdminUser.Password = Encoding.UTF8.GetBytes("admin");

        //        List <User> adminUsers = db.Users.Where(x=>x.UserName=="admin").ToList();
        //        db.Users.RemoveRange(adminUsers);
        //        db.SaveChanges();

        //        List<User> otherUsers = db.Users.Where(x => x.UserName == "other1" || x.UserName == "other2").ToList();
        //        db.Users.RemoveRange(otherUsers);
        //        db.SaveChanges();

        //        User admin = new User();
        //        admin = new User { UserId = Guid.NewGuid(), UserName = "admin", Email = "someemail", Password = Encoding.UTF8.GetBytes("admin"), PasswordSalt = 1234, CreatedDate = DateTime.Now, IsActivated = true, IsLockedOut = false, LastLockedOutDate = DateTime.Now, LastLoginDate = DateTime.Now };
        //        db.Users.Add(admin);
        //        db.SaveChanges();
                
        //        Guid adminGuid = db.Users.FirstOrDefault(x => x.UserName == "admin").UserId;
        //        UserRole adminRole1 = new UserRole();
        //        adminRole1.UserId = adminGuid;
        //        adminRole1.RoleId = db.Roles.FirstOrDefault(x => x.RoleName == "Administrator").RoleId;
        //        db.UserRoles.Add(adminRole1);
        //        db.SaveChanges();
        //        UserRole adminRole2 = new UserRole();
        //        adminRole2.UserId = adminGuid;
        //        adminRole2.RoleId = db.Roles.FirstOrDefault(x => x.RoleName == "Member").RoleId;
        //        db.UserRoles.Add(adminRole2);
        //        db.SaveChanges();


        //        User other1 = new User();
        //        other1 = new User { UserId = Guid.NewGuid(), UserName = "other1", Email = "someemail", Password = Encoding.UTF8.GetBytes("other1"), PasswordSalt = 1234, CreatedDate = DateTime.Now, IsActivated = true, IsLockedOut = false, LastLockedOutDate = DateTime.Now, LastLoginDate = DateTime.Now };
        //        db.Users.Add(other1);
        //        db.SaveChanges();

        //        Guid other1Guid = db.Users.FirstOrDefault(x => x.UserName == "other1").UserId;
        //        UserRole other1Role = new UserRole();
        //        other1Role.UserId = other1Guid;
        //        other1Role.RoleId = db.Roles.FirstOrDefault(x => x.RoleName == "Member").RoleId;
        //        db.UserRoles.Add(other1Role);
        //        db.SaveChanges();

        //        User other2 = new User();
        //        other2 = new User { UserId = Guid.NewGuid(), UserName = "other2", Email = "someemail", Password = Encoding.UTF8.GetBytes("other2"), PasswordSalt = 1234, CreatedDate = DateTime.Now, IsActivated = true, IsLockedOut = false, LastLockedOutDate = DateTime.Now, LastLoginDate = DateTime.Now };
        //        db.Users.Add(other2);
        //        db.SaveChanges();

        //        Guid other2Guid = db.Users.FirstOrDefault(x => x.UserName == "other2").UserId;
        //        UserRole other2Role = new UserRole();
        //        other2Role.UserId = other2Guid;
        //        other2Role.RoleId = db.Roles.FirstOrDefault(x => x.RoleName == "Member").RoleId;
        //        db.UserRoles.Add(other2Role);
        //        db.SaveChanges();

        //        //db.Users.Add(newAdminUser);
        //        //db.SaveChanges();
        //    }
            
        //    return RedirectToAction("UsersCRUD");
        //}


        public ActionResult DataFromXMLFiles()
        {
                    XmlDocument PhotoXmlFile = new XmlDocument();
                    PhotoXmlFile.Load(AppDomain.CurrentDomain.BaseDirectory + "/Content/PhotoGalleryDescription.xml");

                    foreach (XmlNode collection in PhotoXmlFile.DocumentElement.ChildNodes)
                    {

                        MediaCollection MediaCollection = new MediaCollection();
                        MediaCollection.UserName = "admin";
                        MediaCollection.ShortTitle = collection.Attributes["itemTitle"].Value.ToString();
                        MediaCollection.Title = "No title";
                        MediaCollection.Description = "No description";
                        MediaCollection.Location = "mobile/photo/" + collection.Attributes["itemFolder"].Value.ToString();
                        MediaCollection.MediaType = MediaType.Photo;
                        repository.AddMediaCollectionDescription(MediaCollection);

                        foreach (XmlNode collectionitem in collection.ChildNodes)
                        {
                            MediaElement Media = new MediaElement();
                            Media.CollectionId = repository.GetLastCollectionId();
                            Media.File = collectionitem.Attributes["fileName"].Value.ToString();
                            repository.AddMedia(Media);
                        }

                    }


                    XmlDocument GraphicsXmlFile = new XmlDocument();
                    GraphicsXmlFile.Load(AppDomain.CurrentDomain.BaseDirectory + "/Content/GraphicsGalleryDescription.xml");

                    foreach (XmlNode collection in GraphicsXmlFile.DocumentElement.ChildNodes)
                    {

                        MediaCollection MediaCollection = new MediaCollection();
                        MediaCollection.UserName = "admin";
                        MediaCollection.ShortTitle = collection.Attributes["itemTitle"].Value.ToString();
                        MediaCollection.Title = "No title";
                        MediaCollection.Description = "No description";
                        MediaCollection.Location = "mobile/graphics/" + collection.Attributes["itemFolder"].Value.ToString();
                        MediaCollection.MediaType = MediaType.Graphic;
                        repository.AddMediaCollectionDescription(MediaCollection);

                        foreach (XmlNode collectionitem in collection.ChildNodes)
                        {
                            MediaElement Media = new MediaElement();
                            Media.CollectionId = repository.GetLastCollectionId();
                            Media.File = collectionitem.Attributes["fileName"].Value.ToString();
                            repository.AddMedia(Media);
                        }

                    }

                    XmlDocument VideoXmlFile = new XmlDocument();
                    VideoXmlFile.Load(AppDomain.CurrentDomain.BaseDirectory + "/Content/VideoGalleryDescription.xml");

                    foreach (XmlNode collection in VideoXmlFile.DocumentElement.ChildNodes)
                    {

                        MediaCollection MediaCollection = new MediaCollection();
                        MediaCollection.UserName = "admin";
                        MediaCollection.ShortTitle = collection.Attributes["itemTitle"].Value.ToString();
                        MediaCollection.Title = "No title";
                        MediaCollection.Description = "No description";
                        MediaCollection.Location = "mobile/video/" + collection.Attributes["itemFolder"].Value.ToString();
                        MediaCollection.MediaType = MediaType.Video;
                        repository.AddMediaCollectionDescription(MediaCollection);

                        foreach (XmlNode collectionitem in collection.ChildNodes)
                        {
                            MediaElement Media = new MediaElement();
                            Media.CollectionId = repository.GetLastCollectionId();
                            Media.File = collectionitem.Attributes["fileName"].Value.ToString();
                            repository.AddMedia(Media);
                        }

                    }
                
            return RedirectToAction("Index");

        }

        public ActionResult DataFromXMLFilesSQL()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    
                    SqlCommand command0 = new SqlCommand(@"
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MediaElements') DROP TABLE [MediaElements];
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MediaCollections') DROP TABLE [MediaCollections];
    
                    CREATE TABLE [dbo].[MediaCollections](
	                    [Id] [int] IDENTITY (1,1),
	                    [UserName]  [nvarchar](20) COLLATE Cyrillic_General_CI_AS NOT NULL,
	                    [ShortTitle] [nvarchar](100) COLLATE Cyrillic_General_CI_AS NOT NULL,
    	                [Title] [nvarchar](200) COLLATE Cyrillic_General_CI_AS NOT NULL,
	                    [Description] [nvarchar](400) COLLATE Cyrillic_General_CI_AS NOT NULL,
	                    [Location] [nvarchar](50) COLLATE Cyrillic_General_CI_AS NULL,
	                    [MediaType] [int] NOT NULL,
                    CONSTRAINT [PK_MediaCollections] PRIMARY KEY CLUSTERED 
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                        ) ON [PRIMARY];

                    CREATE TABLE [dbo].[MediaElements](
	                    [Id] [int] IDENTITY (1,1),
	                    [CollectionId] [int] NOT NULL,
	                    [File] [nvarchar](40) COLLATE Cyrillic_General_CI_AS NOT NULL,
                    CONSTRAINT [PK_Medias] PRIMARY KEY CLUSTERED 
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                        ) ON [PRIMARY];

                    ALTER TABLE [dbo].[MediaElements]  
                    WITH CHECK ADD  CONSTRAINT [FK_MediaElements_MediaCollections] FOREIGN KEY([CollectionId])
                    REFERENCES [dbo].[MediaCollections] ([Id]);
                ", connection);
                    command0.ExecuteNonQuery();
                  
                    SqlCommand command1 = new SqlCommand(@"insert into MediaCollections (UserName, ShortTitle, Title, Description, Location, MediaType) values (@UserName, @ShortTitle, @Title, @Description, @Location, @MediaType)", connection);
                    SqlCommand command2 = new SqlCommand(@"insert into MediaElements ([CollectionId], [File]) values (@CollectionId, @File)", connection);

                    XmlDocument PhotoXmlFile = new XmlDocument();
                    PhotoXmlFile.Load(AppDomain.CurrentDomain.BaseDirectory + "/Content/PhotoGalleryDescription.xml");

                    foreach (XmlNode collection in PhotoXmlFile.DocumentElement.ChildNodes)
                    {
                        command1.Parameters.AddWithValue("UserName", "admin");
                        command1.Parameters.AddWithValue("ShortTitle", collection.Attributes["itemTitle"].Value.ToString());
                        command1.Parameters.AddWithValue("Title", "No title");
                        command1.Parameters.AddWithValue("Description", "No description");
                        command1.Parameters.AddWithValue("Location", "mobile/photo/" + collection.Attributes["itemFolder"].Value.ToString());
                        command1.Parameters.AddWithValue("MediaType", (int)MediaType.Photo);

                        command1.ExecuteNonQuery();
                        command1.Parameters.Clear();

                        foreach (XmlNode collectionitem in collection.ChildNodes)
                        {
                            command2.Parameters.AddWithValue("@CollectionId", repository.GetLastCollectionId());
                            command2.Parameters.AddWithValue("@File", collectionitem.Attributes["fileName"].Value);
                            command2.ExecuteNonQuery();
                            command2.Parameters.Clear();
                        }
                    }


                    XmlDocument GraphicsXmlFile = new XmlDocument();
                    GraphicsXmlFile.Load(AppDomain.CurrentDomain.BaseDirectory + "/Content/GraphicsGalleryDescription.xml");

                    foreach (XmlNode collection in GraphicsXmlFile.DocumentElement.ChildNodes)
                    {
                        command1.Parameters.AddWithValue("UserName", "admin");
                        command1.Parameters.AddWithValue("ShortTitle", collection.Attributes["itemTitle"].Value.ToString());
                        command1.Parameters.AddWithValue("Title", "No title");
                        command1.Parameters.AddWithValue("Description", "No description");
                        command1.Parameters.AddWithValue("Location", "mobile/graphics/" + collection.Attributes["itemFolder"].Value.ToString());
                        command1.Parameters.AddWithValue("MediaType", (int)MediaType.Graphic);

                        command1.ExecuteNonQuery();
                        command1.Parameters.Clear();

                        foreach (XmlNode collectionitem in collection.ChildNodes)
                        {
                            command2.Parameters.AddWithValue("CollectionId", repository.GetLastCollectionId());
                            command2.Parameters.AddWithValue("File", collectionitem.Attributes["fileName"].Value);
                            command2.ExecuteNonQuery();
                            command2.Parameters.Clear();
                        }
                    }

                    XmlDocument VideoXmlFile = new XmlDocument();
                    VideoXmlFile.Load(AppDomain.CurrentDomain.BaseDirectory + "/Content/VideoGalleryDescription.xml");

                    foreach (XmlNode collection in VideoXmlFile.DocumentElement.ChildNodes)
                    {
                        command1.Parameters.AddWithValue("UserName", "admin");
                        command1.Parameters.AddWithValue("ShortTitle", collection.Attributes["itemTitle"].Value.ToString());
                        command1.Parameters.AddWithValue("Title", "No title");
                        command1.Parameters.AddWithValue("Description", "No description");
                        command1.Parameters.AddWithValue("Location", "mobile/video/" + collection.Attributes["itemFolder"].Value.ToString());
                        command1.Parameters.AddWithValue("MediaType", (int)MediaType.Video);

                        command1.ExecuteNonQuery();
                        command1.Parameters.Clear();

                        foreach (XmlNode collectionitem in collection.ChildNodes)
                        {
                            command2.Parameters.AddWithValue("CollectionId", repository.GetLastCollectionId().ToString());
                            command2.Parameters.AddWithValue("File", collectionitem.Attributes["fileName"].Value.ToString());
                            command2.ExecuteNonQuery();
                            command2.Parameters.Clear();
                        }
                    }
                }

                //catch
                //{
                
                //}
             
                finally
                {
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }


        public ActionResult ClearTables()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                    delete from MediaElements;
                    delete from MediaCollections;
                    ", connection);
                    command.ExecuteNonQuery();
                }
                catch
                {
                }
                finally
                {
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }


        public ActionResult XMLFromData()
        {
            XmlDocument XMLfile = new XmlDocument();
            XmlDeclaration xml_declaration = XMLfile.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = XMLfile.DocumentElement;
            XMLfile.InsertBefore(xml_declaration, root);

            //XMLfile.Load(AppDomain.CurrentDomain.BaseDirectory + "/Content/XMLRepository.xml");
            XmlElement gallery = XMLfile.CreateElement("gallery");
            XMLfile.AppendChild(gallery);

            int i = 1;
            foreach (MembershipUser user in Membership.GetAllUsers())
            {
                foreach (MediaType mediaType in Enum.GetValues(typeof(MediaType)))
                {
                    foreach (MediaCollection mediaCollection in repository.GetMediaCollectionsDescriptions(user.UserName, mediaType))
                    {
                        XmlNode xmlMediaCollection = XMLfile.CreateElement("item" + i++);

                        XmlAttribute attr_userName = XMLfile.CreateAttribute("UserName");
                        attr_userName.Value = user.UserName;
                        xmlMediaCollection.Attributes.Append(attr_userName);

                        XmlAttribute attr_mediaType = XMLfile.CreateAttribute("MediaType");
                        int _mediaType = (int)mediaType;
                        attr_mediaType.Value = _mediaType.ToString();
                        xmlMediaCollection.Attributes.Append(attr_mediaType);

                        XmlAttribute attr_itemFolder = XMLfile.CreateAttribute("Location");
                        attr_itemFolder.Value = mediaCollection.Location;
                        xmlMediaCollection.Attributes.Append(attr_itemFolder);

                        XmlAttribute attr_itemShortTitle = XMLfile.CreateAttribute("ShortTitle");
                        attr_itemShortTitle.Value = mediaCollection.ShortTitle;
                        xmlMediaCollection.Attributes.Append(attr_itemShortTitle);

                        XmlAttribute attr_itemTitle = XMLfile.CreateAttribute("Title");
                        attr_itemTitle.Value = mediaCollection.Title;
                        xmlMediaCollection.Attributes.Append(attr_itemTitle);

                        XmlAttribute attr_itemDescription = XMLfile.CreateAttribute("Description");
                        attr_itemDescription.Value = mediaCollection.Description;
                        xmlMediaCollection.Attributes.Append(attr_itemDescription);

                        gallery.AppendChild(xmlMediaCollection);

                        foreach (MediaElement mediaElement in repository.GetMediaCollection(mediaCollection.Id))
                        {
                            XmlNode xmlMediaElement = XMLfile.CreateElement("item");

                            XmlAttribute fileName = XMLfile.CreateAttribute("fileName");
                            fileName.Value = mediaElement.File;
                            xmlMediaElement.Attributes.Append(fileName);

                            xmlMediaCollection.AppendChild(xmlMediaElement);

                        }

                    }

                }
            }

            XMLfile.Save(AppDomain.CurrentDomain.BaseDirectory + "/Content/XMLRepository.xml");
            return RedirectToAction("Index");
        }

        public ActionResult MicrosoftSQLDataFromXML()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "/Content/XMLRepository.xml");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command1 = new SqlCommand(@"
                        IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MediaElements') DROP TABLE [MediaElements];
                        IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MediaCollections') DROP TABLE [MediaCollections];
        
                        CREATE TABLE [dbo].[MediaCollections](
    	                    [Id] [int] IDENTITY (1,1),
	                        [UserName]  [nvarchar](20) COLLATE Cyrillic_General_CI_AS NOT NULL,
	                        [ShortTitle] [nvarchar](100) COLLATE Cyrillic_General_CI_AS NOT NULL,
    	                    [Title] [nvarchar](200) COLLATE Cyrillic_General_CI_AS NOT NULL,
	                        [Description] [nvarchar](400) COLLATE Cyrillic_General_CI_AS NOT NULL,
	                        [Location] [nvarchar](50) COLLATE Cyrillic_General_CI_AS NULL,
	                        [MediaType] [int] NOT NULL,
                        CONSTRAINT [PK_MediaCollections] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                            ) ON [PRIMARY];

                        CREATE TABLE [dbo].[MediaElements](
	                        [Id] [int] IDENTITY (1,1),
	                        [CollectionId] [int] NOT NULL,
	                        [File] [nvarchar](40) COLLATE Cyrillic_General_CI_AS NOT NULL,
                        CONSTRAINT [PK_Medias] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                            ) ON [PRIMARY];

                        ALTER TABLE [dbo].[MediaElements]  
                        WITH CHECK ADD  CONSTRAINT [FK_MediaElements_MediaCollections] FOREIGN KEY([CollectionId])
                        REFERENCES [dbo].[MediaCollections] ([Id]);
                    ", connection);
                    command1.ExecuteNonQuery();

                    SqlCommand command2 = new SqlCommand("INSERT INTO [MediaCollections] ([UserName], [ShortTitle], [Title], [Description], [Location], [MediaType]) VALUES (@UserName, @ShortTitle, @Title, @Description, @Location, @MediaType)", connection);
                    SqlCommand command3 = new SqlCommand("SELECT TOP (1) [Id] FROM [MediaCollections] ORDER BY [Id] DESC", connection);
                    SqlCommand command4 = new SqlCommand("INSERT INTO [MediaElements] ([CollectionId], [File]) VALUES (@CollectionId, @File)", connection);


                    foreach (XmlElement collection in xmlDocument.DocumentElement.ChildNodes)
                    {
                        command2.Parameters.AddWithValue("@UserName", collection.Attributes["UserName"].Value);
                        command2.Parameters.AddWithValue("@ShortTitle", collection.Attributes["ShortTitle"].Value);
                        command2.Parameters.AddWithValue("@Title", collection.Attributes["Title"].Value);
                        command2.Parameters.AddWithValue("@Description", collection.Attributes["Description"].Value);
                        command2.Parameters.AddWithValue("@Location", collection.Attributes["Location"].Value);
                        command2.Parameters.AddWithValue("@MediaType", collection.Attributes["MediaType"].Value);
                        command2.ExecuteNonQuery();
                        command2.Parameters.Clear();

                        foreach (XmlNode item in collection)
                        {
                            SqlDataReader reader = command3.ExecuteReader();
                            reader.Read();
                            command4.Parameters.AddWithValue("@CollectionId", reader[0]);
                            reader.Close();
                            command4.Parameters.AddWithValue("@File", item.Attributes["fileName"].Value);
                            command4.ExecuteNonQuery();
                            command4.Parameters.Clear();
                        }
                    }
                }//try

                finally
                {
                    connection.Close();
                }

            }// end using

            return RedirectToAction("Index");
        }

        public ActionResult PostgreSQLDataFromXML()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "/Content/XMLRepository.xml");

            using (NpgsqlConnection connection = new NpgsqlConnection (System.Configuration.ConfigurationManager.ConnectionStrings["PostgreSQL"].ConnectionString))
            {
                try
                {
                    connection.Open();

                    NpgsqlCommand command1 = new NpgsqlCommand(@"
                        DROP TABLE IF EXISTS dbo.""MediaElements"";
                        DROP TABLE IF EXISTS dbo.""MediaCollections"";
        
                        CREATE TABLE dbo.""MediaCollections"" (
                        	Id  SERIAL PRIMARY KEY,
                        	UserName  VARCHAR (20) NOT NULL,
                        	ShortTitle VARCHAR (100) NOT NULL,
                        	Title VARCHAR (200) NOT NULL,
	                        Description VARCHAR (400)  NULL,
                        	Location VARCHAR(50)  NULL,
	                        MediaType SMALLINT NOT NULL);

                        CREATE TABLE dbo.""MediaElements""(
	                        Id SERIAL PRIMARY KEY,
	                        CollectionId INTEGER NOT NULL,
	                        File VARCHAR(40) NOT NULL);                      
                    ", connection);
                    command1.ExecuteNonQuery();

                    NpgsqlCommand command2 = new NpgsqlCommand(@"INSERT INTO dbo.""MediaCollections"" (UserName, ShortTitle, Title, Description, Location, MediaType) VALUES (:UserName, :ShortTitle, :Title, :Description, :Location, :MediaType)", connection);
                    NpgsqlCommand command3 = new NpgsqlCommand(@"SELECT Id FROM dbo.""MediaCollections"" ORDER BY Id DESC LIMIT 1", connection);
                    NpgsqlCommand command4 = new NpgsqlCommand(@"INSERT INTO dbo.""MediaElements"" (CollectionId, File) VALUES (:CollectionId, :File)", connection);

                    foreach (XmlElement collection in xmlDocument.DocumentElement.ChildNodes)
                    {
                        command2.Parameters.Add(new NpgsqlParameter ("UserName", NpgsqlDbType.Varchar));
                        command2.Parameters[0].Value = collection.Attributes["UserName"].Value;
                        command2.Parameters.Add(new NpgsqlParameter ("ShortTitle", NpgsqlDbType.Varchar));
                        command2.Parameters[1].Value = collection.Attributes["ShortTitle"].Value;
                        command2.Parameters.Add(new NpgsqlParameter ("Title", NpgsqlDbType.Varchar)); 
                        command2.Parameters[2].Value = collection.Attributes["Title"].Value;
                        command2.Parameters.Add(new NpgsqlParameter ("Description", NpgsqlDbType.Varchar));
                        command2.Parameters[3].Value = collection.Attributes["Description"].Value;
                        command2.Parameters.Add(new NpgsqlParameter ("Location", NpgsqlDbType.Varchar));
                        command2.Parameters[4].Value = collection.Attributes["Location"].Value;
                        command2.Parameters.Add(new NpgsqlParameter ("MediaType", NpgsqlDbType.Smallint));
                        command2.Parameters[5].Value = collection.Attributes["MediaType"].Value;
                        command2.ExecuteNonQuery();
                        command2.Parameters.Clear();

                        foreach (XmlNode item in collection)
                        {
                            NpgsqlDataReader reader = command3.ExecuteReader();
                            reader.Read();
                            command4.Parameters.Add(new NpgsqlParameter("CollectionId", NpgsqlDbType.Integer));
                            command4.Parameters[0].Value = reader[0];
                            reader.Close();
                            command4.Parameters.Add(new NpgsqlParameter("File", NpgsqlDbType.Varchar));
                            command4.Parameters[1].Value = item.Attributes["fileName"].Value;
                            command4.ExecuteNonQuery();
                            command4.Parameters.Clear();
                        }
                    }
                }//try

                finally
                {
                    connection.Close();
                }

            }// end using

            return RedirectToAction("Index");
        }


    }
}

