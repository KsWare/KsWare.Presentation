//#define RegisterInstances
//#define EnableOnDataChanged

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.Core;
using KsWare.Presentation.ViewModelFramework.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> Interface contract for view model objects
	/// </summary>
	/// <remarks></remarks>
	public partial interface IObjectVM:IModel,INotifyPropertyChanged,IHierarchical<IObjectVM>,ISelectable {

		/// <summary> Gets the error provider for this instance. (short for: vmo.Metadata.ErrorProvider)
		/// </summary>
		/// <remarks></remarks>
		IErrorProvider ErrorProvider{get;}

		event EventHandler<UserFeedbackEventArgs> UserFeedbackRequested;

		/// <summary> Occurs when a property has been changed.
		/// </summary>
		/// <remarks></remarks>
		new event EventHandler<ViewModelPropertyChangedEventArgs> PropertyChanged;

		IEventSource<EventHandler<ViewModelPropertyChangedEventArgs>>  PropertyChangedEvent { get;}

	}


	/// <summary> Provides the base class for all view model objects.
	/// </summary>
	/// <remarks>
	/// <para>Generally you should use derived from<br/>
	/// <see cref="BusinessObjectVM{TBusinessObject}"/> for view models with unterling business object (KsWare Business Framework)<br/>
	/// <see cref="DataVM{TData}"/> for view models with unterlying data<br/></para>
	/// <para>All properties starting with <c>Mː</c> (e.g. <see cref="MːData"/>) are aliases for metadata access (see <see cref="Metadata"/>). 
	/// These properties does not raise <see cref="INotifyPropertyChanged.PropertyChanged"/> and should therefore not be used for binding.</para>
	/// Members marked with [EXPERIMENTAL] are preliminary. The name or functionality could be changed in future versions or also the complete member could be moved or deleted.
	/// </remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	public partial class ObjectVM:IObjectVM {

#if(RegisterInstances)
		private static readonly List<WeakReference> s_Instances=new List<WeakReference>();
		private static DateTime s_InstancesLastCheck = DateTime.MinValue;
#endif
		private int m_IsDisposed;
		private string m_PropertyLabel;
		private readonly List<object> m_EnableObjections = new List<object>();
		private readonly List<object> m_ReadOnlyObjections = new List<object>();

		protected int SuppressAnyEvents;

		/// <summary> Initializes a new instance of the <see cref="ObjectVM"/> class.
		/// </summary>
		/// <remarks></remarks>
		public ObjectVM() {
			Interlocked.Increment(ref StatisticsːNumberOfCreatedInstances);
			Interlocked.Increment(ref StatisticsːNumberOfInstances);
			IsEnabled  = true;//true is the default value
			IsReadOnly = false;

			InitPartDebuger();
			InitPartNotifyPropertyChanged();
			InitPartHierarchy();
			InitPartMetadata();
			InitPartFields();
			InitUIProperties();
			InitPartDisposables();
			InitPartWeakEvents();
			InitPartReflection();
//			RegisterInstance(this);
//			OnLanguageChanged(Thread.CurrentThread.CurrentCulture);
		}

		~ObjectVM() {
			Interlocked.Decrement(ref StatisticsːNumberOfInstances);
			Interlocked.Increment(ref StatisticsːMethodInvocationːDestructorːCount);
		}

#if(RegisterInstances)
		private static void RegisterInstance(ObjectVM o) {
			s_Instances.Add(new WeakReference(o));
			if(DateTime.Now.AddSeconds(-30)<s_InstancesLastCheck) {
				for (int i = 0; i < s_Instances.Count; i++) {
					if(!s_Instances[i].IsAlive) {s_Instances.RemoveAt(i);i--;}
				}
			}
		}
#endif



		#region IModel,IDisposable

		/// <summary> Occurs when this instance is disposed.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
			Dispose(true); 
//			GC.SuppressFinalize(this); Interlocked.Increment(ref StatisticsːNumberOfInstances);
		}

		/// <summary> Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="explicitDisposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool explicitDisposing) {
			if (explicitDisposing) {
				if(Interlocked.Exchange(ref m_IsDisposed, 1)>0) return;
				Interlocked.Increment(ref StatisticsːMethodInvocationːDisposeːExplicitːCount);
				EventUtil.RaiseDisposedEvent(Disposed, this);
				if (LazyWeakEventStore.IsValueCreated) {
					LazyWeakEventStore.Value.Dispose();
				}
			}
		}

		#endregion

		#region Providers

		/// <summary> Gets the error provider for this instance. (short for: vmo.Metadata.ErrorProvider)
		/// </summary>
		/// <remarks></remarks>
		public IErrorProvider ErrorProvider {
			get
			{
				return Metadata.ErrorProvider;
			}
		}

		#endregion

#if(EnableOnDataChanged)
		private void AtDataChanged(object sender, DataChangedEventArgs e) {OnDataChanged(e);}
		protected virtual void OnDataChanged(DataChangedEventArgs e) { }
