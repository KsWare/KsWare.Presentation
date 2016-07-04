using System;

namespace KsWare.Presentation.Core.Providers {

	/// <summary> [EXPERIMENTAL] Provides data using reflection
	/// </summary>
	/// <typeparam name="TData">Type of data</typeparam>
	/// <remarks>
	/// Draft Limitation:<br/>
	/// * path must be a name of a property
	/// </remarks>
	public class ReflectionDataProvider<TData> : DataProvider<TData>, IDataProvider {

		private readonly Func<object> _GetObject;
		private readonly string _Path;
		private readonly object _Obj;

		/// <summary> Initializes a new instance of the <see cref="ReflectionDataProvider{TData}"/> class.
		/// </summary>
		/// <param name="getObjectFunc">The getter.</param>
		/// <param name="path">The setter.</param>
		public ReflectionDataProvider(Func<object> getObjectFunc, string path){
			if (getObjectFunc == null) throw new ArgumentNullException("getObjectFunc");
			if (path == null) throw new ArgumentNullException("path");

			_GetObject = getObjectFunc;
			_Path      = path;
		}

		/// <summary> Initializes a new instance of the <see cref="ReflectionDataProvider{TData}"/> class.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="path">The path.</param>
		public ReflectionDataProvider(object obj, string path){
			if (obj == null) throw new ArgumentNullException("obj");
			if (path == null) throw new ArgumentNullException("path");

			_Obj       = obj;
			_GetObject = () => _Obj;
			_Path      = path;
		}

		/// <summary> Initializes a new instance of the <see cref="ReflectionDataProvider{TData}"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		public ReflectionDataProvider(string path){
			if (path == null) throw new ArgumentNullException("path");

			_GetObject = () => ((IParentSupport)this.Parent).Parent; // we assume ValueVM.Metadata.Dataprovider structure
			_Path      = path;
		}
		public override bool IsSupported {get {	return true;}		}

		/// <summary>  Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override TData Data {
			get {
				var obj=this._GetObject();
				if(obj==null) throw new NullReferenceException("{14942C32-C47A-48A9-A8D9-FA455244F2B4}");//TODO
//				var t = obj.GetType();
//				var prop = t.GetProperty(_Path);
//				var value = prop.GetValue(obj, null);
				
				var value=new Json(obj, false, true)[m_Path].NativeValue;

				return (TData)value;
			}
			set {
				if(Equals(value,PreviousData)) return;
				Validate(value);

				var obj=this._GetObject();
//				var t = obj.GetType();
//				var prop = t.GetProperty(_Path);
//				prop.SetValue(obj,value,null);

				new Json(obj, false, false)[m_Path] = new Json(value,false,true);

				OnDataChanged(PreviousData, value);
				PreviousData = value;
			}
		} 

		#region Implementation of ICustomDataProvider

		#endregion

	}

}