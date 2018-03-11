using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides a view-model for Enum
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks></remarks>
	/// <example>
	/// Usage with ComboBox: <br/><br/>
	/// <code>
	/// MyEnumViewModel = new EnumVM&lt;SweepDirection&gt;();
	/// </code>
	/// <b>XAML</b><hr/>
	/// <c>
	/// &lt;ComboBox
	///	  SelectedValue     = "{Binding MyEnumViewModel.Value}" 
	///	  SelectedValuePath = "Value" 
	///	  ItemsSource       = "{Binding MyEnumViewModel.ValueSourceProvider.SourceList}" 
	///	  DisplayMemberPath = "Value" 
	/// /&gt;</c>
	/// </example>
	public class EnumVM<T>:ValueVM<T> where T:/*Enum ValueType*/struct,IComparable,IConvertible,IFormattable {

		private readonly Type BaseType;
		private readonly bool HasFlags;
		private bool _isUpdatingValues;
		private ReadOnlyCollection<EnumMemberVM<T>> _values;
		private bool _isValues;
		private List<EnumMemberVM<T>> _valuesBase;

		public EnumVM() {
			if(!typeof(T).IsEnum) throw new InvalidOperationException("Invalid type!\n\tErrorID: {37C628AD-AC98-46C4-BB49-FA72BA0CDE7B}");
			BaseType = Enum.GetUnderlyingType(typeof(T));
			HasFlags = typeof (T).GetCustomAttributes(typeof (FlagsAttribute), false).Length > 0;
		}

		private ReadOnlyCollection<EnumMemberVM<T>> CreateValues() {
			_valuesBase = EnumVM.CreateValues<T>();
			foreach (var vm in _valuesBase) {
				vm.SetEnabled("No Data", false);
				vm.IsCheckedChanged+=AtValueIsCheckedChanged;
			}
			return _valuesBase.AsReadOnly();
		}

		public void SetValuesFromString(params string[] args) {
			_valuesBase = EnumVM.CreateValuesFromString<T>(args);
			foreach (var vm in _valuesBase) {
				vm.SetEnabled("No Data", false);
				vm.IsCheckedChanged+=AtValueIsCheckedChanged;
			}
			Values= _valuesBase.AsReadOnly();
		}

		public void SetValues(params object[] args) {
			_valuesBase = EnumVM.CreateValues<T>(args);
			foreach (var vm in _valuesBase) {
				vm.SetEnabled("No Data", false);
				vm.IsCheckedChanged+=AtValueIsCheckedChanged;
			}
			Values= _valuesBase.AsReadOnly();
		}

		protected override void Dispose(bool explicitDisposing) {
			if(explicitDisposing) {
				if(_valuesBase!=null) {
					foreach(var vm in _valuesBase) {
						vm.IsCheckedChanged -= AtValueIsCheckedChanged;
						vm.Dispose();
					}
					_valuesBase = null;
					_values = null;
				}
			}
			base.Dispose(explicitDisposing);
		}

		//TODO Values docu
		public ReadOnlyCollection<EnumMemberVM<T>> Values {
			get {
				if(_values==null) _values=CreateValues();
				if (!_isValues) {UpdateValues();_isValues = true;} //TODO revise _isValues
				return _values;
			}
			private set { _values = value; }
		}

		private void UpdateValues() {
			if(!HasMetadata) throw new NotImplementedException("{A9D7CB5D-6C07-439A-8B64-8CD57E6F61AC}");
			if(!Metadata.HasDataProvider) throw new NotImplementedException("{81FA2D0F-740D-4DF6-845C-DE4AD9553283}");
			Exception exception;
			var data = Metadata.DataProvider.TryGetData(out exception);
			UpdateValues(data);
		}

		private void UpdateValues(object data) {
			_isUpdatingValues = true;
			if(_values==null) _values = CreateValues();
			if (data == null || data==DBNull.Value) {
				foreach (var emvm in _values) {
					emvm.SetEnabled("No Data", false);
					emvm.IsChecked = false;
				}
			} else {
				var value = (T) data;
				if (HasFlags) {
					foreach (var emvm in _values) {
						emvm.SetEnabled("No Data", true);
						emvm.IsChecked = IsChecked(value, emvm.Value);
					}
				} else {
					foreach (var emvm in _values) {
						emvm.SetEnabled("No Data", true);
						emvm.IsChecked = Equals(value,emvm.Value);
					}
				}
				
			}
			_isUpdatingValues = false;
		}	

		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);
			UpdateValues(e.NewData);
		}

