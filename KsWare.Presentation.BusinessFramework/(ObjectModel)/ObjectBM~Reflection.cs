using System;
using System.Linq;
using System.Reflection;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Business object
	/// </summary>
	public partial class ObjectBM {

		private void InitPartReflection() {
			
		}

		/// <summary> [EXPERIMENTAL] Gets the reflection information which was used to create this <see cref="ObjectVM"/> instance.
		/// </summary>
		/// <value>The reflection.</value>
		/// <seealso cref="RegisterChildren"/>
		internal ReflectedInfo Reflection { get; set; }
	}

	/// <summary> [EXPERIMENTAL]
	/// </summary>
	internal class ReflectedInfo {

		private readonly ObjectBM _ReflectedObjectBM;

		public ReflectedInfo(ObjectBM reflectedObjectBM, BusinessPropertyInfo propertyInfo) {
			PropertyInfo = propertyInfo;
			_ReflectedObjectBM = reflectedObjectBM;
		}

		public ObjectBM ReflectedObject { get { return _ReflectedObjectBM; } }

		public BusinessPropertyInfo PropertyInfo { get; set; }
	}

	internal class BusinessPropertyInfo {

		private Lazy<HierarchyAttribute[]> _lazyHierarchyAttributes;
		private Lazy<ValueSettingsAttribute> _lazyValueSettingsAttribute;

		public BusinessPropertyInfo() {
			_lazyValueSettingsAttribute=new Lazy<ValueSettingsAttribute>(()=>PropertyInfo.GetCustomAttributes(typeof (ValueSettingsAttribute), false).Cast<ValueSettingsAttribute>().FirstOrDefault());
			_lazyHierarchyAttributes=new Lazy<HierarchyAttribute[]>(()=>PropertyInfo.GetCustomAttributes(typeof (HierarchyAttribute), false).Cast<HierarchyAttribute>().ToArray());
		}

		public BusinessPropertyInfo(PropertyInfo propertyInfo):this() {
			PropertyInfo = propertyInfo;
		}

		public PropertyInfo PropertyInfo { get; set; }

		/// <summary> Gets the name of the current property.
		/// </summary>
		/// <value>A <see cref="System.String"/> containing the name of this property.</value>
		public string Name {get {return PropertyInfo.Name;}}

		/// <summary> Gets the type of this property.
		/// </summary>
		/// <value>TThe type of this property.</value>
		public Type Type {get {return PropertyInfo.PropertyType;}}

		public HierarchyAttribute[] HierarchyAttributes {get {return _lazyHierarchyAttributes.Value;}}

		public ValueSettingsAttribute ValueSettingsAttribute {get {return _lazyValueSettingsAttribute.Value;}}

	}
}
