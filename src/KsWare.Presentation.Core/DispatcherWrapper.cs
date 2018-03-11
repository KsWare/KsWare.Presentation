using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	public partial interface IDispatcher/*:IDispatcher40*/ {

		/// <summary> Gets direct access to the wrapped <see cref="Dispatcher"/>.
		/// </summary>
		/// <value>The deprecated dispatcher.</value>
		[Obsolete("Deprecated")]
		Dispatcher DeprecatedDispatcher { get; }

		Thread Thread { get; }

		/// <summary> Gets a value indicating whether the caller must call an invoke method when making method calls to the UI because the caller is on a different thread than the one the UI was created on.
		/// </summary>
		bool IsInvokeRequired { get; }

		[Browsable(false),EditorBrowsable(EditorBrowsableState.Never)]
		DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method);

		[EditorBrowsable(EditorBrowsableState.Never),Browsable(false)]
		DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args);

		DispatcherOperation BeginInvoke(DispatcherPriority priority, Action method);

		DispatcherOperation BeginInvoke<T>(DispatcherPriority priority, Action<T> method, T arg);

		DispatcherOperation BeginInvoke<T1,T2>(DispatcherPriority priority, Action<T1,T2> method, T1 arg1, T2 arg2);

		DispatcherOperation BeginInvoke<T1,T2,T3>(DispatcherPriority priority, Action<T1,T2,T3> method, T1 arg1, T2 arg2, T3 arg3);

		DispatcherOperation BeginInvoke(Delegate method, params object[] args);

		object Invoke(Delegate method, params object[] args);

		object Invoke(Delegate callback);

		object Invoke(Delegate callback, DispatcherPriority priority);

//		object Invoke(Delegate callback, DispatcherPriority priority, CancellationToken cancellationToken);

//		object Invoke(Delegate callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout);

		TResult Invoke<TResult>(Func<TResult> callback);

		TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority);

//		TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken);

