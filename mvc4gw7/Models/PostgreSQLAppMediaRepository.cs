using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;

using Npgsql;
using NpgsqlTypes;
using mvc4gw7.Models;

namespace mvc4gw7.Models
{
    public class PostgreSQLAppMediaRepository:IAppMediaRepository
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PostgreSQL"].ConnectionString;

        public List<MediaCollection> GetMediaCollectionsDescriptions(string UserName, MediaType MediaType)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT * FROM dbo.""MediaCollections"" WHERE UserName = :UserName AND MediaType = :MediaType", connection);

            command.Parameters.Add(new NpgsqlParameter ("UserName", NpgsqlDbType.Varchar));
            command.Parameters[0].Value = UserName;

            command.Parameters.Add(new NpgsqlParameter("MediaType", NpgsqlDbType.Integer));
            command.Parameters[1].Value = (int)MediaType;

            NpgsqlCommand command2 = new NpgsqlCommand(@"SELECT * FROM dbo.""MediaElements"" WHERE CollectionId = :CollectionId", connection);

            connection.Open();
            NpgsqlDataReader reader = command.ExecuteReader();

            List<MediaCollection> mediacollections = new List<MediaCollection>();

            while (reader.Read())
            {
                MediaCollection mediacollection = new MediaCollection();
                mediacollection.Id = (int)reader[0];
                mediacollection.UserName = (string)reader[1];
                mediacollection.ShortTitle =(string)reader[2];
                mediacollection.Title = (string)reader[3];
                mediacollection.Description = (string)reader[4];
                mediacollection.Location = (string)reader[5];
                mediacollection.MediaType = MediaType;

                mediacollections.Add(mediacollection);
            }
            reader.Dispose();

            List<MediaElement> mediaelements = new List<MediaElement>();
            
            foreach (MediaCollection mediacollection in mediacollections)
            {
                command2.Parameters.Add(new NpgsqlParameter("CollectionId", NpgsqlDbType.Integer));
                command2.Parameters[0].Value = mediacollection.Id;
                NpgsqlDataReader reader2 = command2.ExecuteReader();

                while (reader2.Read())
                {
                    MediaElement mediaelement = new MediaElement();
                    mediaelement.Id = (int)reader2[0];
                    mediaelement.CollectionId = (int)reader2[1];
                    mediaelement.File = (string)reader2[2];

                    mediaelements.Add(mediaelement);
                }
                reader2.Dispose();

                mediacollection.MediaElements = mediaelements;

            }
            connection.Close();

