using System;

namespace KsWare.Presentation {

	/// <summary> Provides data for Executed event
	/// </summary>
	public class ExecutedEventArgs:EventArgs {

		private readonly object m_Parameter;

		/// <summary> Initializes a new instance of the <see cref="ExecutedEventArgs"/> class.
		/// </summary>
		/// <param name="parameter">The action m_Parameter.</param>
		public ExecutedEventArgs(object parameter) { m_Parameter = parameter; }

		/// <summary> Gets the action m_Parameter.
		/// </summary>
		/// <value>The action m_Parameter.</value>
		public object Parameter {
			get {return m_Parameter;}
		}
	}

}