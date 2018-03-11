using System.Windows;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModelFramework.Providers;
using TextBox = System.Windows.Controls.TextBox;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	// TextBoxBehavior.ConnectEditValueProvider

	/// <summary> Provides properties to extent a <see cref="System.Windows.Controls.TextBox"/>
	/// </summary>
	public static class TextBoxBehavior {

		public static readonly DependencyProperty ConnectEditValueProviderProperty =
			DependencyProperty.RegisterAttached("ConnectEditValueProvider", typeof(bool), typeof(TextBoxBehavior), 
			new PropertyMetadata(default(bool), AtConnectEditValueProviderChanged));

		public static void SetConnectEditValueProvider(TextBox element, bool value) {
			element.SetValue(ConnectEditValueProviderProperty, value);
		}

		public static bool GetConnectEditValueProvider(TextBox element) {
			return (bool)element.GetValue(ConnectEditValueProviderProperty);
		}

		private static void AtConnectEditValueProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var textBox = (TextBox)d;
			textBox.DataContextChanged+=AtTextBoxOnDataContextChanged;
			if(textBox.DataContext!=null) AtTextBoxOnDataContextChanged(textBox,new DependencyPropertyChangedEventArgs(FrameworkElement.DataContextProperty,null,textBox.DataContext));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private static void AtTextBoxOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(GetConnectEditValueProvider((TextBox)sender)) ConnectToEditValueProvider((TextBox)sender);
		}

		private static void ConnectToEditValueProvider(TextBox textBox) {
			if(!(textBox.DataContext is ObjectVM)) return;

			var vm = (ObjectVM)textBox.DataContext;
			var editValueProvider = vm.Metadata.EditValueProvider as EditValueProvider;
			if(editValueProvider!=null) {
				textBox.GotFocus           += (o,ea) => editValueProvider.NotifyGotFocus();
				//textBox.GotKeyboardFocus +=
				textBox.LostFocus          += (o,ea) => editValueProvider.NotifyLostFocus();
				//textBox.LostKeyboardFocus+=
//				textBox.KeyDown            += (o,ea) => editValueProvider.NotifyKeyDown(ea,CalculateTextPreview((TextBox)o,ea));
				textBox.KeyUp              += (o,ea) => editValueProvider.NotifyKeyUp(ea);
				textBox.TextChanged        += (o, ea) => editValueProvider.NotifyTextChanged(ea,textBox.Text);
				textBox.PreviewTextInput   += (o, ea) => editValueProvider.NotifyTextInput(ea);
				//textBox.PreviewTextInput += (o, ea) => editValueProvider.NotifyTextInput(ea);
			}
		}

//		static TextBoxTest temp=new TextBoxTest();
//
//		private static string CalculateTextPreview(TextBox textBox, KeyEventArgs ea) {
//			temp.Text = textBox.Text;
//			temp.Select(textBox.SelectionStart,textBox.SelectionLength);
//			temp.TriggerKeyDown(ea);
//			return temp.Text;
//		}

//		class TextBoxTest:TextBox
//		{
//			private static KeysConverter KeysConverter=new KeysConverter();
//
//			public void TriggerKeyDown(KeyEventArgs e) {
//				if(e.KeyboardDevice.Modifiers==ModifierKeys.Control && e.KeyboardDevice.IsKeyDown(Key.V)) this.Paste();
//				var virtualKeyFromKey = KeyInterop.VirtualKeyFromKey(e.Key);
//				var ch=(string)KeysConverter.ConvertTo(virtualKeyFromKey, typeof(string));
//			}
//
//		}

	}
}