#endif

		/// <summary> [OBSOLETE?] Gets or sets the property label.
		/// </summary>
		/// <value>The property label.</value>
		/// <remarks></remarks>
		//TODO revise name and location of PropertyLabel
		[Localizable(true)]
		public string PropertyLabel { // alias Summary/Title/Caption
			get {return m_PropertyLabel;}
			set {
				if (Equals(m_PropertyLabel, value)) return;
				m_PropertyLabel = value;
				OnPropertyChanged("PropertyLabel");
			}
		}


		/// <summary> Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <remarks>Use <see cref="SetReadOnly"/> to change the value.</remarks>
		[DefaultValue(false)]
		public bool IsReadOnly{get;private set;}

		/// <summary> Occurs when <see cref="IsReadOnly"/> has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event RoutedPropertyChangedEventHandler<bool> IsReadOnlyChanged;

		/// <summary> Sets the read only state.
		/// </summary>
		/// <param name="token">The token to change the state</param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool SetReadOnly(object token, bool value) {
			bool oldIsReadOnly = m_ReadOnlyObjections.Count==0;
			if (value==false) {
				m_ReadOnlyObjections.Add(token);
			} else {
				m_ReadOnlyObjections.Remove(token);
			}
			bool newIsReadOnly = m_ReadOnlyObjections.Count==0;
			if(oldIsReadOnly!=newIsReadOnly) {
				IsReadOnly = newIsReadOnly;
				OnPropertyChanged("IsReadOnly");
				if(IsReadOnlyChanged!=null) IsEnabledChanged(this, new RoutedPropertyChangedEventArgs<bool>(oldIsReadOnly, newIsReadOnly));
			}

			return newIsReadOnly;
		}

		#region IsInDesignMode

		private static bool? s_IsInDesignMode;

		/// <summary> Gets a value indicating whether the app is in design mode.
		/// </summary>
		/// <remarks>
		/// For test the value can be set manually to emulate design time behavior</remarks>
		public static bool IsInDesignMode {
			get {
				if(!s_IsInDesignMode.HasValue) {
					s_IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
				}
				return s_IsInDesignMode.Value;
			}
			set { s_IsInDesignMode = value; }
		}

		#endregion


		/// <summary> Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents this instance.</returns>
		/// <remarks></remarks>
		public override string ToString() {
#if DEBUG
			return this.DebugObjectTraceˑToString();
//			return string.Format("{{{0}}}", this.GetObjectKey());
#else
			return base.ToString();
#endif
		}


		/// <summary> Tries to get object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func">The function</param>
		/// <param name="infoText">The info text.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		protected static T TryGetObject<T>(Func<T> func, string infoText) {
			try {return func();} catch (Exception ex) {
				Debug.WriteLine("=>WARNING: Getting object throws " + ex.GetType().Name + "!" + " ObjectType: " + typeof(T).Name + " Info: " + infoText);
				DebuggerːBreak(infoText);
				return default(T);
			}
		}

		#region CultureChanged

		//TODO InputLanguageManager.Current.InputLanguageChanged+=delegate{NotifyCultureChanged();};

		public static void NotifyCultureChanged() {
			EventUtil.Raise(CultureChanged,null,EventArgs.Empty,"{221E109A-417E-40A6-A3E9-86F191A39586}");
		}

		public static event EventHandler CultureChanged;

		/// <summary> Registers the culture changed action.
		/// </summary>
		/// <param name="proc">The procedure to call on language changed</param>
		/// <param name="initialTrigger">if set to <c>true</c> calls the procedure initially.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void RegisterCultureChangedAction(Action proc, bool initialTrigger) {
			CultureChanged += (s,e) => proc();
			if(initialTrigger) proc();
		}

		#endregion

	}



//	public partial class ObjectVM
//	{
//		protected static void SetLanguage(CultureInfo lang) {
//			for (int i = 0; i < s_Instances.Count; i++) {
//				if(!s_Instances[i].IsAlive){s_Instances.RemoveAt(i);i--;continue;}
//				((ObjectVM)s_Instances[i].Target).OnLanguageChanged(lang);
//			}
//		}
//
//		protected virtual void OnLanguageChanged(CultureInfo lang) {}
//	}

	public interface IDataVM<TData>:IObjectVM {
		TData Data { get; set; }
	}

	public interface IDataVM:IObjectVM {
		object Data { get; set; }
	}

	public class BusinessDataVM<TBusinessObject,TData>:ObjectVM,IBusinessDataVM<TBusinessObject,TData>,IDataVM {

		public TData Data { get { return (TData) Metadata.DataProvider.Data; }set { Metadata.DataProvider.Data = value; }}

		object IDataVM.Data{get { return Data; } set { Data = (TData) value; }}
	}

	public interface IBusinessDataVM<TBusinessObject,TData>:IDataVM {
		
	}


	/// <summary> Provides data for <see cref="IObjectVM.PropertyChanged"/>-event
	/// </summary>
	public class ViewModelPropertyChangedEventArgs:PropertyChangedEventArgs {

		private readonly ViewModelProperty m_Property;

		/// <summary> Initializes a new instance of the <see cref="ViewModelPropertyChangedEventArgs"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		public ViewModelPropertyChangedEventArgs(ViewModelProperty property): base(property.Name) {
			if (property == null) throw new ArgumentNullException("property");
			m_Property = property;
		}

		/// <summary> Gets the property identifier.
		/// </summary>
		/// <value>The property identifier.</value>
		public ViewModelProperty Property {get {return m_Property;}}
	}

}
