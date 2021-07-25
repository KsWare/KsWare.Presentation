using System;
using System.Collections.Generic;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior {
	// TODO remove: moved to KsWare.Presentation.ViewFramework.Common

	/// <summary> Provides support for a shared width (attached property)
	/// </summary>
	/// <example>
	/// Both Labels will have same width (largest width of the group).
	/// <code>
	/// xmlns:ab="clr-namespace:KsWare.Presentation.ViewFramework.AttachedBehavior;assembly=KsWare.Presentation" 
	/// &lt;Label Content="longest text" ab:SharedWidth.Group="swGroupA"/&gt;
	/// &lt;Label Content="short" ab:SharedWidth.Group="swGroupA"/&gt;
	/// </code>
	/// </example>
	public static class SharedWidth {

		#region Group Property

		/// <summary> </summary>
		public static readonly DependencyProperty GroupProperty = DependencyProperty.RegisterAttached(
			"Group", typeof(object), typeof(SharedWidth),
			new FrameworkPropertyMetadata(null,AtGroupChanged)
		);

		private static void AtGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { 
			Register(d,e.NewValue);
		}

		/// <summary> Gets the Group property. 
		/// </summary>
		public static object GetGroup(DependencyObject d) {
			return d.GetValue(GroupProperty);
		}

		/// <summary> Sets the Group property.  
		/// </summary>
		public static void SetGroup(DependencyObject d, object value) {
			d.SetValue(GroupProperty, value);
		}

		#endregion

		private static readonly Dictionary<FrameworkElement,object> FrameworkElements=new Dictionary<FrameworkElement,object>();
		private static readonly Dictionary<object,GroupData> Groups=new Dictionary<object, GroupData>();

		private static void Register(object frameworkElement,object group) {
			var elmt = (FrameworkElement) frameworkElement;
			if (elmt == null  /*possible in designer*/ ) return /*ignore*/;
			if(FrameworkElements.ContainsKey(elmt)) {
				var oldGroup = FrameworkElements[elmt];
				FrameworkElements[elmt] = group;
				Groups[oldGroup].FrameworkElements.Remove(elmt);
			} else {
				FrameworkElements.Add(elmt, group);
				elmt.SizeChanged+=AtFrameworkElementSizeChanged;
			}

			if(Groups.ContainsKey(group)) {
				Groups[group].FrameworkElements.Add(elmt);
			} else {
				Groups.Add(group,new GroupData(elmt));
			}
		}

		private static void AtFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e) {
			var frameworkElement = sender as FrameworkElement;
			if(frameworkElement==null){/*Uups*/ return;/*Ignore*/}

			if(!FrameworkElements.ContainsKey(frameworkElement)){/*Uups*/ return; /*Ignore*/}
			var group = FrameworkElements[frameworkElement];
			if(!Groups.ContainsKey(group)){/*Uups*/ return; /*Ignore*/}
			var groupData = Groups[group];

			if(e.NewSize.Width>groupData.MinWidth) {
				if (Equals(groupData.MinWidth, e.NewSize.Width)) return;
				groupData.MinWidth = e.NewSize.Width;
				ApplicationDispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate {
					foreach (var fe in groupData.FrameworkElements) {
					fe.MinWidth = groupData.MinWidth;
				}
				}));
			}
		}

		private class GroupData {
			public GroupData(FrameworkElement frameworkElement) {
				if (frameworkElement == null) throw new ArgumentNullException(nameof(frameworkElement));
				FrameworkElements= new List<FrameworkElement>(new []{frameworkElement});
			}
			public List<FrameworkElement> FrameworkElements{get;set;}
			public double MinWidth{get;set;}
		}
	}
}
