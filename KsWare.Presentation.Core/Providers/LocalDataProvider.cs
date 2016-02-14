using System;

namespace KsWare.Presentation.Core.Providers {

	/// <summary> Provides local data
	/// </summary>
	public class LocalDataProvider : DataProvider,ILocalDataProvider {

		private object m_Data;

		/// <summary> Initializes a new instance of the <see cref="LocalDataProvider"/> class.
		/// </summary>
		public LocalDataProvider() {}

		#region IViewModelProvider

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value>	<see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		public override bool IsSupported {get {return true;}}

		#endregion

		#region Implementation of IDataProvider

		/// <summary> Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override object Data {
			get {
				return m_Data;
			}
			set {
				if(Equals(value,PreviousData)) return;
				Validate(value);
				m_Data=value;
				OnDataChanged(PreviousData,m_Data);
				PreviousData = value;
			}
		}

		#endregion

	}
	public interface ILocalDataProvider {
		
	}

	/// <summary> Provides local data
	/// </summary>
	/// <typeparam name="TData">Type of data</typeparam>
	public class LocalDataProvider<TData>:DataProvider<TData>,IDataProvider,ILocalDataProvider {

		private TData m_Data;

		/// <summary> Initializes a new instance of the <see cref="LocalDataProvider{TData}"/> class.
		/// </summary>
		public LocalDataProvider() {}

		#region IViewModelProvider

		/// <summary> Gets a value indicating whether this instance is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		public override bool IsSupported {get {return true;}}

		#endregion

		#region Implementation of IDataProvider<T>

		/// <summary> Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override TData Data {
			get {return m_Data;}
			set {
				if (Equals(PreviousData, value)) return;
				Validate(value);
				m_Data = value;
				OnDataChanged(PreviousData, m_Data);
				PreviousData = m_Data;
			}
		}

		#endregion

		#region Implementation of IDataProvider
		
		Exception IDataProvider.Validate(object data) { return Validate((TData) data); }

		#endregion
	}
}