using System;
using System.Runtime.CompilerServices;

namespace KsWare.Presentation {

	public abstract class Singleton<T> where T: class {

		private static volatile T s_Instance;

		protected Singleton() { }

		public static T Instance {
			[MethodImpl(MethodImplOptions.Synchronized)]
			get {
				if (s_Instance == null) s_Instance = (T) Activator.CreateInstance(typeof(T),true);
				return s_Instance;
			}
		}

	}

}