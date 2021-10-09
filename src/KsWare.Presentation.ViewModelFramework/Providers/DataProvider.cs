using System;
using System.Diagnostics;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	/// <summary> Interface for data provider for a <see cref="BusinessValue"/>
	/// </summary>
	public interface IBusinessValueDataProvider:IDataProvider {

		/// <summary> Gets or sets the business value.
		/// </summary>
		/// <value>The business value.</value>
		IValueBM BusinessValue{get;set;}

		/// <summary> Occurs when <see cref="BusinessValue"/> property has been changed.
		/// </summary>
		event EventHandler<DataChangedEventArgs> BusinessValueChanged;
	}

	/// <summary> Data provider for a <see cref="BusinessValue"/>
	/// </summary>
	/// <typeparam name="T">ValueBM-type</typeparam>
	public class BusinessValueDataProvider<T>:DataProvider<T>, IBusinessValueDataProvider {

		private IValueBM<T> _businessValue;

		/// <summary> Initializes a new instance of the <see cref="BusinessValueDataProvider{T}"/> class.
		/// </summary>
		public BusinessValueDataProvider() { }

		#region IViewModelProvider

		/// <summary> Gets a value indicating whether this instance is supported.
		/// </summary>
		/// <remarks></remarks>
		public override bool IsSupported => true;

		#endregion

		#region Implementation of IDataProvider<T>

		/// <summary> Gets or sets the value provided by underlying <see cref="IValueBM"/>.
		/// </summary>
		/// <value>The value provided by underlying <see cref="IValueBM"/>.</value>
		public override T Data {
			get {
				DemandValidBusinessObject();
				return _businessValue.Value;
			}
			set {
				Validate(value);
				DemandValidBusinessObject();
				_businessValue.Value=value;

				// NOT REQUIERED: _BusinessValue raises a event -> 
				// var previousData = _BusinessValue.Value;
				// _BusinessValue.Value=value;
				// AtDataChanged(previousData,value);
			}
		}

		/// <summary> Validates the specified data.
		/// </summary>
		/// <param name="data">The data to be validated.</param>
		/// <returns><see langword="true"/> is the data is valid; else see langword="false"/></returns>
		public override Exception Validate(T data) {
			DemandValidBusinessObject();

			var result = base.Validate(data);
			if(result!=null) 
				return result;

			return _businessValue.Validate(data, true);
		}

		#endregion

		#region explicit Implementation of IDataProvider

		Exception IDataProvider.Validate(object data) { return Validate((T) data); }

		#endregion

		/// <summary> Gets or sets the underlying <see cref="IValueBM"/>.
		/// </summary>
		/// <value>The underlying <see cref="IValueBM"/>.</value>
		public IValueBM<T> BusinessValue {
			get => _businessValue;
			set {
				var previousBusinessValue = _businessValue;
				var previousData = BusinessValue == null || !BusinessValue.HasValue ? default(T) : BusinessValue.Value;

				if(_businessValue!=null) {
					_businessValue.ValueChanged       -=AtBusinessValueOnValueChanged;
					_businessValue.IsApplicableChanged-=AtBusinessValueIsApplicableChanged;
				}
				
				_businessValue=value;
				
				if(_businessValue!=null) {
					_businessValue.ValueChanged       +=AtBusinessValueOnValueChanged;
					_businessValue.IsApplicableChanged+=AtBusinessValueIsApplicableChanged; AtBusinessValueIsApplicableChanged(null,null);
				}
				EventUtil.Raise(BusinessValueChanged,this,new DataChangedEventArgs(previousBusinessValue,_businessValue),"{21F1DD1C-BBA4-4673-A414-2D15B9DC6358}");
				
				var newData = BusinessValue == null || !BusinessValue.HasValue ? default(T) : BusinessValue.Value;
				if(!Equals(newData,previousData)) OnDataChanged(previousData,newData);
			}
		}

		private void AtBusinessValueOnValueChanged(object sender, ValueChangedEventArgs e) {
			OnDataChanged(e.OldValue,e.NewValue);
		}

		/// <summary> Gets the parent view model.
		/// </summary>
		/// <value>
		/// The parent view model.
		/// </value>
		private ObjectVM ParentVM{
			get {
				var metadata = (ViewModelMetadata) Parent         ; if(metadata==null) return null;
				var vm       = (ObjectVM         ) metadata.Parent; if(vm==null      ) return null;
				
				return vm;
			}
		}

		private void AtBusinessValueIsApplicableChanged(object sender, EventArgs eventArgs) {
			if(ParentVM==null           ) return;
			if(_businessValue==null) return;

			bool isApplicable = _businessValue.IsApplicable;
			ParentVM.SetEnabled("BusinessObject is not applicable", isApplicable);
		}

		IValueBM IBusinessValueDataProvider.BusinessValue {get => (IValueBM) BusinessValue; set => BusinessValue=(IValueBM<T>) value; }

		/// <summary> Occurs when the <see cref="BusinessValue"/>-property has been changed.
		/// </summary>
		public event EventHandler<DataChangedEventArgs> BusinessValueChanged;

		public override T TryGetData(out Exception exception) {
			exception = null;
			if (_businessValue == null) {
			    var vm = ((ViewModelMetadata)Parent).Parent;
			    string ex = "Underlying business object is null!" +
			        "\n\t" + "Type: " + DebugUtil.FormatTypeFullName(vm) +
			        "\n\t" + "Path: " + vm.MemberPath;
			    Debug.WriteLine("=> " + ex);
			    exception = new InvalidOperationException(ex);
			}

			return _businessValue != null ? _businessValue.Value : default(T);
		}

		/// <summary> Demands the business object is valid.
		/// </summary>
		/// <exception cref="InvalidOperationException">The business object is not valid!</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private void DemandValidBusinessObject() { 
			if(_businessValue==null) {
				var vm = ((ViewModelMetadata) Parent).Parent;
				string ex = "Underlying business object is null!"+
					"\n\t"+"Type: "+DebugUtil.FormatTypeFullName(vm) +
					"\n\t" + "Path: " + vm.MemberPath;
				
				Debug.WriteLine("=> " + ex);
				throw new InvalidOperationException(ex);
			}
//TODO		if(_BusinessValue.IsDisposed==null) throw new InvalidOperationException("Underlying business object is disposed!");
		}
	}

	/// <summary> [RESERVED]
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BusinessObjectDataProvider<T>:CustomDataProvider<T> {

		//like a BusinessValueDataProvider but with business objects

		/// <summary> Initializes a new instance of the <see cref="BusinessObjectDataProvider&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="getter">The getter.</param>
		/// <param name="setter">The setter.</param>
		/// <remarks></remarks>
		public BusinessObjectDataProvider(Func<T> getter, Action<T> setter): base(getter, setter) {}
	}

}
