using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns>Implements a method <see cref="NotifyCollectionChanged"/> to manually raise changed events</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class ObservableNotifyableCollection<T>:ObservableCollection<T>,IDoNotifyCollectionChanged {

		/// <summary> Raises the CollectionChanged event with NotifyCollectionChangedAction.Reset.
		/// </summary>
		public void NotifyCollectionChanged() {
			base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}

	/// <summary>
	/// Interface for collections implementing <see cref="NotifyCollectionChanged"/>
	/// </summary>
	public interface IDoNotifyCollectionChanged {

		/// <summary> Notifies the collection has been changed. This will raise CollectionChanged event.
		/// </summary>
		/// <remarks></remarks>
		void NotifyCollectionChanged();
	}
}
