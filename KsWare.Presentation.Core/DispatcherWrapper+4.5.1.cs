// Type: System.Windows.Threading.Dispatcher
// Assembly: WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0EC0FC16-B9E6-4172-8670-B24E08BAF302
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF\WindowsBase.dll

using System;
using System.ComponentModel;
using System.Runtime;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Threading;

namespace KsWare.Presentation {

	public interface IDispatcher451 {

		/// <summary>
		/// Ruft den Thread ab, dem dieser <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Thread.
		/// </returns>
		Thread Thread { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; }

		/// <summary>
		/// Bestimmt, ob der <see cref="T:System.Windows.Threading.Dispatcher"/> gerade beendet wird.
		/// </summary>
		/// 
		/// <returns>
		/// true, wenn der <see cref="T:System.Windows.Threading.Dispatcher"/> gerade beendet wird, andernfalls false.
		/// </returns>
		bool HasShutdownStarted { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; }

		/// <summary>
		/// Bestimmt, ob der <see cref="T:System.Windows.Threading.Dispatcher"/> die Beendigung abgeschlossen hat.
		/// </summary>
		/// 
		/// <returns>
		/// true, wenn der Verteiler die Beendigung abgeschlossen hat, andernfalls false.
		/// </returns>
		bool HasShutdownFinished { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get; }

		/// <summary>
		/// Ruft die Auflistung von Hooks ab, die zusätzliche Ereignisinformationen zum <see cref="T:System.Windows.Threading.Dispatcher"/> bereitstellen.
		/// </summary>
		/// 
		/// <returns>
		/// Die Hooks, die diesem <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet sind.
		/// </returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		DispatcherHooks Hooks { [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted = true)] get; }

		/// <summary>
		/// Tritt ein, wenn der <see cref="T:System.Windows.Threading.Dispatcher"/> die Beendigung beginnt.
		/// </summary>
		event EventHandler ShutdownStarted;

		/// <summary>
		/// Tritt ein, wenn der <see cref="T:System.Windows.Threading.Dispatcher"/> die Beendigung abschließt.
		/// </summary>
		event EventHandler ShutdownFinished;

		/// <summary>
		/// Tritt ein, wenn eine Threadausnahme ausgelöst und nicht abgefangen wird, während ein Delegat mit <see cref="Overload:System.Windows.Threading.Dispatcher.Invoke"/> oder <see cref="Overload:System.Windows.Threading.Dispatcher.BeginInvoke"/> ausgeführt wird und sich in der Filterstufe befindet.
		/// </summary>
		event DispatcherUnhandledExceptionFilterEventHandler UnhandledExceptionFilter;

		/// <summary>
		/// Tritt ein, wenn eine Threadausnahme ausgelöst und während der Ausführung eines Delegaten mit <see cref="Overload:System.Windows.Threading.Dispatcher.Invoke"/> oder <see cref="Overload:System.Windows.Threading.Dispatcher.BeginInvoke"/> nicht abgefangen wird.
		/// </summary>
		event DispatcherUnhandledExceptionEventHandler UnhandledException;

		/// <summary>
		/// Bestimmt, ob der aufrufende Thread diesem <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// true, wenn der aufrufende Thread diesem <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist, andernfalls false.
		/// </returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool CheckAccess();

		/// <summary>
		/// Bestimmt, ob der aufrufende Thread auf dieses <see cref="T:System.Windows.Threading.Dispatcher"/> zugreifen kann.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Der aufrufende Thread kann nicht auf diesen <see cref="T:System.Windows.Threading.Dispatcher"/> zugreifen.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		void VerifyAccess();

		/// <summary>
		/// Initiiert ein asynchrones Beenden des <see cref="T:System.Windows.Threading.Dispatcher"/>.
		/// </summary>
		/// <param name="priority">Die Priorität, bei der mit dem Beenden des Verteilers begonnen wird.</param>
		[SecurityCritical]
		void BeginInvokeShutdown(DispatcherPriority priority);

