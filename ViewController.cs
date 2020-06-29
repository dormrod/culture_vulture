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

            var conn = GetDatabaseConnection();
            string title = AddMediaMedia.StringValue;
            Console.WriteLine(title);
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
