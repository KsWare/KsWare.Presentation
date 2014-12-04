using System;

namespace KsWare.Presentation {

	/// <summary> Model interface
	/// </summary>
	public interface IModel:IDisposable 
	{
		/// <summary> Occurs when this instance is disposed.
		/// </summary>
		event EventHandler Disposed;

//RESERVED:		bool IsDisposed{get;}
	}
}
