using System;

namespace KsWare.Presentation {

	[AttributeUsage(AttributeTargets.Property)]
	public class HierarchyAttribute : Attribute {

		public HierarchyAttribute(HierarchyType itemType)
			:this(itemType,null) { }

		public HierarchyAttribute(HierarchyType itemType, string referencePath) {
			ItemType        = itemType;
			ReferencePath  = referencePath;
			CreateInstance = true;
		}

		public HierarchyType ItemType { get; private set; }

		public string ReferencePath { get; set; }

		/// <summary> Gets or sets a value indicating whether the property is initialized with an instance. The default ist <c>true</c>
		/// </summary>
		/// <value><c>true</c> automatically registered; otherwise, <c>false</c>.</value>
		/// <remarks><see cref="CreateInstance"/> is used in <see cref="ObjectVM.RegisterChildren"/> </remarks>
		public bool CreateInstance { get; set; }

	}

	public enum HierarchyType {
		None,
		
		Child,
		Parent,
		Reference,
		Other,

		Ignore=None,
		Default=Child,
		TypeCast,

	}

}