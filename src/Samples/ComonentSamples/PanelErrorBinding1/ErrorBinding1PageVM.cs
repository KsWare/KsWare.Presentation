using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ComponentSamples.PanelErrorBinding1 {

	public class ErrorBinding1PageVM : ObjectVM {

		public ErrorBinding1PageVM() {
			RegisterChildren(()=>this);

			
		}

		public Int32VM Int32Value { get; [UsedImplicitly] private set; }

		public int Int32Field { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }
	}
}
