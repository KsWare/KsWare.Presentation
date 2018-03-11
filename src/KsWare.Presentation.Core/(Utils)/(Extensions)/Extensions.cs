using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

// ReSharper disable CheckNamespace
namespace System {

	/// <summary> Provides extension methods
	/// </summary>
	public static class Extensions
	{
		private static readonly IFormatProvider enUS = CultureInfo.CreateSpecificCulture("en-US");

		/// <summary> Does nothing. 
		/// Uses to prevent code inspectors "Make static" rule and prevent inpection of unused arguments
		/// </summary>
		/// <param name="obj"><see langword="this"/></param>
		/// <param name="args"></param>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId="obj")]
		[SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId="System.String.Format(System.String,System.Object,System.Object)")]
		[SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId="System.String.Format(System.String,System.Object)")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="obj")]
		public static void DoNothing(this object obj, params object[]args) {
//			var stackFrame = new StackFrame(1, true);
//			Debug.WriteLine(string.Format("WARNING: {0}.{1} does nothing!",obj.GetType().FullName,stackFrame.GetMethod().Name));
		}

		/// <summary> Returns the name of the specified type inclusive generic arguments
		/// </summary>
		/// <param name="type"></param>
		/// <example><c> return typeof(int?).GetType.NameT(); </c> 
		/// Returns: <c>Nullable&lt;Int32&gt;</c>
		/// </example>
		/// <returns>A string representation of the type name</returns>
		public static string NameT(this Type type) {
			if(type.IsGenericType) {
				var a = type.GetGenericArguments().Select(genericArgument => genericArgument.NameT()).ToArray();
				return type.GetGenericTypeDefinition().Name.Substring(0,type.Name.IndexOf('`')) + "<" + string.Join(",", a) + ">";
			} else {
				return type.Name;
			}
		}

		/// <summary> Returns a <see cref="System.String"/> that represents the type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="format">The format.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents the type.
		/// </returns>
		[SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId="System.String.EndsWith(System.String)")]
		[SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId="System.String.Format(System.String,System.Object,System.Object,System.Object)")]
		[SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId="System.String.StartsWith(System.String)")]
		public static string ToString(this Type type, string format) {
			string s = "";

			string prefix = "";
			string suffix = "";
			if((format.StartsWith("{") && format.EndsWith("}")) ||
			   (format.StartsWith("(") && format.EndsWith(")")) ){
				prefix = format.First().ToString();
				suffix = format.Last().ToString();
			}

			if(string.IsNullOrWhiteSpace(format)) {
				s=string.Format("{0}{1}{2}",prefix,type.ToString(),suffix);
			}else if(type.IsGenericType && format.Contains("G") || format.Contains("g")) {
				var a = type.GetGenericArguments().Select(genericArgument => genericArgument.ToString(format)).ToArray();
				s= string.Format("{0}{1}{2}",
					prefix,
					type.GetGenericTypeDefinition().Name.Substring(0,type.Name.IndexOf('`')) + "<" + string.Join(",", a) + ">",
					suffix
				);
			} else {
				s= string.Format("{0}{1}{2}",
					prefix,
					type.Name,
					suffix
				);
			}

			return s;
		}

		public static string ToInvariantString(this object o) { return string.Format(CultureInfo.InvariantCulture, "{0}", o); }

		/// <summary> Determines whether the specified type is nullable.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><see langword="true"/> if the specified type is nullable; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsNullable(this Type type){return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);}

		/// <summary> Gets the base type of the nullable type.
		/// </summary>
		/// <value>The base type of the nullable type.</value>
		public	static Type NullableBaseType(this Type type){{return (type.IsNullable()) ? type.GetGenericArguments()[0] : type;}}

		/// <summary> Gets the full signature of the method in form of <c>TypeFullName.MethodSignatur</c>.
		/// </summary>
		/// <param name="methodInfo">The method info.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static string GetFullSignatur(this MethodInfo methodInfo) {
			string typeFullName=methodInfo.DeclaringType.FullName;
			var strings = methodInfo.ToString().Split(new[] {' '}, 2, StringSplitOptions.None);
//			var returnType = strings[0];
			var methodSignature = strings[1];
			return typeFullName + "." + methodSignature;
		}

		private static string GetFullMethodSignaturQuickTest() {
			//"System.Extensions.ToString(System.Type, System.String)"
			return GetFullSignatur(typeof(Extensions).GetMethod("ToString",new []{typeof(Type),typeof(string)}));
		}


		/// <summary> Gets the context menu element.
		/// </summary>
		/// <param name="menuItem">A menu item.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static ContextMenu GetContextMenu(this MenuItem menuItem) {
			var d = menuItem.Parent;
			while (d!=null) {
				var cm = d as ContextMenu; if(cm!=null) return cm;
				d = LogicalTreeHelper.GetParent(d);
			}
			return null;
		}

		/// <summary> Gets the UIElement relative to which the ContextMenu is positioned when it opens. 
		/// </summary>
		/// <param name="menuItem">A menu item.</param>
		/// <returns>The element relative to which the ContextMenu is positioned when it opens.</returns>
		public static UIElement GetPlacementTarget(this MenuItem menuItem) {
			var contextMenu = GetContextMenu(menuItem);
			if(contextMenu!=null) return contextMenu.PlacementTarget;
			return null;
		}


//		/// <summary> Converts the value to its equivalent string representation using the en-US format information.
//		/// </summary>
//		/// <param name="value">The value to be converted.</param>
//		/// <returns>The string representation of the value of this instance.</returns>
//		/// <remarks></remarks>
//		public static string ToStringEnUs(this double value) { return value.ToString(enUS); }
//		
//		/// <summary> Converts the value to its equivalent string representation using the en-US format information.
//		/// </summary>
//		/// <param name="value">The value to be converted.</param>
//		/// <returns>The string representation of the value of this instance.</returns>
//		/// <remarks></remarks>
//		public static string ToStringEnUs(this int value) { return value.ToString(enUS); }

		/// <summary> Converts value to its equivalent string representation using the en-US format information.
		/// </summary>
		/// <param name="value">The value to be converted.</param>
		/// <returns>The string representation of the value of this instance.</returns>
		/// <remarks></remarks>
		public static string ToStringEnUs(this object value) { return string.Format(enUS, "{0}", value); }

//		public static string ToStringEnUs(this x value) { return value.ToString(enUS); }
//		public static string ToStringEnUs(this x value) { return value.ToString(enUS); }

		public static string ToStringSortable(this DateTime value) { return value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss}"); }
	}
}

namespace System.Linq
{
}
// ReSharper restore CheckNamespace