//		TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout);

		void Invoke(DispatcherPriority priority, Action callback);

		object Invoke([NotNull] Action action);

		object Invoke<T>([NotNull] Action<T> action, T arg);

		object Invoke<T1,T2>([NotNull] Action<T1,T2> action, T1 arg1, T2 arg2);

		/// <summary> Executes a delegate on the application dispatcher.
		/// </summary>
		/// <param name="method">The method to execute</param>
		/// <param name="args">The method arguments</param>
		/// <returns></returns>
		object InvokeIfRequired(Delegate method, params object[] args);
		object InvokeIfRequired<T>(Action<T> action, T arg);
		object InvokeIfRequired<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2);
		object InvokeIfRequired<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3 );

		/// <summary> Processes all Dispatcher messages currently in the dispatcher queue.
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		void DoEvents();

		DispatcherOperation BeginInvoke(Action action);

		DispatcherOperation BeginInvoke<T>(Action<T> action, T p0);

		DispatcherOperation BeginInvoke<T1, T2>(Action<T1, T2> action, T1 p0, T2 p1);

		DispatcherOperation BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 p0, T2 p1, T3 p2);

	}

	/// <summary> Provides a unit-test friendly wrapper for <see cref="Dispatcher"/>.<br/>
	/// Use this class as 100% compatible replacement for <see cref="Dispatcher"/> (.NET Framework 4.0)
	/// </summary>
	public partial class DispatcherWrapper : IDispatcher {

		#region static 

		private static Dictionary<Dispatcher, IDispatcher> s_Map = new Dictionary<Dispatcher, IDispatcher>(); //TODO use weak references
		
		public static IDispatcher CurrentDispatcher {
			get { return FromDispatcher(Dispatcher.CurrentDispatcher); }
		}

		public static IDispatcher FromDispatcher([NotNull]Dispatcher dispatcher) {
			if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));
			IDispatcher wrapper;
			if (s_Map.TryGetValue(dispatcher, out wrapper)) return wrapper;
			wrapper = new DispatcherWrapper(dispatcher);
			s_Map.Add(dispatcher, wrapper);
			return wrapper;
		}

		internal static ApplicationDispatcher GetApplicationDispatcher([NotNull]Dispatcher dispatcher) {
			if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));
			IDispatcher wrapper;
			if (s_Map.TryGetValue(dispatcher, out wrapper)) {
				if(wrapper is ApplicationDispatcher) return (ApplicationDispatcher) wrapper;
				s_Map.Remove(dispatcher); //remove the existing wrapper, to create an ApplicationDispatcher
			}
			wrapper = new ApplicationDispatcher(dispatcher);
			s_Map.Add(dispatcher, wrapper);
			return (ApplicationDispatcher)wrapper;
		}

		/// <summary>Gets the <see cref="DispatcherWrapper"/> for the specified thread. 
		/// </summary>
		/// <param name="thread">The thread to obtain the <see cref="DispatcherWrapper"/> from.</param>
		/// <returns>The dispatcher for <paramref name="thread"/>.</returns>
		/// <remarks>
		/// <p>If a dispatcher is not available for the specified thread, null will be returned.</p>
		/// <p>FromThread does not create a <see cref="DispatcherWrapper"/> on a thread that does not have a <see cref="DispatcherWrapper"/>. A new <see cref="DispatcherWrapper"/> is created on a thread that does not already have a <see cref="DispatcherWrapper"/> when attempting to get the <see cref="DispatcherWrapper"/> by using the <see cref="DispatcherWrapper.CurrentDispatcher"/> property.</p>
		/// </remarks>
		public static IDispatcher FromThread(Thread thread) {
			var dispatcher = Dispatcher.FromThread(thread);
			// FromThread does not create a Dispatcher on a thread that does not have a Dispatcher. 
			// A new Dispatcher is created on a thread that does not already have a Dispatcher when attempting to get the Dispatcher by using the CurrentDispatcher property.
			if(dispatcher==null) return null;
			
			return FromDispatcher(dispatcher);
		}

		/// <summary>[EXPERIMENTAL] Gets the <see cref="ApplicationDispatcher"/> for the specified thread. 
		/// </summary>
		/// <param name="thread">The thread to obtain the <see cref="ApplicationDispatcher"/> from.</param>
		/// <returns>The dispatcher for <paramref name="thread"/>.</returns>
		/// <remarks>
		/// <p>If a dispatcher is not available for the specified thread, null will be returned.</p>
		/// <p>ApplicationDispatcherFromThread does not create a <see cref="ApplicationDispatcher"/> on a thread that does not have a <see cref="ApplicationDispatcher"/>. A new <see cref="ApplicationDispatcher"/> is created on a thread that does not already have a <see cref="ApplicationDispatcher"/> when attempting to get the <see cref="ApplicationDispatcher"/> by using the <see cref="ApplicationDispatcher.CurrentDispatcher"/> property.</p>
		/// </remarks>
		protected static ApplicationDispatcher ApplicationDispatcherFromThread(Thread thread) {
			var dispatcher = Dispatcher.FromThread(thread);
			// FromThread does not create a Dispatcher on a thread that does not have a Dispatcher. 
			// A new Dispatcher is created on a thread that does not already have a Dispatcher when attempting to get the Dispatcher by using the CurrentDispatcher property.
			if(dispatcher==null) return null;
			
			return GetApplicationDispatcher(dispatcher);
		}

		/// <summary> Requests that all frames exit, including nested frames.
		/// </summary>
		/// <seealso cref="Dispatcher.ExitAllFrames"/>
		public static void ExitAllFrames() { Dispatcher.ExitAllFrames(); }

		/// <summary>Enters an execute loop.
		/// </summary>
		/// <param name="frame">The frame for the dispatcher to process.</param>
		/// <seealso cref="Dispatcher.PushFrame"/>
		public static void PushFrame(DispatcherFrame frame) { Dispatcher.PushFrame(frame);}

		/// <summary>Pushes the main execution frame on the event queue of the Dispatcher. 
		/// </summary>
		/// <seealso cref="Dispatcher.Run"/>
		public static void Run() { Dispatcher.Run();}

		/// <summary>Determines whether the specified DispatcherPriority is a valid priority. 
		/// </summary>
		/// <param name="priority">The priority to check.</param>
		/// <param name="parameterName">A string that will be returned by the exception that occurs if the priority is invalid.</param>
		/// <seealso cref="Dispatcher.ValidatePriority"/>
		public static void ValidatePriority(DispatcherPriority priority, string parameterName) { Dispatcher.ValidatePriority(priority, parameterName);}

#if(CLR_4_5)
		public static DispatcherPriorityAwaitable Yield(DispatcherPriority priority) { Dispatcher.Yield(priority); }

