using System;
using System.Data.SQLite;
using System.IO;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace CultureVulture
{
    public class MediaTableDataSource : NSTableViewDataSource
    {

        public List<MediaModel> MediaRecords = new List<MediaModel>();

        public MediaTableDataSource()
        {
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return MediaRecords.Count;
        }
    }
}
