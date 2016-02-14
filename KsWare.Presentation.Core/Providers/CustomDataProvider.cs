using System;

namespace KsWare.Presentation.Core.Providers {

	public interface ICustomDataProvider:IDataProvider {
		
	}

	/// <summary> Provides custom data
	/// </summary>
	/// <typeparam name="TData">Type of data</typeparam>
	public class CustomDataProvider<TData> : DataProvider<TData>, ICustomDataProvider {

		private readonly Func<TData> m_GetValue;
		private readonly Action<TData> m_SetValue;

		/// <summary> Initializes a new instance of the <see cref="CustomDataProvider&lt;TData&gt;"/> class.
		/// </summary>
		/// <param name="getValueFunc">The getter.</param>
		/// <param name="setValueFunc">The setter.</param>
		public CustomDataProvider(Func<TData> getValueFunc, Action<TData> setValueFunc){
//			if (getValueFunc == null) throw new ArgumentNullException("getValueFunc");
//			if (setValueFunc == null) throw new ArgumentNullException("setValueFunc");

			m_GetValue = getValueFunc;
			m_SetValue = setValueFunc;
		}

		public override bool IsSupported {get {	return true;}}

//		protected IObjectVM ParentVM {
//			get {
//				var metadata = Parent as IParentSupport; if( metadata==null) return null;
//				var vm = metadata.Parent as IObjectVM;
//				return vm;
//			}
//		}

		[Obsolete("TODO should return a IObjectVM")]
		protected object ParentVM { get; private set; }

		/// <summary>  Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override TData Data {
			get {
				if(m_GetValue==null) throw new NotSupportedException("Property get method not defined! UniqueID: {6708C25A-A2CF-4C60-98C5-C129A1FABD19}");
				//possible NullReferenceException, is Data is null!
				//ex.: DataProvider = new CustomDataProvider<String>(() => this.Data.Author, value => this.Data.Author = value)
				return m_GetValue();
			}
			set {
				if(m_SetValue==null) throw new NotSupportedException("Property set method not defined! UniqueID: {D880F93C-25D5-4170-B803-415A38013118}");
				if(Equals(value,PreviousData)) return;
				Validate(value);
				m_SetValue(value);
				OnDataChanged(PreviousData, value);
				PreviousData = value;
			}
		}

		#region Implementation of ICustomDataProvider

		#endregion

	}
	/// <summary> Provides custom data
	/// </summary>
	public class CustomDataProvider:DataProvider,ICustomDataProvider {

		private readonly Func<object> m_GetValue;
		private readonly Action<object> m_SetValue;

		/// <summary> Initializes a new instance of the <see cref="CustomDataProvider"/> class.
		/// </summary>
		/// <param name="getValue">The getter.</param>
		/// <param name="setValue">The setter.</param>
		public CustomDataProvider(Func<object> getValue, Action<object> setValue) {
//			if (getValue == null) throw new ArgumentNullException("getValue");
//			if (setValue == null) throw new ArgumentNullException("setValue");

			m_GetValue = getValue;
			m_SetValue = setValue;
		}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>. </value>
		public override bool IsSupported {get {return true;}}

		/// <summary> Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override object Data {
			get {
				if(m_GetValue==null) throw new InvalidOperationException("Data has not getter!");
				return m_GetValue();
			}
			set {
				if(m_SetValue==null) throw new InvalidOperationException("Data has not setter!");
				if(Equals(value,PreviousData)) return;
				Validate(value);
				m_SetValue(value);
				//NotifyDataChanged();
				OnDataChanged(PreviousData, value);
				PreviousData = value;
			}
		}
	}

}