            return mediacollections;
        }

        public MediaCollection GetMediaCollectionDescription(int CollectionId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT * FROM dbo.""MediaCollections"" WHERE Id = :CollectionId", connection);

            command.Parameters.Add(new NpgsqlParameter("CollectionId", NpgsqlDbType.Integer));
            command.Parameters[0].Value = CollectionId;

            NpgsqlCommand command2 = new NpgsqlCommand(@"SELECT * FROM dbo.""MediaElements"" WHERE CollectionId = :CollectionId", connection);

            connection.Open();
            NpgsqlDataReader reader = command.ExecuteReader();

            MediaCollection mediacollection = new MediaCollection();

            reader.Read();
            
            mediacollection.Id = (int)reader[0];
            mediacollection.UserName = (string)reader[1];
            mediacollection.ShortTitle = (string)reader[2];
            mediacollection.Title = (string)reader[3];
            mediacollection.Description = (string)reader[4];
            mediacollection.Location = (string)reader[5];

            Int16 mediatype = (Int16)reader[6];
            switch (mediatype)
            {
                case 0:
                    mediacollection.MediaType = MediaType.Photo;
                    break;
                case 1:
                    mediacollection.MediaType = MediaType.Graphic;
                    break;
                case 2:
                    mediacollection.MediaType = MediaType.Video;
                    break;
            }
            reader.Dispose();

            List<MediaElement> mediaelements = new List<MediaElement>();

            command2.Parameters.Add(new NpgsqlParameter("CollectionId", NpgsqlDbType.Integer));
            command2.Parameters[0].Value = mediacollection.Id;
            NpgsqlDataReader reader2 = command2.ExecuteReader();

            while (reader2.Read())
            {
                    MediaElement mediaelement = new MediaElement();
                    mediaelement.Id = (int)reader2[0];
                    mediaelement.CollectionId = (int)reader2[1];
                    mediaelement.File = (string)reader2[2];

                    mediaelements.Add(mediaelement);
            }
            reader2.Dispose();

            mediacollection.MediaElements = mediaelements;

            connection.Close();

            return mediacollection;
        }

        public List<MediaElement> GetMediaCollection(int CollectionId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT * FROM dbo.""MediaElements"" WHERE CollectionId = :CollectionId", connection);
            command.Parameters.Add(new NpgsqlParameter("CollectionId", NpgsqlDbType.Integer));
            command.Parameters[0].Value = CollectionId;

            List<MediaElement> mediaelements = new List<MediaElement>();

            connection.Open();            
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                MediaElement mediaelement = new MediaElement();
                mediaelement.Id = (int)reader[0];
                mediaelement.CollectionId = (int)reader[1];
                mediaelement.File = (string)reader[2];

                mediaelements.Add(mediaelement);
            }
            reader.Dispose();

            connection.Close();

            return mediaelements;

        }

        public List<VideoMediaElement> TransformToVideoMediaCollection(int CollectionId)
        {
            List<MediaElement> mediaCollection = GetMediaCollection(CollectionId);
            string mediaFolder = GetMediaCollectionDescription(CollectionId).Location;

            List<VideoMediaElement> videoMediaCollection = new List<VideoMediaElement>();
            foreach (MediaElement mediaElement in mediaCollection)
            {
                VideoMediaElement videoMediaElement = new VideoMediaElement();
                videoMediaElement.VideoFile = "/Content/" + mediaFolder + mediaElement.File;
                videoMediaElement.ImageFile = "/Content/" + mediaFolder + Path.GetFileNameWithoutExtension(mediaElement.File) + ".jpg";
                videoMediaCollection.Add(videoMediaElement);
            }

            return videoMediaCollection;
        }

        public void AddMediaCollectionDescription(MediaCollection MediaCollection)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO dbo.""MediaCollections"" (UserName, ShortTitle, Title, Description, Location, MediaType) VALUES (:UserName, :ShortTitle, :Title, :Description, :Location, :MediaType)", connection);
            command.Parameters.Add(new NpgsqlParameter("UserName", NpgsqlDbType.Varchar));
            command.Parameters[0].Value = MediaCollection.UserName;
            command.Parameters.Add(new NpgsqlParameter("ShortTitle", NpgsqlDbType.Varchar));
            command.Parameters[1].Value = MediaCollection.ShortTitle;
            command.Parameters.Add(new NpgsqlParameter("Title", NpgsqlDbType.Varchar));
            command.Parameters[2].Value = MediaCollection.Title;
            command.Parameters.Add(new NpgsqlParameter("Description", NpgsqlDbType.Varchar));
            command.Parameters[3].Value = MediaCollection.Description;
            command.Parameters.Add(new NpgsqlParameter("Location", NpgsqlDbType.Varchar));
            command.Parameters[4].Value = MediaCollection.Location;
            command.Parameters.Add(new NpgsqlParameter("MediaType", NpgsqlDbType.Smallint));
            command.Parameters[5].Value = (Int16)MediaCollection.MediaType;

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void AddMedia(MediaElement Media)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO dbo.""MediaElements"" (CollectionId, File) VALUES (:CollectionId, :File)", connection);

            command.Parameters.Add(new NpgsqlParameter("CollectionId", NpgsqlDbType.Integer));
            command.Parameters[0].Value = Media.CollectionId;
            command.Parameters.Add(new NpgsqlParameter("File", NpgsqlDbType.Varchar));
            command.Parameters[1].Value = Media.File;

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public int GetLastCollectionId()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT Id FROM dbo.""MediaCollections"" ORDER BY Id DESC LIMIT 1", connection);
            connection.Open();
            NpgsqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int lastId = (int)reader[0];
            reader.Dispose();
            connection.Close();

            return lastId;
        }

        public void EraseMediaCollection(int CollectionId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            
            NpgsqlCommand command = new NpgsqlCommand(@"
                    SELECT * FROM dbo.""MediaElements"" WHERE CollectionId = :CollectionId;
                    DELETE FROM dbo.""MediaElements"" WHERE CollectionId = :CollectionId;
                    ", connection);
            command.Parameters.Add(new NpgsqlParameter("CollectionId", NpgsqlDbType.Integer));
            command.Parameters[0].Value = CollectionId;
            NpgsqlCommand command3 = new NpgsqlCommand(@"DELETE FROM dbo.""MediaCollections"" WHERE Id = :CollectionId", connection);
            command3.Parameters.Add(new NpgsqlParameter("CollectionId", NpgsqlDbType.Integer));
            command3.Parameters[0].Value = CollectionId;

            connection.Open();
            
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string mediafile = AppDomain.CurrentDomain.BaseDirectory + "/Content/MembersFiles/" + (string)reader[2];
                if (File.Exists(mediafile)) File.Delete(mediafile);
            }
            reader.Dispose();

            command3.ExecuteNonQuery();
            connection.Close();
        }

        public List<MediaCollection> GetAllMediaCollectionsDescriptions()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT * FROM dbo.""MediaCollections""", connection);

            connection.Open();
            NpgsqlDataReader reader = command.ExecuteReader();

            List<MediaCollection> mediacollections = new List<MediaCollection>();

            while (reader.Read())
            {
                MediaCollection mediacollection = new MediaCollection();
                mediacollection.Id = (int)reader[0];
                mediacollection.UserName = (string)reader[1];
                mediacollection.ShortTitle = (string)reader[2];
                mediacollection.Title = (string)reader[3];
                mediacollection.Description = (string)reader[4];
                mediacollection.Location = (string)reader[5];

                Int16 mediatype = (Int16)reader[6];
                switch (mediatype)
                {
                    case 0:
                        mediacollection.MediaType = MediaType.Photo;
                        break;
                    case 1:
                        mediacollection.MediaType = MediaType.Graphic;
                        break;
                    case 2:
                        mediacollection.MediaType = MediaType.Video;
                        break;
                }
                
                mediacollections.Add(mediacollection);
            }
            reader.Dispose();
            connection.Close();

            return mediacollections;
        }
    }
}