//		private void AtValueChanged(object sender, EventArgs e) {
//			var value = Value;
//			if(HasFlags){
//				foreach (var emvm in _Values) {
//					emvm.IsChecked = IsChecked(value, emvm.Value);
//				}
//			} else {
//				foreach (var emvm in _Values) {
//					emvm.IsChecked = Equals(value, emvm.Value);
//				}
//			}
//		}

		private void AtValueIsCheckedChanged(object sender, EventArgs e) {
			if(_isUpdatingValues)return;

			var newValue = (T)Enum.ToObject(typeof(T),0);
			if (HasFlags) {
				foreach (var emvm in _values) {
					if (emvm.IsChecked) newValue = BinaryOr(newValue, emvm.Value);
				}
			} else {
				var em = ((EnumMemberVM<T>) sender);
				if (em.IsChecked) {
					newValue = em.Value;
					_isUpdatingValues = true;
					//Uncheck all other
					foreach (var emvm in _values) {if(emvm.IsChecked && !Equals(emvm.Value,newValue)) emvm.IsChecked=false;}
				} else {
					var defaultValue = (T)Enum.ToObject(typeof(T), 0);
					_isUpdatingValues = true;
					var dispatcher = ApplicationDispatcher.CurrentDispatcher;
					if(_values.Any(vm => Equals(vm.Value,defaultValue))){
						dispatcher.BeginInvoke(() => { em.IsChecked = true; _isUpdatingValues = false; });
					} else 	if(Equals(em.Value,defaultValue)) dispatcher.Invoke(new Action(delegate {
						em.IsChecked = true;
						_isUpdatingValues = false;
					}));
				}
			}
			
			if(Equals(Value,newValue))return;
			Value = newValue;
		}



		/// <summary> Creates the default metadata.
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected override ViewModelMetadata CreateDefaultMetadata() {
			var metadata = base.CreateDefaultMetadata();

			var ls = new List<EnumMemberVM<T>>();

			foreach (T value in Enum.GetValues(typeof(T))) {
				ls.Add(new EnumMemberVM<T>(value));
			}
			metadata.ValueSourceProvider.SourceList = ls;

			return metadata;
		}

		private bool IsChecked(T v1, T v2) {
			if (BaseType == typeof (Int32 )) return (Convert.ToInt32 (v1) & Convert.ToInt32 (v2)) != 0;
			if (BaseType == typeof (Int16 )) return (Convert.ToInt16 (v1) & Convert.ToInt16 (v2)) != 0;
			if (BaseType == typeof (Int64 )) return (Convert.ToInt64 (v1) & Convert.ToInt64 (v2)) != 0;
			if (BaseType == typeof (Byte  )) return (Convert.ToByte  (v1) & Convert.ToByte  (v2)) != 0;
			if (BaseType == typeof (SByte )) return (Convert.ToSByte (v1) & Convert.ToSByte (v2)) != 0;
			if (BaseType == typeof (UInt32)) return (Convert.ToUInt32(v1) & Convert.ToUInt32(v2)) != 0;
			if (BaseType == typeof (UInt16)) return (Convert.ToUInt16(v1) & Convert.ToUInt16(v2)) != 0;
			if (BaseType == typeof (UInt64)) return (Convert.ToUInt64(v1) & Convert.ToUInt64(v2)) != 0;
			throw new NotSupportedException("{3070B225-C532-4955-9184-C66409131A4B}");
		}

		private T BinaryOr(T v1, T v2) {
			if (BaseType == typeof (Int32 )) return (T)Enum.ToObject(typeof(T), Convert.ToInt32 (v1) | Convert.ToInt32 (v2));
			if (BaseType == typeof (Int16 )) return (T)Enum.ToObject(typeof(T), Convert.ToInt16 (v1) | Convert.ToInt16 (v2));
			if (BaseType == typeof (Int64 )) return (T)Enum.ToObject(typeof(T), Convert.ToInt64 (v1) | Convert.ToInt64 (v2));
			if (BaseType == typeof (Byte  )) return (T)Enum.ToObject(typeof(T), Convert.ToByte  (v1) | Convert.ToByte  (v2));
			if (BaseType == typeof (SByte )) return (T)Enum.ToObject(typeof(T), Convert.ToSByte (v1) | Convert.ToSByte (v2));
			if (BaseType == typeof (UInt32)) return (T)Enum.ToObject(typeof(T), Convert.ToUInt32(v1) | Convert.ToUInt32(v2));
			if (BaseType == typeof (UInt16)) return (T)Enum.ToObject(typeof(T), Convert.ToUInt16(v1) | Convert.ToUInt16(v2));
			if (BaseType == typeof (UInt64)) return (T)Enum.ToObject(typeof(T), Convert.ToUInt64(v1) | Convert.ToUInt64(v2));
			throw new NotSupportedException("{3070B225-C532-4955-9184-C66409131A4B}");
		}

		public override string ToString() {
			return string.Format("{{Value:{0} {{{1}<{2}>}}{3}}}", Value, this.GetType().Name, typeof(T).Name, HasFlags?"[Flags]":"");
		}
	}

	/// <summary> Provides functions for all EnumVMs
	/// </summary>
	public static class EnumVM
	{

		private static readonly Dictionary<object,IValueVM> __cache=new Dictionary<object, IValueVM>();

		/// <summary> Gets the VM object from the specified enum value.
		/// </summary>
		/// <param name="value">The enum value.</param>
		/// <returns>The matching EnumMemberVM</returns>
		/// <remarks></remarks>
		/// <exception cref="ArgumentException">Value is not an enum value!</exception>
		private static IValueVM GetVMInternal(object value) { 
			if(!value.GetType().IsEnum) throw new ArgumentException("Value is not an enum value!",nameof(value));
			
			if(!__cache.ContainsKey(value)) {
				var vm = CreateVMInternal(value);
				__cache.Add(value,vm);
				return vm;
			}
			return __cache[value];
		}

		/// <summary> Creates the VM object from the specified enum value.
		/// </summary>
		/// <param name="value">The enum value.</param>
		/// <returns>The matching EnumMemberVM</returns>
		/// <remarks></remarks>
		/// <exception cref="ArgumentException">Value is not an enum value!</exception>
		private static IValueVM CreateVMInternal(object value) {
			if(!value.GetType().IsEnum) throw new ArgumentException("Value is not an enum value!",nameof(value));

			var vmType = typeof (EnumMemberVM<>).MakeGenericType(value.GetType());
			var vm = (IValueVM)Activator.CreateInstance(vmType, value);
			SetDisplayValue(vm);
			return vm;
		}

		public static object GetVM(Enum value) {return GetVMInternal(value);}
		public static object CreateVM(Enum value) {return CreateVMInternal(value);}
		public static EnumMemberVM<T> GetVM<T>(T value) {return (EnumMemberVM<T>)GetVMInternal(value);}
		public static EnumMemberVM<T> CreateVM<T>(T value) {return (EnumMemberVM<T>)CreateVMInternal(value);}

		public static void SetDisplayValue(IValueVM vm) {
			var value = vm.Value;
			var displayValue = GetDescriptionFromAttribute(value);
			if(string.IsNullOrEmpty(displayValue)) displayValue = value.ToString();
			((EnumDisplayValueProvider)vm.DisplayValueProvider).SetValue(displayValue);
		}

		/// <summary> Updates all enum properties of the specified object.
		/// </summary>
		/// <param name="objectVM">The object VM.</param>
		/// <remarks></remarks>
		public static void UpdateEnums(ObjectVM objectVM) {
			if (objectVM == null) throw new ArgumentNullException(nameof(objectVM));

			var enumVMs = (from child in objectVM.Children
						   where child.GetType().IsGenericType && child.GetType().GetGenericTypeDefinition() == typeof(EnumVM<>)
						   select child).ToList();

			foreach (var vm in enumVMs) {
				var provider=((IValueVM) vm).ValueSourceProvider;
				if (provider is BusinessValueSourceProvider) {
#pragma warning disable 612,618
					((BusinessValueSourceProvider)((IValueVM)vm).ValueSourceProvider).NotifySourceListChanged();
#pragma warning restore 612,618
				} 
//				else if (provider is ValueSourceProvider) {
//					//TODO?
//					((ValueSourceProvider)((IValueVM)vm).ValueSourceProvider).NotifySourceListChanged();
//				}
				
			}
		}

		public static string GetDescriptionFromAttribute(object value) {
			var t = value.GetType();
			var memberInfos = t.GetMember(value.ToString());
			var attributes = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			if(attributes.Length==0) return null;
			var description = (DescriptionAttribute)attributes[0];
			return description.Description;
		}

		public static List<EnumMemberVM<T>> GetValues<T>() {
			var ls = new List<EnumMemberVM<T>>();
//			var t = typeof(T);
			foreach (T value in Enum.GetValues(typeof(T))) {
				var vm = GetVM<T>(value);
				ls.Add(vm);
			}
			return ls;
		}

		public static List<EnumMemberVM<T>> CreateValues<T>() {
			var ls = new List<EnumMemberVM<T>>();
//			var t = typeof(T);
			foreach (T value in Enum.GetValues(typeof(T))) {
				var vm = new EnumMemberVM<T>(value);
				SetDisplayValue(vm);
				ls.Add(vm);
			}
			return ls;
		}

		public static List<EnumMemberVM<T>> GetValuesFromString<T>(params string[] args) {
			var list= new List<EnumMemberVM<T>>();
			foreach(var s in args) {
				list.Add(GetVM<T>((T)Enum.Parse(typeof(T),s)));
			}
			return list;
		}

		public static List<EnumMemberVM<T>> CreateValuesFromString<T>(string[] enumMemberNames) {
			var list= new List<EnumMemberVM<T>>();
			foreach(var s in enumMemberNames) {
				list.Add(CreateVM<T>((T)Enum.Parse(typeof(T),s)));
			}
			return list;
		}

		public static List<EnumMemberVM<T>> CreateValues<T>(object[] enumMembers) {
			var list= new List<EnumMemberVM<T>>();
			foreach(var s in enumMembers) {
				list.Add(CreateVM<T>((T)s));
			}
			return list;
		}

	}
}
