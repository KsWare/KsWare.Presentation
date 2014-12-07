using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;

[module: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope="type", Target="KsWare.Presentation.Logging.Flow+MethodDescriptorExpression")]
[module: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope="type", Target="KsWare.Presentation.Logging.Flow+PositionExpression")]
[module: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope="type", Target="KsWare.Presentation.Logging.Flow+ExpressionResult")]
[module: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope="type", Target="KsWare.Presentation.Logging.Log+FlowMemberPosition")]
[module: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope="type", Target="KsWare.Presentation.Logging.Log+FlowMemberType")]
[module: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope="type", Target="KsWare.Presentation.Logging.Flow+FlowExpression")]
[module: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope="type", Target="KsWare.Presentation.Logging.Flow+ParameterExpression")]
[module: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope="type", Target="KsWare.Presentation.Logging.Flow+MethodTypeExpression")]
[module: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Scope="member", Target="KsWare.Presentation.Logging.Flow.#Log(System.Object,KsWare.Presentation.Logging.Flow+FlowExpression)", MessageId="obj")]

#pragma warning disable 1591 //TODO enable warnings

namespace KsWare.Presentation.Core.Logging
{
	public static class Flow
	{
		static readonly ThreadLocal<Exception> loggedException=new ThreadLocal<Exception>();

		public static Exception LoggedException {
			get {return loggedException.Value;}
		}

