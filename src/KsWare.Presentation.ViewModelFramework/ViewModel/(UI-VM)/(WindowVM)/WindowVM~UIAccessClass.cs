using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework {

	partial class WindowVM {

		/// <summary> Provides direct access to the UI (<see cref="Window"/>).
		/// </summary>
		public sealed class UIAccessClass : INotifyPropertyChanged, IDisposable {

			public readonly BackingFieldsStore Fields;

			public UIAccessClass() {
				Fields = new BackingFieldsStore(this, OnPropertyChanged);
				Fields[nameof(Window)].ValueChanged += (s, e) => EventUtil.Raise(WindowChanged, this, new ValueChangedEventArgs<Window>((Window)e.NewValue, (Window)e.OldValue), "{5AC93EA1-BB23-49CE-86F4-7DA533825945}");
			}

			/// <summary> Gets or sets a value indicating whether direct UI access is enabled.
			/// </summary>
			/// <value><c>true</c> if direct UI access is enabled; otherwise, <c>false</c>.</value>
			/// <remarks>
			/// If <see cref="IsDirectAccessEnabled"/> is set to <c>true</c> The view model can access the <see cref="Window"/> directly. That means read/write properties, register for events, call methods, etc.
			/// <para>Note: This feature should be used only if <see cref="Binding"/> is not possible.</para>
			/// <para>Because direct access to Window (reference) setting to true violates the MVVM pattern and lets act this object as 'Controller' in a MVC pattern)</para>
			/// </remarks>
			public bool IsDirectAccessEnabled { get; set; }

			/// <summary> Gets the <see cref="Window"/>.
			/// </summary>
			/// <value>The <see cref="Window"/>.</value>
			public Window Window { get { DemandAccess(); return Fields.GetValue<Window>(); } }

			/// <summary> Gets a value indicating whether this instance has window.
			/// </summary>
			/// <value><c>true</c> if this instance has window; otherwise, <c>false</c>.</value>
			public bool HasWindow => Fields.GetValue<Window>(nameof(Window)) != null;

			internal void AssignWindowInternal(Window window) {
				if (window == null) throw new ArgumentNullException(nameof(window));
				DemandAssign();
				Fields.SetValue(window, nameof(Window));
			}

			public void ReleaseWindowInternal() {
				Fields.SetValue<Window>(null, nameof(Window));
			}

			public event EventHandler<ValueChangedEventArgs<Window>> WindowChanged;

			/// <summary> Creates an instance of the specified type using that type's default constructor.
			/// </summary>
			/// <param name="view">The type of window to create. </param>
			public void CreateInstance(Type view) {
				DemandAccess(); DemandAssign();
				var window = (Window) Activator.CreateInstance(view);
				AssignWindowInternal(window);
			}

			/// <summary> Creates an instance of the specified window type using the constructor that best matches the specified parameters.
			/// </summary>
			/// <param name="view">The type of window to create. </param>
			/// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If args is an empty array or null, the constructor that takes no parameters (the default constructor) is invoked. </param>
			public void CreateInstance(Type view, params object[] args) {
				DemandAccess();DemandAssign();
				var window = (Window) Activator.CreateInstance(view,args);
				AssignWindowInternal(window);
			}

			private void DemandAccess() {if(!IsDirectAccessEnabled) throw new InvalidOperationException("Direct UI access is not allowed!");}

			private void DemandAssign() {if(HasWindow) throw new InvalidOperationException("Window already assigned!");}

			#region INotifyPropertyChanged
			public event PropertyChangedEventHandler PropertyChanged;

			[NotifyPropertyChangedInvocator]
			private void OnPropertyChanged(string propertyName) {
				var handler = PropertyChanged;
				if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
			}
			#endregion

			#region Implementation of IDisposable

			public void Dispose() {
				Fields.Dispose();
			}

			#endregion

		}
	}
}
