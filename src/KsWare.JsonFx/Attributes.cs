using System;

namespace KsWare.JsonFx {

	[AttributeUsage(AttributeTargets.All)]
	public sealed class JsonNameAttribute:Attribute {
		
		public JsonNameAttribute(string name) { Name = name; }

		public string Name { get; set; }
	}

	[AttributeUsage(AttributeTargets.All)]
	public class JsonTypeAttribute:Attribute {
		
		public JsonTypeAttribute(JsonType type) { Type = type; }

		public JsonType Type { get; set; }
	}

	[AttributeUsage(AttributeTargets.All)]
	public sealed class JsonIgnoreAttribute:Attribute {
		
		public JsonIgnoreAttribute() {}

	}
	
	public enum JsonType {
		None,
		Null,
		Numeric,
		Boolean,
		String,
		Array,
		Object,
	}
}
