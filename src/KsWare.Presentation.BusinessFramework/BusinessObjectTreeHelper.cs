/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : BusinessObjectTreeHelper.cs
 * OriginalNamespace: KsWare.Presentation.BusinessFramework
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides static helper methods for logical business tree element queries.
	/// </summary>
	public static class BusinessObjectTreeHelper {

		/// <summary> Returns the logical parent business object of the specified business object.
		/// </summary>
		/// <param name="current">The element to find the parent for. </param>
		/// <returns>The logical parent business object or null</returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static IObjectBM GetParent(IObjectBM current) { return (IObjectBM) current.Parent; }

		/// <summary> Returns the logical parent business object of the specified business object.
		/// </summary>
		/// <param name="current">The element to find the children for. </param>
		/// <returns>The logical children business objects or null</returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static ICollection<IObjectBM> GetChildren(IObjectBM current) {
			/*REVISE:*/
			return current.Children.Cast<IObjectBM>().ToList().AsReadOnly();
		}

		/// <summary> Gets the level of the business object.
		/// </summary>
		/// <param name="businessObject">The business object.</param>
		/// <returns>Level begin with 0</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static int GetLevel(IObjectBM businessObject){
			int level = 0;
			var parentObject = (IObjectBM)businessObject.Parent;
			while (parentObject != null){
			    parentObject = (IObjectBM)parentObject.Parent;
			    level++;
			}
			return level;
		}

		/// <summary> Iterate over all childs and lists from business object root with a recursive function.
		/// </summary>
		/// <param name="businessObject">The business object root.</param>
		/// <param name="action">The standard action delegate.</param>
		public static void ForEach(IObjectBM businessObject, Action<IObjectBM> action) {
			action.DynamicInvoke(new object[] {businessObject});

			var businessList = businessObject as IListBM;
			if (businessList!=null) {
			    foreach (IItemBM childObject in businessList) {
			        ForEach((ObjectBM) childObject, action); //TODO REVISE suspipious cast
			    }
			} else if(businessObject!=null) {
			    foreach (ObjectBM childObject in businessObject.Children) {
			        ForEach(childObject, action);
			    }
			} else {
				throw new NotImplementedException("{526690D2-0E07-46F4-98BC-2EFC636A919A}");
			}
		}

		/// <summary> Iterate over all childs and lists from business object root with a recursive function.
		/// </summary>
		/// <param name="businessObject">The business object root.</param>
		/// <param name="action">The standard action delegate.</param>
		public static void ForEachChild(IObjectBM businessObject, Action<IObjectBM> action) {
			action.DynamicInvoke(new object[] {businessObject});
			foreach (ObjectBM childObject in businessObject.Children) {
			    ForEachChild(childObject, action);
			}
		}

		/// <summary> Iterate over all childs and lists from business object root with a recursive function.
		/// </summary>
		/// <param name="businessObject">The business object root.</param>
		/// <param name="action">The standard action delegate.</param>
		public static void ForEachItem(IObjectBM businessObject, Action<IObjectBM> action) {
			action.DynamicInvoke(new object[] {businessObject});
			var bList = businessObject as IListBM;
			if (bList!=null) {
			    foreach (IItemBM childObject in bList) {
			    	var bObj = childObject as ObjectBM;
			        if(bObj!=null) ForEach(bObj, action);
					else throw new NotImplementedException("{3F920045-C968-41BB-A77C-0120EED027A6}");
			    }
			} else {
				throw new NotImplementedException("{10CACD37-AD48-4818-BCB5-3D0CDE5BA430}");
			}
		}

		#region TreeChanged event utils

		private static int s_DelayTreeChangedEventsCount;
		private static int s_SkipTreeChangedEventsCount;
		private static readonly List<Tuple<EventHandler<TreeChangedEventArgs>, ObjectBM, TreeChangedEventArgs>> s_DelayedTreeChangedEvents = new List<Tuple<EventHandler<TreeChangedEventArgs>, ObjectBM, TreeChangedEventArgs>>();
		private static bool s_RaisingDelayedTreeChangeEvents;

		/// <summary> Occurs when tree has been changed.
		/// </summary>
		public static event EventHandler<TreeChangedEventArgs> TreeChanged;

		/// <summary> Pauses the tree changed events. Set only in the case that more than one values are set in the same event
		/// </summary>
		/// <returns></returns>
		public static IDisposable PauseTreeChangedEvents() {
//            Log.Write(null, Flow.Enter);
			s_DelayTreeChangedEventsCount++;
			return new PauseTreeChangedEventsTransaction();
		}

		/// <summary> Stops the tree changed events.
		/// Use this by loadfrom file, set default and undo and for all performance increase
		/// IMPORTANT: Use this not for performance if any ObjectBM is changed
		/// and they trigger changes of other ObjectBM in On TreeChange
		/// ValueChange and CollectionChange is set every time
		/// </summary>
		/// <returns></returns>
		public static IDisposable StopTreeChangedEvents() {
//            Log.Write(null, Flow.Enter);
			s_SkipTreeChangedEventsCount++;
			return new StopTreeChangedEventsTransaction();
		}

		/// <summary> Continues the tree change events.
		/// </summary>
		private static void ContinueTreeChangeEvents(bool isStop) {
//	       using (typeof (BusinessObjectMetadata).Log(Flow.Enter)) {
				if(isStop) {
					if(s_SkipTreeChangedEventsCount==0) 
						throw new InvalidOperationException("Continue w/o Stop!"+"\r\n\tUniqueID: {551E5B13-C818-4986-BF9F-32DFD6637DC6}");
						s_SkipTreeChangedEventsCount--;	
				} else {
					if(s_DelayTreeChangedEventsCount==0) 
						throw new InvalidOperationException("Continue w/o pause!"+"\r\n\tUniqueID: {E922623A-FCBD-40FF-9ADD-86E2B041B01A}");
						s_DelayTreeChangedEventsCount--;
				}

				if (s_SkipTreeChangedEventsCount  > 0) return;
				if (s_DelayTreeChangedEventsCount > 0) return;
				if (s_RaisingDelayedTreeChangeEvents ) return;
				if (isStop                         ) return;

				RaiseDelayedEvents();
//			}
		}

		private static void RaiseDelayedEvents() { 
			s_RaisingDelayedTreeChangeEvents = true;
			try {
				while (s_DelayedTreeChangedEvents.Count > 0) {
					var tuple = s_DelayedTreeChangedEvents[0];
					//Debug.WriteLine("=>Raise TreeChanged " + tuple.Item2.GetType().Name);
//	        		typeof (BusinessObjectTreeHelper).Log(Flow.RaiseEvent("TreeChanged").);
					EventUtil.Raise(tuple.Item1, tuple.Item2, tuple.Item3, "{F126564B-958D-4768-BA8D-C736DC3C9925}");
					s_DelayedTreeChangedEvents.RemoveAt(0);
				}        		
			} 
			catch { throw; } 
			finally { s_RaisingDelayedTreeChangeEvents = false; }		
		}

		/// <summary> Gets a value indicating whether tree events are raised.
		/// </summary>
		/// <value><see langword="true"/> if tree event are raised; otherwise, <see langword="false"/>.
		/// </value>
		public static bool IsTreeChangedEventDelayed => s_DelayTreeChangedEventsCount > 0;

		/// <summary> Gets a value indicating whether tree events are raised.
		/// </summary>
		/// <value><see langword="true"/> if tree event are skiped; otherwise, <see langword="false"/>.
		/// </value>
		public static bool IsTreeChangedEventSkipped => s_SkipTreeChangedEventsCount > 0;

		/// <summary> Helper class to manage StopTreeChangedEvents/StartTreeChangeEvents
		/// </summary>
		private class StopTreeChangedEventsTransaction: IDisposable {

			bool _Busy = true;

			void IDisposable.Dispose() {
				if (!_Busy) return;
				ContinueTreeChangeEvents(true);
				_Busy = false;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary> Helper class to manage StopTreeChangedEvents/StartTreeChangeEvents
		/// </summary>
		private class PauseTreeChangedEventsTransaction: IDisposable {

			bool _Busy = true;

			void IDisposable.Dispose() {
				if (!_Busy) return;
				ContinueTreeChangeEvents(false);
				_Busy = false;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary> Called by <see cref="IObjectBM"/>.OnTreeChanged(TreeChangedEventArgs) when tree changed.
		/// </summary>
		/// <param name="eventDelegate">The event delegate.</param>
		/// <param name="businessObject">The business object.</param>
		/// <param name="eventArgs">The <see cref="TreeChangedEventArgs"/> instance containing the event data.</param>
		internal static void OnTreeChanged(EventHandler<TreeChangedEventArgs> eventDelegate, ObjectBM businessObject, TreeChangedEventArgs eventArgs) {
			if (eventDelegate == null) return;
			if (!IsTreeChangedEventDelayed && !IsTreeChangedEventSkipped && s_DelayedTreeChangedEvents.Count == 0) {
				EventUtil.Raise(eventDelegate, businessObject, eventArgs, "{3B5112B8-C8D9-4E00-AC37-AE191EA26E79}");
			}else if (!IsTreeChangedEventSkipped) {
				s_DelayedTreeChangedEvents.Add(new Tuple<EventHandler<TreeChangedEventArgs>, ObjectBM, TreeChangedEventArgs>(eventDelegate, businessObject, eventArgs));
			}
		}

		/// <summary> Called by <see cref="IObjectBM"/>.OnTreeChanged() when tree changed.
		/// </summary>
		/// <param name="businessObject">The business object.</param>
		/// <param name="eventArgs">The <see cref="TreeChangedEventArgs"/> instance containing the event data.</param>
		internal static void OnTreeChanged(ObjectBM businessObject, TreeChangedEventArgs eventArgs) { OnTreeChanged(TreeChanged, businessObject, eventArgs); }

		/// <summary> [TEST]
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public static void RaiseTreeChangedEventDirect() { if (TreeChanged != null) TreeChanged(null, null); }

		/// <summary> [TEST]
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public static void RaiseTreeChangedEventWithHelper() { EventUtil.Raise(TreeChanged, null, null, "{07434A30-743D-4BB8-8F49-1161BA900183}"); }

		#endregion

		/// <summary> Gets the ancestor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="start">The start object</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		[CanBeNull]
		public static T GetAncestor<T>(IObjectBM start) where T:IObjectBM { 
			var parent = (IObjectBM) start.Parent;
			if (parent == null) return default(T);
			IObjectBM p = parent;
			while (p != null) {
				if(p is T) return (T) p;
			    p = (IObjectBM) p.Parent;
			}
			return default(T);
		}
	}
}