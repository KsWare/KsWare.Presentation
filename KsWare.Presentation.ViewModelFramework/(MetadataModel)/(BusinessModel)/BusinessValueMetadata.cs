using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides meta data for a <see cref="ValueVM{T}"/> with underlying <see cref="ValueBM{T}"/>
	/// </summary>
	/// <typeparam name="T">Type of value</typeparam>
	public sealed class BusinessValueMetadata<T> : ViewModelMetadata {

		/// <summary> Initializes a new instance of the <see cref="BusinessValueMetadata{T}"/> class.
		/// </summary>
		/// <remarks></remarks>
		public BusinessValueMetadata() {
			base.DataProvider        = CreateDefaultDataProvider();
			base.ErrorProvider       = CreateDefaultErrorProvider();
			base.ValueSourceProvider = CreateDefaultValueSourceProvider();
		}

		/// <summary> Gets or sets the data provider.
		/// </summary>
		/// <value>The data provider.</value>
		/// <remarks></remarks>
		public new BusinessValueDataProvider<T> DataProvider{get {return (BusinessValueDataProvider<T>) base.DataProvider;}}

		/// <summary> Creates the default data provider.
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected override IDataProvider CreateDefaultDataProvider() {return new BusinessValueDataProvider<T>();}

		/// <summary> Creates the default error provider.
		/// </summary>
		/// <returns></returns>
		protected override IErrorProvider CreateDefaultErrorProvider() {return new BusinessValueErrorProvider();}

		/// <summary> Gets the value source provider.
		/// </summary>
		/// <remarks></remarks>
		public new BusinessValueSourceProvider ValueSourceProvider{get {return (BusinessValueSourceProvider) base.ValueSourceProvider;}}

		/// <summary>
		/// Creates the default edit value provider.
		/// </summary>
		/// <returns></returns>
		protected override IValueSourceProvider CreateDefaultValueSourceProvider() {return new BusinessValueSourceProvider();}

		/// <summary> Called when <see cref="ViewModelMetadata.Parent"/>-property has been changed.
		/// This indicates metadata has been assigned to an view model object and all metadata properties are now read-only.
		/// Raises the <see cref="ViewModelMetadata.ParentChanged"/>-event.
		/// </summary>
		/// <remarks></remarks>
		override protected void OnParentChanged() {
			DataProvider.BusinessValueChanged+=OnBusinessValueChanged;
			OnBusinessValueChanged(this,null);//initial request
		}

		/// <summary> Called when this.DataProvider.BusinessValue has been changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="DataChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks></remarks>
		private void OnBusinessValueChanged(object sender, DataChangedEventArgs e) {
			var bv = DataProvider.BusinessValue;
			ValueSourceProvider.BusinessValue=(IValueBM) bv;
		}
	}
}