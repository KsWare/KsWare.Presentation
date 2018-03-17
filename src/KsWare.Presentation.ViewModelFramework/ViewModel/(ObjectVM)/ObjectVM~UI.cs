using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework.UIProperties;

namespace KsWare.Presentation.ViewModelFramework {

	public partial interface IObjectVM {

		UIPropertiesRoot UI { get; }

	}

	public partial class ObjectVM {

		private Lazy<UIPropertiesRoot> _ui;

		private void InitUIProperties() {
			_ui = new Lazy<UIPropertiesRoot>(() => new UIPropertiesRoot(this));
		}

		/// <summary> Provides additional strong typed, bindable properties
		/// </summary>
		/// <remarks>
		/// All properties are implemented as "only property" w/o further logic.
		/// <code>public object Icon {get { return GetValue&lt;object>("Icon"); }set { SetValue("Icon",value);}}</code>
		/// </remarks>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public UIPropertiesRoot UI => _ui.Value;
	}
}

namespace KsWare.Presentation.ViewModelFramework.UIProperties {

	public class UIPropertiesBase:INotifyPropertyChanged {
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")] 
		private readonly IObjectVM _Owner;
		private readonly BackingFieldsStore _Fields;

		/// <summary> Initializes a new instance of the <see cref="UIPropertiesBase" /> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		public UIPropertiesBase(IObjectVM owner) {
			_Owner = owner;
			_Fields=new BackingFieldsStore(this,OnPropertyChanged);
		}

		public BackingFieldsStore Fields => _Fields;

