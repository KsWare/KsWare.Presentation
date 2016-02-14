using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace KsWare.Presentation.Core {

	[Obsolete("Unknown usage",true)]
	internal class RuntimeMethodData {

		private readonly List<int> m_ThreadIds=new List<int>();
		private readonly ThreadLocal<int> m_RecursiveInvokes=new ThreadLocal<int>();
		private volatile int m_SimultaneousInvokes;

		public RuntimeMethodData() {
			ThreadIDs=new ReadOnlyCollection<int>(m_ThreadIds);
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
		public int SimultaneousInvokes {get { return m_SimultaneousInvokes; }}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public int RecursiveInvokes { 
			get {
				if (!m_RecursiveInvokes.IsValueCreated) m_RecursiveInvokes.Value = 0;
				return m_RecursiveInvokes.Value; 
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public void Trigger() {
			Count++;
			LastAccess = DateTime.Now;
			if(EnableThreadObservation && !m_ThreadIds.Contains(Thread.CurrentThread.ManagedThreadId)) m_ThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void TriggerEnter() {
			Count++;
			m_SimultaneousInvokes++; //does mean recursive calls AND multiple thread calls
			m_RecursiveInvokes.Value++;
			LastAccess = DateTime.Now;
			if(EnableThreadObservation && !m_ThreadIds.Contains(Thread.CurrentThread.ManagedThreadId)) m_ThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
		}

		private void TriggerExit() {
			m_SimultaneousInvokes--;
			m_RecursiveInvokes.Value--;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public IDisposable Access{get { return new ExitTrigger(this); }}

		private class ExitTrigger:IDisposable  {
			private RuntimeMethodData m_RuntimeMethodData;

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			public ExitTrigger(RuntimeMethodData runtimeMethodData) {
				m_RuntimeMethodData = runtimeMethodData;
				m_RuntimeMethodData.TriggerEnter();
			}

			void IDisposable.Dispose() {
				m_RuntimeMethodData.TriggerExit();
				m_RuntimeMethodData = null;
			}
		}
	}
}