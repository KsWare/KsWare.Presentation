using System.Reflection;

namespace KsWare.Presentation.ViewModelFramework {

	public partial class ObjectVM {

		private void InitPartReflection() {
			
		}

		/// <summary> [EXPERIMENTAL] Gets the reflection information which was used to create this <see cref="ObjectVM"/> instance.
		/// </summary>
		/// <value>The reflection.</value>
		internal ReflectedPropertyInfo Reflection { get; set; }

	}

	/// <summary> [EXPERIMENTAL] Class ReflectedPropertyInfo. used in <see cref="ObjectVM"/>
	/// </summary>
	internal class ReflectedPropertyInfo {

		private readonly ObjectVM m_ReflectedObjectVM;

		public ReflectedPropertyInfo(ObjectVM reflectedObjectVM, PropertyInfo propertyInfo) {
			PropertyInfo = propertyInfo;
			m_ReflectedObjectVM = reflectedObjectVM;
		}

		public ObjectVM ReflectedObject { get { return m_ReflectedObjectVM; } }

		public PropertyInfo PropertyInfo { get; set; }

		public HierarchyAttribute[] HierarchyAttributes { get; set; }

		public ReflectedPropertyInfo ToInstance(ObjectVM instance) {
			return new ReflectedPropertyInfo(instance,PropertyInfo) {
				HierarchyAttributes=HierarchyAttributes
			};
		}

	}
}
