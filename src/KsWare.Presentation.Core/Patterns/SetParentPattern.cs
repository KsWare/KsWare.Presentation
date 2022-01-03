using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KsWare.Presentation.Core.Patterns {

	/// <summary> Pattern: Only the parent object itself should set the Parent property!
	/// </summary>
	/// <example>
	/// <code>
	/// class MyClass {
	///		IHirarchical parentBackingField;
	///		public IHirarchical Parent {
	///			get {return this.parent;}
	///			set {SetParentPattern.Execute(ref parentBackingField, value,"Parent");}
	///		}
	/// }
	/// </code>
	/// </example>
	public static class SetParentPattern {
		//RESERVED: static Dictionary<Type,Type> dic=new Dictionary<Type, Type>();


		/// <summary> Executes the pattern
		/// </summary>
		/// <typeparam name="TParent">Type of parent</typeparam>
		/// <param name="parentBackingFieldRef">The backing field reference of Parent-property.</param>
		/// <param name="newParent">The new parent.</param>
		/// <param name="propertyName">Name of the property.</param>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public static void Execute<TParent>(ref TParent parentBackingFieldRef, TParent newParent, string propertyName)
			where TParent : class {
			if (parentBackingFieldRef == newParent) return;
			if (string.IsNullOrWhiteSpace(propertyName)) propertyName = "Parent";
			if (newParent != null && parentBackingFieldRef != null) {
				//change parent
				throw new InvalidOperationException(propertyName + " already assigned!");
			}
#if false //FIXME: Later activate this. Error in test of release version
			var stackFrame = new StackFrame(2);
			var caller = stackFrame.GetMethod().ReflectedType; //ex: "ListBM`1"
			if(caller==typeof(SetPropertyWithParentPattern)) {
			    // access allowed
			} else {
			    var parentType0 =
 newParent != null ? newParent.GetType() : parentBackingFieldRef.GetType(); //ex: DerivedList
			    var parentType = parentType0;
			    while (true) {
	//FIXME:		if(dic.ContainsKey(parentType0) && dic[parentType0]==caller) break; //test with the cached type if available
			        var parentType2 = parentType.IsGenericType ? parentType.GetGenericTypeDefinition() : parentType;
			        if(parentType2==caller) {
			            // for performance cache the matching parent type
	//FIXME:			dic.Add(parentType0,parentType2);
			            break;
			        }
			        parentType = parentType.BaseType;
			        Trace.Write(parentType0 + " "+caller);
			        if (parentType == typeof(object))
			            throw new InvalidOperationException("Access denied! Only the parent/owner can set this property!" +
			            "\r\n\tProperty: "+propertyName);
			    }
			    if(parentType.IsGenericType) parentType = parentType.GetGenericTypeDefinition();				
			}
#endif
			if (newParent == null && parentBackingFieldRef != null) {
				//OnParentChanging(oldParent: this.parent, newParent: null);
				parentBackingFieldRef = null;
			}
			else if (newParent != null && parentBackingFieldRef == null) {
				//OnParentChanging(oldParent: this.parent, newParent: value);
				parentBackingFieldRef = newParent;
			}
			else {
				Trace.TraceWarning("Not implemented! UniqueID: {42072673-D475-4CEE-A411-15E0DEC18DB4}");
			}
		}
	}

}
