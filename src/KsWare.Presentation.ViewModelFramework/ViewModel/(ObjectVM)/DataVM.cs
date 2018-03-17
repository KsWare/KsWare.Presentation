using System;
using System.Windows.Data;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TData">The type of the data.</typeparam>
	/// <remarks>When <paramref name="TData"/> is a business object you should use <see cref="BusinessObjectVM{TBusinessObject}"/> or <see cref="BusinessDataVM{TBusinessObject,TData}"/></remarks>
	public class DataVM<TData>:ObjectVM,IDataVM<TData>,IDataVM where TData: class {

		protected const string NoDataToken = "No data";

		/// <summary> Initializes a new instance of the <see cref="DataVM{TData}" /> class.
		/// </summary>
		public DataVM() {
			SetEnabled(NoDataToken, false);
			//BindingDirection.OneWay|TwoWay
			//BindingMode.Default|OneTime|OneWay|OneWayToSource|TwoWay
			//UpdateSourceTrigger.Default|Explicit|LostFocus|PropertyChanged
			UpdateSourceTrigger=UpdateSourceTrigger.Explicit;
		}

		/// <summary> Gets or sets the provided data.
		/// </summary>
		/// <value>The data.</value>
		public TData Data { get => (TData) Metadata.DataProvider.Data; set => Metadata.DataProvider.Data = value; }

		object IDataVM.Data{get => Data; set => Data = (TData) value; }

		/// <summary> Called when Metadata.DataProvider.Data has been changed
		/// </summary>
		/// <param name="e">The <see cref="DataChangedEventArgs" /> instance containing the event data.</param>
		protected override void OnDataChanged(DataChangedEventArgs e) {
			SetEnabled(NoDataToken, e.NewData != null);
			OnPropertyChanged("Data");
		}
		
		/// <summary> [EXPERIMENTAL] Gets or sets a value that determines the timing of binding source (the <see cref="Data"/> property) updates.
		/// </summary>
		/// <value>One of the <see cref="UpdateSourceTrigger"/> values. </value>
		/// <seealso cref="Binding.UpdateSourceTrigger"/>
		public UpdateSourceTrigger UpdateSourceTrigger { get; set; }


		/// <summary> [EXPERIMENTAL] Called when update source. (the <see cref="Data"/> property)
		/// </summary>
		/// <param name="property">The property.</param>
		protected virtual void OnUpdateSource(string property) { }


		/// <summary> [EXPERIMENTAL] Sends the current binding target value to the binding source property in TwoWay or OneWayToSource bindings.
		/// </summary>
		/// <seealso cref="BindingExpression.UpdateSource"/>
		public void UpdateSource() { OnUpdateSource("*"); }

		/// <summary> Provides mapping using the <see cref="MappedDataProvider{TDataRoot,TData}"/>
		/// </summary>
		[Obsolete("DEACTIVETED",true)]
		protected class Map:Map<TData> {}

		/// <summary> Provides mapping using the <see cref="MappedDataProvider{TDataRoot,TData}"/>
		/// </summary>
		[Obsolete("DEACTIVETED",true)]
		protected class Map<TRootData> {

//			public static StringVM StringVM(string propertyName) {
//				return new StringVM {
//					Metadata = new ViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData, String>(propertyName)
//					}
//				};
//			}
//
//			public static StringVM StringVM(string propertyName, ILocalizationProvider localization) {
//				return new StringVM {
//					Metadata = new ViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData, String>(propertyName),LocalizationProvider = localization
//					}
//				};
//			}
//
//			public static BoolVM BoolVM(string propertyName) {
//				return new BoolVM {
//					Metadata = new ViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData, bool>(propertyName)
//					}
//				};
//			}
//
//			public static DoubleVM DoubleVM(string propertyName) {
//				return new DoubleVM {
//					Metadata = new ViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData, double>(propertyName)
//					}
//				};
//			}
//
//			public static Int32VM Int32(string propertyName) {
//				return new Int32VM {
//					Metadata = new ViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData, Int32>(propertyName)
//					}
//				};
//			}
//
//			public static UInt32VM UInt32(string propertyName) {
//				return new UInt32VM {
//					Metadata = new ViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData, UInt32>(propertyName)
//					}
//				};
//			}
//
//			public static GuidVM GuidVM(string propertyName) {
//				return new GuidVM {
//					Metadata = new ViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData, Guid>(propertyName)
//					}
//				};
//			}
//
//			public static EnumVM<TEnum> EnumVM<TEnum>(string propertyName) where TEnum:struct, IComparable, IConvertible, IFormattable {
//				return new EnumVM<TEnum> {
//					Metadata = new ViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData, TEnum>(propertyName)
//					}
//				};
//			}
//
//			public static EnumVM<TEnum> EnumVM<TEnum>(string propertyName, ILocalizationProvider localization) where TEnum:struct, IComparable, IConvertible, IFormattable {
//				return new EnumVM<TEnum> {
//					Metadata = new ViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData, TEnum>(propertyName),
//						LocalizationProvider = localization
//					}
//				};
//			}
//
//			public static ListVM<TItem> ListVM<TItem,TProperty>(string propertyName) where TItem:class, IObjectVM {
//				return new ListVM<TItem > {
//					Metadata = new ListViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData,TProperty>(propertyName)
//					}
//				};
//			}
//
//			public static ListVM<TItem> ListVM<TItem,TProperty>(string propertyName,INewItemProvider newItemProvider) where TItem:class, IObjectVM {
//				return new ListVM<TItem > {
//					Metadata = new ListViewModelMetadata {
//						DataProvider    = new MappedDataProvider<TRootData,TProperty>(propertyName),
//						NewItemProvider = newItemProvider
//					}
//				};
//			}
//
//			public static T Custom<T,TProperty>(string propertyName) where T:IObjectVM,new() {
//				return new T {
//					Metadata = new ListViewModelMetadata {
//						DataProvider = new MappedDataProvider<TRootData,TProperty>(propertyName)
//					}
//				};
//			}

		}
	}

	public abstract class DataVM {

		
	}

}