//		public static DispatcherPriorityAwaitable Yield() { Dispatcher.Yield(); }
#endif

		#endregion

		private Dispatcher _wrappedDispatcher;

		private const DispatcherPriority PriorityDefault = DispatcherPriority.Normal;
		private const DispatcherPriority DefaultInvokePriority = DispatcherPriority.Send;

	    // ReSharper disable once InconsistentNaming
		private readonly TimeSpan TimeSpanMinusOne = TimeSpan.FromMilliseconds(-1.0);

		/// <summary> Initializes a new instance of the <see cref="DispatcherWrapper"/> class.
		/// </summary>
		/// <param name="dispatcher">The dispatcher.</param>
		/// <exception cref="System.ArgumentNullException">dispatcher</exception>
		public DispatcherWrapper([NotNull] Dispatcher dispatcher) {
			if(dispatcher==null) throw new ArgumentNullException(nameof(dispatcher));
			_wrappedDispatcher = dispatcher;
		}

		/// <summary> Gets direct access to the wrapped <see cref="Dispatcher"/>.
		/// </summary>
		/// <value>The deprecated dispatcher.</value>
		[Obsolete("Deprecated")]
		public Dispatcher DeprecatedDispatcher { get { return _wrappedDispatcher; } private set { _wrappedDispatcher = value; }}

		private DispatcherOperation LegacyBeginInvokeImpl(DispatcherPriority priority, Delegate method, object args,int numArgs) {
//			Debug.WriteLine(string.Format("*   {0}\n==> BeginInvoke {1}",DebugUtil.FormatMethod(GetCallingMethod()),DebugUtil.FormatDelegate(method)));
//			InvokeArgumentsCheck(method.Method, args); // OPTIONAL, usefull for debug
			if(HasHook) HookInternal("BeginInvoke", method, ref priority);
			if (numArgs==0) return _wrappedDispatcher.BeginInvoke(priority,method);
			if (numArgs==1) return _wrappedDispatcher.BeginInvoke(priority,method,args);
			                return _wrappedDispatcher.BeginInvoke(method, priority,(object[])args);
		}


		private object LegacyInvokeImpl(DispatcherPriority priority, TimeSpan timeout, Delegate method, object args, int numArgs) {
//			Debug.WriteLine(string.Format("*   {0}\n==> BeginInvoke {1}",DebugUtil.FormatMethod(GetCallingMethod()),DebugUtil.FormatDelegate(method)));
//			InvokeArgumentsCheck(method.Method, args); // OPTIONAL, usefull for debug
			if(HasHook) HookInternal("Invoke", method, ref priority);
			if (numArgs==0) return _wrappedDispatcher.Invoke(priority,timeout,method);
			if (numArgs==1) return _wrappedDispatcher.Invoke(priority,timeout,method,args);
			                return _wrappedDispatcher.Invoke(method, priority,timeout,(object[])args);
		}

//		private void InvokeArgumentsCheck(MethodBase method, object args) {
//			var paramsArray = args as object[];
//			if(args==null) paramsArray=new object[0];
//			else if(paramsArray==null) paramsArray=new object[]{args};
//			var parameterInfos = method.GetParameters();
//			if (paramsArray.Length != parameterInfos.Length) throw new ArgumentException("args");
//		}

//		private object[] InvokeArgumentsCheck(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) {
//			Signature signature = this.Signature;
//			int length = signature.Arguments.Length;
//			int num = parameters != null ? parameters.Length : 0;
//			if ((this.InvocationFlags & (INVOCATION_FLAGS.INVOCATION_FLAGS_NO_INVOKE | INVOCATION_FLAGS.INVOCATION_FLAGS_CONTAINS_STACK_POINTERS)) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN) this.ThrowNoInvokeException();
//			this.CheckConsistency(obj);
//			if (length != num) throw new TargetParameterCountException(Environment.GetResourceString("Arg_ParmCnt"));
//			if (num != 0) return this.CheckArguments(parameters, binder, invokeAttr, culture, signature);
//			else return (object[]) null;
//		}

		private MethodBase GetCallingMethod() {
			for (int i = 3; i < 10; i++) {
				var method = new StackFrame(i, false).GetMethod();
				if(method.DeclaringType==typeof(DispatcherWrapper)) continue;
				if(method.DeclaringType==typeof(ApplicationDispatcher)) continue;
				return method;
			}
			return null;
		}

//		private object LegacyInvokeImpl(DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout, Delegate method, object args) {
//			if(HasHook) HookInternal("Invoke", method, ref priority);
//			return _WrappedDispatcher.Invoke(priority, timeout, method, args);
//		}

		[Browsable(false),EditorBrowsable(EditorBrowsableState.Never)]
		public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method) {return LegacyBeginInvokeImpl(priority, method, null, 0);}

		[EditorBrowsable(EditorBrowsableState.Never),Browsable(false)]
		public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg) {return LegacyBeginInvokeImpl(priority, method, (object) arg,1);}

		[EditorBrowsable(EditorBrowsableState.Never),Browsable(false)]

		public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args) {return LegacyBeginInvokeImpl(priority, method, (object) CombineParameters(arg, args),1+Math.Min(args.Length,1));}

		public DispatcherOperation BeginInvoke(DispatcherPriority priority, Action method) {return LegacyBeginInvokeImpl(priority, method, null,0);}

		public DispatcherOperation BeginInvoke<T>(DispatcherPriority priority, Action<T> method, T arg) {return this.LegacyBeginInvokeImpl(priority, method, (object) arg, 1);}

		public DispatcherOperation BeginInvoke<T1,T2>(DispatcherPriority priority, Action<T1,T2> method, T1 arg1, T2 arg2) {return LegacyBeginInvokeImpl(priority, method, (object) new object[]{arg1,arg2},2);}

		public DispatcherOperation BeginInvoke<T1,T2,T3>(DispatcherPriority priority, Action<T1,T2,T3> method, T1 arg1, T2 arg2, T3 arg3) {return LegacyBeginInvokeImpl(priority, method, (object) new object[]{arg1,arg2,arg3},3);}

		public DispatcherOperation BeginInvoke(Delegate method, params object[] args) {return LegacyBeginInvokeImpl(PriorityDefault, method, (object) args,+2);}

		public DispatcherOperation BeginInvoke(Action action) {return LegacyBeginInvokeImpl(PriorityDefault, action, null,0);}

		public DispatcherOperation BeginInvoke<T>(Action<T> action, T p0) {return LegacyBeginInvokeImpl(PriorityDefault, action, p0,1);}

		public DispatcherOperation BeginInvoke<T1, T2>(Action<T1, T2> action, T1 p0, T2 p1) {return LegacyBeginInvokeImpl(PriorityDefault, action, new object[]{p0,p1},2);}

		public DispatcherOperation BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 p0, T2 p1, T3 p2) {return LegacyBeginInvokeImpl(PriorityDefault, action, new object[]{p0,p1,p2},3);}

		//

		public object Invoke(Delegate method, params object[] args) { return LegacyInvokeImpl(DefaultInvokePriority, TimeSpanMinusOne, method, args,+2); }

		public object Invoke(Delegate method) {return LegacyInvokeImpl(DefaultInvokePriority, TimeSpanMinusOne, method, null,0); }

		public object Invoke(Delegate method, DispatcherPriority priority) {return LegacyInvokeImpl(priority, TimeSpanMinusOne, method, null,0);}

