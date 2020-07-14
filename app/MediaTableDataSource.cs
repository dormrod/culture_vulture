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

        public void Sort(string key, bool ascending)
        {
            //Sort table by given column

			switch (key)
            {
                case "Media":
                    if (ascending)
                    {
                        MediaRecords.Sort((x, y) => x.Media.CompareTo(y.Media));
                    }
                    else
                    {
                        MediaRecords.Sort((x, y) => -1 * x.Media.CompareTo(y.Media));
                    }
                    break;
                case "Title":
                    if (ascending)
                    {
                        MediaRecords.Sort((x, y) => x.Title.CompareTo(y.Title));
                    }
                    else
                    {
                        MediaRecords.Sort((x, y) => -1 * x.Title.CompareTo(y.Title));
                    }
                    break;
                case "Creator":
                    if (ascending)
                    {
                        MediaRecords.Sort((x, y) => x.Creator.CompareTo(y.Creator));
                    }
                    else
                    {
                        MediaRecords.Sort((x, y) => -1 * x.Creator.CompareTo(y.Creator));
                    }
                    break;
                case "Language":
                    if (ascending)
                    {
                        MediaRecords.Sort((x, y) => x.Language.CompareTo(y.Language));
                    }
                    else
                    {
                        MediaRecords.Sort((x, y) => -1 * x.Language.CompareTo(y.Language));
                    }
                    break;
                case "Rating":
                    if (ascending)
                    {
                        MediaRecords.Sort((x, y) => x.Rating.CompareTo(y.Rating));
                    }
                    else
                    {
                        MediaRecords.Sort((x, y) => -1 * x.Rating.CompareTo(y.Rating));
                    }
                    break;
                case "Status":
                    if (ascending)
                    {
                        MediaRecords.Sort((x, y) => x.Status.CompareTo(y.Status));
                    }
                    else
                    {
                        MediaRecords.Sort((x, y) => -1 * x.Status.CompareTo(y.Status));
                    }
                    break;
            }

        }

        public override void SortDescriptorsChanged(NSTableView tableView, NSSortDescriptor[] oldDescriptors)
        {
            // Sort the data
            if (oldDescriptors.Length > 0)
            {
                // Update sort
                Sort(oldDescriptors[0].Key, oldDescriptors[0].Ascending);
            }
            else
            {
                // Grab current descriptors and update sort
                NSSortDescriptor[] tbSort = tableView.SortDescriptors;
                Sort(tbSort[0].Key, tbSort[0].Ascending);
            }

            // Refresh table
            tableView.ReloadData();
        }
    }
}
