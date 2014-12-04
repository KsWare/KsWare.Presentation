using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using KsWare.Presentation.Providers;
using KsWare.Presentation.Core;
using KsWare.Presentation.Core.Patterns;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Business object
	/// </summary>
	public partial class ObjectBM {

		internal static long StatisticsːNumberOfCreatedInstances;
		internal static long StatisticsːNumberOfInstances;
		internal static long StatisticsːMethodInvocationːDisposeːCount;
		internal static long StatisticsːMethodInvocationːDestructorːCount;

		/// <summary> ALIAS for <c>Metadata.DataProvider.Data</c> </summary>
		[EditorBrowsable(EditorBrowsableState.Never),Browsable(false)]
		//[Obsolete("Use Data or Metadata.DataProvider.Data",true)]/*enable this to find data accessor/for debug)
		public virtual object MːData {get { return Metadata.DataProvider.Data; }set {  Metadata.DataProvider.Data=value; }}

		public string DebugːGetTypeːName{get { return DebugUtil.FormatTypeName(this); }}
		
		public string DebugːGetTypeːFullName{get { return this.GetType().FullName; }}
	}


}
