using System.Collections.Generic;
using Microsoft.Win32;

namespace KsWare.Presentation {

	/// <summary> Provides event arguments for the standard <see cref="SaveFileDialog"/>
	/// </summary>
	public class SaveFileFeedbackEventArgs : UserFeedbackEventArgs {

		/// <summary> Initializes a new instance of the <see cref="SaveFileFeedbackEventArgs"/> class.
		/// </summary>
		public SaveFileFeedbackEventArgs() {}

		public override FeedbackType FeedbackType => FeedbackType.SaveFile;


		/// <seealso cref="SaveFileDialog.CreatePrompt"/>
		public bool CreatePrompt { get; set; }

		/// <seealso cref="SaveFileDialog.OverwritePrompt"/>
		public bool OverwritePrompt { get; set; }

		/// <seealso cref="FileDialog.AddExtension"/>
		public bool AddExtension { get; set; }

		/// <seealso cref="FileDialog.CheckFileExists"/>
		public bool CheckFileExists { get; set; }

		/// <seealso cref="FileDialog.CheckPathExists"/>
		public bool CheckPathExists { get; set; }

		/// <seealso cref="FileDialog.CustomPlaces"/>
		public IList<FileDialogCustomPlace> CustomPlaces { get; set; }
		
		/// <seealso cref="FileDialog.DefaultExt"/>
		public string DefaultExt { get; set; }

		/// <seealso cref="FileDialog.DereferenceLinks"/>
		public bool DereferenceLinks { get; set; }
		
		/// <seealso cref="FileDialog.FileName"/>
		public string FileName { get; set; }

		/// <seealso cref="FileDialog.FileNames"/>
		public string[] FileNames { get; set; }

		/// <seealso cref="FileDialog.Filter"/>
		public string Filter { get; set; }

		/// <seealso cref="FileDialog.FilterIndex"/>
		public int FilterIndex { get; set; }

		/// <seealso cref="FileDialog.InitialDirectory"/>
		public string InitialDirectory { get; set; }

		/// <seealso cref="FileDialog.RestoreDirectory"/>
		public bool RestoreDirectory { get; set; }
		
		/// <summary> Gets or sets the name of the safe file.
		/// </summary>
		/// <value>The name of the safe file.</value>
		/// <seealso cref="FileDialog.SafeFileName"/>
		public string SafeFileName { get; set; }
		
		/// <seealso cref="FileDialog.SafeFileNames"/>
		public string[] SafeFileNames { get; set; }
		
		/// <seealso cref="FileDialog.Title"/>
		public string Title { get; set; }
		
		/// <seealso cref="FileDialog.ValidateNames"/>
		public bool ValidateNames { get; set; }



		/// <summary> Gets or sets the dialog result.
		/// </summary>
		/// <value>The dialog result.</value>
		public bool? DialogResult { get; set; }

		/// <summary> Creates the dialog.
		/// </summary>
		/// <returns><see cref="SaveFileDialog"/></returns>
		public SaveFileDialog CreateDialog() { 
			var dlg=new SaveFileDialog{
				AddExtension	 = AddExtension    ,
				CheckFileExists	 = CheckFileExists ,
				CheckPathExists	 = CheckPathExists ,
				CreatePrompt     = CreatePrompt    ,
				CustomPlaces	 = CustomPlaces    ,
				DefaultExt		 = DefaultExt      ,
				DereferenceLinks = DereferenceLinks,
				FileName		 = FileName        ,
//				FileNames		 = FileNames       ,
				Filter			 = Filter          ,
				FilterIndex		 = FilterIndex     ,
				InitialDirectory = InitialDirectory,
				OverwritePrompt	 = OverwritePrompt ,
				RestoreDirectory = RestoreDirectory,
//				SafeFileName	 = SafeFileName    ,
//				SafeFileNames	 = SafeFileNames   ,
//				Tag				 = Tag             ,
				Title			 = Title           ,
				ValidateNames	 = ValidateNames   ,
			};
			return dlg;
		}

		/// <summary> Updates this instance using the specified dialog result and dialog.
		/// </summary>
		/// <param name="dialogResult">The dialog result</param>
		/// <param name="dlg">The dialog.</param>
		public void Update(bool? dialogResult, SaveFileDialog dlg) { 
			AddExtension	 = dlg.AddExtension     ;
			CheckFileExists	 = dlg.CheckFileExists  ;
			CheckPathExists	 = dlg.CheckPathExists  ;
			CreatePrompt     = dlg.CreatePrompt     ;
			CustomPlaces	 = dlg.CustomPlaces     ;
			DefaultExt		 = dlg.DefaultExt       ;
			DereferenceLinks = dlg.DereferenceLinks ;
			FileName		 = dlg.FileName         ;
			FileNames		 = dlg.FileNames        ;
			Filter			 = dlg.Filter           ;
			FilterIndex		 = dlg.FilterIndex      ;
			InitialDirectory = dlg.InitialDirectory ;
			OverwritePrompt	 = dlg.OverwritePrompt  ;
			RestoreDirectory = dlg.RestoreDirectory ;
			SafeFileName	 = dlg.SafeFileName     ;
			SafeFileNames	 = dlg.SafeFileNames    ;
//			Tag				 = dlg.Tag              ;
			Title			 = dlg.Title            ;
			ValidateNames	 = dlg.ValidateNames    ;

			DialogResult = dialogResult;
			Handled = true;
		}

		/// <summary> Shows the dialog.
		/// </summary>
		/// <returns>The dialog result</returns>
		public bool? ShowDialog() {
			var dlg = CreateDialog();
			var rslt = dlg.ShowDialog();
			Update(rslt,dlg);
			return rslt;
		}

	}
}