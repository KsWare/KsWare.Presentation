using System;
using System.Windows.Data;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.Providers;

namespace KsWare.Presentation.ViewModelFramework.Providers {

	public interface IBindingProvider:IProvider {
		object Source { get; set; }
		BindingMode Mode { get; set; }
		BindingBase Binding { get; set; }
	}

	public abstract class BindingProvider : ViewModelProvider, IBindingProvider {

		private object _source;

		public BindingProvider() {}

		/// <summary> Gets a value indicating whether the provider is supported.
		/// </summary>
		/// <value><see langword="true"/> if this instance is supported; otherwise, <see langword="false"/>.</value>
		/// <remarks></remarks>
		public override bool IsSupported => true;

		/// <summary> [NOT IMPLEMENTED] Always BindingMode.Auto
		/// </summary>
		public BindingMode Mode { get; set; }

		/// <summary>  [NOT IMPLEMENTED]
		/// </summary>
		public BindingBase Binding { get; set; }

		/// <summary> Gets or sets the source.
		/// </summary>
		/// <value>The source.</value>
		public object Source {
			get => _source;
			set {
				var prev = _source;
				_source = value;
				OnSourceChanged(new ValueChangedEventArgs(prev, value));
				OnPropertyChanged("Source");
			}
		}

		protected virtual void OnSourceChanged(ValueChangedEventArgs e) {
			
		}

		protected override void OnParentChanged() {
			base.OnParentChanged();
			Parent.ParentChanged += AtViewModelChanged;
			if (Parent.Parent != null) AtViewModelChanged(null, null);
		}

		private void AtViewModelChanged(object sender, EventArgs e) {
			var parent = (ViewModelMetadata) Parent;
			var vm=parent.Parent;
			OnViewModelChanged(vm);
		}

		protected virtual void OnViewModelChanged(IObjectVM viewModel) {
			
		}

	}

	public class ValueBindingProvider:BindingProvider {
		//TODO only IValueVM/DataProvider.Data is bound at present

		/*\					Local		Source
		 *  ----------------------------------
		 *  DataProvider	Data    <-> Data
		 * 
		 * 
		\*/

		private bool _dataChangedLock;

		public ValueBindingProvider() {}

		public new ViewModelMetadata Parent { get => (ViewModelMetadata) base.Parent; set => base.Parent = value; }
		public new IValueVM ViewModel => (IValueVM) base.ViewModel;
		public new IValueVM Source { get => (IValueVM) base.Source; set => base.Source = value; }

		protected override void OnViewModelChanged(IObjectVM viewModel) {
			base.OnViewModelChanged(viewModel);
			var parent = (ViewModelMetadata) Parent;
			parent.DataProvider.DataChanged+=LocalValueDataChanged;
		}

		private void LocalValueDataChanged(object sender, DataChangedEventArgs e) {
			if(_dataChangedLock) return;
			_dataChangedLock = true;
			try {
				var source = Source as IValueVM;
				source.Metadata.DataProvider.Data = e.NewData;
			}
			finally {_dataChangedLock = false;}
		}

		protected override void OnSourceChanged(ValueChangedEventArgs e) {
			base.OnSourceChanged(e);

			var source = Source as IValueVM;
			if (source != null) {
				source.Metadata.DataProvider.DataChanged+=LinkedValueDataChanged;
				LinkedValueDataChanged(null,new DataChangedEventArgs(null,source.Metadata.DataProvider.Data));				
			}
		}

		private void LinkedValueDataChanged(object sender, DataChangedEventArgs e) {
			if(_dataChangedLock)return;
			_dataChangedLock = true;
			try {
				var parent = (ViewModelMetadata) Parent;
				parent.DataProvider.Data = e.NewData;
			}
			finally {_dataChangedLock = false;}
		}

	}

	public class ActionBindingProvider : BindingProvider {

		/*\					Local		Source
		 *  --------------------------------------
		 *	ActionProvider			<-- CanExecute
		 *					Execute  --> 
		\*/

		private ActionVM LinkedAction => (ActionVM) Source;

		/// <summary> Initializes a new instance of the <see cref="ActionBindingProvider"/> class.
		/// </summary>
		public ActionBindingProvider() {}

		public new ActionMetadata Parent { get => (ActionMetadata) base.Parent; set => base.Parent = value; }
		public new ActionVM ViewModel => (ActionVM) base.ViewModel;
		public new ActionVM Source { get => (ActionVM) base.Source; set => base.Source = value; }

		protected override void OnViewModelChanged(IObjectVM viewModel) {
			Parent.ActionProvider.ExecutedCallback=LocalActionProviderExecutedCallback;
		}

		private void LocalActionProviderExecutedCallback(object sender, ExecutedEventArgs e) {
			Source.Metadata.ActionProvider.Execute(e.Parameter);
		}

		protected override void OnSourceChanged(ValueChangedEventArgs e) {
			base.OnSourceChanged(e);
			if (Source == null) return;
			Source.Metadata.ActionProvider.CanExecuteChanged+=AtSourceActionProviderOnCanExecuteChanged;
			AtSourceActionProviderOnCanExecuteChanged(null,null);
		}

		private void AtSourceActionProviderOnCanExecuteChanged(object sender, EventArgs e) {
			Parent.ActionProvider.SetCanExecute("Source can not execute",Source.Metadata.ActionProvider.CanExecute);
		}

	}

	[Obsolete("Not Implemented",true)]
	public class ListBindingProvider : BindingProvider { }
}