		/// <summary> Occurs when a property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged(string propertyName) {
			EventUtil.Raise(PropertyChanged,this,new PropertyChangedEventArgs(propertyName),"{D8D0E6E6-4ADA-4828-BF1A-80E538170B48}");
		}

		protected class LazyObjectFactory:BackingFieldsStore.IValueFactory {
			public object CreateInstance(Type type, BackingFieldsStore store) {
				return Activator.CreateInstance(type,store.Owner);
			}
		}
	}

	/// <summary> <see cref="UIElement"/>/<see cref="FrameworkElement"/>
	/// </summary>
	public sealed class UIPropertiesRoot:UIPropertiesBase {
		private readonly Lazy<UIEventConnector> _EventConnector;
		private readonly Lazy<UIEvents> _Events        ;
		private readonly Lazy<InputGestureCollection> _InputGestures ;

		/// <summary> Initializes a new instance of the <see cref="UIPropertiesRoot" /> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		public UIPropertiesRoot(IObjectVM owner):base(owner) {
			Fields.AddLazy("Button"         , new Lazy<Button       >(()=> new Button(owner)));
			Fields.AddLazy("TreeViewItem"   , new Lazy<TreeViewItem >(()=> new TreeViewItem(owner)));
			Fields.AddLazy("MenuItem"       , new Lazy<MenuItem     >(()=> new MenuItem(owner)));
			Fields.AddLazy("Selector"       , new Lazy<Selector     >(()=> new Selector(owner)));
			Fields.AddLazy("Item"           , new Lazy<ItemContainer>(()=> new ItemContainer(owner)));
			Fields.AddLazy("ComboBox"       , new Lazy<ComboBox     >(()=> new ComboBox(owner)));
			_EventConnector = new Lazy<UIEventConnector        >(()=> new UIEventConnector(owner));
			_Events         = new Lazy<UIEvents                >(()=> new UIEvents(owner));
			_InputGestures  = new Lazy<InputGestureCollection  >(()=> new InputGestureCollection());
		}


		/// <summary> Gets the button properties.
		/// </summary>
		/// <value>
		/// The button properties.
		/// </value>
		[BackingFieldsStore.Lazy(typeof(LazyObjectFactory))]
		public Button Button => Fields.GetLazy<Button>("Button");

		/// <summary> Gets the TreeViewItem properties.
		/// </summary>
		/// <value>
		/// The TreeViewItem properties.
		/// </value>
		[BackingFieldsStore.Lazy(typeof(LazyObjectFactory))]
		public TreeViewItem TreeViewItem => Fields.GetLazy<TreeViewItem>("TreeViewItem");

		/// <summary> Gets the MenuItem properties.
		/// </summary>
		/// <value>
		/// The MenuItem properties.
		/// </value>
		[BackingFieldsStore.Lazy(typeof(LazyObjectFactory))]
		public MenuItem MenuItem => Fields.GetLazy<MenuItem>("MenuItem");

		public UIEventConnector EventConnector => _EventConnector.Value;

		public UIEvents Events => _Events.Value;

		/// <summary> System.Windows.Controls.Primitives.Selector
		/// </summary>
		[BackingFieldsStore.Lazy(typeof(LazyObjectFactory))]
		public Selector Selector => Fields.GetLazy<Selector>("Selector");

		/// <summary> System.Windows.Controls.Primitives.Selector
		/// </summary>
		[BackingFieldsStore.Lazy(typeof(LazyObjectFactory))]
		public ItemContainer Item => Fields.GetLazy<ItemContainer>("Item");

		public InputGestureCollection InputGestures => _InputGestures.Value;

		/// <summary> System.Windows.Controls.ComboBox
		/// </summary>
		[BackingFieldsStore.Lazy(typeof(LazyObjectFactory))]
		public ComboBox ComboBox => Fields.GetLazy<ComboBox>("ComboBox");


		// ###

		/// <summary> see <see cref="FrameworkElement.ToolTip"/>
		/// </summary>
		public object ToolTip { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }

		/// <summary> see <see cref="UIElement.Visibility"/>
		/// </summary>
		public Visibility Visibility { get => Fields.GetValue<Visibility>(); set => Fields.SetValue(value); }
		
		/// <summary> see <see cref="FrameworkElement.Tag"/>
		/// </summary>
		public object Tag { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }


	}

	public class UIEvents {

		private readonly IObjectVM _Owner;

		public UIEvents(IObjectVM owner) {
			_Owner = owner;
		}

		public event EventHandler<KeyEventArgs> PreviewKeyDown {
			add => _Owner.UI.EventConnector.PreviewKeyDownEventHandler+=value;
			remove => _Owner.UI.EventConnector.PreviewKeyDownEventHandler-=value;
		}

		public event EventHandler<ValueChangedEventArgs> DataContextChanged {
			add => _Owner.UI.EventConnector.DataContextChangedEventHandler+=value;
			remove => _Owner.UI.EventConnector.DataContextChangedEventHandler-=value;
		}

	}

	public class UIEventConnector {

		private readonly object _Owner;

		public UIEventConnector(object owner) {
			_Owner = owner;
		}
		#region PreviewKeyDown

		public void PreviewKeyDown(object sender, KeyEventArgs e) {EventUtil.Raise(PreviewKeyDownEventHandler,_Owner,e,"{B78F3AB0-F36E-4255-AAF0-9EB05EBE3C76}");}
		internal event EventHandler<KeyEventArgs> PreviewKeyDownEventHandler;

		#endregion

		#region DataContextChanged

		/// <summary> Called by <see cref="FrameworkElement"/> when the data context changes.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event data.</param>
		/// <seealso cref="System.Windows.FrameworkElement.DataContextChanged"/>
		public void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) { EventUtil.Raise(DataContextChangedEventHandler, _Owner, new ValueChangedEventArgs(e), "{402B2416-5C5C-4D81-B63F-005270F52137}"); }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataContext"></param>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <example><code>this.DataContextChanged += (s, e) => UIEventConnector.DataContextChanged(this.DataContext, s, e);</code>Where <c>this</c> is a <see cref="FrameworkElement"/></example>
		public static void DataContextChanged(object dataContext, object sender, DependencyPropertyChangedEventArgs e) {
			var vm = (IObjectVM) dataContext; if(vm==null) return;
			var ec = vm.UI.EventConnector;
			EventUtil.Raise(ec.DataContextChangedEventHandler, ec._Owner, new ValueChangedEventArgs(e), "{402B2416-5C5C-4D81-B63F-005270F52137}");
		}		
		internal event EventHandler<ValueChangedEventArgs> DataContextChangedEventHandler;

		#endregion
	}

}

namespace KsWare.Presentation.ViewModelFramework.UIProperties {

	/// <summary> Button
	/// </summary>
	public sealed class Button:UIPropertiesBase {

		/// <summary> Initializes a new instance of the <see cref="Button" /> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		public Button(IObjectVM owner):base(owner) {}