		/// <summary>
		/// Initiiert die synchrone Beendigung des <see cref="T:System.Windows.Threading.Dispatcher"/>.
		/// </summary>
		[SecurityCritical]
		void InvokeShutdown();

		/// <summary>
		/// Führt den angegebenen Delegaten asynchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="Overload:System.Windows.Threading.Dispatcher.BeginInvoke"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="method">Der Delegat zu einer Methode, die keine Argumente erwartet und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> ist null.</exception><exception cref="T:System.ComponentModel.InvalidEnumArgumentException"><paramref name="priority"/> ist kein gültiger <see cref="T:System.Windows.Threading.DispatcherPriority"/>.</exception>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method);

		/// <summary>
		/// Führt den angegebenen Delegaten asynchron mit der angegebenen Priorität und dem angegebenen Argument auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="Overload:System.Windows.Threading.Dispatcher.BeginInvoke"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="method">Ein Delegat zu einer Methode, die ein Argument erwartet und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="arg">Das Objekt, das als Argument an die angegebene Methode übergeben wird.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> ist null.</exception><exception cref="T:System.ComponentModel.InvalidEnumArgumentException"><paramref name="priority"/> ist kein gültiger <see cref="T:System.Windows.Threading.DispatcherPriority"/>.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg);

		/// <summary>
		/// Führt den angegebenen Delegaten asynchron mit der angegebenen Priorität und dem angegebenen Argumentarray auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="Overload:System.Windows.Threading.Dispatcher.BeginInvoke"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Warteschlange steht.
		/// </returns>
		/// <param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="method">Ein Delegat zu einer Methode, die mehrere Argumente erwartet und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="arg">Das Objekt, das als Argument an die angegebene Methode übergeben wird.</param><param name="args">Ein Array von Objekten, die als Argumente an die angegebene Methode übergeben werden sollen.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> ist null.</exception><exception cref="T:System.ComponentModel.InvalidEnumArgumentException"><see cref="T:System.Windows.Threading.DispatcherPriority"/> ist keine gültige Priorität.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args);

		/// <summary>
		/// Führt den angegebenen Delegaten asynchron mit den angegebenen Argumenten für den Thread aus, für den der <see cref="T:System.Windows.Threading.Dispatcher"/> erstellt wurde.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="Overload:System.Windows.Threading.Dispatcher.BeginInvoke"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="method">Der Delegat für eine Methode, die in <paramref name="args"/> angegebene Parameter akzeptiert und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="args">Ein Array von Objekten, die als Argumente an die angegebene Methode übergeben werden sollen. Kann null sein.</param>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		DispatcherOperation BeginInvoke(Delegate method, params object[] args);

