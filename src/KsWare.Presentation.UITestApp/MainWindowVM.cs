﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Baml2006;
using System.Xaml;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.UITestApp {

	public class MainWindowVM : WindowVM {

		public MainWindowVM() {
			RegisterChildren(()=>this);
			MenuItems.Add(new MenuItemVM {
				Caption = "_File",
				Items = {
					new MenuItemVM{Caption = "_Exit", CommandAction = { MːDoAction = DoExit}}
				}
			});
			MenuItems.Add(new MenuItemVM {
				Caption = "_View",
				Items   = {
					new MenuItemVM {Caption = "Dialog Window", CommandAction = {MːDoAction = DoShowDialog}},
					new MenuItemVM {Caption = "Start Page", CommandAction = {MːDoAction = DoShowStartPage}}
				}
			});

			if(IsInDesignMode) return;

			ScanPages();

			Fields[nameof(SelectedPage)].ValueChangedEvent.add = AtSelectedPageChanged;
		}

		private void AtSelectedPageChanged(object sender, ValueChangedEventArgs e) {
			if (e.NewValue == null) {
				FrameController.Navigate((FrameworkElement)null);
			}
			else {
				var info= (PageInfoVM)e.NewValue;
				FrameController.Navigate(info.View);
			}
		}

		public FrameControllerVM FrameController { get; [UsedImplicitly] private set; }

		private void ScanPages() {
			// KsWare.Presentation.UITestApp.g.resources
			var asm     = Assembly.GetEntryAssembly();
			var resName = asm.GetName().Name + ".g.resources";
			using (var stream = asm.GetManifestResourceStream(resName))
			using (var reader = new System.Resources.ResourceReader(stream)) {
				var resources = reader.Cast<DictionaryEntry>().Select(entry => (string) entry.Key).ToArray();
				foreach (var resource in resources) {
					if(!resource.StartsWith("pages/")) continue;
					var uri = new Uri(resource.Substring(0, resource.Length - 5) + ".xaml", UriKind.Relative);
					var restream = Application.GetResourceStream(uri).Stream;
					var bamlReader = new Baml2006Reader(restream);
					var writer = new XamlObjectWriter(bamlReader.SchemaContext);
					try {
						while (bamlReader.Read()) writer.WriteNode(bamlReader);
					}
					catch (Exception ex) {
						var lineInfo = (IXamlLineInfo)bamlReader;
						Debug.WriteLine($"Error loading {resource}: {lineInfo.LineNumber},{lineInfo.LinePosition}. Message: {ex.Message}");
						MessageBox.Show($"Error loading {resource}: {lineInfo.LineNumber},{lineInfo.LinePosition}. \nMessage: \n{ex.Message}","Error");
						continue;
					}
					var xamlObject = writer.Result;
					var name = xamlObject.GetType().Name;

					Pages.Add(new PageInfoVM() {
						DisplayName = name,
						Uri         = uri,
						View        = (FrameworkElement) xamlObject
					});
				}
			}
		}

		private void DoShowStartPage() {

		}

		public ListVM<MenuItemVM> MenuItems { get; [UsedImplicitly] private set; }

		public ListVM<PageInfoVM> Pages { get; [UsedImplicitly] private set; }

		public PageInfoVM SelectedPage { get => Fields.GetValue<PageInfoVM>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to ShowDialog
		/// </summary>
		public ActionVM ShowDialogAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="ShowDialogAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoShowDialog() {
			var result = new SampleDialogWindowVM{Owner = this}.ShowDialog();
			var s = result.HasValue ? result.ToString() : "null";
			MessageBox.Show($"DialogResult: {s}");
		}

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Exit
		/// </summary>
		public ActionVM ExitAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="ExitAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoExit() {
			Close();
		}
	}

}