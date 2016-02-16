using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework { 

	partial class ObjectVM {

		private void InitPartDebuger() {
			DebuggerFlags=new ClassDebuggerFlags();
		}

		/// <summary> Gets the debugger flags.
		/// </summary>
		/// <value>The debugger flags.</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
		public ClassDebuggerFlags DebuggerFlags{get; protected set;}


		/// <summary> Provides flags for class debugging
		/// </summary>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public class ClassDebuggerFlags {

			/// <summary> Initializes a new instance of the <see cref="KsWare.Presentation.ViewModelFramework.ValueVM{T}.ClassDebuggerFlags"/> class.
			/// </summary>
			/// <remarks></remarks>
			public ClassDebuggerFlags() {
				Breakpoints=new ClassDebuggerFlagsBreakpoints();
			}

			/// <summary> Gets the breakpoints.
			/// </summary>
			/// <remarks></remarks>
			public ClassDebuggerFlagsBreakpoints Breakpoints{get; protected set;}
		}

		/// <summary> provides available breakpoints for <see cref="ClassDebuggerFlags"/>
		/// </summary>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public class ClassDebuggerFlagsBreakpoints {

			/// <summary> Gets or sets the Metadata{get} breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Metadata{get} breakpoint; otherwise, <see langword="false"/> disable the Metadata{get} breakpoint.
			/// </value>
			public bool MetadataGet{get;set;}

			/// <summary> Gets or sets the Metadata{get} breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Metadata{set} breakpoint; otherwise, <see langword="false"/> disable the Metadata{set} breakpoint.
			/// </value>
			public bool MetadataSet{get;set;}

			/// <summary> Gets or sets the HasMetadata{get} breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the HasMetadata{get} breakpoint; otherwise, <see langword="false"/> disable the HasMetadata{get} breakpoint.
			/// </value>
			public bool HasMetadataGet{get;set;}


			public bool IsSelectedGet { get; set; }
			public bool IsSelectedSet { get; set; }
		}


		private void DebugːTraceInternal(string message) {
			var typeName=DebugUtil.FormatTypeName(this);
			var s = new StringBuilder();
			s.Append("TRACE:" + " ");
			s.Append(typeName + " ");
			s.Append(message);
			Debug.WriteLine(message);
		} 
		
		protected bool DebugːEnableTrace = false;

		internal static long StatisticsːNumberOfInstances;
		internal static long StatisticsːNumberOfCreatedInstances;
		internal static long StatisticsːMethodInvocationːDisposeːExplicitːCount;
		internal static long StatisticsːMethodInvocationːDestructorːCount;

		[Conditional("DEBUG")]
		protected void DebugːTrace(string message) {DebugːTraceInternal(message);} 
		[Conditional("DEBUG")][StringFormatMethod("format")]
		protected void DebugːTrace(string format,params object[] args) {DebugːTraceInternal(string.Format(format,args));} 
		[Conditional("DEBUG")]
		protected void DebugːTrace(bool condition, string message) {if(condition) DebugːTraceInternal(message);} 
		[Conditional("DEBUG")][StringFormatMethod("format")]
		protected void DebugːTrace(bool condition, string format,params object[] args) {if(condition) DebugːTraceInternal(string.Format(format,args));} 

		public string DebugːGetTypeːName{get { return DebugUtil.FormatTypeName(this); }}
		
		public string DebugːGetTypeːFullName{get { return this.GetType().FullName; }}

		/// <summary> Signals a breakpoint to the debugger.
		/// <br/>(If an debugger is attached; else no action)
		/// </summary>
		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak() {DebugUtil.Break();}

		/// <summary> Show a message box and signals a breakpoint to the debugger.
		/// <br/>(If an debugger is attached; else no action)
		/// </summary>
		/// <param name="message">The message to show</param>
		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak(string message) {DebugUtil.Break(message);}

		/// <summary> Show a message box and signals a breakpoint to the debugger.
		/// <br/>(If an debugger is attached; else no action)
		/// </summary>
		/// <param name="sender">The calling object</param>
		/// <param name="message">The message to show</param>
		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak(ObjectVM sender, string message) { DebuggerːBreak(true, sender, message); }

		/// <summary> Signals a breakpoint to the debugger.
		/// <br/>(If an debugger is attached; else no action)
		/// </summary>
		/// <param name="condition"></param>
		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak(bool condition) {if(!condition) return; DebugUtil.Break();}

		/// <summary> Show a message box and signals a breakpoint to the debugger.
		/// <br/>(If an debugger is attached; else no action)
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message">The message to show</param>
		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak(bool condition, string message) {if(!condition) return; DebugUtil.Break(message);}

		/// <summary> Show a message box and signals a breakpoint to the debugger.
		/// <br/>(If an debugger is attached; else no action)
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="sender">The calling object</param>
		/// <param name="message">The message to show</param>
		[Conditional("DEBUG"),DebuggerStepThrough,DebuggerHidden]
		static protected void DebuggerːBreak(bool condition, ObjectVM sender, string message) {
			if(!condition) return;
			DebugUtil.Break(message + 
				"\nType: " + DebugUtil.FormatTypeName(sender) + 
				"\nPath: " + DebugUtil.Try(() => sender.MemberPath,"??")
				);
		}
	}
}
