using System;
using System.Data.SQLite;
using System.IO;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace CultureVulture
{
    public class MediaTableDelegate : NSTableViewDelegate
    {
        //Based on tutorial: https://docs.microsoft.com/en-us/xamarin/mac/user-interface/table-view

        private const string CellIdentifier = "ProdCell";

        private MediaTableDataSource DataSource;

        public MediaTableDelegate(MediaTableDataSource datasource)
        {
            this.DataSource = datasource;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            // This pattern allows you reuse existing views when they are no-longer in use.
            // If the returned view is null, you instance up a new view
            // If a non-null view is returned, you modify it enough to reflect the new data
            NSTextField view = (NSTextField)tableView.MakeView(CellIdentifier, this);
            if (view == null)
            {
                view = new NSTextField();
                view.Identifier = CellIdentifier;
                view.BackgroundColor = NSColor.Clear;
                view.Bordered = false;
                view.Selectable = false;
                view.Editable = false;
            }

            // Setup view based on the column selected
            switch (tableColumn.Title)
            {
                case "Media":
                    view.StringValue = DataSource.MediaRecords[(int)row].Media;
                    break;
                case "Title":
                    view.StringValue = DataSource.MediaRecords[(int)row].Title;
                    break;
                case "Creator":
                    view.StringValue = DataSource.MediaRecords[(int)row].Creator;
                    break;
                case "Language":
                    view.StringValue = DataSource.MediaRecords[(int)row].Language;
                    break;
                case "Rating":
                    view.StringValue = DataSource.MediaRecords[(int)row].Rating.ToString();
                    break;
                case "Date":
                    view.StringValue = DataSource.MediaRecords[(int)row].Date;
                    break;
            }

            return view;
        }
    }
}
