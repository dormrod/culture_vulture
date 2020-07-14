using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using AppKit;
using Foundation;

namespace CultureVulture
{
    public partial class ViewController : NSViewController
    {

        GoodreadsHandler goodreads;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.

            // Create the media table data source and populate it
            var DataSource = new MediaTableDataSource();
            MediaTable.DataSource = DataSource;
            MediaTable.Delegate = new MediaTableDelegate(this,DataSource);

            //Set up goodreads api
            goodreads = new GoodreadsHandler();
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
            }
        }

        partial void AddMediaClicked(NSObject sender)
        {
            //Add media to database from user form

            //Create new media record and add to local database
            string media = AddMediaMedia.TitleOfSelectedItem;
            string title = AddMediaTitle.StringValue;
            string creator = AddMediaCreator.StringValue;
            string language = AddMediaLanguage.StringValue;
            int rating = AddMediaRating.IntValue;
            var dateTime = DateTime.Parse(AddMediaDate.DateValue.ToString());
            string date = dateTime.ToString("dd-MM-yyyy");
            string completion = AddMediaCompletion.TitleOfSelectedItem;
            var record = new MediaModel(media,title,creator,language,date,rating,completion);
            record.Edited = true;
            var conn = GetDatabaseConnection();
            record.Create(conn);
            SearchMedia(); //refresh search
        }

        partial void SearchMediaClicked(NSObject sender)
        {
            //Search database for user query

            SearchMedia();
        }

        public void SearchMedia()
        {
            //Search database for user query

            //Open database connection 
            var conn = GetDatabaseConnection();
            conn.Open();
            var command = conn.CreateCommand();

            //Get search string
            string field = SearchMediaField.TitleOfSelectedItem;
            string search = SearchMediaSearch.StringValue;
            command.CommandText = string.Format("SELECT * FROM media WHERE {0} LIKE '{1}';", field, search);

            //Get IDs of matching results
            SQLiteDataReader reader = command.ExecuteReader();
            List<string> recordIDs = new List<string>();
            while (reader.Read())
            {
                string ID = reader.GetString(0);
                recordIDs.Add(ID);
            }
            conn.Close();

            //Get media records and update table
            var DataSource = new MediaTableDataSource();
            foreach (string ID in recordIDs)
            {
                var record = new MediaModel();
                record.Load(conn, ID);
                DataSource.MediaRecords.Add(record);
            }
            MediaTable.DataSource = DataSource;
            MediaTable.Delegate = new MediaTableDelegate(this, DataSource);
            MediaTable.ReloadData();
        }

        public void ReloadMediaTable()
        {
            MediaTable.ReloadData();
        }

        partial void UnlockResetClicked(NSObject sender)
        {
            //Make hard reset button visible

            if (UnlockReset.State.ToString() == "Off")
			{
                HardReset.Transparent = true;
			}
            else HardReset.Transparent = false;

        }

        partial void HardResetClicked(NSObject sender)
        {
            //Completely reset database

            if (UnlockReset.State.ToString() == "On")
			{
				var conn = GetDatabaseConnection();
				conn.Open();
				var command = conn.CreateCommand();
                command.CommandText = "DROP TABLE IF EXISTS media;";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE TABLE media(id TEXT PRIMARY KEY, media TEXT, title TEXT, creator TEXT, language TEXT, date TEXT, rating INTEGER, status TEXT, edited BIT, goodreadsBookId TEXT, goodreadsReviewId TEXT)";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE UNIQUE INDEX mtc on media(media, title, creator);";
                command.ExecuteNonQuery();
                conn.Close();
			}
        }

		partial void GoodreadsAuthClicked(NSObject sender)
        {
            //Generate auth link for goodreads
			
			var url = goodreads.GenerateAuthURL();
            GoodreadsAuthURL.Editable = true;
            GoodreadsAuthURL.StringValue = url;
        }

        partial void GoodreadsVerifyClicked(NSObject sender)
        {
            //Check goodreads auth successful

            var verified = goodreads.VerifyAuth();
            if(verified)
            {
                GoodreadsLoginName.StringValue = string.Format("Welcome {0}", goodreads.UserName);
            }
            else
            {
                GoodreadsLoginName.StringValue = string.Format("Could not log in");
            }
        }

        partial void GoodreadsSyncClicked(NSObject sender)
        {
            //Sync with goodreads account

            //Get sync options
            string syncType = GoodreadsSyncType.TitleOfSelectedItem;
            bool force;
            var conn = GetDatabaseConnection();
            if (ForceSyncOption.State.ToString() == "On") force = true;
            else force = false;

            //Pull from goodreads
            if (syncType == "Pull") 
			{
                //Pull completed books
                var pulledBooks = goodreads.Pull("read");
                foreach(MediaModel Book in pulledBooks)
                {
                    Book.Create(conn,force);
				}
               
                //Pull in progress books 
				pulledBooks = goodreads.Pull("currently-reading");
                foreach(MediaModel Book in pulledBooks)
                {
                    Book.Create(conn,force);
				}
                
				//Pull wish list books 
				pulledBooks = goodreads.Pull("to-read");
                foreach(MediaModel Book in pulledBooks)
                {
                    Book.Create(conn,force);
				}
			}
            //Push to goodreads
            else
            {
                //Push edited books to goodreads
                var editedBookIDs = new List<string>();
                var editedBooks = new List<MediaModel>();
                
				//Get IDs of edited books
                var command = conn.CreateCommand();
                command.CommandText = string.Format("SELECT * FROM media WHERE media='Book' AND edited=1;");
                conn.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string ID = reader.GetString(0);
                    editedBookIDs.Add(ID);
                }
                conn.Close();

                //Get edited books 
                foreach (string ID in editedBookIDs)
                {
                    var book = new MediaModel();
                    book.Load(conn, ID);
                    editedBooks.Add(book);
                }

				//Push media
                foreach (MediaModel book in editedBooks)
                {
                    //Add media without goodreads id
                    if(book.GoodreadsBookID=="X")
                    {
                        var res = goodreads.PushNew(book);
                        book.GoodreadsBookID = res.bookID;
                        book.GoodreadsReviewID = res.reviewID;
                        book.Edited = false;
					}
					//Add media with existing id
					else
                    {
                        goodreads.PushExisting(book);
                        book.Edited = false;
					}
				}                    
            }
            SearchMedia(); //refresh search
        }

        private SQLiteConnection GetDatabaseConnection()
        {
            //Open database connection

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string db = Path.Combine(documents, ".media.db3");

            // Create connection to the database
            var conn = new SQLiteConnection("Data Source=" + db);

            // Return new connection
            return conn;
        }
    }
}
