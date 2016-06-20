using System;

namespace KsWare.Presentation.Core.Providers {

	/// <summary> Provides arguments for the DataChanged event
	/// </summary>
	public class DataChangedEventArgs: EventArgs {

		private readonly object m_PreviousData;
		private readonly object m_NewData;

		/// <summary> Initializes a new instance of the <see cref="DataChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousData">The previous data if unknown</param>
		/// <param name="newData">The new data.</param>
		public DataChangedEventArgs(object previousData, object newData) {
			m_PreviousData = previousData;
			m_NewData = newData;
		}

		/// <summary> Gets the previous data or null if previous data is unknown
		/// </summary>
		/// <value>The previous data or null.</value>
		public object PreviousData {get {return m_PreviousData;}}

		/// <summary> Gets the new data.
		/// </summary>
		/// <value>The new data.</value>
		public object NewData {get {return m_NewData;}}

		public string Cause { get; set; }
	}

	/// <summary> Provides arguments for the DataChanged event
	/// </summary>
	public class DataChangedEventArgs<TData>: EventArgs {

		private readonly TData m_PreviousData;
		private readonly TData m_NewData;

		/// <summary> Initializes a new instance of the <see cref="DataChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousData">The previous data if unknown</param>
		/// <param name="newData">The new data.</param>
		public DataChangedEventArgs(TData previousData, TData newData) {
			m_PreviousData = previousData;
			m_NewData = newData;
		}

		/// <summary> Gets the previous data or null if previous data is unknown
		/// </summary>
		/// <value>The previous data or null.</value>
		public TData PreviousData {get {return m_PreviousData;}}

		/// <summary> Gets the new data.
		/// </summary>
		/// <value>The new data.</value>
		public TData NewData {get {return m_NewData;}}

		public string Cause { get; set; }
	}
}