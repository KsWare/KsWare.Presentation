using System.Collections.Generic;
using Microsoft.Win32;

namespace KsWare.Presentation {

	/// <summary> Provides event arguments for the standard <see cref="OpenFileDialog"/>
	/// </summary>
	public class OpenFileFeedbackEventArgs : UserFeedbackEventArgs {

		/// <summary> Initializes a new instance of the <see cref="OpenFileFeedbackEventArgs"/> class.
		/// </summary>
		public OpenFileFeedbackEventArgs() {}

		public override FeedbackType FeedbackType => FeedbackType.OpenFile;


		/// <seealso cref="OpenFileDialog.Multiselect"/>
		public bool Multiselect { get; set; }

		/// <seealso cref="OpenFileDialog.ReadOnlyChecked"/>
		public bool ReadOnlyChecked { get; set; }

		/// <seealso cref="OpenFileDialog.ShowReadOnly"/>
		public bool ShowReadOnly { get; set; }

		/// <seealso cref="OpenFileDialog.AddExtension"/>
		public bool AddExtension { get; set; }

		/// <seealso cref="OpenFileDialog.CheckFileExists"/>
		public bool CheckFileExists { get; set; }

		/// <seealso cref="OpenFileDialog.CheckPathExists"/>
		public bool CheckPathExists { get; set; }

		/// <seealso cref="OpenFileDialog.CustomPlaces"/>
		public IList<FileDialogCustomPlace> CustomPlaces { get; set; }

		/// <seealso cref="OpenFileDialog.DefaultExt"/>
		public string DefaultExt { get; set; }

		/// <seealso cref="OpenFileDialog.DereferenceLinks"/>
		public bool DereferenceLinks { get; set; }

		/// <seealso cref="OpenFileDialog.FileName"/>
		public string FileName { get; set; }

		/// <seealso cref="OpenFileDialog.FileNames"/>
		public string[] FileNames { get; set; }

		/// <seealso cref="OpenFileDialog.Filter"/>
		public string Filter { get; set; }

		/// <seealso cref="OpenFileDialog.FilterIndex"/>
		public int FilterIndex { get; set; }

		/// <seealso cref="OpenFileDialog.InitialDirectory"/>
		public string InitialDirectory { get; set; }

		/// <seealso cref="OpenFileDialog.RestoreDirectory"/>
		public bool RestoreDirectory { get; set; }

		/// <summary>
		/// Gets or sets the name of the safe file.
		/// </summary>
		/// <value>The name of the safe file.</value>
		/// <seealso cref="OpenFileDialog.SafeFileName"/>
		public string SafeFileName { get; set; }

		/// <seealso cref="OpenFileDialog.SafeFileNames"/>
		public string[] SafeFileNames { get; set; }

		/// <seealso cref="OpenFileDialog.Title"/>
		public string Title { get; set; }

		/// <seealso cref="OpenFileDialog.ValidateNames"/>
		public bool ValidateNames { get; set; }

		/// <summary> Gets or sets the dialog result.
		/// </summary>
		/// <value>The dialog result.</value>
		public bool? DialogResult { get; set; }


		/// <summary> Creates the dialog.
		/// </summary>
		/// <returns><see cref="OpenFileDialog"/></returns>
		public OpenFileDialog CreateDialog() {
			var dlg = new OpenFileDialog {
				AddExtension     = AddExtension    ,
				CheckFileExists  = CheckFileExists ,
				CheckPathExists  = CheckPathExists ,
				CustomPlaces     = CustomPlaces    ,
				DefaultExt       = DefaultExt      ,
				DereferenceLinks = DereferenceLinks,
				FileName         = FileName        ,
//				FileNames		 = FileNames	   ,
				Filter           = Filter          ,
				FilterIndex      = FilterIndex     ,
				InitialDirectory = InitialDirectory,
				Multiselect      = Multiselect     ,
				ReadOnlyChecked  = ReadOnlyChecked,
				RestoreDirectory = RestoreDirectory,
//				SafeFileName	 = SafeFileName	   ,
//				SafeFileNames	 = SafeFileNames   ,
				ShowReadOnly     = ShowReadOnly    ,
//				Tag			     = Tag			   ,
				Title            = Title           ,
				ValidateNames    = ValidateNames   ,
			};
			return dlg;
		}

		/// <summary> Updates this instance using the specified dialog result and dialog.
		/// </summary>
		/// <param name="dialogResult">The dialog result</param>
		/// <param name="dlg">The dialog.</param>
		public void Update(bool? dialogResult, OpenFileDialog dlg) {
			AddExtension     = dlg.AddExtension;
			CheckFileExists  = dlg.CheckFileExists;
			CheckPathExists  = dlg.CheckPathExists;
			CustomPlaces     = dlg.CustomPlaces;
			DefaultExt       = dlg.DefaultExt;
			DereferenceLinks = dlg.DereferenceLinks;
			FileName         = dlg.FileName;
			FileNames        = dlg.FileNames;
			Filter           = dlg.Filter;
			FilterIndex      = dlg.FilterIndex;
			InitialDirectory = dlg.InitialDirectory;
			Multiselect      = dlg.Multiselect;
			ReadOnlyChecked  = dlg.ReadOnlyChecked;
			RestoreDirectory = dlg.RestoreDirectory;
			SafeFileName     = dlg.SafeFileName;
			SafeFileNames    = dlg.SafeFileNames;
			ShowReadOnly     = dlg.ShowReadOnly;
//			Tag			     = dlg.Tag			 ;
			Title            = dlg.Title;
			ValidateNames    = dlg.ValidateNames;

			DialogResult     = dialogResult;
			Handled          = true;
		}

		/// <summary> Shows the dialog.
		/// </summary>
		/// <returns>The dialog result</returns>
		/// <seealso cref="OpenFileDialog.ShowDialog()"/>
		public bool? ShowDialog() {
			var dlg = CreateDialog();
			var rslt = dlg.ShowDialog();
			Update(rslt,dlg);
			return rslt;
		}

	}

}