//		public object Invoke(Delegate callback, DispatcherPriority priority, CancellationToken cancellationToken) {
//			return LegacyInvokeImpl(priority, cancellationToken, TimeSpanMinusOne, callback, null);
//		}
//
//		public object Invoke(Delegate callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout) {
//			if(HasHook) HookInternal("Invoke", callback, ref priority);
//			return _WrappedDispatcher.Invoke(callback, priority, cancellationToken, timeout);
//		}

		public TResult Invoke<TResult>(Func<TResult> method) {return (TResult)LegacyInvokeImpl(DefaultInvokePriority, TimeSpanMinusOne, method, null,0);}

		public TResult Invoke<TResult>(Func<TResult> method, DispatcherPriority priority){return (TResult)LegacyInvokeImpl(priority, TimeSpanMinusOne, method, null,0);}

//		public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken) { return this.Invoke<TResult>(callback, priority, cancellationToken, TimeSpan.FromMilliseconds(-1.0)); }

//		public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout) {
//			if(HasHook) HookInternal("Invoke", callback, ref priority);
//			return (TResult)_WrappedDispatcher.Invoke(callback,priority,cancellationToken,timeout);
//		}


		public void Invoke(DispatcherPriority priority, Action method) {LegacyInvokeImpl(priority, TimeSpanMinusOne, method, null,0);}
		public object Invoke([NotNull] Action action) {return LegacyInvokeImpl(DefaultInvokePriority, TimeSpanMinusOne, action, null,0);}
		public object Invoke<T>([NotNull] Action<T> action, T arg) {return LegacyInvokeImpl(DefaultInvokePriority, TimeSpanMinusOne, action, arg,1);}
		public object Invoke<T1,T2>([NotNull] Action<T1,T2> action, T1 arg1, T2 arg2) {return LegacyInvokeImpl(DefaultInvokePriority,TimeSpanMinusOne,action,new object[]{arg1,arg2},2);}


//		public DispatcherOperation InvokeAsync(Action callback) { return this.InvokeAsync(callback, DispatcherPriority.Normal, CancellationToken.None); }
//		public DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority) { return this.InvokeAsync(callback, priority, CancellationToken.None); }
//		public DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken) {
//			return _WrappedDispatcher.InvokeAsync(...)
//		}
//		public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback) { return this.InvokeAsync<TResult>(callback, DispatcherPriority.Normal, CancellationToken.None); }
//		public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority) { return this.InvokeAsync<TResult>(callback, priority, CancellationToken.None); }
//		public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken) {
//			return _WrappedDispatcher.InvokeAsync(...)
//		}

		public Thread Thread { get { return _wrappedDispatcher.Thread; } }
		
		/// <summary> Gets a value indicating whether the caller must call an invoke method when making method calls to the UI because the caller is on a different thread than the one the UI was created on.
		/// </summary>
//		public bool IsInvokeRequired {get { return _WrappedDispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId; }}
		public bool IsInvokeRequired {get { return _wrappedDispatcher.Thread != Thread.CurrentThread; }}

		/// <summary> Executes a delegate on the application dispatcher.
		/// </summary>
		/// <param name="method">The method to execute</param>
		/// <param name="args">The method arguments</param>
		/// <returns></returns>
		public object InvokeIfRequired(Delegate method, params object[] args) {
			if (IsInvokeRequired) return _wrappedDispatcher.Invoke(method, args);
			return method.DynamicInvoke(args);
		}
		public object InvokeIfRequired<T>(Action<T> action, T arg) { return InvokeIfRequired((Delegate)action, arg); }
		public object InvokeIfRequired<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2){ return InvokeIfRequired((Delegate)action, arg1,arg2); }
		public object InvokeIfRequired<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3 ){ return InvokeIfRequired((Delegate)action, arg1,arg2,arg3); }

