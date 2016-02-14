using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KsWare.Presentation
{
//	public interface IDebugHistory
//	{
//		Stack<string> DebugHistory { get; }
//	}

	/// <summary> Extension for debug object instances
	/// </summary>
	/// <remarks></remarks>
	public static class ObjectDebugExtensions
	{
// ReSharper disable InconsistentNaming
		private static readonly IFormatProvider enUS = CultureInfo.CreateSpecificCulture("en-US");
// ReSharper restore InconsistentNaming
 
		#region InstanceId / TypeInstanceId

		private static readonly object instanceIdLock = new object();
		private static readonly List<Tuple<WeakReference,long,long>> instanceIds=new List<Tuple<WeakReference, long, long>>();
		private static long lastInstanceId;
		private static readonly Dictionary<Type,long> lastTypeInstanceIds=new Dictionary<Type, long>();

		/// <summary> Gets the instance ID
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId="obj")]
		public static long InstanceId(this object obj) {
			lock(instanceIdLock) {
				long instanceId;
				for(int i=0;i<instanceIds.Count;i++) {
					if(!instanceIds[i].Item1.IsAlive){instanceIds.RemoveAt(i);i--;continue;}
					if(instanceIds[i].Item1.Target==obj) {
						instanceId=instanceIds[i].Item2;
						if(instanceId==0) instanceId = ++lastInstanceId;
						instanceIds[i]=new Tuple<WeakReference, long, long>(instanceIds[i].Item1,instanceId,instanceIds[i].Item3);
						return instanceId;
					}
				}
				instanceIds.Add(new Tuple<WeakReference, long,long>(new WeakReference(obj), ++lastInstanceId,0));
				return lastInstanceId;
			}
		}

		/// <summary> Gets the type specific instance ID
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId="obj")]
		public static long TypeInstanceId(this object obj) {
			lock(instanceIdLock) {
				long typeInstanceId;
				for(int i=0;i<instanceIds.Count;i++) {
					if(!instanceIds[i].Item1.IsAlive){instanceIds.RemoveAt(i);i--;continue;}
					if(instanceIds[i].Item1.Target==obj) {
						typeInstanceId = instanceIds[i].Item3;//could be '0'
						if(typeInstanceId==0) typeInstanceId = NewTypeInstanceId(obj);
						instanceIds[i]=new Tuple<WeakReference, long, long>(instanceIds[i].Item1,instanceIds[i].Item2,typeInstanceId);
						return typeInstanceId;
					}
				}
				typeInstanceId = NewTypeInstanceId(obj);
				instanceIds.Add(new Tuple<WeakReference, long, long>(new WeakReference(obj), 0, typeInstanceId));
				return typeInstanceId;
			}
		}

		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId="obj")]
		private static long NewTypeInstanceId(object obj) {
			long typeInstanceId;
			var type = obj.GetType();
			if(lastTypeInstanceIds.ContainsKey(type)) {
				typeInstanceId = lastTypeInstanceIds[type] = lastTypeInstanceIds[type]+1;
			} else {
				typeInstanceId = 1;
				lastTypeInstanceIds[type] = typeInstanceId;
			}
			return typeInstanceId;
		}
		


		#endregion

		#region DebugObjectTrace

		private class DebugObjectTraceːEntry
		{
			public DateTime Timestamp { get; set; }
			public String Message { get; set; }
		}

		private static readonly Dictionary<string, List<DebugObjectTraceːEntry>> DebugObjectTraceEntries = new Dictionary<string, List<DebugObjectTraceːEntry>>();
		private static readonly Dictionary<string,object> DebugObjectTraceStopedObjects=new Dictionary<string, object>();
		private static readonly Dictionary<string,WeakReference> DebugObjectTraceRegisteredObjects=new Dictionary<string, WeakReference>();

		/// <summary>
		/// Adds a new trace entry to the object trace
		/// </summary>
		/// <param name="o">The traced object</param>
		/// <param name="message">The message to add</param>
		[Conditional("DEBUG"),DebuggerStepThrough]
		public static void DebugObjectTrace(this object o, string message) {
			List<DebugObjectTraceːEntry> trace;
			var key = GetObjectKey(o);
			if(DebugObjectTraceEntries.ContainsKey(key)) trace = DebugObjectTraceEntries[key];
			else DebugObjectTraceEntries.Add(key,trace=new List<DebugObjectTraceːEntry>());
			trace.Add(new DebugObjectTraceːEntry{Timestamp = DateTime.Now,Message = message});

			//store object for later use
			if(!DebugObjectTraceRegisteredObjects.ContainsKey(key))
				DebugObjectTraceRegisteredObjects.Add(key,new WeakReference(o));
		}

		/// <summary> Debugs the object traceˑ get object.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static object DebugObjectTraceˑGetObject(string key) {
			WeakReference wref = DebugObjectTraceRegisteredObjects[key];
			if(wref==null) return null;
			if(!wref.IsAlive) return null;
			return wref.Target;
		}

		/// <summary>  Debugs the object traceˑ get object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public static object DebugObjectTraceˑGetObject(Type type,long id) {
			string key = type.FullName + "#" + id;
			return DebugObjectTraceˑGetObject(key);
		}

		/// <summary> Adds a new trace entry to the object trace
		/// </summary>
		/// <param name="o">The traced object</param>
		/// <param name="format">format</param>
		/// <param name="args">args</param>
		/// <remarks></remarks>
		[Conditional("DEBUG"),DebuggerStepThrough]
		public static void DebugObjectTrace(this object o, string format, params string[] args) {
			if(args==null || args.Length==0) DebugObjectTrace(o, format);
			else DebugObjectTrace(o, string.Format(enUS, format, args));
		}


		/// <summary>  Debugs the object traceˑ create.
		/// </summary>
		/// <param name="o">The o.</param>
		[Conditional("DEBUG"),DebuggerStepThrough]
		public static void DebugObjectTraceˑCreate(this object o) {
			DebugObjectTrace(o, string.Format(enUS, "Create {0}", o.GetObjectKey()));
		}

		/// <summary>  Debugs the object traceˑ assign.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <param name="what">The what.</param>
		/// <param name="who">The who.</param>
		[Conditional("DEBUG"),DebuggerStepThrough]
		public static void DebugObjectTraceˑAssign(this object o,string what, object who) {
			DebugObjectTrace(o, string.Format(enUS, "Assign {0} {1}", what, who==null?"NULL":who.GetObjectKey()));
		}

		/// <summary>  Debugs the object traceˑ assign.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <param name="what">The what.</param>
		/// <param name="newRef">The new ref.</param>
		/// <param name="oldRef">The old ref.</param>
		[Conditional("DEBUG"),DebuggerStepThrough]
		public static void DebugObjectTraceˑAssign(this object o, string what, object newRef, object oldRef) {
			if(oldRef!=null) DebugObjectTrace(o, string.Format(enUS, "Unassign {0} {1}", what, oldRef.GetObjectKey()));
			DebugObjectTrace(o, string.Format(enUS, "Assign {0} {1}", what, newRef==null?"NULL":newRef.GetObjectKey()));
		}

		/// <summary>  Usage: e.g. add watch ObjectDebugExtensions.DebugObjectTraceToString(this)
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static string DebugObjectTraceˑToString(this object o) {
			var key = GetObjectKey(o);
			return DebugObjectTraceˑToString(key);
		}

		/// <summary> Clears the trace for the object
		/// </summary>
		/// <param name="o">The object</param>
		/// <remarks></remarks>
		public static void DebugObjectTraceˑClear(this object o) {
			var k = GetObjectKey(o);
			if(!DebugObjectTraceEntries.ContainsKey(k))return;
			DebugObjectTraceEntries.Remove(k);
		}

		/// <summary> Stops the trace for the object
		/// </summary>
		/// <param name="o">The object</param>
		/// <remarks></remarks>
		[Conditional("DEBUG"),DebuggerStepThrough]
		public static void DebugObjectTraceˑStop(this object o) {
			var k = GetObjectKey(o);
			if(DebugObjectTraceStopedObjects.ContainsKey(k)) {
				return;
			} else {
				DebugObjectTraceStopedObjects.Add(k,null);
			}
		}

		/// <summary> Starts the trace for the object
		/// </summary>
		/// <param name="o">The object</param>
		/// <remarks></remarks>
		[Conditional("DEBUG"),DebuggerStepThrough]
		public static void DebugObjectTraceˑStart(this object o) {
			var k = GetObjectKey(o);
			if(DebugObjectTraceStopedObjects.ContainsKey(k)) {
				DebugObjectTraceStopedObjects.Remove(k);
			} else {
				return;
			}
		}


		/// <summary> Gets the object key.
		/// </summary>
		/// <param name="o">The object to get tye key</param>
		/// <returns>An unique object key, or "null" if <paramref name="o"/> is <c>null</c>.</returns>
		/// <remarks></remarks>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public static string GetObjectKey(this object o) {
			if(o==null) return "null";
			if(o is Type) return ((Type) o).FullName;
			return o.GetType().FullName + "#" + o.TypeInstanceId();
		}

		/// <summary> Gets the trace for the specified object key
		/// </summary>
		/// <param name="key">the object key</param>
		/// <returns></returns>
		public static string DebugObjectTraceˑToString(string key) {
			if(!DebugObjectTraceEntries.ContainsKey(key)) return null;
			var trace = DebugObjectTraceEntries[key];
			StringBuilder b=new StringBuilder();
			b.AppendFormat("Object trace: {0}", key);
			foreach (var entry in trace) {
				b.AppendFormat("\r\n{0:yyyy-MM-dd HH:mm:ss,fff}: {1}",entry.Timestamp, entry.Message);
			}
			return b.ToString();
		}

		/// <summary> Call this if the object is disposed.</summary>
		/// <param name="o">The object</param>
		/// <param name="explicitDisposing">if set to <c>true</c> explicit disposing.</param>
		/// <remarks></remarks>
		[Conditional("DEBUG"),DebuggerStepThrough]
		public static void DebugObjectTraceˑDispose(this object o, bool explicitDisposing) {
			
		}


		#endregion


		/// <summary>
		/// Gets the object type name and id.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public static string GetObjectTypeNameAndId(this object o) {
			if(o==null) return "null";
			if(o is Type) return ((Type) o).Name;
			return o.GetType().Name + "#" + o.TypeInstanceId();
		}

		
	}
}

// abcᆞdef
// abcˑdef
// abcːdef
// abcꀧdef
// abcㅿdef
// abcᅀdef
// abcᇫdef
// abcｰdef
// abcǀdef
// abcǁdef
// abcǂdef
// abcǃdef
// abcㅤdef
