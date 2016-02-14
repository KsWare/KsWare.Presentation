using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace KsWare.Presentation.ViewModelFramework {

	// ReSharper disable UnusedMember.Global

	/// <summary> Provides a single DependencyProperty 
	/// Usable for cases where you internally need one (e.g. for binding operations)
	/// </summary>
	/// <remarks></remarks>
	public class DependencyValueWrapper:DependencyObject {

		/// <summary> Value Property
		/// </summary>
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof (object), typeof (DependencyValueWrapper), new PropertyMetadata(default(object),(o, args) => ((DependencyValueWrapper)o).AtValueChanged(args)));

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "e")]
		private void AtValueChanged(DependencyPropertyChangedEventArgs e) {
			if(ValueChanged==null)return;
			ValueChanged(this, EventArgs.Empty);
		}

		/// <summary> Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		public object Value {
			get { return (object) GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		/// <summary> Occurs when <see cref="Value"/> property has been changed
		/// </summary>
		/// <remarks></remarks>
		public event EventHandler ValueChanged;

		/// <summary> Creates and associates a binding with <see cref="Value"/>.
		/// </summary>
		/// <param name="binding">The binding base.</param>
		/// <remarks></remarks>
		public void BindValue(BindingBase binding) {
			BindingOperations.SetBinding(this, ValueProperty,binding);
		}

		/// <summary> Creates and associates a binding with <see cref="Value"/>.
		/// </summary>
		/// <param name="source">the object to use as the binding source.</param>
		/// <param name="path">the path to the binding source property.</param>
		/// <remarks></remarks>
		public void BindValue(object source, string path) {
			BindingOperations.SetBinding(this, ValueProperty,new Binding(path){Mode = System.Windows.Data.BindingMode.TwoWay, Source = source});
		}

		/// <summary> Creates and associates a binding with <see cref="Value"/>.
		/// </summary>
		/// <param name="source">the object to use as the binding source.</param>
		/// <param name="path">the path to the binding source property.</param>
		/// <param name="mode"> a value that indicates the direction of the data flow in the binding.</param>
		/// <remarks></remarks>
		public void BindValue(object source, string path,System.Windows.Data.BindingMode mode) {
			BindingOperations.SetBinding(this, ValueProperty,new Binding(path){Mode = mode, Source = source});
		}
	}
	// ReSharper restore UnusedMember.Global
}
