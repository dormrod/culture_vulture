using System;
using System.Data;
using System.IO;
using System.Data.SQLite;
using Foundation;
using AppKit;
using System.Globalization;

namespace CultureVulture
{
	[Register("MediaModel")]
    public class MediaModel: NSObject
    {
        //Model for SQLite database entry

        #region Variables
        private string id;
        private string media;
		private string title;
		private string creator;
        private string language;
        private string date;
        private int rating;
        private string goodreadsId;
        private bool edited;
		private SQLiteConnection conn = null;
        #endregion

        #region Properties
        public SQLiteConnection Connection
		{
			get { return conn; }
			set { conn = value; }
		}

        [Export("ID")]
        public string ID
        {
            get { return id; }
            set
            {
                WillChangeValue("ID");
                id = value;
                DidChangeValue("ID");
            }
        }
        
		[Export("Media")]
        public string Media
        {
            get { return media; }
            set
            {
                WillChangeValue("Media");
                media = value;
                DidChangeValue("Media");
                if (conn != null) Update(conn);
            }
        }
		
		[Export("Title")]
        public string Title
        {
            get { return title; }
            set
            {
                WillChangeValue("Title");
                title = value;
                DidChangeValue("Title");
                if (conn != null) Update(conn);
            }
        }
		
		[Export("Creator")]
        public string Creator
        {
            get { return creator; }
            set
            {
                WillChangeValue("Creator");
                creator = value;
                DidChangeValue("Creator");
                if (conn != null) Update(conn);
            }
        }
		
		[Export("Language")]
        public string Language
        {
            get { return language; }
            set
            {
                WillChangeValue("Language");
                language = value;
                DidChangeValue("Language");
                if (conn != null) Update(conn);
            }
        }
		
		[Export("Date")]
        public string Date
        {
            get { return date; }
            set
            {
                WillChangeValue("Date");
                date = value;
                DidChangeValue("Date");
                if (conn != null) Update(conn);
            }
        }
		
		[Export("Rating")]
        public int Rating
        {
            get { return rating; }
            set
            {
                WillChangeValue("Rating");
                rating = value;
                DidChangeValue("Rating");
                if (conn != null) Update(conn);
            }
        }
		
		[Export("GoodreadsID")]
        public string GoodreadsID
        {
            get { return goodreadsId; }
            set
            {
                WillChangeValue("GoodreadsID");
                goodreadsId = value;
                DidChangeValue("GoodreadsID");
                if (conn != null) Update(conn);
            }
        }
		
		[Export("Edited")]
        public bool Edited
        {
            get { return edited; }
            set
            {
                WillChangeValue("Edited");
                edited = value;
                DidChangeValue("Edited");
                if (conn != null) Update(conn);
            }
        }
        #endregion

        #region Constructors
        public MediaModel(){}

        public MediaModel(string mediaIn, string titleIn, string creatorIn, string languageIn, string dateIn, int ratingIn, string goodreadsIdIn)
        {
            //Initialise
            media = mediaIn;
            title = titleIn;
            creator = creatorIn;
            language = languageIn;
            date = dateIn;
            rating = ratingIn;
            goodreadsId = goodreadsIdIn;
		}

        public MediaModel(SQLiteConnection conn, string id)
        {
            // Load from database
            Load(conn, id);
        }
        #endregion

        #region SQLite Routines
        public void Create(SQLiteConnection connection)
        {
            //Add record to SQLite table

            // Clear last connection to prevent circular call to update
            conn = null;

            // Update parameters
            ID = Guid.NewGuid().ToString();
            edited = true;
	     
            // Execute query
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Create new command
                command.CommandText = "INSERT INTO media(id, media, title, creator, language, date, rating, edited, goodreadsId) VALUES(@id, @media, @title, @creator, @language, @date, @rating, @edited, @goodreadsId)";

                // Populate with data from the record
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@media", media);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@creator", creator);
                command.Parameters.AddWithValue("@language", language);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@rating", rating);
                command.Parameters.AddWithValue("@edited", edited);
                command.Parameters.AddWithValue("@goodreadsId", goodreadsId);

                // Write to database
                command.ExecuteNonQuery();
            }
            connection.Close();

            // Save last connection
            conn = connection;
        }

        public void Update(SQLiteConnection connection)
        {
            //Update record in SQLite table

            // Clear last connection to prevent circular call to update
            conn = null;
            
			//Update parameters
			edited = true;

            // Execute query
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Create new command
                command.CommandText = "UPDATE INTO media(id, media, title, creator, language, date, rating, edited, goodreadsId) VALUES(@id, @media, @title, @creator, @language, @date, @rating, @edited, @goodreadsId)";

                // Populate with data from the record
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@media", media);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@creator", creator);
                command.Parameters.AddWithValue("@language", language);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@rating", rating);
                command.Parameters.AddWithValue("@edited", edited);
                command.Parameters.AddWithValue("@goodreadsId", goodreadsId);

                // Write to database
                command.ExecuteNonQuery();
            }
            connection.Close();

            // Save last connection
            conn = connection;
        }

        public void Load(SQLiteConnection connection, string id)
        {
            //Load record from SQLite table
            
			bool shouldClose = false;

            // Clear last connection to prevent circular call to update
            conn = null;

            // Check if database already open
            //if (connection.State != ConnectionState.Open)
            //{
            //    shouldClose = true;
            //    connection.Open();
            //}

            // Execute query
            using (var command = connection.CreateCommand())
            {
                // Create new command
                command.CommandText = "SELECT * FROM media WHERE id=@id";

                // Populate with data from the record
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Retrieve record
                        id = (string)reader[0];
                        media = (string)reader[1];
                        title = (string)reader[2];
                        creator = (string)reader[3];
                        language = (string)reader[4];
                        date = (string)reader[5];
                        rating = (int)reader[6];
                        edited = (bool)reader[7];
                        goodreadsId = (string)reader[8];
                    }
                }
            }

            // Should we close the connection to the database
            if (shouldClose) connection.Close();

            // Save last connection
            conn = connection;
        }

        public void Delete(SQLiteConnection connection)
        {
            //Delete record from SQLite table

            // Clear last connection to prevent circular call to update
            conn = null;

            // Execute query
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Create new command
                command.CommandText = "DELETE FROM media WHERE ID = @id";

                // Populate with data from the record
                command.Parameters.AddWithValue("@id", id);

                // Write to database
                command.ExecuteNonQuery();
            }
            connection.Close();

            // Save last connection
            conn = connection;
        }
        #endregion
    }
}

