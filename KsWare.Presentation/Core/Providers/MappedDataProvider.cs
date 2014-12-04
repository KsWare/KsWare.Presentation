/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : MappedDataProvider.cs
 * OriginalNamespace: KsWare.Presentation.Providers
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Diagnostics;
using System.Reflection;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.Core;
using KsWare.JsonFx;

namespace KsWare.Presentation.Core.Providers
{
	/// <summary> [DRAFT] Provides data using reflection
	/// </summary>
	/// <typeparam name="TDataRoot">Type of root data</typeparam>
	/// <typeparam name="TData">Type of data</typeparam>
	/// <remarks>
	/// DRAFT Limitation:<br/>
	/// * path must be a name of a property
	/// </remarks>
	public class MappedDataProvider<TDataRoot,TData> : DataProvider<TData>, IDataProvider
	{
		private readonly string _path;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		private ViewModelDataProviderObserver _observer;

		/// <summary> Initializes a new instance of the <see cref="MappedDataProvider{TDataRoot,TData}"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		public MappedDataProvider(string path){
			if (path == null) throw new ArgumentNullException("path");
			_path      = path;
			VerifyPath();
			_observer = new ViewModelDataProviderObserver(this) {ParentVMParentDataChanged = AtParentVMParentDataChanged};
		}

		/// <summary> Occurs if this.Parent.Parent.Parent.Metadata.DataProvider.DataChanged
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AtParentVMParentDataChanged(object sender, DataChangedEventArgs e) {
			//maybe use NotifyDataChanged();

			Exception exception;
			var d = TryGetData(out exception);
			if(exception!=null) return;
			if(Equals(d,PreviousData))return;

			if(!Equals(d,PreviousData)) OnDataChanged(PreviousData,d);
			PreviousData = d;
		}


		private void VerifyPath() {
			if(_path.Contains(".")) {VerifyPathEx();return;}

			var tDataRoot = typeof (TDataRoot);
			PropertyInfo propertyInfo;
			try{propertyInfo = tDataRoot.GetProperty(_path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);}
			catch(AmbiguousMatchException ex){this.DoNothing(ex); /*TODO*/return;}
			catch(Exception ex) {this.DoNothing(ex); throw;}
			if (propertyInfo == null) {
				var msg=("ERROR: Property not found!"+
					"\n\tPath: "+_path+
					"\n\tRoot: "+DebugUtil.FormatTypeName(tDataRoot)+
					//"\n\tModel: "+DebugUtil.FormatTypeName(DebugUtil.Try(()=>((IMetadata)Parent).Parent))+
					"\n\tModel: "+DebugUtil.Try(()=>DebugUtil.FormatTypeName(DebugUtil.FindStackAncestor2(typeof(IObjectVM))))
					//"\n\tModelPath: "+ DebugUtil.Try(()=>new Json(this).GetValue("Parent.Parent.MemberPath",true))+
					);
				var frame = DebugUtil.FindStackFrame(typeof (IObjectVM), 1);
				Debug.WriteLine("CLICK TO NAVIGATE>>\n"+ frame.GetFileName() + "(" + frame.GetFileLineNumber() + "," + frame.GetFileColumnNumber() + ")");
				DebuggerːBreak(msg);
				throw new MissingMemberException(tDataRoot.FullName,_path);
			}
			if(propertyInfo.GetGetMethod()==null) throw new MissingMemberException(tDataRoot.FullName,_path+ ";get");

			//check return type
			if (!propertyInfo.PropertyType.IsAssignableFrom(typeof (TData))) {
				throw new MemberAccessException(
					"Incompatible property type!"+
					"\r\n\tExpected: "+DebugUtil.FormatTypeName(typeof (TData))+
					"\r\n\tAvailable: "+DebugUtil.FormatTypeName(propertyInfo.PropertyType)+
					"\r\n\tPath: "+_path+
					(ParentVM==null?"":"\r\n\tViewmodel: "+ParentVM.MemberPath + " "+((ObjectVM)ParentVM).DebugːGetTypeːName)+
					"\r\n\tUniqueID: {85C102E4-4408-40F9-B198-F2F1CCDB7F55}"
				);
			}
		}

		private void VerifyPathEx() {
			
		}

		/// <summary> Gets a value indicating whether this instance is supported.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is supported; otherwise, <c>false</c>.
		/// </value>
		public override bool IsSupported {get {	return true;}		}

