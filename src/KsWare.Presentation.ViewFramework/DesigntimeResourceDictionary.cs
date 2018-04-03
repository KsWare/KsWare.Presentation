﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace KsWare.Presentation.ViewFramework {

	/// <summary>
	/// Provides a <see cref="ResourceDictionary"/> which is used only at design time.
	/// </summary>
	/// <seealso cref="System.Windows.ResourceDictionary" />
	/// <autogeneratedoc />
	public class DesigntimeResourceDictionary : ResourceDictionary {

		private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

		private static readonly Uri EmptyResourceDictionary=new Uri("pack://application:,,,/KsWare.Presentation.ViewFramework;component/Resources/EmptyResourceDictionary.xaml",UriKind.Absolute);

		private readonly ObservableCollection<ResourceDictionary> _mergedDictionaries = new DesigntimeObservableCollection<ResourceDictionary>();

		/// <summary>
		/// Initializes a new instance of the <see cref="DesigntimeResourceDictionary"/> class.
		/// </summary>
		/// <autogeneratedoc />
		public DesigntimeResourceDictionary() {
			var fieldInfo = typeof(ResourceDictionary).GetField("_mergedDictionaries", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fieldInfo != null) fieldInfo.SetValue(this, _mergedDictionaries);
		}

		/// <inheritdoc cref="ResourceDictionary.Source"/>
		/// <remarks>Source is allways null at runtime.</remarks>
		public new Uri Source {
			get => IsInDesignMode ? base.Source : EmptyResourceDictionary;
			set => base.Source = IsInDesignMode ? value : EmptyResourceDictionary;
		}

		private class DesigntimeObservableCollection<T> : ObservableCollection<T> {
			protected override void InsertItem(int index, T item) {
				// Only insert items while in Design Mode (VS is hosting the visualization)
				if (IsInDesignMode) base.InsertItem(index, item);
			}
		}

	}
}