//		public object InvokeIfRequired(Action method) {  return InvokeIfRequired(method,new object[0]); }
//		public object InvokeIfRequired<T>(Action<T> method, T arg) {  return InvokeIfRequired(method,arg); }
//		public object InvokeIfRequired<T1,T2>(Action<T1,T2> method, T1 arg1, T2 arg2) {  return InvokeIfRequired(method,arg1,arg2); }


		/// <summary> Processes all Dispatcher messages currently in the dispatcher queue.
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void DoEvents() {
			var frame = new DispatcherFrame();	
			BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(o => {frame.Continue = false;return null;}), frame);
			Dispatcher.PushFrame(frame);
		}


		private object[] CombineParameters(object arg, object[] args) {
			object[] objArray = new object[1 + (args == null ? 1 : args.Length)];
			objArray[0] = arg;
			if (args != null) Array.Copy((Array) args, 0, (Array) objArray, 1, args.Length);
			else objArray[1] = (object) null;
			return objArray;
		}
	}

	public partial class DispatcherWrapper {

		ThreadLocal<DispatcherPriority> _ForcePriorityForThread=new ThreadLocal<DispatcherPriority>(()=>DispatcherPriority.Invalid);
//		long invokeCount;

		public DispatcherPriority ForcePriority { get; set; }
		public DispatcherPriority ForcePriorityForThread { get { return _ForcePriorityForThread.Value; } set { _ForcePriorityForThread.Value = value; } }

		private void HookInternal(string invokeMethod, Delegate callback, ref DispatcherPriority priority) {
//			var methodName = DebugUtil.FormatTypeName(callback.Target)+"."+callback.Method.Name;
//			var prio = (int) (priority == DispatcherPriority.Invalid ? DispatcherPriority.Normal : priority);
//			Debug.WriteLine(string.Format("T{0:D4} {1} (#{2}) P:{4} => {3}",System.Threading.Thread.CurrentThread.ManagedThreadId, invokeMethod, ++invokeCount, methodName,prio));
			if (ForcePriority > 0) priority = ForcePriority;
			if (ForcePriorityForThread > 0) priority = ForcePriorityForThread;
		}

		public bool HasHook { get; set; }

	}

