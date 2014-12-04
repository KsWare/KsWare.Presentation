using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

		private INewItemProvider m_NewItemProvider;
		private IExternalListControllerProvider m_ExternalControlProvider;

		/// <summary> Gets or sets the ExternalControlProvider.
		/// </summary>
		/// <value>The ExternalControlProvider.</value>
		/// <remarks>
		/// The ExternalControlProvider allows to change the logic of this ListVM w/o to write a new ListVM.
		/// </remarks>
		public IExternalListControllerProvider ExternalControlProvider {
			get {return m_ExternalControlProvider;}
			set {
				MemberAccessUtil.DemandWriteOnce(m_ExternalControlProvider==null,null,this,"ExternalControlProvider","{91AD3CA1-DDE5-47A4-A441-D19CD62CA690}");
				m_ExternalControlProvider = value;
				m_ExternalControlProvider.Parent = this;
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
				if(m_NewItemProvider==null) {
					m_NewItemProvider = CreateDefaultNewItemProvider();
					m_NewItemProvider.Parent = this;
				}
				return m_NewItemProvider;
			}
			set {
				DemandPropertySet();
				m_NewItemProvider = value;
				m_NewItemProvider.Parent = this;
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

		private Type m_NewItemProvider;
		private Type m_ExternalControlProvider;

		/// <summary> Gets or sets the type of external control provider. 
		/// The external control provider must implement <see cref="IExternalListControllerProvider"/>
		/// </summary>
		/// <value>The type of external control provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="IExternalListControllerProvider"/>!</exception>
		public Type ExternalControlProvider {
			get { return m_ExternalControlProvider; }
			set {
				if (!typeof (IExternalListControllerProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException("value","The type does not implement IExternalListControllerProvider!");
				m_ExternalControlProvider = value;
			}
		}

		/// <summary> Gets or sets the type of new item provider. 
		/// The new item provider must implement <see cref="INewItemProvider"/>
		/// </summary>
		/// <value>The type of new item provider.</value>
		/// <exception cref="System.ArgumentOutOfRangeException">The type does not implement <see cref="INewItemProvider"/>!</exception>
		public Type NewItemProvider {
			get { return m_NewItemProvider; }
			set {
				if (!typeof (INewItemProvider).IsAssignableFrom(value)) throw new ArgumentOutOfRangeException("value","The type does not implement INewItemProvider!");
				m_NewItemProvider = value;
			}
		}

	}
}
