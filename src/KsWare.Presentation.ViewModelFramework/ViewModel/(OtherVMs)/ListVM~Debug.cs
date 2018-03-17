namespace KsWare.Presentation.ViewModelFramework {

	partial class ListVM<TItem> {
		#region Debug

// ReSharper disable UnusedAutoPropertyAccessor.Global
		/// <summary> Gets the debugger flags.
		/// </summary>
		/// <value>The debugger flags.</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
		public new ClassDebuggerFlags DebuggerFlags => (ClassDebuggerFlags)base.DebuggerFlags;
// ReSharper restore UnusedAutoPropertyAccessor.Global


		/// <summary> Provides flags for class debugging
		/// </summary>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public new class ClassDebuggerFlags:ObjectVM.ClassDebuggerFlags {

			/// <summary> Initializes a new instance of the <see cref="ListVM{TItem}.ClassDebuggerFlags"/> class.
			/// </summary>
			/// <remarks></remarks>
			public ClassDebuggerFlags() {
				base.Breakpoints=new ClassDebuggerFlagsBreakpoints();
			}
// ReSharper disable UnusedAutoPropertyAccessor.Global
			/// <summary> Gets the breakpoints.
			/// </summary>
			/// <remarks></remarks>
			public new ClassDebuggerFlagsBreakpoints Breakpoints => (ClassDebuggerFlagsBreakpoints)base.Breakpoints;
// ReSharper restore UnusedAutoPropertyAccessor.Global
		}

		/// <summary> provides availabe breakpoints for <see cref="ClassDebuggerFlags"/>
		/// </summary>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public new class ClassDebuggerFlagsBreakpoints:ObjectVM.ClassDebuggerFlagsBreakpoints {
// ReSharper disable UnusedAutoPropertyAccessor.Global

			/// <summary> Gets or sets the Add breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Add breakpoint; otherwise, <see langword="false"/> disable the Add breakpoint.
			/// </value>
			public bool Add{get;set;}

			/// <summary> Gets or sets the Clear breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Clear breakpoint; otherwise, <see langword="false"/> disable the Clear breakpoint.
			/// </value>
			public bool Clear{get;set;}

			/// <summary> Gets or sets the Contains breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Contains breakpoint; otherwise, <see langword="false"/> disable the Contains breakpoint.
			/// </value>
			public bool Contains{get;set;}

			/// <summary> Gets or sets the IndexOf breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the IndexOf breakpoint; otherwise, <see langword="false"/> disable the IndexOf breakpoint.
			/// </value>
			public bool IndexOf{get;set;}

			/// <summary> Gets or sets the Insert breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Insert breakpoint; otherwise, <see langword="false"/> disable the Insert breakpoint.
			/// </value>
			public bool Insert{get;set;}

			/// <summary> Gets or sets the Remove breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Remove breakpoint; otherwise, <see langword="false"/> disable the Remove breakpoint.
			/// </value>
			public bool Remove{get;set;}

			/// <summary> Gets or sets the RemoveAt breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the RemoveAt breakpoint; otherwise, <see langword="false"/> disable the RemoveAt breakpoint.
			/// </value>
			public bool RemoveAt{get;set;}

			/// <summary> Gets or sets the this[index]{get} breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the this[index]{get} breakpoint; otherwise, <see langword="false"/> disable the this[index]{get} breakpoint.
			/// </value>
			public bool ThisGet{get;set;}

			/// <summary> Gets or sets the this[index]{set} breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the this[index]{set} breakpoint; otherwise, <see langword="false"/> disable the this[index]{set} breakpoint.
			/// </value>
			public bool ThisSet{get;set;}

			/// <summary> Gets or sets the Count{get} breakpoint.
			/// </summary>
			/// <value>
			/// 	<see langword="true"/> enable the Count{get} breakpoint; otherwise, <see langword="false"/> disable the Count{get} breakpoint.
			/// </value>
			public bool Count{get;set;}

			public bool OnDataChanged { get; set; }

// ReSharper restore UnusedAutoPropertyAccessor.Global

		}

		#endregion
	}
}