//	public partial interface IDispatcher {
//		DispatcherPriority ForcePriority { get; set; }
//		DispatcherPriority ForcePriorityForThread { get; set; }
//		bool HasHook { get; set; }
//	}

	/// <summary> Provides a static <see cref="IDispatcher"/> for the application thread.
	/// </summary>
	public class ApplicationDispatcher:IDispatcher {

		private static DispatcherWrapper s_Wrapper;
		private static IDispatcher s_SingletonInstance;

		public static int StatisticsːOperationsˑCurrentˑCount;
		public static int StatisticsːOperationsˑTotalˑCount;

		private static DispatcherWrapper InternalWrapper {
			get {
				if (s_Wrapper == null) { var dummy = Instance; /*Init Instance*/}
				return s_Wrapper;
			}
		}

		public static IDispatcher Instance {
			get {
				if (s_SingletonInstance != null) return s_SingletonInstance;
				if (Application.Current != null) {
					s_SingletonInstance=DispatcherWrapper.GetApplicationDispatcher(Application.Current.Dispatcher);
					InitHooks();
				} else {
					s_SingletonInstance=DispatcherWrapper.GetApplicationDispatcher(System.Windows.Threading.Dispatcher.CurrentDispatcher);
					InitHooks();
				}
				return s_SingletonInstance;
			}
			set { s_SingletonInstance = value; }
		}


		private static PropertyInfo DispatcherOperationˑName = typeof (DispatcherOperation).GetProperty("Name", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		private static FieldInfo DispatcherOperationˑ_method = typeof (DispatcherOperation).GetField("_method", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		
		private static void InitHooks() {
//			s_SingletonInstance.DeprecatedDispatcher.Hooks.OperationPosted += (s, e) => {
//				Interlocked.Increment(ref StatisticsːOperationsˑCurrentˑCount);
//				Interlocked.Increment(ref StatisticsːOperationsˑTotalˑCount);
//			};
//			s_SingletonInstance.DeprecatedDispatcher.Hooks.OperationAborted += (s, e) => Interlocked.Decrement(ref StatisticsːOperationsˑCurrentˑCount);
//			s_SingletonInstance.DeprecatedDispatcher.Hooks.OperationCompleted += (s, e) => Interlocked.Decrement(ref StatisticsːOperationsˑCurrentˑCount);


//			s_SingletonInstance.DeprecatedDispatcher.Hooks.OperationPosted += (s, e) => {
//				//string name;try{name=(string)DispatcherOperationˑName.GetValue(e.Operation, BindingFlags.Instance | BindingFlags.NonPublic, null, null, null);}catch(Exception ex){name="?";}
//				Delegate _method;try{_method=(Delegate)DispatcherOperationˑ_method.GetValue(e.Operation);}catch(Exception ex){_method=null;}
//				var name=DebugUtil.FormatDelegate(_method);
//				Debug.WriteLine(string.Format("{1,2} {0}",name,(int)e.Operation.Priority));
//			};
		}


		/// <summary> Initializes a new instance of the <see cref="ApplicationDispatcher"/> class.
		/// </summary>
		/// <param name="dispatcher">The dispatcher.</param>
		/// <exception cref="System.ArgumentNullException">dispatcher</exception>
		internal ApplicationDispatcher([NotNull] Dispatcher dispatcher) {
			if(dispatcher==null) throw new ArgumentNullException(nameof(dispatcher));
			s_Wrapper = new DispatcherWrapper(dispatcher);

//			//TODO EXPERIMENTAL
//			s_Wrapper.ForcePriority=DispatcherPriority.Background;
//			s_Wrapper.HasHook = true;
		}

		#region Static

		/// <summary> [COMPATIBILITY] Gets the <see cref="IDispatcher"/> for the application thread and creates a new <see cref="IDispatcher"/> if one is not already associated with the thread.
		/// </summary>
		/// <returns>The dispatcher associated with the application thread.</returns>
		[Obsolete("Direct use static ApplicationDispatcher methods",false)]
		public static IDispatcher CurrentDispatcher {get { return Instance; } set { Instance = value; }}

		/// <summary> [DEPRECATED] Gets direct access to the wrapped <see cref="Dispatcher" /> for the application thread.
		/// </summary>
		/// <value>The <see cref="Dispatcher"/></value>
		public static Dispatcher DeprecatedDispatcher { get { return InternalWrapper.DeprecatedDispatcher; } }
		public static Thread Thread { get { return InternalWrapper.Thread; } }
		public static bool IsInvokeRequired { get { return InternalWrapper.IsInvokeRequired; } }

		public static DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method) { return InternalWrapper.BeginInvoke(priority, method); }
		public static DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg) { return InternalWrapper.BeginInvoke(priority, method, arg); }
		public static DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args) { return InternalWrapper.BeginInvoke(priority, method,args); }
		public static DispatcherOperation BeginInvoke(DispatcherPriority priority, Action method) {return InternalWrapper.BeginInvoke(priority, method); }
		public static DispatcherOperation BeginInvoke<T>(DispatcherPriority priority, Action<T> method, T arg) {return InternalWrapper.BeginInvoke(priority, method,arg); }
		public static DispatcherOperation BeginInvoke<T1, T2>(DispatcherPriority priority, Action<T1, T2> method, T1 arg1, T2 arg2) { return InternalWrapper.BeginInvoke(priority, method,arg1,arg2); }
		public static DispatcherOperation BeginInvoke<T1, T2, T3>(DispatcherPriority priority, Action<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3) {return InternalWrapper.BeginInvoke(priority, method,arg1,arg2,arg3); }
		public static DispatcherOperation BeginInvoke(Delegate method, params object[] args) { return InternalWrapper.BeginInvoke(method,args); }
		public static DispatcherOperation BeginInvoke(Action action) { return InternalWrapper.BeginInvoke(action); }
		public static DispatcherOperation BeginInvoke<T>(Action<T> action, T p0) { return InternalWrapper.BeginInvoke(action,p0); }
		public static DispatcherOperation BeginInvoke<T1, T2>(Action<T1, T2> action, T1 p0, T2 p1) { return InternalWrapper.BeginInvoke(action,p0,p1); }
		public static DispatcherOperation BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 p0, T2 p1, T3 p2) { return InternalWrapper.BeginInvoke(action,p0,p1,p2); }
		public static object Invoke(Delegate method, params object[] args) { return InternalWrapper.Invoke(method,args); }
		public static object Invoke(Delegate callback) {  return InternalWrapper.Invoke(callback); }
		public static object Invoke(Delegate callback, DispatcherPriority priority) { return InternalWrapper.Invoke(callback,priority); }
		public static object Invoke(Delegate callback, DispatcherPriority priority, CancellationToken cancellationToken) { return InternalWrapper.Invoke(callback,priority,cancellationToken); }
		public static object Invoke(Delegate callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout) {return InternalWrapper.Invoke(callback,priority,cancellationToken,timeout); }
		public static TResult Invoke<TResult>(Func<TResult> callback) { return InternalWrapper.Invoke(callback); }
		public static TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority) { return InternalWrapper.Invoke(callback,priority);}
