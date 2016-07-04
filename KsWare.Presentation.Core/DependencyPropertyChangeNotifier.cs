using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	/// <summary>
	///     Alternative to the leaking DependencyPropertyDescriptor.AddValueChanged.
	///     See article http://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/.
	///     Note that the sender in the ValueChanged handler is not the object exposing the changed property but the changenotifier.
	/// </summary>
	[PublicAPI]
	public sealed class DependencyPropertyChangeNotifier:DependencyObject, IDisposable {

		#region Value

		/// <summary> Identifies the <see cref="Value" /> dependency property
		/// </summary>
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
			"Value", typeof(object), typeof(DependencyPropertyChangeNotifier), new FrameworkPropertyMetadata(null, (o, e) =>((DependencyPropertyChangeNotifier)o).AtPropertyChanged(e) ));
		
		/// <summary> Returns/sets the value of the property
		/// </summary>
		/// <seealso cref="ValueProperty" />
		[Description("Returns/sets the value of the property")]
		[Category("Behavior")]
		[Bindable(true)]
		[PublicAPI]
		public object Value {
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		private void AtPropertyChanged(DependencyPropertyChangedEventArgs e) {
			EventUtil.Raise(ValueChanged,this,EventArgs.Empty,"{BDA6DF23-FB08-4FCE-8148-D972BE4228D4}");
		}

		/// <summary> Occurs when <see cref="Value"/> changes.
		/// </summary>
		[PublicAPI]
		public event EventHandler ValueChanged;

		#endregion

		private readonly WeakReference _PropertySource;

		public DependencyPropertyChangeNotifier(DependencyObject propertySource, string path)
			:this(propertySource, new PropertyPath(path)) {}

		public DependencyPropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
			:this(propertySource, new PropertyPath(property)) {}

		public DependencyPropertyChangeNotifier(DependencyObject propertySource, PropertyPath property) {
			if(propertySource==null) throw new ArgumentNullException(nameof(propertySource));
			if(property      ==null) throw new ArgumentNullException(nameof(property));

			_PropertySource=new WeakReference(propertySource);
			var binding = new Binding {
				Path   = property,
				Mode   = System.Windows.Data.BindingMode.OneWay,
				Source = propertySource
			};
			BindingOperations.SetBinding(this, ValueProperty, binding);
		}

		/// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			BindingOperations.ClearBinding(this, ValueProperty);
		}

		/// <summary> Gets the source of the property.
		/// </summary>
		/// <value>The property source.</value>
		/// <remarks>The property source is stored as weak reference. If the property source is no more alive the <see cref="PropertySource"/> returns null.</remarks>
		[PublicAPI]
		public DependencyObject PropertySource {
			get {
				try {
					// note, it is possible that accessing the target property will result in an exception 
					// so we have wrapped this check in a try catch
					return _PropertySource.IsAlive?_PropertySource.Target as DependencyObject:null;
				} catch {
					return null;
				}
			}
		}
	}

}