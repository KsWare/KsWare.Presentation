using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework 
{
	//
	// Implements "Focus" functionality for ObjectVM
	// this works together with the "FocusBehavior" (see ViewFramework-project)

	public partial class ObjectVM 
	{

		private bool _isKeyboardFocused;
		private bool _focusable;

		/// <summary> Gets or sets a value indicating whether this <see cref="ObjectVM"/> is focusable.
		/// </summary>
		/// <value><c>true</c> if focusable; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>
		[UsedImplicitly]
		public bool Focusable {
			get => _focusable;
			set {
				if (_focusable == value) return;
				_focusable = value;
				OnPropertyChanged("Focusable");
			}
		}

		/// <summary> Gets or sets a value indicating whether this instance is keyboard focused.
		/// </summary>
		/// <value><c>true</c> if this instance is keyboard focused; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>
		[UsedImplicitly]
		public bool IsKeyboardFocused {
			get => _isKeyboardFocused;
			set {
				if (_isKeyboardFocused == value) return;
				_isKeyboardFocused = value;
				OnPropertyChanged("IsKeyboardFocused");
			}
		}

		/// <summary> Sets the keyboard focus to the bound FrameworkElement
		/// </summary>
		/// <remarks></remarks>
		[UsedImplicitly]
		public void Focus() {
			IsKeyboardFocused = true;
		}

	}
}
