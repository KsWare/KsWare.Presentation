using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.Documentation {

	public class docːObjectBM {

		/// <summary>
		/// <list type="table">
		///		<listheader><term>Type</term><description>Description</description></listheader>
		///		<item><term><see cref="ObjectBM"/></term>
		///			<description>manages any/none data (base class for all business objects)</description>
		///		</item>
		///		<item><term><see cref="DataBM{TData}"/></term>
		///			<description>manages strong typed data (<see cref="DataBM{TData}.Data"/>)</description>
		///		</item>
		///     <item><term><see cref="DynamicBM"/></term>
		///			<description>manages dynamic typed data (<see cref="DynamicBM.Value"/> and <see cref="DynamicBM.ValueType"/>)</description>
		///		</item>
		///     <item><term>?</term>
		///			<description>manages untyped data (at present use <see cref="ObjectBM"/>.<see cref="ObjectBM.MːData"/>) </description>
		///		</item>
		/// </list>
		/// </summary>
		public object dataˑmanagementˑofˑbasicˑbusinessˑmodels;

		/// <example>
		/// Assuming you have object like this:
		/// <code> public class MyBusinessObject : ObjectBM {
		///	    public BoolBM MyProperty { get; private set; }
		/// }</code>
		/// and anywhere in the hierarchy above you want to know when the MyProperty is changed. 
		/// Implement in the object which wants to be notificated:
		/// <code> protected override void OnTreeChanged(TreeChangedEventArgs e) {
		///    base.OnTreeChanged(e);
		///    var source = e.OriginalSource as IObjectBM;
		///    if (source is BoolBM &amp;&amp; source.MemberName=="MyProperty" &amp;&amp; source.Parent is MyBusinessObject) {
		///        // no you have a MyBusinessObject.MyProperty notification
		///    }
		/// }
		/// </code>
		/// Alternatively you can subscribe to <see cref="IObjectBM.TreeChanged"/> event anywhere above in the hierarchy.
		/// </example>
		public object TreeChangedˑexample1;

	}
}
