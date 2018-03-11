//RESERVED: class CollectionBM
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Text;
//using KsWare.Presentation.Providers;
//
//namespace KsWare.Presentation.BusinessFramework 
//{
//
//	/// <summary>
//	/// The business object for the collections
//	/// </summary>
//	/// <typeparam name="T"></typeparam>
//	public class CollectionBM<T>:ObjectBM,IEnumerable<T>, INotifyCollectionChanged 
//	{
//
//		/// <summary>
//		/// Initializes a new instance of the <see cref="CollectionBM&lt;T&gt;"/> class.
//		/// </summary>
//		public CollectionBM() {
//			Metadata=new BusinessMetadata {
//				DataProvider = new LocalDataProvider()
//			};
//
//		}
//
//		private IEnumerable<T> Data {get {return (IEnumerable<T>) this.Metadata.DataProvider.Data;}}
//
//		/// <summary> Gets or sets the items.
//		/// </summary>
//		/// <value>The items.</value>
//		public IEnumerable<T> Items{get;private set;}
//
//		/// <summary>  Occures when the Metadata.DataProvider.Data has been changed
//		/// </summary>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		protected override void AtDataChanged(object sender, Presentation.Providers.DataChangedEventArgs e) {
//			base.AtDataChanged(sender, e);
//			OnCollectionChanged();
//		}
//
//		private void OnCollectionChanged() { 
//			//if(CollectionChanged==null)CollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
//			EventUtil.Raise(CollectionChanged,this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),"{0D1582F8-2818-4994-B963-C93A6CF780EF}");
//		}
//
//		/// <summary>
//		/// Occurs when the collection changes.
//		/// </summary>
//		public event NotifyCollectionChangedEventHandler CollectionChanged;
//
//		/// <summary>
//		/// Returns an enumerator that iterates through the collection.
//		/// </summary>
//		/// <returns>
//		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
//		/// </returns>
//		/// <filterpriority>1</filterpriority>
//		public IEnumerator<T> GetEnumerator() { return Items.GetEnumerator(); }
//
//		/// <summary>
//		/// Returns an enumerator that iterates through a collection.
//		/// </summary>
//		/// <returns>
//		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
//		/// </returns>
//		/// <filterpriority>2</filterpriority>
//		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
//	}
//}
