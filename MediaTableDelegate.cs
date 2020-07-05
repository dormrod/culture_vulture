using System;
using System.Data.SQLite;
using System.IO;
using AppKit;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;

namespace CultureVulture
{
    public class MediaTableDelegate : NSTableViewDelegate
    {
        //Based on tutorial: https://docs.microsoft.com/en-us/xamarin/mac/user-interface/table-view

        private const string CellIdentifier = "ProdCell";

        private MediaTableDataSource DataSource;
        private ViewController Controller;

        //public MediaTableDelegate(MediaTableDataSource datasource)
        //{
        //    this.DataSource = datasource;
        //}

        public MediaTableDelegate(ViewController controller, MediaTableDataSource datasource)
        {
            this.Controller = controller;
            this.DataSource = datasource;
        }

        private void ConfigureTextField(NSTableCellView view, nint row)
        {
            // Add to view
            view.TextField.AutoresizingMask = NSViewResizingMask.WidthSizable;
            view.AddSubview(view.TextField);

            // Configure
            view.TextField.BackgroundColor = NSColor.Clear;
            view.TextField.Bordered = false;
            view.TextField.Selectable = false;
            view.TextField.Editable = true;

            // Wireup events
            view.TextField.EditingEnded += (sender, e) =>
            {
                DataSource.MediaRecords[(int)view.TextField.Tag].Connection = GetDatabaseConnection();
                // Take action based on type
                switch (view.Identifier)
                {
                    case "Media":
                        if (view.TextField.StringValue == "Book" || view.TextField.StringValue == "book")
                        {
                            DataSource.MediaRecords[(int)view.TextField.Tag].Media = "Book";
                        }
                        else if (view.TextField.StringValue == "Film" || view.TextField.StringValue == "film")
                        {
                            DataSource.MediaRecords[(int)view.TextField.Tag].Media = "Film";
                        }
                        break;
                    case "Title":
                        DataSource.MediaRecords[(int)view.TextField.Tag].Title = view.TextField.StringValue;
                        break;
                    case "Creator":
                        DataSource.MediaRecords[(int)view.TextField.Tag].Creator = view.TextField.StringValue;
                        break;
                    case "Language":
                        DataSource.MediaRecords[(int)view.TextField.Tag].Language = view.TextField.StringValue;
                        break;
                    case "Rating":
                        try
                        {
                            DataSource.MediaRecords[(int)view.TextField.Tag].Rating = Convert.ToInt32(view.TextField.StringValue);
                        }
                        catch
                        {
                            DataSource.MediaRecords[(int)view.TextField.Tag].Rating = 0;
                        }
                        break;
                    case "Date":
                        DataSource.MediaRecords[(int)view.TextField.Tag].Date = view.TextField.StringValue;
                        break;
                }
                Controller.ReloadMediaTable();
            };

            // Tag view
            view.TextField.Tag = row;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            // This pattern allows you reuse existing views when they are no-longer in use.
            // If the returned view is null, you instance up a new view
            // If a non-null view is returned, you modify it enough to reflect the new data
            NSTableCellView view = (NSTableCellView)tableView.MakeView(tableColumn.Title, this);
            if (view == null)
            {
                view = new NSTableCellView();

                // Configure the view
                view.Identifier = tableColumn.Title;

                // Take action based on title
                switch (tableColumn.Title)
                {
                    case "Media":
                        ////view.ImageView = new NSImageView(new CGRect(0, 0, 16, 16));
                        //view.AddSubview(view.ImageView);
                        view.TextField = new NSTextField(new CGRect(20, 0, 400, 16));
                        ConfigureTextField(view, row);
                        break;
                    case "Title":
                        view.TextField = new NSTextField(new CGRect(0, 0, 400, 16));
                        ConfigureTextField(view, row);
                        break;
                    case "Creator":
                        view.TextField = new NSTextField(new CGRect(0, 0, 400, 16));
                        ConfigureTextField(view, row);
                        break;
                    case "Language":
                        view.TextField = new NSTextField(new CGRect(0, 0, 400, 16));
                        ConfigureTextField(view, row);
                        break;
                    case "Rating":
                        view.TextField = new NSTextField(new CGRect(0, 0, 400, 16));
                        ConfigureTextField(view, row);
                        break;
                    case "Date":
                        view.TextField = new NSTextField(new CGRect(0, 0, 400, 16));
                        ConfigureTextField(view, row);
                        break;
                    case "Delete":
                        // Create new button
                        var button = new NSButton(new CGRect(0, 0, 81, 16));
                        button.SetButtonType(NSButtonType.MomentaryPushIn);
                        button.Title = "Delete";
                        button.Tag = row;

                        // Wireup events
                        button.Activated += (sender, e) => {
                            // Get button and product
                            var btn = sender as NSButton;
                            var record = DataSource.MediaRecords[(int)btn.Tag];

                            // Configure alert
                            var alert = new NSAlert()
                            {
                                AlertStyle = NSAlertStyle.Informational,
                                InformativeText = $"Are you sure you want to delete the {record.Media} {record.Title} by {record.Creator}? This operation cannot be undone.",
                                MessageText = $"Delete {record.Title}?",
                            };
                            alert.AddButton("Cancel");
                            alert.AddButton("Delete");
                            alert.BeginSheetForResponse(Controller.View.Window, (result) => {
                                // Should we delete the requested row?
                                if (result == 1001)
                                {
                                    // Remove the given row from the dataset
                                    DataSource.MediaRecords.RemoveAt((int)btn.Tag);
                                    Controller.ReloadMediaTable();
                                }
                            });
                        };
                        // Add to view
                        view.AddSubview(button);
                        break;
                }
            }

            // Setup view based on the column selected
            switch (tableColumn.Title)
            {
                case "Media":
                    view.TextField.StringValue = DataSource.MediaRecords[(int)row].Media;
                    break;
                case "Title":
                    view.TextField.StringValue = DataSource.MediaRecords[(int)row].Title;
                    break;
                case "Creator":
                    view.TextField.StringValue = DataSource.MediaRecords[(int)row].Creator;
                    break;
                case "Language":
                    view.TextField.StringValue = DataSource.MediaRecords[(int)row].Language;
                    break;
                case "Rating":
                    view.TextField.StringValue = DataSource.MediaRecords[(int)row].Rating.ToString();
                    break;
                case "Date":
                    view.TextField.StringValue = DataSource.MediaRecords[(int)row].Date;
                    break;
                case "Delete":
                    foreach (NSView subview in view.Subviews)
                    {
                        var btn = subview as NSButton;
                        if (btn != null)
                        {
                            btn.Tag = row;
                        }
                    }
                    break;
            }

            return view;
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