		private bool ValidateNotNull(object value,string name, string path ,string guid, out Exception exception) {
			if(value!=null) {exception = null; return true;}
			exception=new NullReferenceException("Object is null! Path: "+path+ " UniqueID: "+guid);
			return false;
		}

		public override TData TryGetData(out Exception exception) {
			try {
				var metadata = (IParentSupport) Parent   ;if(!ValidateNotNull(metadata,"Metadata","Parent","{FB2701BA-78A8-4E72-811E-6D254188AC91}", out exception)) return default(TData);
				var vm       = (IObjectVM)metadata.Parent;if(!ValidateNotNull(vm,"Viewmodel","Parent.Parent","{D8F4FB1A-6C1B-4345-AF2E-A1FF2809A9D0}", out exception)) return default(TData);
				var pvm      = vm.Parent                 ;//if(!ValidateNotNull(pvm,"Viewmodel","Parent.Parent.Parent","{DC61F92C-82AB-4C9A-9E5E-0F31D968B90B}", out exception)) return default(TData);
				if(pvm==null){
//					Debug.WriteLine("WARNING: throw NullReferenceException {602C1F25-9625-4B9C-B8F0-B45069067EC3}"
//						+"\r\n\t"+"ObjectVM.Parent"
//						+"\r\n\t"+vm.GetType().FullName
//						+"\r\n\t"+vm.MemberPath
//					);
					exception= new NullReferenceException("Parent view model is null! UniqueID: {602C1F25-9625-4B9C-B8F0-B45069067EC3}");//TODO
					return default(TData);
				}
				if(!ValidateNotNull(pvm.Metadata,"Metadata","*.Metadata","{FB2701BA-78A8-4E72-811E-6D254188AC91}", out exception)) return default(TData);
				if(!pvm.Metadata.HasDataProvider) {
					exception=new NullReferenceException("Dataprovider ist null! UniqueID: {20BE48F4-B5DF-4B3B-930B-37D019AB9AF6}");
					return default(TData);
				}
				var rootData= pvm.Metadata.DataProvider.TryGetData(out exception);
				if(exception!=null){
					exception=new MemberAccessException("Get data of parent model failed!",exception);
					return default(TData);
				}
				
				if(rootData==null) {
					//Debug.WriteLine("throw NullReferenceException {28A9F70F-5977-49C0-893D-9CEEB95E9066}");
					exception=new NullReferenceException("RootData is null! UniqueID: {28A9F70F-5977-49C0-893D-9CEEB95E9066}");//TODO
					return default(TData);
				}
//				var t = rootData.GetType();
//				var prop = t.GetProperty(_path);
//				var value = prop.GetValue(rootData, null);
				
				var value=new Json(rootData,parse:false,isReadonly:true)[_path].NativeValue;

				exception = null;
				return (TData) value;
			}catch(Exception ex) {
				exception = ex;
				return default(TData);
			}
		}

		object IDataProvider.TryGetData(out Exception exception){return TryGetData(out exception);}

		/// <summary>  Gets or sets the provided data.
		/// </summary>
		/// <value>The provided data.</value>
		public override TData Data {
			get {
				Exception exception;
				var value=TryGetData(out exception);
				if(exception!=null) throw exception;
				return value;
			}
			set {
				if(Equals(value,PreviousData)) return;
				Validate(value);

				var vm = (IObjectVM)((IParentSupport) Parent).Parent;
				var pvm=vm.Parent;
				var rootData = pvm.Metadata.DataProvider.Data;
//				var t = rootData.GetType();
//				var prop = t.GetProperty(_path);
//				prop.SetValue(rootData,value,null);

				new Json(rootData, false, false)[_path] = new Json(value,false,true);

				OnDataChanged(PreviousData, value);
				PreviousData = value;
			}
		} 

		#region Implementation of ICustomDataProvider

		#endregion

		/// <summary> Gets the parent metadata. Alias for <c>(IMetadata)this.Parent</c>
		/// </summary>
		/// <value>
		/// The parent metadata.
		/// </value>
		protected IMetadata ParentMetadata {
			get { return (IMetadata) Parent; }
		}