		[Conditional("DEBUG")]
		public static void Log(this object obj, FlowExpression flow) {
			Logging.Log.Write(obj,flow);
		}
		
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId="obj")]
		public static IDisposable LogBlock(this object obj, FlowExpression flow) {
			Logging.Log.Write(obj, flow);
			return new ExitLogger(obj, flow);
		}


		public static IDisposable Log(this Type type, FlowExpression flow) {
			Logging.Log.Write(type,flow);
			return new ExitLogger(type, flow);
		}

		private class ExitLogger:IDisposable
		{
			private readonly object instanceOrType;
			private readonly FlowExpression flow;

			public ExitLogger(object instanceOrType, FlowExpression flow) {
				this.instanceOrType = instanceOrType;
				this.flow = flow;
				((IExpressionResult)this.flow).Result.Position=FlowMemberPosition.ExitMethod;
			}

			public void Dispose() { Dispose(true);GC.SuppressFinalize(this); }
			private void Dispose(bool writeLog) { 
				if(writeLog) Logging.Log.Write(instanceOrType,flow);
			}
		}

		#region Expressions

		public static PositionExpression Enter {
			get {return new PositionExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.EnterMethod));}
		}

		public static PositionExpression Any {
			get {return new PositionExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.None));}
		}

		public static PositionExpression Exit {
			get {return new PositionExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.ExitMethod));}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "KsWare.Presentation.Logging.Flow+ExpressionResult.set_Message(System.String)")]
		public static PositionExpression StartThread(Thread thread) {
			return new PositionExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.StartThread) {
				Message = "Start thread"+(thread!=null ? " "+thread.ManagedThreadId:"")+(thread!=null && thread.Name!=null ? " '"+thread.Name+"'":"")
			});
		}
		//RESERVED: public static PositionExpression StartThread(Thread thread, string methodDescriptor)

		public static FlowExpression Throws(Exception ex) {
			loggedException.Value = ex;
			return new FlowExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.ThrowException));
		}

		public static FlowExpression Retrow() {
			return new FlowExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.RethrowException));
		}

		
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="ex")]
		public static FlowExpression Catch(Exception ex) {
			return new FlowExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.CatchException));
		}

		public static FlowExpression Output(string message) {
			{return new PositionExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.Output)).This.Current.Message(message);}
		}
		public static FlowExpression Warning(string message) {
			{return new PositionExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.Output)).This.Current.Warning(message);}
		}
		public static FlowExpression Error(string message) {
			{return new PositionExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.Output)).This.Current.Error(message);}
		}

		public static FlowExpression UnexpectedCondition(string message) {
			{return new PositionExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.Output)).This.Current.Message(message);}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "KsWare.Presentation.Logging.Flow+MethodDescriptorExpression.Message(System.String)"), SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // FxCop false warning
		public static FlowExpression RaiseEvent(string eventName) {
			{return new PositionExpression(new ExpressionResult(new StackTrace(1), FlowMemberPosition.RaiseEvent)).This.Current.Message("Raise event '"+eventName+"'");}
		}
		#endregion 

		#region static helpers

		/// <summary> Investigates the flow member type
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId="System.String.StartsWith(System.String)")]
		private static MemberType GetFlowMemberType(ExpressionResult data) {
			var methodBase = data.StackFrame.GetMethod();
			var constructor = methodBase as ConstructorInfo;
			if(constructor!=null                                                ) return MemberType.Constructor;
			if(methodBase.IsSpecialName && methodBase.Name.StartsWith("get_"   )) return MemberType.PropertySetter;
			if(methodBase.IsSpecialName && methodBase.Name.StartsWith("set_"   )) return MemberType.PropertyGetter;
			if(methodBase.IsSpecialName && methodBase.Name.StartsWith("add_"   )) return MemberType.AddEventHandler;
			if(methodBase.IsSpecialName && methodBase.Name.StartsWith("remove_")) return MemberType.RemoveEventHandler;
			if(methodBase.ToString()=="Void Dispose(Boolean)"                   ) return MemberType.Dispose;
//			return FlowMemberType.DispatcherMethod;
//			return FlowMemberType.Destructor;
//			return FlowMemberType.EventHandler;
//			return FlowMemberType.OverrideMethod;
//			return FlowMemberType.ThreadMethod;
			return MemberType.None;
		}

		private static string GetMethodName(ExpressionResult data) {
			return data.StackFrame.GetMethod().Name;
		}

		private static Type GetType(ExpressionResult data) { 
			return data.StackFrame.GetMethod().DeclaringType;
		}

		#endregion

		internal interface IExpressionResult
		{
			ExpressionResult Result{get;}
		}

		public class FlowExpression:IExpressionResult
		{
			protected FlowExpression() {}
			internal FlowExpression(ExpressionResult data) { Data = data; }

			protected ExpressionResult Data{get;set;}

			ExpressionResult IExpressionResult.Result {get {return Data;}}
		}

		public class ExpressionResult
		{
			internal ExpressionResult(StackTrace stackTrace, FlowMemberPosition flowType) {
				this.Parameters       = new List<LogParameter>();
				this.StackTrace       = stackTrace;
				this.StackFrame       = stackTrace.GetFrame(0);
				this.Type             = Flow.GetType(this);
				this.MethodType       = GetFlowMemberType(this);
				this.Position         = flowType;
				this.MethodDescriptor = GetMethodName(this);
			}

			public ExpressionResult(StackTrace stackTrace) {
				this.Parameters       = new List<LogParameter>();
				this.StackTrace       = stackTrace;
				this.StackFrame       = stackTrace.GetFrame(0);
				this.Type             = Flow.GetType(this);
				this.MethodType       = GetFlowMemberType(this);
				this.Position         = FlowMemberPosition.None;
				this.MethodDescriptor = GetMethodName(this);
			}

			/// <summary> Gets or sets the stack trace.
			/// </summary>
			/// <value>The stack trace.</value>
			/// <remarks></remarks>
			public StackTrace StackTrace{get;set;}

			/// <summary> Gets or sets the stack frame.
			/// </summary>
			/// <value>The stack frame.</value>
			/// <remarks></remarks>
			public StackFrame StackFrame{get;set;}

			/// <summary> Gets or sets the position.
			/// </summary>
			/// <value>The position.</value>
			/// <remarks></remarks>
			public FlowMemberPosition Position{get;set;}

			/// <summary> Gets or sets the MethodType.
			/// </summary>
			/// <value>The type of the method.</value>
			/// <remarks></remarks>
			public MemberType MethodType{get;set;}

			/// <summary> Gets or sets the method descriptor.
			/// </summary>
			/// <value>The method descriptor.</value>
			/// <remarks></remarks>
			public string MethodDescriptor{get;set;}

			/// <summary> Gets the parameters.
			/// </summary>
			/// <remarks></remarks>
			[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
			public List<LogParameter> Parameters {get;private set;}

			/// <summary> Gets or sets the instance.
			/// </summary>
			/// <value>The instance.</value>
			/// <remarks></remarks>
			public object Instance{get;set;}


			/// <summary> Gets or sets the type.
			/// </summary>
			/// <value>The type.</value>
			/// <remarks></remarks>
			[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
			public Type Type{get;set;}

			/// <summary>
			/// Gets or sets the message.
			/// </summary>
			/// <value>The message.</value>
			/// <remarks></remarks>
			public string Message{get;set;}

			/// <summary> Gets the assembly.
			/// </summary>
			/// <remarks></remarks>
			public Assembly Assembly{get {return Type == null ? null : Type.Assembly;}}

			/// <summary> Gets the module.
			/// </summary>
			/// <remarks></remarks>
			public Module Module{get {return Type == null ? null : Type.Module;}}
		}

//		public class Flow:Constraint
//		{
//			public Position Enter      {get{return new Position(new ConstraintData(new StackTrace(2), Log.FT.EnterMethod));}}
//			public Position Exit       {get{return new Position(new ConstraintData(new StackTrace(2), Log.FT.ExitMethod ));}}
//			public Position Dispatcher {get{return new Position(new ConstraintData(new StackTrace(2), Log.FT.Dispatcher ));}}
//			public Position Any        {get{return new Position(new ConstraintData(new StackTrace(2), Log.FT.None       ));}}
//		}

		public class PositionExpression:FlowExpression
		{

			internal PositionExpression(ExpressionResult data) {this.Data=data;}

			public MethodTypeExpression This            {get{Data.MethodType=GetFlowMemberType(Data)    ; return new MethodTypeExpression(Data);}}
			public MethodTypeExpression Method          {get{Data.MethodType=MemberType.Method          ; return new MethodTypeExpression(Data);}}
			public MethodTypeExpression EventHandler    {get{Data.MethodType=MemberType.EventHandler    ; return new MethodTypeExpression(Data);}}
			public MethodTypeExpression DispatcherMethod{get{Data.MethodType=MemberType.DispatcherMethod; return new MethodTypeExpression(Data);}}
			public MethodTypeExpression OverrideMethod  {get{Data.MethodType=MemberType.OverrideMethod  ; return new MethodTypeExpression(Data);}}
			public MethodTypeExpression PropertySetter  {get{Data.MethodType=MemberType.PropertySetter  ; return new MethodTypeExpression(Data);}}
			public MethodTypeExpression PropertyGetter  {get{Data.MethodType=MemberType.PropertyGetter  ; return new MethodTypeExpression(Data);}}
			public MethodTypeExpression Constructor     {get{Data.MethodType=MemberType.Constructor     ; return new MethodTypeExpression(Data);}}
			public FlowExpression       Dispose(bool explicitDisposing) { return This.Current.Parameter("explicitDisposing",explicitDisposing);}
		}

		public class MethodTypeExpression:FlowExpression
		{
			internal MethodTypeExpression(ExpressionResult data) { this.Data = data; }

			[SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId="0#")]
			public MethodDescriptorExpression Name(string name) {     Data.MethodDescriptor=name     ; return new MethodDescriptorExpression(Data); }
			
			public MethodDescriptorExpression Current           {get {Data.MethodDescriptor=GetMethodName(Data); return new MethodDescriptorExpression(Data);}}
		}

		public class MethodDescriptorExpression:FlowExpression
		{
			internal MethodDescriptorExpression(ExpressionResult data) { this.Data = data; }

			public ParameterExpression Parameter(string name, object value){Data.Parameters.Add(new LogParameter(name, value));  return new ParameterExpression(Data);}
			
			[SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId="0#")]
			public FlowExpression      Message  (string message)           {Data.Message=message             ;  return new FlowExpression(Data);}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "KsWare.Presentation.Logging.Flow+ExpressionResult.set_Message(System.String)")]
			public FlowExpression      Warning  (string message)           {Data.Message="WARNING: "+ message;  return new FlowExpression(Data);}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "KsWare.Presentation.Logging.Flow+ExpressionResult.set_Message(System.String)")]
			public FlowExpression      Error    (string message)           {Data.Message="ERROR: "  + message;  return new FlowExpression(Data);}
		}

		public class ParameterExpression:FlowExpression
		{
			internal ParameterExpression(ExpressionResult data) {Data=data;}

			public ParameterExpression Parameter(string name, object value){Data.Parameters.Add(new LogParameter(name, value));  return new ParameterExpression(Data);}
		}

		/// <summary> FlowMemberType
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public enum MemberType
		{
			None              ,
			Method            ,
			Constructor       ,
			PropertySetter    , // Autodetect possible: using method name: starts with "set_" AND IsSpecialName
			PropertyGetter    , // Autodetect possible: using method name: starts with "get_" AND IsSpecialName
			AddEventHandler   , // Autodetect possible: using method name: starts with "add_" AND IsSpecialName
			RemoveEventHandler, // Autodetect possible: using method name: starts with "remove_" AND IsSpecialName
			EventHandler      ,
			ThreadMethod      ,
			DispatcherMethod  ,
			OverrideMethod    ,
			Destructor        ,
			Dispose             // Autodetect possible: using method name: "Void Dispose(Boolean)"
		}

		/// <summary> FlowMemberPosition
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public enum FlowMemberPosition
		{
			None,
			EnterMethod,
			ExitMethod,
			StartThread,
			RaiseEvent,
			ThrowException,
			CatchException,
			RethrowException,
			Output,
		}

		/// <summary> Visualizes the specified <see cref="MemberType"/>
		/// </summary>
		/// <param name="memberType"></param>
		/// <returns></returns>
		internal static string Visualize(Flow.MemberType memberType) {
			switch (memberType) {
				case MemberType.None              : return "⇣"; // ↓ ⇊ ⇓
				case MemberType.DispatcherMethod  : return "↧";
				case MemberType.EventHandler      : return "↯";
				case MemberType.OverrideMethod    : return "⇓";
				case MemberType.ThreadMethod      : return "⇟";
				case MemberType.Constructor       : return "⊛";
				case MemberType.Destructor        : return "⊗";
				case MemberType.Dispose           : return "⊠";
				case MemberType.PropertyGetter    : return "⊲";
				case MemberType.PropertySetter    : return "≌";
				case MemberType.AddEventHandler   : return "+↯";
				case MemberType.RemoveEventHandler: return "-↯";
				default                               : return "?"; 
			}
		}

		public static string Visualize(FlowMemberPosition position) {
			switch (position) {
				case FlowMemberPosition.EnterMethod     : return "◆"; // →◆■▶◇▣▤▥▦▧▨
				case FlowMemberPosition.ExitMethod      : return "◯"; // ↳○◌◍◎●◯◬◭◮△▲▼▽◁◀◇◊○
				case FlowMemberPosition.ThrowException  : return "☢";
				case FlowMemberPosition.CatchException  : return "☂";
				case FlowMemberPosition.RethrowException: return "☣";
				case FlowMemberPosition.RaiseEvent      : return "↯";
				case FlowMemberPosition.StartThread     : return "⇞";
//				case FlowMemberPosition.Dispatcher      : return "↥";
//				case FlowMemberPosition.BeginAsync      : retunn "↟";
				case FlowMemberPosition.Output          : return "⌘"; //❒
				case FlowMemberPosition.None            : return " ";
				default                                 : return "?";
			}
		}
	}
}
/*
←↚↜↞↢↤↩↫↰↲↼↽⇍⇐⇚⇜⇠⇤⇦
→↛↝↠↣↦↪↬↱↳⇀⇁⇏⇒⇛⇝⇢⇥⇨
↑↟↥↾↿⇑⇞⇡⇧⇪

*/