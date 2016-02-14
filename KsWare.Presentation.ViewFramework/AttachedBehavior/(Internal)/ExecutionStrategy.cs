using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace KsWare.Presentation.ViewFramework.AttachedBehavior 
{
	/// <summary>
	/// Defines the interface for a strategy of execution for the CommandBehaviorBinding
	/// </summary>
	internal interface IExecutionStrategy 
	{
		/// <summary> Gets or sets the Behavior that we execute this strategy
		/// </summary>
		CommandBehaviorBinding Behavior { get; set; }

		/// <summary>
		/// Executes according to the strategy type
		/// </summary>
		/// <param name="parameter">The parameter to be used in the execution</param>
		void Execute(object parameter);
	}

	/// <summary> Executes a command 
	/// </summary>
	internal class CommandExecutionStrategy : IExecutionStrategy 
	{
		#region IEventExecutionStrategy Members

		/// <summary> Gets or sets the Behavior that we execute this strategy
		/// </summary>
		public CommandBehaviorBinding Behavior { get; set; }

		/// <summary> Executes the Command that is stored in the CommandProperty of the CommandExecution
		/// </summary>
		/// <param name="parameter">The parameter for the command</param>
		public void Execute(object parameter) {
			if (Behavior == null)
				throw new InvalidOperationException("Behavior property cannot be null when executing a strategy");

			if (Behavior.Command.CanExecute(Behavior.CommandParameter))
				Behavior.Command.Execute(Behavior.CommandParameter);
		}

		#endregion
	}

	/// <summary> executes a delegate
	/// </summary>
	internal class ActionExecutionStrategy : IExecutionStrategy 
	{

		#region IEventExecutionStrategy Members

		/// <summary> Gets or sets the Behavior that we execute this strategy
		/// </summary>
		public CommandBehaviorBinding Behavior { get; set; }

		/// <summary> Executes an Action delegate
		/// </summary>
		/// <param name="parameter">The parameter to pass to the Action</param>
		public void Execute(object parameter) {
			Behavior.Action(parameter);
		}

		#endregion
	}

	/// <summary> executes a method
	/// </summary>
	internal class MethodInvokeExecutionStrategy : IExecutionStrategy 
	{
		private static readonly Dictionary<string,MethodInfo> StaticMethodInfoCache=new Dictionary<string, MethodInfo>();

		#region IEventExecutionStrategy Members

		/// <summary>
		/// Gets or sets the Behavior that we execute this strategy
		/// </summary>
		public CommandBehaviorBinding Behavior { get; set; }

		/// <summary>
		/// Executes a method 
		/// </summary>
		/// <param name="parameter">The parameter to pass to the Action</param>
		public void Execute(object parameter) {
			var dataContext = ((FrameworkElement) Behavior.Owner).DataContext;
			if(Behavior.InvokeMethodName==null) return;
			if(Behavior.InvokeObject==null && dataContext==null) return;

			var method = GetInvokeMethod(Behavior); if(method==null) return;
			var flags = 
				(method.IsStatic ? BindingFlags.Static : BindingFlags.Instance) |
				(method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic);
			object obj = Behavior.InvokeObject is Type ? null : (Behavior.InvokeObject == null ? dataContext : Behavior.InvokeObject); 
			method.Invoke(obj,BindingFlags.Instance|flags,null,new[] {parameter},null);
		}

		
		[SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId="System.Int32.ToString")]
		[SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId="System.String.Format(System.String,System.Object[])")]
		private static MethodInfo GetInvokeMethod(CommandBehaviorBinding behavior) {
			var owner = ((FrameworkElement)behavior.Owner);
			var dataContext = ((FrameworkElement) behavior.Owner).DataContext;
			object obj = behavior.InvokeObject is Type ? null : (behavior.InvokeObject == null ? dataContext : null); 
			Type type = behavior.InvokeObject is Type ? (Type)behavior.InvokeObject : (behavior.InvokeObject != null ? behavior.InvokeObject.GetType() : dataContext.GetType()); 
			var flags=behavior.InvokeObject is Type ? BindingFlags.Static : BindingFlags.Instance;

			var key = behavior.InvokeMethodName+"(object); "+type.AssemblyQualifiedName;
			if (StaticMethodInfoCache.ContainsKey(key)) return StaticMethodInfoCache[key];

			var method = type.GetMethod(behavior.InvokeMethodName,flags|BindingFlags.Public,null, new[] {typeof (object)},null);
			if (method != null) {
				StaticMethodInfoCache.Add(key,method);
				return method;
			}

			#region Search private method. need full trust!
			method = type.GetMethod(behavior.InvokeMethodName, flags | BindingFlags.NonPublic, null, new[] {typeof (object)}, null);
			if(method!=null) {
				Debug.WriteLine("=>" + string.Format("KsWare.AttachedBehavior Warning: '{0}(object)' method should be public on '{3}' ''{1}' (HashCode={2})'",
					behavior.InvokeMethodName,
					type.Name,
					(obj!=null ? obj.GetHashCode().ToString() : ""),
					obj!=null ? "object":"type"
				));
				StaticMethodInfoCache.Add(key,method);
				return method;
			}
			#endregion

			Debug.WriteLine("=>" + string.Format("KsWare.AttachedBehavior Error: public '{0}(object)' method not found on 'object' ''{1}' (HashCode={2})'" +
				"; target element is '{3}' (Name='{4}')"+
				"; target property is 'Invoke' (type 'String')",
				behavior.InvokeMethodName,
				owner.DataContext.GetType().Name,
				owner.DataContext.GetHashCode(),
				owner.GetType().Name,
				owner.Name
			));

			//throw new MissingMethodException( ((FrameworkElement) d).DataContext.GetType().Name,methodName);
			StaticMethodInfoCache.Add(key,null);
			return null;
		}

		#endregion
	}
}