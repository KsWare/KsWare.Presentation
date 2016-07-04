using System;

namespace KsWare.Presentation {

	public class CanExecuteEventArgs:EventArgs {

		private readonly object _parameter;

		/// <summary> Initializes a new instance of the <see cref="CanExecuteEventArgs"/> class.
		/// </summary>
		/// <param name="parameter">The event parameter.</param>
		/// <remarks></remarks>
		public CanExecuteEventArgs(object parameter) {_parameter = parameter;}

		/// <summary> Gets the event parameter.
		/// </summary>
		/// <remarks></remarks>
		public object Parameter {get {return _parameter;}}

		public bool CanExecute { get; set; }
	}

}