//		public static TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken) { return InternalWrapper.Invoke(callback,priority,cancellationToken); }
//		public static TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout) { return InternalWrapper.Invoke(callback,priority,cancellationToken,timeout);}
		public static void Invoke(DispatcherPriority priority, Action callback)  {InternalWrapper.Invoke(priority,callback); }
		public static object Invoke(Action action) { return InternalWrapper.Invoke(action); }
		public static object Invoke<T>(Action<T> action, T arg) {  return InternalWrapper.Invoke(action,arg); }
		public static object Invoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2) { return InternalWrapper.Invoke(action,arg1,arg2); }
		public static object InvokeIfRequired(Delegate method, params object[] args) {  return InternalWrapper.InvokeIfRequired(method,args); }
		public static object InvokeIfRequired(Action method) {  return InternalWrapper.InvokeIfRequired(method,new object[0]); }
		public static object InvokeIfRequired<T>(Action<T> method, T arg) {  return InternalWrapper.InvokeIfRequired(method,arg); }
		public static object InvokeIfRequired<T1,T2>(Action<T1,T2> method, T1 arg1, T2 arg2) {  return InternalWrapper.InvokeIfRequired(method,arg1,arg2); }
		public static object InvokeIfRequired<T1,T2,T3>(Action<T1,T2,T3> method, T1 arg1, T2 arg2, T3 arg3) {  return InternalWrapper.InvokeIfRequired(method,arg1,arg2,arg3); }
		public static void DoEvents() { InternalWrapper.DoEvents(); }

		#endregion

		#region Singleton
		Dispatcher IDispatcher.DeprecatedDispatcher { get { return InternalWrapper.DeprecatedDispatcher; } }
		Thread IDispatcher.Thread { get { return InternalWrapper.Thread; } }
		bool IDispatcher.IsInvokeRequired { get { return InternalWrapper.IsInvokeRequired; } }
		public static IDispatcher Wrapper { get { return s_Wrapper; } }

		DispatcherOperation IDispatcher.BeginInvoke(DispatcherPriority priority, Delegate method) { return InternalWrapper.BeginInvoke(priority, method); }
		DispatcherOperation IDispatcher.BeginInvoke(DispatcherPriority priority, Delegate method, object arg) { return InternalWrapper.BeginInvoke(priority, method, arg); }
		DispatcherOperation IDispatcher.BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args) { return InternalWrapper.BeginInvoke(priority, method,args); }
		DispatcherOperation IDispatcher.BeginInvoke(DispatcherPriority priority, Action method) {return InternalWrapper.BeginInvoke(priority, method); }
		DispatcherOperation IDispatcher.BeginInvoke<T>(DispatcherPriority priority, Action<T> method, T arg) {return InternalWrapper.BeginInvoke(priority, method,arg); }
		DispatcherOperation IDispatcher.BeginInvoke<T1, T2>(DispatcherPriority priority, Action<T1, T2> method, T1 arg1, T2 arg2) { return InternalWrapper.BeginInvoke(priority, method,arg1,arg2); }
		DispatcherOperation IDispatcher.BeginInvoke<T1, T2, T3>(DispatcherPriority priority, Action<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3) {return InternalWrapper.BeginInvoke(priority, method,arg1,arg2,arg3); }
		DispatcherOperation IDispatcher.BeginInvoke(Delegate method, params object[] args) { return InternalWrapper.BeginInvoke(method,args); }
		DispatcherOperation IDispatcher.BeginInvoke(Action action) { return InternalWrapper.BeginInvoke(action); }
		DispatcherOperation IDispatcher.BeginInvoke<T>(Action<T> action, T p0) { return InternalWrapper.BeginInvoke(action,p0); }
		DispatcherOperation IDispatcher.BeginInvoke<T1, T2>(Action<T1, T2> action, T1 p0, T2 p1) { return InternalWrapper.BeginInvoke(action,p0,p1); }
		DispatcherOperation IDispatcher.BeginInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 p0, T2 p1, T3 p2) { return InternalWrapper.BeginInvoke(action,p0,p1,p2); }
		object IDispatcher.Invoke(Delegate method, params object[] args) { return InternalWrapper.Invoke(method,args); }
		object IDispatcher.Invoke(Delegate callback) {  return InternalWrapper.Invoke(callback); }
		object IDispatcher.Invoke(Delegate callback, DispatcherPriority priority) { return InternalWrapper.Invoke(callback,priority); }
//		object IDispatcher.Invoke(Delegate callback, DispatcherPriority priority, CancellationToken cancellationToken) { return InternalWrapper.Invoke(callback,priority,cancellationToken); }
//		object IDispatcher.Invoke(Delegate callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout) {return InternalWrapper.Invoke(callback,priority,cancellationToken,timeout); }
		TResult IDispatcher.Invoke<TResult>(Func<TResult> callback) { return InternalWrapper.Invoke(callback); }
		TResult IDispatcher.Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority) { return InternalWrapper.Invoke(callback,priority);}
