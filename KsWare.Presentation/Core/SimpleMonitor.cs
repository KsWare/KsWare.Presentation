using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KsWare.Presentation.Core {

	/// <summary>  Provides a simple monitor
	/// </summary>
	/// <remarks>
	/// The <see cref="SimpleMonitor"/> can be used to prevent calling methods from event handlers.
	/// </remarks>
	/// <example> Usage:
	/// <code>
	/// SimpleMonitor myEventMonitor = new SimpleMonitor();
	/// 
	/// protected IDisposable BlockReentrancy() {
	///     this.myEventMonitor.Enter();
	///	    return this.myEventMonitor;
	///	}
	///
	///	protected void CheckReentrancy() {
	///	    if ((this.myEventMonitor.IsBusy &amp;&amp; (this.MyEvent != null)) &amp;&amp; (this.MyEvent.GetInvocationList().Length &gt; 1)) {
	///	        throw new InvalidOperationException("Cannot change {ClassName} during a {MyEvent} event.");
	///	    }
	///	}
	/// 
	/// protected virtual void OnMyEvent(EventArgs e) {
	///     if (this.MyEvent != null) {
	///         using (this.BlockReentrancy()) {
	///             this.MyEvent(this, e);
	///         }
	///     }
	/// }
	/// 
	/// public event EventHandler MyEvent;
	/// 
	/// public void MyMonitoredEventRaisingMethod() {
	///     this.CheckReentrancy();
	///     // do anything
	///     OnMyEvent(EventArgs.Empty);
	/// }
	/// </code>
	/// </example>
	[Serializable]
	public sealed class SimpleMonitor:IDisposable
	{
		private int busyCount;

		/// <summary> Initializes a new instance of the <see cref="SimpleMonitor"/> class.
		/// </summary>
		public SimpleMonitor() {}

		/// <summary> Acquires a lock
		/// </summary>
		public SimpleMonitor Enter() { this.busyCount++; return this;}

		/// <summary> Releases a lock 
		/// </summary>
		public void Exit() { this.busyCount--; }
		
		[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		void IDisposable.Dispose() { Exit(); /*GC.SuppressFinalize(this);*/}

		/// <summary> Gets a value indicating whether this <see cref="SimpleMonitor"/> is busy.
		/// </summary>
		/// <value><c>true</c> if busy; otherwise, <c>false</c>.</value>
		public bool IsBusy {get {return (this.busyCount > 0);}}


		static readonly Dictionary<object,SimpleMonitor> instances=new Dictionary<object, SimpleMonitor>();

		/// <summary>
		/// Enters the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static IDisposable Enter(object key) {
			SimpleMonitor simpleMonitor;
			if(instances.ContainsKey(key)) {
				simpleMonitor = instances[key];
			} else {
				simpleMonitor = new SimpleMonitor();
				instances.Add(key,simpleMonitor);
			}
			simpleMonitor.Enter();
			return simpleMonitor;
		}

		/// <summary>
		/// Exits the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		public static void Exit(object key) {
			var simpleMonitor = instances[key];
			simpleMonitor.Exit();
			if(!simpleMonitor.IsBusy) instances.Remove(key);
		}

		/// <summary>
		/// Gets the is busy.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static bool GetIsBusy(object key) {
			if(!instances.ContainsKey(key)) return false;
			return instances[key].IsBusy;
		}
	}
}
