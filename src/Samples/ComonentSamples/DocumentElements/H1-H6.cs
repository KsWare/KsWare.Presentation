using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.ComponentSamples.DocumentElements {

	public class H : TextBlock {
		protected H() { }
	}

	public class H1 : H {
		static H1() { DefaultStyleKeyProperty.OverrideMetadata(typeof(H1), new FrameworkPropertyMetadata(typeof(H1))); }
	}

	public class H2 : H {
		static H2() { DefaultStyleKeyProperty.OverrideMetadata(typeof(H2), new FrameworkPropertyMetadata(typeof(H2))); }
	}

	public class H3 : H {
		static H3() { DefaultStyleKeyProperty.OverrideMetadata(typeof(H3), new FrameworkPropertyMetadata(typeof(H3))); }
	}

	public class H4 : H {
		static H4() { DefaultStyleKeyProperty.OverrideMetadata(typeof(H4), new FrameworkPropertyMetadata(typeof(H4))); }
	}

	public class H5 : H {
		static H5() { DefaultStyleKeyProperty.OverrideMetadata(typeof(H5), new FrameworkPropertyMetadata(typeof(H5))); }
	}

	public class H6 : H {
		static H6() { DefaultStyleKeyProperty.OverrideMetadata(typeof(H6), new FrameworkPropertyMetadata(typeof(H6))); }
	}
}
