using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	public class MouseDoubleClickBehavior:Behavior<FrameworkElement> {

		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof (ICommand), typeof (MouseDoubleClickBehavior), new PropertyMetadata(default(ICommand)));
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof (object), typeof (MouseDoubleClickBehavior), new PropertyMetadata(default(object)));

		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.PreviewMouseDown+=AtMouseDown;
		}

		private void AtMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ClickCount == 2 && e.ChangedButton==MouseButton.Left) {
				if(Command!=null)Command.Execute(CommandParameter);
			} 
		}

		public ICommand Command {
			get => (ICommand) GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public object CommandParameter {
			get => (object) GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}
	}
}