using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using KsWare.Presentation.ViewModelFramework.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Interface for a list in view model (ListVM)
	/// </summary>
	/// <remarks></remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public interface IListVM<TItem> : IObjectVM, IList<TItem>, IList, INotifyCollectionChanged, INotifyPropertyChanged {}

	public interface IListVM : IObjectVM, IList, INotifyCollectionChanged, INotifyPropertyChanged {}

	public interface IListItemVM {
		object DataRequired();
	}

	partial class ListVM<TItem> {

		private class ItemMap{
			public ItemMap() {}
			public ItemMap(TItem item, object data) {Item = item;Data = data;}
			public TItem Item{get;set;}
			public object Data{get;set;}
		}

	}

	public class RefListVM<T>:ListVM<T> where T:class,IObjectVM {

		public RefListVM() {
			IsReferenceList = true;
		}

		/// <summary> [EXPERIMENTAL] Gets or sets the list of available targets.
		/// </summary>
		/// <value>The list of available targets</value>
		/// <remarks>uses the ValueSourceProvider: <c>Metadata.ValueSourceProvider.SourceList</c></remarks>
		public IList<T> MːSourceList { get { return (IList<T>) Metadata.ValueSourceProvider.SourceList; } set{Metadata.ValueSourceProvider.SourceList=value;} }

	}

	public interface IListViewModelMetadata:IViewModelMetadata {

		IExternalListControllerProvider ExternalControlProvider { get; }
		INewItemProvider NewItemProvider { get; }

	}

	/// <summary> Provides a metadata implementation for lists (<see cref="ListVM{TItem}"/>) 
	/// </summary>
	public class ListViewModelMetadata:ViewModelMetadata,IListViewModelMetadata {

		private INewItemProvider _newItemProvider;
		private IExternalListControllerProvider _externalControlProvider;

		/// <summary> Gets or sets the ExternalControlProvider.
		/// </summary>
		/// <value>The ExternalControlProvider.</value>
		/// <remarks>
		/// The ExternalControlProvider allows to change the logic of this ListVM w/o to write a new ListVM.
		/// </remarks>
		public IExternalListControllerProvider ExternalControlProvider {
			get {return _externalControlProvider;}
			set {
				MemberAccessUtil.DemandWriteOnce(_externalControlProvider==null,null,this,nameof(ExternalControlProvider),"{91AD3CA1-DDE5-47A4-A441-D19CD62CA690}");
				_externalControlProvider = value;
				_externalControlProvider.Parent = this;
			}
		}

		/// <summary> Creates the default external list control provider.
		/// </summary>
		/// <returns></returns>
		protected virtual IExternalListControllerProvider CreateDefaultExternalControlProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var a = propertyInfo.GetCustomAttributes(typeof(ListMetadataAttribute),true).Cast<ListMetadataAttribute>().FirstOrDefault();
					if (a != null) {
						if (a.ExternalControlProvider != null) {
							var provider = (IExternalListControllerProvider)Activator.CreateInstance(a.ExternalControlProvider);
							return provider;
						}				        
					}
				}
			}
			return new ExternalListControllerProvider();
		}

		/// <summary> Gets or sets the <see cref="INewItemProvider"/>.
		/// </summary>
		/// <value>The <see cref="INewItemProvider"/>.</value>
		/// <remarks>
		/// A "NewItemProvider" is provider which provides a method to create a new item. This allows to change the logic how the item is created, w/o to write a new ListVM.
		/// </remarks>
		public INewItemProvider NewItemProvider {
			get {
				if(_newItemProvider==null) {
					_newItemProvider = CreateDefaultNewItemProvider();
					_newItemProvider.Parent = this;
				}
				return _newItemProvider;
			}
			set {
				DemandPropertySet();
				_newItemProvider = value;
				_newItemProvider.Parent = this;
			}
		}

		/// <summary> Creates the default new item provider.
		/// </summary>
		/// <returns></returns>
		protected virtual INewItemProvider CreateDefaultNewItemProvider() {
			if (Reflection != null) {
				if (Reflection.PropertyInfo != null) {
					var propertyInfo = Reflection.PropertyInfo;
					var a = propertyInfo.GetCustomAttributes(typeof(ListMetadataAttribute),true).Cast<ListMetadataAttribute>().FirstOrDefault();
					if (a != null) {
						if (a.NewItemProvider != null) {
							var provider = (INewItemProvider)Activator.CreateInstance(a.NewItemProvider);
							return provider;
						}				        
					}
				}
			}
			return new DefaultNewItemProvider();
		}

	}

	public class ListMetadataAttribute : ViewModelMetadataAttribute {

		private Type _newItemProvider;
		private Type _externalControlProvider;

		/// <summary> Gets or sets the type of external control provider. 
		/// The external control provider must implement <see cref="IExternalListControllerProvider"/>
		/// </summary>
		/// <value>The type of external control provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IExternalListControllerProvider"/>!</exception>
		public Type ExternalControlProvider {
			get { return _externalControlProvider; }
			set {
				if (!typeof (IExternalListControllerProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement IExternalListControllerProvider!");
				_externalControlProvider = value;
			}
		}

		/// <summary> Gets or sets the type of new item provider. 
		/// The new item provider must implement <see cref="INewItemProvider"/>
		/// </summary>
		/// <value>The type of new item provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="INewItemProvider"/>!</exception>
		public Type NewItemProvider {
			get { return _newItemProvider; }
			set {
				if (!typeof (INewItemProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException(nameof(value),"The type does not implement INewItemProvider!");
				_newItemProvider = value;
			}
		}

	}
}
