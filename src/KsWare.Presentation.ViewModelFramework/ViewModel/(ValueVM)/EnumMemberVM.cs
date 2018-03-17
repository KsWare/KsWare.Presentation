using System;
using KsWare.Presentation.ViewModelFramework.Providers;

namespace KsWare.Presentation.ViewModelFramework
{

	public interface IEnumMemberVM:IValueVM
	{
		bool IsChecked { get; set; }
		event EventHandler IsCheckedChanged;
	}

	/// <summary> Provides a view model for a enum member
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EnumMemberVM<T>:ValueVM<T>
	{

		private bool _isChecked;

		/// <summary> Initializes a new instance of the <see cref="EnumMemberVM&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <remarks></remarks>
		public EnumMemberVM(T value) {
			Metadata=new ViewModelMetadata {
				DisplayValueProvider = new EnumDisplayValueProvider()
			};
			((EnumDisplayValueProvider)Metadata.DisplayValueProvider).SetValue(value.ToString());
			base.Value=value;
		}

		/// <summary> Gets the native enum value.
		/// </summary>
		/// <value>The value.</value>
		/// <remarks>Enum members are read-only. Set the value will throw a <see cref="InvalidOperationException"/>. </remarks>
		/// <exception cref="InvalidOperationException">Value ist allways read-only for enum members</exception>
		public override T Value { get => base.Value; set => throw new InvalidOperationException("Can not set readonly enum member value!"); }

		public bool IsChecked {
			get => _isChecked;
			set {
				if(Equals(_isChecked,value))return;
				_isChecked = value;
				OnPropertyChanged("IsChecked");
				EventUtil.Raise(IsCheckedChanged,this,EventArgs.Empty,"{057D44DD-0BB5-4347-8CDB-4399758B7F1F}");
			}
		}

		public event EventHandler IsCheckedChanged;

//		public override string ToString() {
//			return Metadata.DisplayValueProvider.AsString;
//		}
	}
}