		/// <summary> see <see cref="System.Windows.Controls.Button.IsDefault"/>
		/// </summary>
		public bool IsDefault { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary> see <see cref="ToggleButton.IsChecked"/>
		/// </summary>
		public bool IsChecked { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary> see <see cref="Button.Command"/>
		/// </summary>
		public ICommand Command { get => Fields.GetValue<ICommand>(); set => Fields.SetValue(value); }

		//### Additional Properties ###

		public bool IsHighlighted { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

	}

	/// <summary> TreeViewItem </summary>
	public sealed class TreeViewItem:UIPropertiesBase {

		//
		// this implements the TreeViewItem part of ObjectVM
		// 
		// IsExpanded 
		// IsSelected (implemented on other part)
		// 
		// NOT IMPLEMENTED: IsSelectionActive/SetKeyboardFocus

		/// <summary> Initializes a new instance of the <see cref="TreeViewItem" /> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		internal TreeViewItem(IObjectVM owner) :base(owner) {}

		/// <summary> Gets or sets a value indicating whether this object is expanded.
		/// </summary>
		/// <value><c>true</c> if this object is expanded; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>

		public bool IsExpanded {
			get => Fields.GetValue<bool>();
			set { Fields.SetAndRaise("IsExpanded", value, _=> EventUtil.Raise(IsExpandedChanged,this,EventArgs.Empty,"{355AFA11-F463-429A-AB9F-42F51626DAC8}")); }
		}	
		
		/// <summary> Occurs when <see cref="IsExpanded"/> changed.
		/// </summary>
		public event EventHandler IsExpandedChanged ;

		/// <summary>  Gets or sets a value indicating whether the TreeViewItem has no header.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has no header; otherwise, <c>false</c>.
		/// </value>
		public bool HasNoHeader { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary> Gets or sets a value indicating whether the TreeViewItem has no expander.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has no expander; otherwise, <c>false</c>.
		/// </value>
		public bool HasNoExpander { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary> Gets or sets a value indicating whether the TreeViewItem is the first node.
		/// </summary>
		/// <value>
		///   <c>true</c> if the TreeViewItem is last node; otherwise, <c>false</c>.
		/// </value>
		public bool IsFirst { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary> Gets or sets a value indicating whether the TreeViewItem is the last node.
		/// </summary>
		/// <value>
		///   <c>true</c> if the TreeViewItem is last node; otherwise, <c>false</c>.
		/// </value>
		public bool IsLast { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public IList List { get => Fields.GetValue<IList>(); set => Fields.SetValue(value); }

		public bool HasItems { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public bool IsTopLevel { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
	}

	/// <summary> MenuItem </summary>
	public sealed class MenuItem:UIPropertiesBase {

		/// <summary> Initializes a new instance of the <see cref="TreeViewItem" /> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		internal MenuItem(IObjectVM owner) :base(owner) {}

		/// <summary>
		/// <see cref="System.Windows.Controls.MenuItem.Icon"/>
		/// </summary>
		public object Icon { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }

		/// <summary>
		/// <see cref="HeaderedItemsControl.Header"/>
		/// </summary>
		public object Header { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }

		/// <summary>
		/// <see cref="System.Windows.Controls.MenuItem.IsCheckable"/>
		/// </summary>
		public bool IsCheckable { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary>
		/// <see cref="System.Windows.Controls.MenuItem.IsChecked"/>
		/// </summary>
		public bool IsChecked { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
	}

	/// <summary>
	/// System.Windows.Controls.Primitives.Selector
	/// </summary>
	public sealed class Selector : UIPropertiesBase {

		internal Selector(IObjectVM owner) :base(owner) {}

		/// <summary> Gets or sets the first item in the current selection or returns null if the selection is empty 
		/// </summary>
		/// <value>The first item in the current selection or null if the selection is empty.</value>
		/// <seealso cref="System.Windows.Controls.Primitives.Selector.SelectedItem"/>
		public object SelectedItem { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }

		/// <summary> Gets or sets the index of the first item in the current selection or returns negative one (-1) if the selection is empty. 
		/// </summary>
		/// <value>The index of first item in the current selection. The default value is negative one (-1).</value>
		/// <seealso cref="System.Windows.Controls.Primitives.Selector.SelectedIndex"/>
		public int SelectedIndex { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }
	}

	public sealed class ComboBox:UIPropertiesBase {

		/// <summary> Initializes a new instance of the <see cref="ComboBox" /> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		internal ComboBox(IObjectVM owner) :base(owner) {}

		/// <summary> Gets or sets a value that enables or disables editing of the text in text box of the System.Windows.Controls.ComboBox.
		/// </summary>
		/// <value><c>true</c> if the <see cref="System.Windows.Controls.ComboBox"/> can be edited; otherwise <c>false</c>. The default is false.</value>
		/// <seealso cref="System.Windows.Controls.ComboBox.IsEditable"/>
		public bool IsEditable { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary> Gets or sets a value that indicates whether the drop-down for a combo box is currently open.
		/// </summary>
		/// <value>true if the drop-down is open; otherwise, false. The default is false.</value>
		/// <seealso cref="System.Windows.Controls.ComboBox.IsDropDownOpen"/>
		public bool IsDropDownOpen { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

	}

	public sealed class ItemContainer : UIPropertiesBase {

		internal ItemContainer(IObjectVM owner) :base(owner) {}

		/// <summary> Gets or sets a value that indicates whether an item is selected.
		/// </summary>
		/// <value><c>true</c> if this item is selected; otherwise, <c>false</c>.</value>
		/// <seealso cref="System.Windows.Controls.Primitives.Selector.IsSelectedProperty"/>
		public bool IsSelected { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }


		/// <summary> Gets a value that indicates whether the keyboard focus is within the value returned by a Selector.
		/// </summary>
		/// <value><c>true</c> if the keyboard focus is within the Selector control; otherwise, <c>false</c>.</value>
		/// <seealso cref="System.Windows.Controls.Primitives.Selector.IsSelectionActiveProperty"/>
		public int IsSelectionActive { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }

	}
}