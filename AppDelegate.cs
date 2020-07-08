using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using AppKit;
using Foundation;

namespace CultureVulture
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application

            InitialiseSQLiteInterop();

            GetDatabaseConnection();
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        private bool InitialiseSQLiteInterop()
        {
            //Copy SQLite interop dll at runtime to correct directory

            var interopFileName = "SQLite.Interop.dll";
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;
            var env = Environment.Is64BitProcess ? "x64" : "x86";
            var resource = $"{assemblyName}.{env}.{interopFileName}";
            var assemblyDirectory = Path.GetDirectoryName(assembly.Location);
            var dir = Directory.CreateDirectory($@"{assemblyDirectory}/{env}");
            var interopFilePath = Path.Combine(dir.FullName, interopFileName);

            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    // Can't find the resource
                    return false;
                }
                using (var fs = new FileStream(interopFilePath, FileMode.Create, FileAccess.Write))
                {
                    Console.WriteLine(interopFilePath);
                    stream.CopyTo(fs);
                }
            }
            return true;
        }

        private SQLiteConnection GetDatabaseConnection()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string db = Path.Combine(documents, ".media.db3");

            //Create the database if it doesn't already exist
            bool exists = File.Exists(db);
            if (!exists) SQLiteConnection.CreateFile(db);

            // Create connection to the database
            Console.WriteLine(string.Format("Data Source={0};", db));
            var conn = new SQLiteConnection(string.Format("Data Source={0};",db));

            // Set the structure of the database
            if (!exists)
            {
                string commandText = "CREATE TABLE media(id TEXT PRIMARY KEY, media TEXT, title TEXT, creator TEXT, language TEXT, date TEXT, rating INTEGER, edited BIT, goodreadsId TEXT)";
		    
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                command.CommandText = "CREATE UNIQUE INDEX mtc on media(media, title, creator);";
                command.ExecuteNonQuery();
                conn.Close();
            }

            // Return new connection
            return conn;
        }
    }
}
