using System.ComponentModel;

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
		public virtual object MːData {get => Metadata.DataProvider.Data; set => Metadata.DataProvider.Data=value; }

		public string DebugːGetTypeːName => DebugUtil.FormatTypeName(this);

		public string DebugːGetTypeːFullName => this.GetType().FullName;
	}


}
