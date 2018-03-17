using System;

namespace KsWare.Presentation.Core.Providers {

	/// <summary> Provides arguments for the DataChanged event
	/// </summary>
	public class DataChangedEventArgs: EventArgs {

		private readonly object _PreviousData;
		private readonly object _NewData;

		/// <summary> Initializes a new instance of the <see cref="DataChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousData">The previous data if unknown</param>
		/// <param name="newData">The new data.</param>
		public DataChangedEventArgs(object previousData, object newData) {
			_PreviousData = previousData;
			_NewData = newData;
		}

		/// <summary> Gets the previous data or null if previous data is unknown
		/// </summary>
		/// <value>The previous data or null.</value>
		public object PreviousData => _PreviousData;

		/// <summary> Gets the new data.
		/// </summary>
		/// <value>The new data.</value>
		public object NewData => _NewData;

		public string Cause { get; set; }
	}

	/// <summary> Provides arguments for the DataChanged event
	/// </summary>
	public class DataChangedEventArgs<TData>: EventArgs {

		private readonly TData _PreviousData;
		private readonly TData _NewData;

		/// <summary> Initializes a new instance of the <see cref="DataChangedEventArgs"/> class.
		/// </summary>
		/// <param name="previousData">The previous data if unknown</param>
		/// <param name="newData">The new data.</param>
		public DataChangedEventArgs(TData previousData, TData newData) {
			_PreviousData = previousData;
			_NewData = newData;
		}

		/// <summary> Gets the previous data or null if previous data is unknown
		/// </summary>
		/// <value>The previous data or null.</value>
		public TData PreviousData => _PreviousData;

		/// <summary> Gets the new data.
		/// </summary>
		/// <value>The new data.</value>
		public TData NewData => _NewData;

		public string Cause { get; set; }
	}
}