//		TResult IDispatcher.Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken) { return InternalWrapper.Invoke(callback,priority,cancellationToken); }
//		TResult IDispatcher.Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout) { return InternalWrapper.Invoke(callback,priority,cancellationToken,timeout);}
		void IDispatcher.Invoke(DispatcherPriority priority, Action callback)  {InternalWrapper.Invoke(priority,callback); }
		object IDispatcher.Invoke(Action action) { return InternalWrapper.Invoke(action); }
		object IDispatcher.Invoke<T>(Action<T> action, T arg) {  return InternalWrapper.Invoke(action,arg); }
		object IDispatcher.Invoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2) { return InternalWrapper.Invoke(action,arg1,arg2); }
		object IDispatcher.InvokeIfRequired(Delegate method, params object[] args) {  return InternalWrapper.InvokeIfRequired(method,args); }
		object IDispatcher.InvokeIfRequired<T>(Action<T> method, T arg) {  return InternalWrapper.InvokeIfRequired(method,arg); }
		object IDispatcher.InvokeIfRequired<T1,T2>(Action<T1,T2> method, T1 arg1, T2 arg2) {  return InternalWrapper.InvokeIfRequired(method,arg1,arg2); }
		object IDispatcher.InvokeIfRequired<T1,T2,T3>(Action<T1,T2,T3> method, T1 arg1, T2 arg2, T3 arg3) {  return InternalWrapper.InvokeIfRequired(method,arg1,arg2,arg3); }
		void IDispatcher.DoEvents() { InternalWrapper.DoEvents(); }
		#endregion

//moved to viewmodelframework as extension method
//		public static DelayedDispatcherTask BeginInvoke(TimeSpan delay, Action method) { return InternalWrapper.BeginInvoke(delay, method); }

	}

//	/// <summary> Provides a wrapped version of <see cref="System.Windows.Threading.Dispatcher"/>
//	/// </summary>
//	public class ApplicationDispatcher:DispatcherWrapper {
//
//		private static ApplicationDispatcher s_ApplicationDispatcher;
//
//		public static new ApplicationDispatcher CurrentDispatcher {
//			get {
//				if (s_ApplicationDispatcher != null) return s_ApplicationDispatcher;
//				if (Application.Current != null) {
//					s_ApplicationDispatcher=GetApplicationDispatcher(Application.Current.Dispatcher);
//				}
//				else {
//					s_ApplicationDispatcher=GetApplicationDispatcher(System.Windows.Threading.Dispatcher.CurrentDispatcher);
//				}
//				return s_ApplicationDispatcher;
//			}
//			set { s_ApplicationDispatcher = value; }
//		}
//
//		public static new ApplicationDispatcher FromThread(Thread thread) { return DispatcherWrapper.ApplicationDispatcherFromThread(thread); }
//
////		/// <summary> Gets the Dispatcher for the current Application
////		/// </summary>
////		public static DispatcherWrapper ApplicationDispatcher {
////			get {
////				if (s_ApplicationDispatcher != null) return s_ApplicationDispatcher;
////				//TODO Application.Current could be null in tests
////				if(Application.Current==null && Debugger.IsAttached) Debugger.Break();
////				return FromDispatcher(Application.Current.Dispatcher);
////			}
////			set { s_ApplicationDispatcher = value; }
////		}
//
//		protected internal ApplicationDispatcher([NotNull] Dispatcher dispatcher):base(dispatcher) {}
//
//	}


/*
DispatchPriority-Prioritätsebenen (nach Priorität sortiert)

Priorität 			Beschreibung
Inactive 		 0	Arbeitsaufgaben stehen in einer Warteschlage, werden aber nicht verarbeitet.
SystemIdle 		 1	Arbeitsaufgaben werden nur an den UI-Thread verteilt, wenn sich das System im Leerlauf befindet. Hierbei handelt es sich um die geringste Priorität für Aufgaben, die verarbeitet werden.
ApplicationIdle  2	Arbeitsaufgaben werden nur an den UI-Thread verteilt, wenn sich die Anwendung im Leerlauf befindet.
ContextIdle 	 3	Arbeitsaufgaben werden nur an den UI-Thread verteilt, wenn Arbeitsaufgaben einer höheren Priorität verarbeitet wurden.
Background 		 4	Arbeitsaufgaben werden verteilt, wenn alle Layout-, Rendering- und Eingabeaufgaben verarbeitet wurden.
Input 			 5	Arbeitsaufgaben werden mit derselben Priorität an den UI-Thread verteilt wie Benutzereingaben.
Loaded 			 6	Arbeitsaufgaben werden an den UI-Thead verteilt, wenn alle Layout- und Renderingaufgaben abgeschlossen sind.
Render 			 7	Arbeitsaufgaben werden mit derselben Priorität an den UI-Thread verteilt wie das Renderingmodul.
DataBind	 	 8	Arbeitsaufgaben werden mit derselben Priorität an den UI-Thread verteilt wie die Datenbindung.
Normal 			 9	Arbeitsaufgaben werden mit normaler Priorität an den UI-Thead verteilt. Die meisten Arbeitsaufgaben in Anwendungen sollten mit dieser Priorität verteilt werden.
Send 			10	Arbeitsaufgaben werden mit höchster Priorität an den UI-Thead verteilt. 
*/

/*
seealso DispatcherSynchronizationContext
SynchronizationContext
BackgroundWorker
*/
}