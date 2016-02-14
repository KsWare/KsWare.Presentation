using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KsWare.Presentation.Documentation {

	/// <summary> Markup extensions overview </summary>
	[Category("View Framework")]
	public abstract class docːmarkupˑextensionsˑoverview {

		#region hidden members
		private new string ToString() { return null; }
		private new object MemberwiseClone(){ return null; }
		private new Type GetType() { return null;}
		private new int GetHashCode() { return 0;}
		private new  bool Equals(Object obj) { return false;}
		#endregion

		/// <summary> <see cref="VisibilityBinding"/></summary>
		public object VisibilityBinding;

		/// <summary> <see cref="BindingWithPrefixAndSuffix"/></summary>
		public object BindingWithPrefixAndSuffix;

		/// <summary> <see cref="BindingWithPrefix"/></summary>
		public object BindingWithPrefix;

		/// <summary> <see cref="BindingWithSuffix"/></summary>
		public object BindingWithSuffix;

		/// <summary> <see cref="BindingWithValidation"/></summary>
		public object BindingWithValidation;



		/// <summary> <see cref="BindingWithValidation"/></summary>
		public object DebugViewer;

		/// <summary> <see cref="BindingUsesMultiBinding"/></summary>
		public object BindingUsesMultiBinding;

	}

}