		/// <summary>
		/// Führt den angegebenen Delegaten asynchron mit den angegebenen Argumenten und der angegebenen Priorität für den Thread aus, für den der <see cref="T:System.Windows.Threading.Dispatcher"/> erstellt wurde.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="Overload:System.Windows.Threading.Dispatcher.BeginInvoke"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="method">Der Delegat für eine Methode, die in <paramref name="args"/> angegebene Parameter akzeptiert und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="args">Ein Array von Objekten, die als Argumente an die angegebene Methode übergeben werden sollen. Kann null sein.</param>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		DispatcherOperation BeginInvoke(Delegate method, DispatcherPriority priority, params object[] args);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Action"/> synchron auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param>
		void Invoke(Action callback);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Action"/> synchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param>
		void Invoke(Action callback, DispatcherPriority priority);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Action"/> synchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param><param name="cancellationToken">Ein Objekt, das angibt, ob die Aktion abgebrochen werden kann.</param>
		void Invoke(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Action"/> synchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param><param name="cancellationToken">Ein Objekt, das angibt, ob die Aktion abgebrochen werden kann.</param><param name="timeout">Die minimale Zeitspanne, die auf den Start des Vorgangs gewartet wird.</param>
		[SecuritySafeCritical]
		void Invoke(Action callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Func`1"/> synchron auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><typeparam name="TResult">Der Rückgabewerts des angegebenen Delegaten.</typeparam>
		TResult Invoke<TResult>(Func<TResult> callback);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Func`1"/> synchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param><typeparam name="TResult">Der Rückgabewerts des angegebenen Delegaten.</typeparam>
		TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Func`1"/> synchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param><param name="cancellationToken">Ein Objekt, das angibt, ob der Vorgang abgebrochen werden kann.</param><typeparam name="TResult">Der Rückgabewerts des angegebenen Delegaten.</typeparam>
		TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Func`1"/> synchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param><param name="cancellationToken">Ein Objekt, das angibt, ob der Vorgang abgebrochen werden kann.</param><param name="timeout">Die minimale Zeitspanne, die auf den Start des Vorgangs gewartet wird.</param><typeparam name="TResult">Der Rückgabewerts des angegebenen Delegaten.</typeparam>
		[SecuritySafeCritical]
		TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Action"/> asynchron auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="M:System.Windows.Threading.Dispatcher.InvokeAsync(System.Action)"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param>
		DispatcherOperation InvokeAsync(Action callback);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Action"/> asynchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="M:System.Windows.Threading.Dispatcher.InvokeAsync(System.Action,System.Windows.Threading.DispatcherPriority)"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param>
		DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Action"/> asynchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="M:System.Windows.Threading.Dispatcher.InvokeAsync(System.Action,System.Windows.Threading.DispatcherPriority,System.Threading.CancellationToken)"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param><param name="cancellationToken">Ein Objekt, das angibt, ob die Aktion abgebrochen werden kann.</param>
		[SecuritySafeCritical]
		DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Func`1"/> asynchron auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="M:System.Windows.Threading.Dispatcher.InvokeAsync``1(System.Func{``0})"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><typeparam name="TResult">Der Rückgabewerts des angegebenen Delegaten.</typeparam>
		DispatcherOperation InvokeAsync<TResult>(Func<TResult> callback);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Func`1"/> asynchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="M:System.Windows.Threading.Dispatcher.InvokeAsync``1(System.Func{``0},System.Windows.Threading.DispatcherPriority)"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param><typeparam name="TResult">Der Rückgabewerts des angegebenen Delegaten.</typeparam>
		DispatcherOperation InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority);

		/// <summary>
		/// Führt die angegebene <see cref="T:System.Func`1"/> synchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Ein Objekt, das unmittelbar nach dem Aufruf von <see cref="M:System.Windows.Threading.Dispatcher.InvokeAsync``1(System.Func{``0},System.Windows.Threading.DispatcherPriority,System.Threading.CancellationToken)"/> zurückgegeben wird und für die Interaktion mit dem Delegaten verwendet werden kann, während im Delegaten die Ausführung einer Aufgabe in der Warteschlange steht.
		/// </returns>
		/// <param name="callback">Ein Delegat, der über den Verteiler aufgerufen werden soll.</param><param name="priority">Die Priorität, die bestimmt, in welcher Reihenfolge der angegebene Rückruf relativ zu den anderen ausstehenden Vorgängen in <see cref="T:System.Windows.Threading.Dispatcher"/> aufgerufen wird.</param><param name="cancellationToken">Ein Objekt, das angibt, ob der Vorgang abgebrochen werden kann.</param><typeparam name="TResult">Der Rückgabewerts des angegebenen Delegaten.</typeparam>
		[SecuritySafeCritical]
		DispatcherOperation InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken);

