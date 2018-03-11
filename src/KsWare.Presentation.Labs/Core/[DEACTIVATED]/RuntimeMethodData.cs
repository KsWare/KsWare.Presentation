using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace KsWare.Presentation {

	[Obsolete("Unknown usage",true)]
	internal class RuntimeMethodData {

		private readonly List<int> _ThreadIds=new List<int>();
		private readonly ThreadLocal<int> _RecursiveInvokes=new ThreadLocal<int>();
		private volatile int _SimultaneousInvokes;

		public RuntimeMethodData() {
			ThreadIDs=new ReadOnlyCollection<int>(_ThreadIds);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public bool EnableThreadObservation { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public int Count { get; private set; }
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public DateTime LastAccess { get; private set; }
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public ReadOnlyCollection<int> ThreadIDs { get; private set; }
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public int SimultaneousInvokes {get { return _SimultaneousInvokes; }}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public int RecursiveInvokes { 
			get {
				if (!_RecursiveInvokes.IsValueCreated) _RecursiveInvokes.Value = 0;
				return _RecursiveInvokes.Value; 
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public void Trigger() {
			Count++;
			LastAccess = DateTime.Now;
			if(EnableThreadObservation && !_ThreadIds.Contains(Thread.CurrentThread.ManagedThreadId)) _ThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void TriggerEnter() {
			Count++;
			_SimultaneousInvokes++; //does mean recursive calls AND multiple thread calls
			_RecursiveInvokes.Value++;
			LastAccess = DateTime.Now;
			if(EnableThreadObservation && !_ThreadIds.Contains(Thread.CurrentThread.ManagedThreadId)) _ThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
		}

		private void TriggerExit() {
			_SimultaneousInvokes--;
			_RecursiveInvokes.Value--;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public IDisposable Access{get { return new ExitTrigger(this); }}

		private class ExitTrigger:IDisposable  {
			private RuntimeMethodData _RuntimeMethodData;

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			public ExitTrigger(RuntimeMethodData runtimeMethodData) {
				_RuntimeMethodData = runtimeMethodData;
				_RuntimeMethodData.TriggerEnter();
			}

			void IDisposable.Dispose() {
				_RuntimeMethodData.TriggerExit();
				_RuntimeMethodData = null;
			}
		}
	}
}