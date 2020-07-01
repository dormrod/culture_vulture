using System;
using System.Data.SQLite;
using System.IO;
using AppKit;
using Foundation;

namespace CultureVulture
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
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
                // Update the view, if already loaded.
            }
        }

        partial void AddMediaClicked(NSObject sender)
        {
            //Add media to database

            //Create new media record
            string media = AddMediaMedia.StringValue;
            string title = AddMediaTitle.StringValue;
            string creator = AddMediaCreator.StringValue;
            string language = AddMediaLanguage.StringValue;
            int rating = AddMediaRating.IntValue;
            var dateTime = DateTime.Parse(AddMediaDate.DateValue.ToString());
            string date = dateTime.ToString("dd-MM-yyyy");
            var record = new MediaModel(media,title,creator,language,date,rating);
            var conn = GetDatabaseConnection();
            record.Create(conn);
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
                command.CommandText = "CREATE TABLE media(id TEXT PRIMARY KEY, media TEXT, title TEXT, creator TEXT, language TEXT, date TEXT, rating INTEGER, edited BIT, goodreadsId TEXT)";
                command.ExecuteNonQuery();
                conn.Close();
			}
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