		/// <summary>
		/// Führt den angegebenen Delegaten synchron mit der angegebenen Priorität auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="method">Ein Delegat zu einer Methode, die keine Argumente erwartet und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><exception cref="T:System.ArgumentException"><paramref name="priority"/> ist gleich <see cref="F:System.Windows.Threading.DispatcherPriority.Inactive"/>.</exception><exception cref="T:System.ComponentModel.InvalidEnumArgumentException"><paramref name="priority"/> ist keine gültige Priorität.</exception><exception cref="T:System.ArgumentNullException"><paramref name="method"/> ist null.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		object Invoke(DispatcherPriority priority, Delegate method);

		/// <summary>
		/// Führt den angegebenen Delegaten mit der angegebenen Priorität und dem angegebenen Argument synchron auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="method">Ein Delegat zu einer Methode, die ein Argument erwartet und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="arg">Ein Objekt, das als Argument an die angegebene Methode übergeben werden soll.</param><exception cref="T:System.ArgumentException"><paramref name="priority"/> ist gleich <see cref="F:System.Windows.Threading.DispatcherPriority.Inactive"/>.</exception><exception cref="T:System.ComponentModel.InvalidEnumArgumentException"><paramref name="priority"/> ist keine gültige Priorität.</exception><exception cref="T:System.ArgumentNullException"><paramref name="method"/> ist null.</exception>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		object Invoke(DispatcherPriority priority, Delegate method, object arg);

		/// <summary>
		/// Führt den angegebenen Delegaten mit der angegebenen Priorität und den angegebenen Argumenten synchron auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="method">Ein Delegat zu einer Methode, die mehrere Argumente erwartet und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="arg">Ein Objekt, das als Argument an die angegebene Methode übergeben werden soll.</param><param name="args">Ein Array von Objekten, die als Argumente an die angegebene Methode übergeben werden sollen.</param><exception cref="T:System.ArgumentException"><paramref name="priority"/> ist gleich <see cref="F:System.Windows.Threading.DispatcherPriority.Inactive"/>.</exception><exception cref="T:System.ComponentModel.InvalidEnumArgumentException"><paramref name="priority"/> ist keine gültige Priorität.</exception><exception cref="T:System.ArgumentNullException"><paramref name="method"/> ist null.</exception>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		object Invoke(DispatcherPriority priority, Delegate method, object arg, params object[] args);

		/// <summary>
		/// Führt den angegebenen Delegaten synchron mit der angegebenen Priorität und dem angegebenen Timeoutwert auf dem Thread aus, in dem der <see cref="T:System.Windows.Threading.Dispatcher"/> erstellt wurde.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="timeout">Die maximale Zeit, die auf den Abschluss der Operation gewartet wird.</param><param name="method">Der Delegat zu einer Methode, die keine Argumente erwartet und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		object Invoke(DispatcherPriority priority, TimeSpan timeout, Delegate method);

		/// <summary>
		/// Führt den angegebenen Delegaten mit der angegebenen Priorität und dem angegebenen Argument synchron auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="timeout">Die maximale Zeit, die auf den Abschluss der Operation gewartet wird.</param><param name="method">Ein Delegat zu einer Methode, die mehrere Argumente erwartet und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="arg">Ein Objekt, das als Argument an die angegebene Methode übergeben werden soll. Dies kann null sein, wenn keine Argumente benötigt werden.</param><exception cref="T:System.ArgumentException"><paramref name="priority"/> ist gleich <see cref="F:System.Windows.Threading.DispatcherPriority.Inactive"/>.</exception><exception cref="T:System.ComponentModel.InvalidEnumArgumentException"><paramref name="priority"/> ist keine gültige Priorität.</exception><exception cref="T:System.ArgumentNullException"><paramref name="method"/> ist null.</exception>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		object Invoke(DispatcherPriority priority, TimeSpan timeout, Delegate method, object arg);