		/// <summary> Gets the parent view model. Alias for <c>(IObjectVM)this.Parent.Parent</c>
		/// </summary>
		/// <value>
		/// The parent VM.
		/// </value>
		protected IObjectVM ParentVM {
			get {
				var metadata = ParentMetadata; if (metadata == null) return null;
				return (IObjectVM) metadata.Parent;
			}
		}

		/// <summary> Gets the parent of parent view model. Alias for <c>(IObjectVM)this.Parent.Parent.Parent</c>
		/// </summary>
		/// <value>
		/// The parent VM parent.
		/// </value>
		protected IObjectVM ParentVMParent {
			get {
				var pvm = ParentVM; if (pvm == null) return null;
				return pvm.Parent;
			}
		}

	}

	public class ViewModelDataProviderObserver
	{
		private readonly IDataProvider _provider;

		//public ViewModelDataProviderObserver() {}

		public ViewModelDataProviderObserver(IDataProvider provider) {
			_provider = provider;
			_provider.ParentChanged+=AtParentChanged;if(_provider.Parent!=null)AtParentChanged(_provider,EventArgs.Empty);
		}

		private void AtParentChanged(object sender, EventArgs e) {
			if(_provider.Parent==null) return;
			var metadata = (IParentSupport) _provider.Parent;
			metadata.ParentChanged+=AtViewModelChanged; if(metadata.Parent!=null) AtViewModelChanged(metadata,EventArgs.Empty);
		}

		private void AtViewModelChanged(object sender, EventArgs e) {
			var vm = ParentVM;
			vm.ParentChanged+=AtVMParentChanged;if(vm.Parent!=null) AtVMParentChanged(vm,EventArgs.Empty);
		}

		private void AtVMParentChanged(object sender, EventArgs e) {
			var vm = ParentVMParent;
			vm.MetadataChanged+=AtParentVMParentMetadataChanged;
			if (vm.HasMetadata) AtParentVMParentMetadataChanged(vm, EventArgs.Empty);
		}

		private void AtParentVMParentMetadataChanged(object sender, EventArgs eventArgs) {
			ParentVMParent.Metadata.DataProviderChanged+=AtParentVMParentDataProviderChanged;
			if (ParentVMParent.Metadata.HasDataProvider) AtParentVMParentDataProviderChanged(ParentVMParent.Metadata, EventArgs.Empty);
		}

		private void AtParentVMParentDataProviderChanged(object sender, EventArgs eventArgs) {
			//Parent     .Parent     .Parent     .Metadata            .DataProvider
			//{IMetadata}.{IObjectVM}.{IObjectVM}.{IViewModelMetadata}.{IDataProvider}
			//Blickt noch einer durch?^^
			ParentVMParent.Metadata.DataProvider.DataChanged+=AtParentVMDataChanged;
		}

		private void AtParentVMDataChanged(object sender, DataChangedEventArgs e) {
			//Parent     .Parent     .Parent     .Metadata            .DataProvider   .Data
			//{IMetadata}.{IObjectVM}.{IObjectVM}.{IViewModelMetadata}.{IDataProvider}.

			if(ParentVMParentDataChanged!=null) ParentVMParentDataChanged(sender, e);
		}

		/// <summary>
		/// {IProvider}.Parent.Parent.Parent.Metadata.DataProvider.DataChanged
		/// </summary>
		public EventHandler<DataChangedEventArgs> ParentVMParentDataChanged;


		/// <summary> Gets the parent metadata. Alias for <c>(IMetadata)_provider.Parent</c>
		/// </summary>
		/// <value>
		/// The parent metadata.
		/// </value>
		protected IMetadata ParentMetadata {
			get { return (IMetadata) _provider.Parent; }
		}

		/// <summary> Gets the parent view model. Alias for <c>(IObjectVM)_provider.Parent.Parent</c>
		/// </summary>
		/// <value>
		/// The parent VM.
		/// </value>
		protected IObjectVM ParentVM {
			get {
				var metadata = ParentMetadata; if (metadata == null) return null;
				return (IObjectVM) metadata.Parent;
			}
		}

		/// <summary> Gets the parent of parent view model. Alias for <c>(IObjectVM)_provider.Parent.Parent.Parent</c>
		/// </summary>
		/// <value>
		/// The parent VM parent.
		/// </value>
		protected IObjectVM ParentVMParent {
			get {
				var pvm = ParentVM; if (pvm == null) return null;
				return pvm.Parent;
			}
		}
	}

}