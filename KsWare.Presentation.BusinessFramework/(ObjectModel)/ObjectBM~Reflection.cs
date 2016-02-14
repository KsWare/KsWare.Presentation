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

		private readonly ObjectBM m_ReflectedObjectBM;

		public ReflectedInfo(ObjectBM reflectedObjectBM, BusinessPropertyInfo propertyInfo) {
			PropertyInfo = propertyInfo;
			m_ReflectedObjectBM = reflectedObjectBM;
		}

		public ObjectBM ReflectedObject { get { return m_ReflectedObjectBM; } }

		public BusinessPropertyInfo PropertyInfo { get; set; }
	}

	internal class BusinessPropertyInfo {

		private Lazy<HierarchyAttribute[]> m_LazyHierarchyAttributes;
		private Lazy<ValueSettingsAttribute> m_LazyValueSettingsAttribute;

		public BusinessPropertyInfo() {
			m_LazyValueSettingsAttribute=new Lazy<ValueSettingsAttribute>(()=>PropertyInfo.GetCustomAttributes(typeof (ValueSettingsAttribute), false).Cast<ValueSettingsAttribute>().FirstOrDefault());
			m_LazyHierarchyAttributes=new Lazy<HierarchyAttribute[]>(()=>PropertyInfo.GetCustomAttributes(typeof (HierarchyAttribute), false).Cast<HierarchyAttribute>().ToArray());
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

		public HierarchyAttribute[] HierarchyAttributes {get {return m_LazyHierarchyAttributes.Value;}}

		public ValueSettingsAttribute ValueSettingsAttribute {get {return m_LazyValueSettingsAttribute.Value;}}

	}
}