		/// <summary>
		/// Führt den angegebenen Delegaten mit der angegebenen Priorität und den angegebenen Argumenten synchron auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="timeout">Die maximale Zeit, die auf den Abschluss der Operation gewartet wird.</param><param name="method">Ein Delegat zu einer Methode, die mehrere Argumente erwartet und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="arg">Ein Objekt, das als Argument an die angegebene Methode übergeben wird.</param><param name="args">Ein Array von Objekten, die als Argumente an die angegebene Methode übergeben werden sollen.</param><exception cref="T:System.ArgumentException"><paramref name="priority"/> ist gleich <see cref="F:System.Windows.Threading.DispatcherPriority.Inactive"/>.</exception><exception cref="T:System.ComponentModel.InvalidEnumArgumentException"><paramref name="priority"/> ist kein gültiger <see cref="T:System.Windows.Threading.DispatcherPriority"/>.</exception><exception cref="T:System.ArgumentNullException"><paramref name="method"/> ist null.</exception>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		object Invoke(DispatcherPriority priority, TimeSpan timeout, Delegate method, object arg, params object[] args);

		/// <summary>
		/// Führt den angegebenen Delegaten synchron mit den angegebenen Argumenten für den Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="method">Ein Delegat für eine Methode, die in <paramref name="args"/> angegebene Parameter akzeptiert und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="args">Ein Array von Objekten, die als Argumente an die angegebene Methode übergeben werden sollen. Kann null sein.</param>
		object Invoke(Delegate method, params object[] args);

		/// <summary>
		/// Führt den angegebenen Delegaten mit der angegebenen Priorität und den angegebenen Argumenten synchron auf dem Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="method">Ein Delegat für eine Methode, die in <paramref name="args"/> angegebene Parameter akzeptiert und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="args">Ein Array von Objekten, die als Argumente an die angegebene Methode übergeben werden sollen. Kann null sein.</param>
		object Invoke(Delegate method, DispatcherPriority priority, params object[] args);

		/// <summary>
		/// Führt den angegebenen Delegaten in der angegebenen Zeitspanne mit der angegebenen Priorität und den angegebenen Argumenten synchron für den Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="method">Ein Delegat für eine Methode, die in <paramref name="args"/> angegebene Parameter akzeptiert und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="timeout">Die maximale Zeitspanne, die auf den Abschluss der Operation gewartet wird.</param><param name="args">Ein Array von Objekten, die als Argumente an die angegebene Methode übergeben werden sollen. Kann null sein.</param>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		object Invoke(Delegate method, TimeSpan timeout, params object[] args);

		/// <summary>
		/// Führt den angegebenen Delegaten in der angegebenen Zeitspanne mit der angegebenen Priorität und den angegebenen Argumenten synchron für den Thread aus, dem der <see cref="T:System.Windows.Threading.Dispatcher"/> zugeordnet ist.
		/// </summary>
		/// 
		/// <returns>
		/// Der Rückgabewert des aufgerufenen Delegaten bzw. null, wenn der Delegat keinen Wert zurückgibt.
		/// </returns>
		/// <param name="method">Ein Delegat für eine Methode, die in <paramref name="args"/> angegebene Parameter akzeptiert und in die <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange gestellt wird.</param><param name="timeout">Die maximale Zeitspanne, die auf den Abschluss der Operation gewartet wird.</param><param name="priority">Die Priorität, relativ zu den anderen anstehenden Operationen in der <see cref="T:System.Windows.Threading.Dispatcher"/>-Ereigniswarteschlange, mit der die angegebene Methode aufgerufen wird.</param><param name="args">Ein Array von Objekten, die als Argumente an die angegebene Methode übergeben werden sollen. Kann null sein.</param>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		object Invoke(Delegate method, TimeSpan timeout, DispatcherPriority priority, params object[] args);

		/// <summary>
		/// Deaktiviert Verarbeitung der <see cref="T:System.Windows.Threading.Dispatcher"/>-Warteschlange.
		/// </summary>
		/// 
		/// <returns>
		/// Eine Struktur, mit der die Dispatcherverarbeitung wieder aktiviert wird.
		/// </returns>
		DispatcherProcessingDisabled DisableProcessing();

	}

}