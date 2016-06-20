using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Provides function for all ValueVMs
	/// </summary>
	public static class ValueVM {

		private class MapItem {public MapItem(Type vm, Type bm, Type dm) {VM = vm;BM = bm;DM = dm;}public Type VM { get; set; }public Type BM { get; set; }public Type DM { get; set; }}
		private static MapItem M<TVM, TBM, TDM>() {return new MapItem(typeof(TVM),typeof(TBM),typeof(TDM));}
		private static readonly List<MapItem>  TypeMap = new List<MapItem>{
			M<StringVM  ,StringBM  ,String  >(),
			M<BoolVM    ,BoolBM    ,Boolean >(),
			M<DoubleVM  ,DoubleBM  ,Double  >(),
			M<SingleVM  ,SingleBM  ,Single  >(),
			M<ByteVM    ,ByteBM    ,Byte    >(),
			M<Int16VM   ,Int16BM   ,Int16   >(),
			M<Int32VM   ,Int32BM   ,Int32   >(),
			M<Int64VM   ,Int64BM   ,Int64   >(),
			M<GuidVM    ,GuidBM    ,Guid    >(),
			M<TimeSpanVM,TimeSpanBM,TimeSpan>(),
			M<DateTimeVM,DateTimeBM,DateTime>(),
		};

		public static Type GetDataType(Type type) {
			foreach (var item in TypeMap) {
				if(item.VM==type) return item.DM;
				if(item.BM==type) return item.DM;
				if(item.DM==type) return item.DM;
			}
			return null;
		}
		public static Type GetBusinessType(Type type) {
			foreach (var item in TypeMap) {
				if(item.VM==type) return item.BM;
				if(item.BM==type) return item.BM;
				if(item.DM==type) return item.BM;
			}
			return null;
		}
		public static Type GetViewModelType(Type type) {
			foreach (var item in TypeMap) {
				if(item.VM==type) return item.VM;
				if(item.BM==type) return item.VM;
				if(item.DM==type) return item.VM;
			}
			return null;
		}
		public static bool IsValueVM(Type type) {
			foreach (var item in TypeMap) {
				if (item.VM == type) return true;
			}
			return false;
		}
		
	}

	/// <summary> Provides a view-model for <see cref="String"/>
	/// </summary>
	/// <remarks></remarks>
	public class StringVM:ValueVM<string> {

		/// <summary> Initializes a new instance of the <see cref="StringVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public StringVM() {}

	}

	/// <summary> Provides a view-model for <see cref="Byte"/>
	/// </summary>
	/// <remarks></remarks>
	public class NumericVM<T>:ValueVM<T> {

		/// <summary> Initializes a new instance of the <see cref="ByteVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		protected NumericVM() {
			SetMinMaxValue();
			
		}

		private void SetMinMaxValue() {
			switch (typeof(T).Name) {
				case "Byte"   : MinValue = (T)(object)Byte   .MinValue; MaxValue = (T)(object)Byte   .MaxValue; break;
				case "Int16"  : MinValue = (T)(object)Int16  .MinValue; MaxValue = (T)(object)Int16  .MaxValue; break;
				case "Int32"  : MinValue = (T)(object)Int32  .MinValue; MaxValue = (T)(object)Int32  .MaxValue; break;
				case "Int64"  : MinValue = (T)(object)Int64  .MinValue; MaxValue = (T)(object)Int64  .MaxValue; break;
				case "SByte"  : MinValue = (T)(object)Byte   .MinValue; MaxValue = (T)(object)SByte  .MaxValue; break;
				case "UInt16" : MinValue = (T)(object)UInt16 .MinValue; MaxValue = (T)(object)UInt16 .MaxValue; break;
				case "UInt32" : MinValue = (T)(object)UInt32 .MinValue; MaxValue = (T)(object)UInt32 .MaxValue; break;
				case "UInt64" : MinValue = (T)(object)UInt64 .MinValue; MaxValue = (T)(object)UInt64 .MaxValue; break;
				case "Single" : MinValue = (T)(object)Single .MinValue; MaxValue = (T)(object)Single .MaxValue; break;
				case "Double" : MinValue = (T)(object)Double .MinValue; MaxValue = (T)(object)Double .MaxValue; break;
				case "Decimal": MinValue = (T)(object)Decimal.MinValue; MaxValue = (T)(object)Decimal.MaxValue; break;
			}
		}

		public T MinValue{get { return Fields.Get<T>("MinValue"); } set{Fields.Set("MinValue",value);}}
		public T MaxValue{get { return Fields.Get<T>("MaxValue"); } set{Fields.Set("MaxValue",value);}}
	}

	/// <summary> Provides a view-model for <see cref="Byte"/>
	/// </summary>
	/// <remarks></remarks>
	public class ByteVM:NumericVM<Byte> {

		/// <summary> Initializes a new instance of the <see cref="ByteVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public ByteVM() {
			
		}
	}

	/// <summary> Provides a view-model for <see cref="Int16"/>
	/// </summary>
	/// <remarks></remarks>
	public class Int16VM:NumericVM<Int16> {

		/// <summary> Initializes a new instance of the <see cref="Int16VM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public Int16VM() {
			
		}
	}

	/// <summary> Provides a view-model for <see cref="Int32"/>
	/// </summary>
	/// <remarks></remarks>
	public class Int32VM:NumericVM<Int32> {

		/// <summary> Initializes a new instance of the <see cref="Int32VM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public Int32VM() {
			
		}
	}

	/// <summary> Provides a view-model for <see cref="Byte"/>
	/// </summary>
	/// <remarks></remarks>
	public class SByteVM:NumericVM<SByte> {

		/// <summary> Initializes a new instance of the <see cref="ByteVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public SByteVM() {
			
		}
	}

		/// <summary> Provides a view-model for <see cref="UInt16"/>
	/// </summary>
	/// <remarks></remarks>
	public class UInt16VM : NumericVM<ushort> {

		/// <summary>
		/// Initializes a new instance of the <see cref="UInt16VM"/> class.
		/// </summary>
		public UInt16VM() {
			
		}

	}

	/// <summary> Provides a view-model for <see cref="UInt32"/>
	/// </summary>
	/// <remarks></remarks>
	public class UInt32VM : NumericVM<uint> {

		/// <summary>
		/// Initializes a new instance of the <see cref="UInt32VM"/> class.
		/// </summary>
		public UInt32VM() {
			
		}

	}

	/// <summary> Provides a view-model for <see cref="Int64"/>
	/// </summary>
	/// <remarks></remarks>
	public class Int64VM:NumericVM<Int64> {

		/// <summary> Initializes a new instance of the <see cref="Int64VM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public Int64VM() {
			
		}

	}

	/// <summary> Provides a view-model for <see cref="Int64"/>
	/// </summary>
	/// <remarks></remarks>
	public class UInt64VM:NumericVM<UInt64> {

		/// <summary> Initializes a new instance of the <see cref="UInt64VM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public UInt64VM() {
			
		}
	}

	/// <summary> Provides a view-model for <see cref="Single"/>
	/// </summary>
	/// <remarks></remarks>
	public class SingleVM:NumericVM<Single> {

		/// <summary> Initializes a new instance of the <see cref="SingleVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public SingleVM() {
			
		}

		
	}

	/// <summary> Provides a view-model for <see cref="Double"/>
	/// </summary>
	/// <remarks></remarks>
	public class DoubleVM:NumericVM<Double> {

		/// <summary> Initializes a new instance of the <see cref="DoubleVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public DoubleVM() {
			
		}

		//REVIEW [DRAFT]
		//### EventBehavior

		/// <summary> Called by EventBehavior
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sender")]
		[UsedImplicitly]
		//public void AtPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
		public void AtPreviewKeyDown(object sender) {
			// Called by EventBehavior
		}

	}

	/// <summary> Provides a view-model for <see cref="Decimal"/>
	/// </summary>
	/// <remarks></remarks>
	public class DecimalVM:NumericVM<Decimal> {

		/// <summary> Initializes a new instance of the <see cref="DecimalVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public DecimalVM() {
			
		}

	}


	/// <summary> Provides a view-model for <see cref="Boolean"/>
	/// </summary>
	/// <remarks></remarks>
	public class BoolVM:ValueVM<Boolean> {

		/// <summary> Initializes a new instance of the <see cref="BoolVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public BoolVM() {

			ValueChanged+=(sender, args) => {
					OnPropertyChanged("IsTrue"); 
					OnPropertyChanged("IsFalse"); 
					OnPropertyChanged("IsTrueAndEnabled"); 
					OnPropertyChanged("IsFalseAndEnabled");
				};
			IsEnabledChanged+=(sender, args) => {
					OnPropertyChanged("IsTrueAndEnabled"); 
					OnPropertyChanged("IsFalseAndEnabled");
				};
		}

		/// <summary> Gets or sets a value indicating whether this instance is true.
		/// </summary>
		/// <value><c>true</c> if this instance is true; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>
		public bool IsTrue {get { return Value == true; }set { Value = value; }}

		/// <summary> Gets or sets a value indicating whether this instance is false.
		/// </summary>
		/// <value><c>true</c> if this instance is false; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>
		public bool IsFalse {get { return Value == false; }set { Value = !value; }}

		/// <summary> Gets a value indicating whether this instance is false and enabled.
		/// </summary>
		/// <remarks></remarks>
		public bool IsFalseAndEnabled {
			get { return Value == false && IsEnabled; }
			set { Value = !value; }
		}

		/// <summary> Gets a value indicating whether this instance is true and enabled.
		/// </summary>
		/// <remarks></remarks>
		public bool IsTrueAndEnabled {
			get { return Value == true && IsEnabled; }
			set { Value = value; }
		}
	}
	
	/// <summary> Provides a view-model for <see cref="DateTime"/>
	/// </summary>
	/// <remarks></remarks>
	public class DateTimeVM:ValueVM<DateTime> {

		/// <summary> Initializes a new instance of the <see cref="DateTimeVM"/> class.
		/// </summary>
		public DateTimeVM() {}
	}

	/// <summary> Provides a view-model for <see cref="TimeSpan"/>
	/// </summary>
	/// <remarks></remarks>
	public class TimeSpanVM:ValueVM<TimeSpan> {

		/// <summary> Initializes a new instance of the <see cref="TimeSpanVM"/> class.
		/// </summary>
		public TimeSpanVM() {}
	}

	/// <summary> Provides a view-model for <see cref="Guid"/>
	/// </summary>
	/// <remarks></remarks>
	public class GuidVM:ValueVM<Guid> {

		/// <summary> Initializes a new instance of the <see cref="GuidVM"/> class.
		/// </summary>
		public GuidVM() {}
	}
}
