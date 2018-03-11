using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IEditValueProviderQuantizationExtension:IEditValueProviderExtension {
		int MaxStep { get; }
		int Step { get; set; }
		void Init(int[] steps);void Init(double[] steps);
		object MinValue { get; }
		object MaxValue { get; }
	}

	partial class EditValueProvider {

		/// <summary> [EXPERIMENTAL] Quantization Extension.
		/// </summary>
		class QuantizationExtension:Extension,IEditValueProviderQuantizationExtension {

			private Array _steps=new int[]{1,2,5,10,20,50,100};
			private int _maxStep = 6;
			private int _step = 0;
			private bool _isInitialized;

			public QuantizationExtension(EditValueProvider provider):base(provider) {
			
			}

			public void Init(int[] steps) {
				_steps = steps;
				_maxStep = steps.Length-1;
				MinValue = _steps.GetValue(0);
				MaxValue = _steps.GetValue(_steps.GetLength(0)-1);
			}

			public void Init(double[] steps) {
				_steps = steps;
				_maxStep = steps.Length-1;
				MinValue = _steps.GetValue(0);
				MaxValue = _steps.GetValue(_steps.GetLength(0)-1);
			}

			public object MinValue { get; private set; }
			public object MaxValue { get; private set; }

			[Bindable(BindableSupport.Yes,BindingDirection.OneWay),PublicAPI]
			public int MinStep { get { return 0; } }

			[Bindable(BindableSupport.Yes,BindingDirection.OneWay),PublicAPI]
			public int MaxStep { get { return _maxStep; } }

			[Bindable(BindableSupport.Yes,BindingDirection.TwoWay),PublicAPI]
			public int Step {
				get {
					if(!_isInitialized) UpdateValue(false);
					return _step;
				}
				set {
					if(_step==value)return;
					_step = value;
					OnPropertyChanged(nameof(Step));

					//TODO [xgksc 2013-03-05] revise
					var v= _steps.GetValue(_step);
					object v1;
					try {
						v1 = Provider.TypeConverter.ConvertTo(v, Provider.ViewModel.ValueType); //throws an exception if conversation failed
					} catch(Exception ex) {
						((IErrorProviderController)Provider.ViewModel.Metadata.ErrorProvider).SetError(ex.Message); //TODO localize message
						return;
					}
					Provider.UpdateSource(v1);
				}
			}

			internal void UpdateValue(bool raiseEvents) {
				var v = Provider.ViewModel.Value;
				if(IsNumeric(v)){
					var step = 0;
					if(Compare(v,_steps.GetValue(0))<0) step = 0;
					else if(Compare(v,_steps.GetValue(_steps.GetLength(0)-1))>=0) step = _steps.Length-1;
					else {
						for(int i = 0; i<_steps.GetLength(0)-1; i++) {
							if(Compare(v,_steps.GetValue(i))==0) {step = i;break;}
							if(Compare(v,_steps.GetValue(i+1))<0) {step = i+1;break;}//immer der nächst kleinere schritt
						}
					}
					if(_step==step) return;
					_step = step;
					_isInitialized = true;
					if(raiseEvents) OnPropertyChanged(nameof(Step));
				} else {
					//throw new NotSupportedException("Not supported! ErrorID:{18C1445F-ABF2-4241-8E8C-5A994F93A0AA}");
				}
			}

			private int Compare(object a, object b) {
				var bWithTypeOfA= System.Convert.ChangeType(b, a.GetType());
				return ((IComparable)a).CompareTo(bWithTypeOfA);
			}
		}

	}
}
