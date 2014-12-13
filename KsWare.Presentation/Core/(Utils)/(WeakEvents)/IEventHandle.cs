using System;

namespace KsWare.Presentation {

	/// <summary> Interface for weak event handle
	/// </summary>
	public interface IEventHandle:IDisposable {

		/// <summary> Releases this weak event.
		/// </summary>
		void Release();
	}

}