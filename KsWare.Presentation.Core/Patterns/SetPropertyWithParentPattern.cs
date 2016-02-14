using System.Diagnostics.CodeAnalysis;
using KsWare.Presentation;

namespace KsWare.Presentation.Core.Patterns {

	/// <summary> Sets a backing field and simultaneously the Parent of the value. Used in property-setter
	/// </summary>
	/// <example>
	/// Usage:
	/// <code>
	/// class Sample {
	///		IParentSupport backingFieldWithParentSupport;
	///		public IParentSupport Property {
	///			set {
	///				SetPropertyWithParentPattern.Execute(ref this.backingFieldWithParentSupport, value, this);
	///			}
	///		}
	/// }
	/// </code>
	/// </example>
	public static class SetPropertyWithParentPattern {
		//RESERVED: static Dictionary<Type,Type> dic=new Dictionary<Type, Type>();

		/// <summary> Sets the backing field and the parent 
		/// </summary>
		/// <typeparam name="TValue">Type of value</typeparam>
		/// <param name="backingFieldRef">The backing field reference.</param>
		/// <param name="newValue">The new value.</param>
		/// <param name="parent">The parent (Usual <see langword="this"/>)</param>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId="0#")]
		public static void Execute<TValue>(ref TValue backingFieldRef, TValue newValue, object parent) where TValue:class,IParentSupport{ 
			if(backingFieldRef!=null) backingFieldRef.Parent = null;
			backingFieldRef = newValue;
			newValue.Parent = parent; 
		}
	}
}
