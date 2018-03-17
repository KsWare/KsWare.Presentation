using System.ComponentModel;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary>  [EXPERIMENTAL] ActionVM with progress and activity state
	/// </summary>
	/// <remarks></remarks>
	public class ActionActiveProgressVM:ActionVM {

		/// <summary> Initializes a new instance of the <see cref="ActionActiveProgressVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public ActionActiveProgressVM() {}

		/// <summary> Gets the progress in percent.
		/// </summary>
		/// <remarks></remarks>
		[Bindable(true)]
		public double ProgressPercent { get => Fields.GetValue<double>(); set => Fields.SetValue(value); }

		/// <summary> Gets a value indicating whether the action is done.
		/// </summary>
		/// <remarks></remarks>
		[Bindable(true)]
		public bool IsDone { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary> Sets the progress.
		/// </summary>
		/// <param name="progressPercent">The progress percent.</param>
		/// <remarks></remarks>
		public void SetProgress(double progressPercent) {
			ProgressPercent=progressPercent;
			IsDone = (progressPercent >= 100.0);
		}

		/// <summary> Sets the progress.
		/// </summary>
		/// <param name="done">if set to <c>true</c> ProgressPercent = 100; else ProgressPercent = 0</param>
		/// <remarks></remarks>
		public void SetProgress(bool done) {
			ProgressPercent=done?100.0:0.0;
			IsDone = done;
		}

	}


}
