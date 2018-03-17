using KsWare.Presentation.BusinessFramework.Providers;
using KsWare.Presentation.Core.Patterns;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides metadata for business list
	/// </summary>
	public class BusinessListMetadata:BusinessMetadata {

		private INewItemProvider _newItemProvider;
		private IListLogicProvider _logicProvider;

		/// <summary> Initializes a new instance of the <see cref="BusinessListMetadata"/> class.
		/// </summary>
		public BusinessListMetadata() {}

		/// <summary> Gets or sets the new item provider.
		/// </summary>
		/// <value>The new item provider.</value>
		public INewItemProvider NewItemProvider {
			get {
				if(this._newItemProvider==null) {
					if (this._newItemProvider == null) {
						//LOG					Debug.WriteLine("=>WARNING: Create default NewItemProvider!");
						this._newItemProvider = new DefaultNewItemProvider();
					}
				}
				return this._newItemProvider;
			}
			set {
				DemandWrite();
				SetPropertyWithParentPattern.Execute(ref this._newItemProvider, value, this);
			}
		}

		/// <summary> Gets or sets the list logic provider.
		/// </summary>
		/// <value>The list logic provider.</value>
		public IListLogicProvider LogicProvider {
			get => this._logicProvider;
			set {
				DemandWrite();
				SetPropertyWithParentPattern.Execute(ref this._logicProvider, value, this);
			}
		}
	}
}