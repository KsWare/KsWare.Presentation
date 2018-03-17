using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework {

	public interface ISlimObjectVM : INotifyPropertyChanged,IDisposable {

	}


	public class SlimObjectVM: ISlimObjectVM {

		private readonly Lazy<BackingFieldsStore> _LazyFields;

		public SlimObjectVM() {
			_LazyFields=new Lazy<BackingFieldsStore>(()=>new BackingFieldsStore(this,OnPropertyChanged));
		}

		public BackingFieldsStore Fields => _LazyFields.Value;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName) {
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool explicitDispose) {  }

	}

}