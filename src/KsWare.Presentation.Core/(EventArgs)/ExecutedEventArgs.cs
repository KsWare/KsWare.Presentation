using System;

namespace KsWare.Presentation {

	/// <summary> Provides data for Executed event
	/// </summary>
	public class ExecutedEventArgs:EventArgs {

		private readonly object _Parameter;

		/// <summary> Initializes a new instance of the <see cref="ExecutedEventArgs"/> class.
		/// </summary>
		/// <param name="parameter">The action _Parameter.</param>
		public ExecutedEventArgs(object parameter) { _Parameter = parameter; }

		/// <summary> Gets the action _Parameter.
		/// </summary>
		/// <value>The action _Parameter.</value>
		public object Parameter => _Parameter;
	}

}