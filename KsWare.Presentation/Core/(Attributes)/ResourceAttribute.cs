using System;

namespace KsWare.Presentation {

	[AttributeUsage(AttributeTargets.Field)]
	public class ResourceAttribute : Attribute {

		public ResourceAttribute() {}
		public ResourceAttribute(string name) {Name=name;}

		public string Name { get; private set; }
	}
}