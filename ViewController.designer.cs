// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CultureVulture
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSPopUpButton AddMediaCompletion { get; set; }

		[Outlet]
		AppKit.NSTextField AddMediaCreator { get; set; }

		[Outlet]
		AppKit.NSDatePicker AddMediaDate { get; set; }

		[Outlet]
		AppKit.NSTextField AddMediaLanguage { get; set; }

		[Outlet]
		AppKit.NSPopUpButton AddMediaMedia { get; set; }

		[Outlet]
		AppKit.NSTextField AddMediaRating { get; set; }

		[Outlet]
		AppKit.NSTextField AddMediaTitle { get; set; }

		[Outlet]
		AppKit.NSTableColumn CreatorColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn DateColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn DeleteColumn { get; set; }

		[Outlet]
		AppKit.NSButton ForceSyncOption { get; set; }

		[Outlet]
		AppKit.NSTextField GoodreadsAuthURL { get; set; }

		[Outlet]
		AppKit.NSTextField GoodreadsLoginName { get; set; }

		[Outlet]
		AppKit.NSPopUpButton GoodreadsSyncType { get; set; }

		[Outlet]
		AppKit.NSButton GoodreadsVerify { get; set; }

		[Outlet]
		AppKit.NSButton HardReset { get; set; }

		[Outlet]
		AppKit.NSTableColumn LanguageColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn MediaColumn { get; set; }

		[Outlet]
		AppKit.NSTableView MediaTable { get; set; }

		[Outlet]
		AppKit.NSTableColumn RatingColumn { get; set; }

		[Outlet]
		AppKit.NSPopUpButton SearchMediaField { get; set; }

		[Outlet]
		AppKit.NSSearchField SearchMediaSearch { get; set; }

		[Outlet]
		AppKit.NSTableColumn StatusColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn TitleColumn { get; set; }

		[Outlet]
		AppKit.NSButton UnlockReset { get; set; }

		[Action ("AddMediaClicked:")]
		partial void AddMediaClicked (Foundation.NSObject sender);

		[Action ("GoodreadsAuthClicked:")]
		partial void GoodreadsAuthClicked (Foundation.NSObject sender);

		[Action ("GoodreadsSyncClicked:")]
		partial void GoodreadsSyncClicked (Foundation.NSObject sender);

		[Action ("GoodreadsVerifyClicked:")]
		partial void GoodreadsVerifyClicked (Foundation.NSObject sender);

		[Action ("HardResetClicked:")]
		partial void HardResetClicked (Foundation.NSObject sender);

		[Action ("SearchMediaClicked:")]
		partial void SearchMediaClicked (Foundation.NSObject sender);

		[Action ("UnlockResetClicked:")]
		partial void UnlockResetClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddMediaCompletion != null) {
				AddMediaCompletion.Dispose ();
				AddMediaCompletion = null;
			}

			if (AddMediaCreator != null) {
				AddMediaCreator.Dispose ();
				AddMediaCreator = null;
			}

			if (AddMediaDate != null) {
				AddMediaDate.Dispose ();
				AddMediaDate = null;
			}

			if (AddMediaLanguage != null) {
				AddMediaLanguage.Dispose ();
				AddMediaLanguage = null;
			}

			if (AddMediaMedia != null) {
				AddMediaMedia.Dispose ();
				AddMediaMedia = null;
			}

			if (AddMediaRating != null) {
				AddMediaRating.Dispose ();
				AddMediaRating = null;
			}

			if (AddMediaTitle != null) {
				AddMediaTitle.Dispose ();
				AddMediaTitle = null;
			}

			if (CreatorColumn != null) {
				CreatorColumn.Dispose ();
				CreatorColumn = null;
			}

			if (DateColumn != null) {
				DateColumn.Dispose ();
				DateColumn = null;
			}

			if (StatusColumn != null) {
				StatusColumn.Dispose ();
				StatusColumn = null;
			}

			if (DeleteColumn != null) {
				DeleteColumn.Dispose ();
				DeleteColumn = null;
			}

			if (ForceSyncOption != null) {
				ForceSyncOption.Dispose ();
				ForceSyncOption = null;
			}

			if (GoodreadsAuthURL != null) {
				GoodreadsAuthURL.Dispose ();
				GoodreadsAuthURL = null;
			}

			if (GoodreadsLoginName != null) {
				GoodreadsLoginName.Dispose ();
				GoodreadsLoginName = null;
			}

			if (GoodreadsSyncType != null) {
				GoodreadsSyncType.Dispose ();
				GoodreadsSyncType = null;
			}

			if (GoodreadsVerify != null) {
				GoodreadsVerify.Dispose ();
				GoodreadsVerify = null;
			}

			if (HardReset != null) {
				HardReset.Dispose ();
				HardReset = null;
			}

			if (LanguageColumn != null) {
				LanguageColumn.Dispose ();
				LanguageColumn = null;
			}

			if (MediaColumn != null) {
				MediaColumn.Dispose ();
				MediaColumn = null;
			}

			if (MediaTable != null) {
				MediaTable.Dispose ();
				MediaTable = null;
			}

			if (RatingColumn != null) {
				RatingColumn.Dispose ();
				RatingColumn = null;
			}

			if (SearchMediaField != null) {
				SearchMediaField.Dispose ();
				SearchMediaField = null;
			}

			if (SearchMediaSearch != null) {
				SearchMediaSearch.Dispose ();
				SearchMediaSearch = null;
			}

			if (TitleColumn != null) {
				TitleColumn.Dispose ();
				TitleColumn = null;
			}

			if (UnlockReset != null) {
				UnlockReset.Dispose ();
				UnlockReset = null;
			}
		}
	}
}
