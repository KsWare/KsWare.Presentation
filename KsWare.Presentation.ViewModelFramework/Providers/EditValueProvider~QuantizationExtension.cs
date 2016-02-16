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

			private Array m_Steps=new int[]{1,2,5,10,20,50,100};
			private int m_MaxStep = 6;
			private int m_Step = 0;
			private bool m_IsInitialized;

			public QuantizationExtension(EditValueProvider provider):base(provider) {
			
			}

			public void Init(int[] steps) {
				m_Steps = steps;
				m_MaxStep = steps.Length-1;
				MinValue = m_Steps.GetValue(0);
				MaxValue = m_Steps.GetValue(m_Steps.GetLength(0)-1);
			}

			public void Init(double[] steps) {
				m_Steps = steps;
				m_MaxStep = steps.Length-1;
				MinValue = m_Steps.GetValue(0);
				MaxValue = m_Steps.GetValue(m_Steps.GetLength(0)-1);
			}

			public object MinValue { get; private set; }
			public object MaxValue { get; private set; }

			[Bindable(BindableSupport.Yes,BindingDirection.OneWay),PublicAPI]
			public int MinStep { get { return 0; } }

			[Bindable(BindableSupport.Yes,BindingDirection.OneWay),PublicAPI]
			public int MaxStep { get { return m_MaxStep; } }

			[Bindable(BindableSupport.Yes,BindingDirection.TwoWay),PublicAPI]
			public int Step {
				get {
					if(!m_IsInitialized) UpdateValue(false);
					return m_Step;
				}
				set {
					if(m_Step==value)return;
					m_Step = value;
					OnPropertyChanged("Step");

					//TODO [xgksc 2013-03-05] revise
					var v= m_Steps.GetValue(m_Step);
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
					if(Compare(v,m_Steps.GetValue(0))<0) step = 0;
					else if(Compare(v,m_Steps.GetValue(m_Steps.GetLength(0)-1))>=0) step = m_Steps.Length-1;
					else {
						for(int i = 0; i<m_Steps.GetLength(0)-1; i++) {
							if(Compare(v,m_Steps.GetValue(i))==0) {step = i;break;}
							if(Compare(v,m_Steps.GetValue(i+1))<0) {step = i+1;break;}//immer der nächst kleinere schritt
						}
					}
					if(m_Step==step) return;
					m_Step = step;
					m_IsInitialized = true;
					if(raiseEvents) OnPropertyChanged("Step");
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
