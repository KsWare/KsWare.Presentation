using System.ComponentModel;

namespace KsWare.Presentation.ViewModelFramework {

	//
	// Implements "IDataErrorInfo" functionality for ObjectVM

	public partial class ObjectVM : IDataErrorInfo {

		/// <summary> Gets the error message for the property with the given name.
		/// </summary>
		/// <param name="propertyName">The name of the property whose error message to get.</param>
		/// <returns>The error message for the property. The default is an empty string ("").</returns>
		string IDataErrorInfo.this[string propertyName] {
			get {
				var errorProvider = Metadata.ErrorProvider as IDataErrorInfo;
				if (errorProvider == null) return "";
				return errorProvider[propertyName];
			}
		}

		/// <summary> Gets an error message indicating what is wrong with this object.
		/// </summary>
		string IDataErrorInfo.Error {
			get {
				var errorProvider = Metadata.ErrorProvider as IDataErrorInfo;
				if (errorProvider == null) return "";
				return errorProvider.Error;
			}
		}
	}
}
