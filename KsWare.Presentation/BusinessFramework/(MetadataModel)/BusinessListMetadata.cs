using System.Collections.Specialized;
using System.Diagnostics;
using KsWare.Presentation.BusinessFramework.Providers;
using KsWare.Presentation.Providers;
using KsWare.Presentation.Core.Patterns;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides metadata for business list
	/// </summary>
	public class BusinessListMetadata:BusinessMetadata {

		private INewItemProvider m_NewItemProvider;
		private IListLogicProvider m_LogicProvider;

		/// <summary> Initializes a new instance of the <see cref="BusinessListMetadata"/> class.
		/// </summary>
		public BusinessListMetadata() {}

		/// <summary> Gets or sets the new item provider.
		/// </summary>
		/// <value>The new item provider.</value>
		public INewItemProvider NewItemProvider {
			get {
				if(this.m_NewItemProvider==null) {
					if (this.m_NewItemProvider == null) {
						//LOG					Debug.WriteLine("=>WARNING: Create default NewItemProvider!");
						this.m_NewItemProvider = new DefaultNewItemProvider();
					}
				}
				return this.m_NewItemProvider;
			}
			set {
				DemandWrite();
				SetPropertyWithParentPattern.Execute(ref this.m_NewItemProvider, value, this);
			}
		}

		/// <summary> Gets or sets the list logic provider.
		/// </summary>
		/// <value>The list logic provider.</value>
		public IListLogicProvider LogicProvider {
			get {return this.m_LogicProvider;}
			set {
				DemandWrite();
				SetPropertyWithParentPattern.Execute(ref this.m_LogicProvider, value, this);
			}
		}
	}
}