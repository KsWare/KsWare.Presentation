using System.Collections.Specialized;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IExternalListControllerProvider:IViewModelProvider {

		/// <summary> Gets or sets the collection changing callback.
		/// </summary>
		/// <value>The collection changing callback.</value>
		/// <remarks></remarks>
		NotifyCollectionChangedEventHandler CollectionChangingCallback { get; set; }

		/// <summary> Gets or sets the collection changed callback.
		/// </summary>
		/// <value>The collection changed callback.</value>
		/// <remarks></remarks>
		NotifyCollectionChangedEventHandler CollectionChangedCallback { get; set; }

	}

	/// <summary> Provider to implemnt external logic for a List view model />
	/// </summary>
	/// <remarks></remarks>
	public class ExternalListControllerProvider:ViewModelProvider, IExternalListControllerProvider {

		private NotifyCollectionChangedEventHandler m_CollectionChangedCallback;
		private NotifyCollectionChangedEventHandler m_CollectionChangingCallback;

		/// <summary> Gets a value indicating whether this instance is supported.
		/// </summary>
		/// <remarks></remarks>
		public override bool IsSupported {get {return true;}}

		/// <summary> Gets or sets the collection changing callback.
		/// </summary>
		/// <value>The collection changing callback.</value>
		/// <remarks></remarks>
		public NotifyCollectionChangedEventHandler CollectionChangingCallback {
			get {return this.m_CollectionChangedCallback;}
			set {
				MemberAccessUtil.DemandWriteOnce(m_CollectionChangedCallback==null,null,this,"CollectionChangingCallback","{FD9170EF-4342-4C1E-A42C-E8EBFBC06471}");
				this.m_CollectionChangedCallback = value;
			}
		}

		/// <summary> Gets or sets the collection changed callback.
		/// </summary>
		/// <value>The collection changed callback.</value>
		/// <remarks></remarks>
		public NotifyCollectionChangedEventHandler CollectionChangedCallback {
			get {return this.m_CollectionChangingCallback;}
			set {
				MemberAccessUtil.DemandWriteOnce(m_CollectionChangedCallback==null,null,this,"CollectionChangedCallback","{D3C97757-06BF-4E66-A0A7-96685EE7D14B}");
				this.m_CollectionChangingCallback = value;
			}
		}

	}
}