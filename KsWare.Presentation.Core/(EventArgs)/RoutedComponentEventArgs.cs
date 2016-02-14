using System;

namespace KsWare.Presentation {

	/// <summary> Provides arguments for for a routed component event
	/// </summary>
	public class RoutedComponentEventArgs : EventArgs {

		private readonly object m_OriginalSource;

		/// <summary> Initializes a new instance of the <see cref="RoutedComponentEventArgs"/> class.
		/// </summary>
		/// <param name="originalSource">The original source.</param>
		public RoutedComponentEventArgs(object originalSource) { m_OriginalSource = originalSource; }

		/// <summary> Gets the original source of the event.
		/// </summary>
		/// <value>The original source.</value>
		public object OriginalSource { get { return this.m_OriginalSource; } }

	}

}
