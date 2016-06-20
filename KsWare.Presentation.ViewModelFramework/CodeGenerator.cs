using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework.DesignTime
{
	// ReSharper disable LocalizableElement

	public class CodeGeneratorBase
	{
		public static List<PropertyInfo> GetProperties(Type underlyingType, Type currentType, bool missingPropertiesOnly) { 
			var underlayingProps = underlyingType.GetProperties();
			var currentProps = currentType.GetProperties();
			var properties = new List<PropertyInfo>();
			Console.WriteLine("Properties to implement:");
			foreach (var p in underlayingProps) {
				if(missingPropertiesOnly && currentProps.Any(x=>x.Name==p.Name)) continue;
				properties.Add(p);
				Console.WriteLine("{0,-32} {1}",p.Name, p.PropertyType.Name);
			}
			return properties;
		}

		public static void WriteLine(string format,params object[] args) {
			string s = string.Format(format, args);
			WriteLineInternal(0,s);
		}
		public static void WriteLine(string s) {
			WriteLineInternal(0,s);
		}
		public static void WriteLine(int indent, string format,params object[] args) {
			if(args==null)args=new object[0];
			string s = string.Format(format, args);
			WriteLineInternal(indent,s);
		}
		public static void WriteLine(int indent, string s) {
			WriteLineInternal(indent,s);
		}
		public static void WriteLine() {Console.WriteLine();}

		public static void WriteLineInternal(int indent, string s) {
			var lines=s.Split(new string[] {"\r\n", "\n", "\r"},StringSplitOptions.None).ToList<string>();
			if(string.IsNullOrWhiteSpace(lines[0])) lines.RemoveAt(0);
			if(string.IsNullOrWhiteSpace(lines[lines.Count-1])) lines.RemoveAt(lines.Count-1);

			var min = 1000;
			foreach (var line in lines) {
				if(!string.IsNullOrWhiteSpace(line)) {
					var i=line.Length - line.TrimStart('\t').Length;
					if(i<min) min = i;
				}
			}
			for (int i = 0; i < lines.Count; i++) {
				if(!string.IsNullOrWhiteSpace(lines[i])) {
					lines[i] = lines[i].Substring(min);
					lines[i] = new string('\t', indent) + lines[i];
				}
			}
			s = string.Join("\r\n",lines);
			Console.WriteLine(s);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "isGeneric"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "isArray"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "interfaces"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "elementType")]
		public static VMType GetVMType(PropertyInfo p) {
			var isArray = typeof(Array).IsAssignableFrom(p.PropertyType);
			var isGeneric = p.PropertyType.IsGenericType;
			var interfaces = p.PropertyType.GetInterfaces();
			var elementType = GetElementType(p);

			if (typeof(IList).IsAssignableFrom(p.PropertyType)) { //all arrays implement IList
				return VMType.ListVM;
			} else if (p.PropertyType.BaseType == typeof(Enum)) {
				return VMType.EnumVM;
			} else if (p.PropertyType.BaseType == typeof(Object) && p.PropertyType.Name != "String") {
				return VMType.CustomVM;
			} else {
				return VMType.ValueVM;
			}
		}

		public static Type GetElementType(PropertyInfo p) {
			if(p.PropertyType.IsGenericType) {
				return p.PropertyType.GetGenericArguments()[0];
			} else if(p.PropertyType.IsArray){
				return p.PropertyType.GetElementType();
			} else {
				return typeof(object);
			}
		}
	}

	public enum VMType{None,ListVM,EnumVM,CustomVM,ValueVM}

	public class ViewModelCodeGenerator4DataObjects:CodeGeneratorBase
	{
		public const string ThisTData = "ThisTData";
		public const string ThisTModel = "ThisTModel";

		public static void GenerateMissingProperties(Type dataType, Type viewmodelType) {
			var missingBusinessProbs=GetProperties(dataType,viewmodelType,true);

			DesignerPreview(viewmodelType,dataType);
			Constructor(viewmodelType,missingBusinessProbs);
//			DataAlias(dataType);
			SetHeader(missingBusinessProbs);
			//OnBusinessObjectChanged(properties);
			Properties(missingBusinessProbs);
		}

		private static void DesignerPreview(Type vmType,Type dataType) {
			WriteLine();
			WriteLine(2,@"
				/// <summary> Gets the designer preview.
				/// </summary>
				/// <value> The designer preview. </value>
				public static {0} DesignerPreview {{
					get {{
						var vm = new {0}{{Data=new {1}()}};
						return vm;
					}}
				}}
				",
				 vmType.Name,dataType.Name
			);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private static void DataAlias(Type dataType) {
			WriteLine(3,@"
				/// <summary> Alias for strong typed access to Metadata.DataProvider.Data </summary>
				private {0} Data {{ get {{ return ({0}) Metadata.DataProvider.Data; }} set {{ Metadata.DataProvider.Data = value; }}}}",
			dataType.Name);
		}


		private static void Constructor(Type viewmodelType, List<PropertyInfo> properties) {
			WriteLine(2,@"
				/// <summary> Initializes a new instance of the <see cref=""{0}"" /> class.
				/// </summary>
				public {0}() {{",
				viewmodelType.Name
			);
			foreach (var p in properties) {ConstructorExpression(p);}
			WriteLine(2,@"
					/* PLACEHOLDER {{Next entry}} */

					SetHeaders(); //ObjectVM.CultureChanged += (sender, args) => SetHeaders(); 
				}}",
				null);
		}

//		private static void ConstructorExpression(PropertyInfo p) {
//			if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
//				var propertyType = p.PropertyType.GetGenericArguments()[0].Name;
//				var vmPropertyType = propertyType.Replace("Data", "")+"VM";
//				if(vmPropertyType=="BooleanVM") vmPropertyType = "BoolVM";
//				WriteLine(3, @"
//					{0} = RegisterChild(new ListVM<{1}>() {{
//						MemberName=""{0}"",
//						Metadata = new ListViewModelMetadata {{
//							DataProvider = new CustomDataProvider<List<{2}>>(() => this.Data.{0}, value => this.Data.{0} = value)
//						}}
//					}});
//					".TrimStart('\r', '\n').TrimEnd('\r', '\n','\t',' '),
//					 p.Name, 
//					 vmPropertyType,
//					 propertyType
//				 );
//
////				Console.WriteLine("\t\t\t" + "{0} = RegisterChild(new ListVM<{1}>() {{", p.Name, p.PropertyType.GetGenericArguments()[0].Name.Replace("Data", ""));
////				Console.WriteLine("\t\t\t\t" + "MemberName=\"{0}\",", p.Name);
////				Console.WriteLine("\t\t\t\t" + "Metadata = new ListViewModelMetadata {");
////				Console.WriteLine("\t\t\t\t\t" + "DataProvider = new CustomDataProvider<List<{1}>>(() => this.Data.{0}, value => this.Data.{0} = value)", p.Name, p.PropertyType.GetGenericArguments()[0].Name);
////				Console.WriteLine("\t\t\t\t" + "}");
////				Console.WriteLine("\t\t\t" + "});");
//			} else if (p.PropertyType.BaseType == typeof(Enum)) {
//				Console.WriteLine("\t\t\t" + "{0} = RegisterChild(new EnumVM<{1}>() {{", p.Name, p.PropertyType.Name);
//				Console.WriteLine("\t\t\t\t" + "MemberName=\"{0}\",", p.Name);
//				Console.WriteLine("\t\t\t\t" + "Metadata = (ViewModelMetadata)EnumVM<{1}>.CreateDefaultMetadataS(new CustomDataProvider<{1}>(() => this.Data.{0}, value => this.Data.{0} = value)),", p.Name, p.PropertyType.Name);
//				Console.WriteLine("\t\t\t" + "});");
//			} else if (p.PropertyType.BaseType == typeof(Object) && p.PropertyType.Name != "String") {
//				var dataTypeName = p.PropertyType.Name;
//				var vmTypeName=dataTypeName.Replace("Data", "")+"VM";
//				Console.WriteLine("\t\t\t" + "{0} = RegisterChild(new {1}() {{", p.Name, vmTypeName);
//				Console.WriteLine("\t\t\t\t" + "MemberName=\"{0}\",", p.Name);
//				Console.WriteLine("\t\t\t\t" + "Metadata = new ViewModelMetadata {");
//				Console.WriteLine("\t\t\t\t\t" + "DataProvider = new CustomDataProvider<{1}>(() => this.Data.{0}, value => this.Data.{0} = value)", p.Name, dataTypeName);
//				Console.WriteLine("\t\t\t\t" + "}");
//				Console.WriteLine("\t\t\t" + "});");
//			} else {
//				var propertyType = p.PropertyType.Name;
//				var vmPropertyType = propertyType+"VM";
//				if(vmPropertyType=="BooleanVM") vmPropertyType = "BoolVM";
//				Console.WriteLine("\t\t\t" + "{0} = RegisterChild(new {1} {{", p.Name, vmPropertyType);
//				Console.WriteLine("\t\t\t\t" + "MemberName=\"{0}\",", p.Name);
//				Console.WriteLine("\t\t\t\t" + "Metadata = new ViewModelMetadata {");
//				Console.WriteLine("\t\t\t\t\t" + "DataProvider = new CustomDataProvider<{1}>(() => this.Data.{0}, value => this.Data.{0} = value)", p.Name, propertyType);
//				Console.WriteLine("\t\t\t\t" + "}");
//				Console.WriteLine("\t\t\t" + "});");
//			}
//		}

		private static void ConstructorExpression(PropertyInfo p) {
			switch(GetVMType(p)) {
				case VMType.ListVM  : ConstructorExpressionːList(p);break;
				case VMType.EnumVM  : ConstructorExpressionːEnum(p);break;
				case VMType.ValueVM : ConstructorExpressionːValue(p);break;
				case VMType.CustomVM: ConstructorExpressionːCustom(p);break;
			}
		}

		private static void ConstructorExpressionːList(PropertyInfo p) {
			var listType=DebugUtil.FormatTypeName(p.PropertyType,false);
			var propertyType = GetElementType(p).NameT();
			var vmPropertyType = propertyType.Replace("Data", "")+"VM"; if(vmPropertyType=="BooleanVM") vmPropertyType = "BoolVM";
			WriteLine(3, @"
				{0} = RegisterChild(""{0}"",new ListVM<{1}> {{Metadata = new ListViewModelMetadata {{DataProvider = new MappedDataProvider<ThisTData,{2}>(""{0}"")}}}});
				",
					p.Name, 
					vmPropertyType,
					listType
				);
		}

		private static void ConstructorExpressionːEnum(PropertyInfo p) {
			WriteLine(3, @"{0} = RegisterChild(""{0}"", new EnumVM<{1}>{{Metadata = new ViewModelMetadata{{DataProvider = new MappedDataProvider<ThisTData,{1}>(""{0}"")}}}});",
				p.Name,p.PropertyType.Name
			);
		}

		private static void ConstructorExpressionːCustom(PropertyInfo p) {
			var dataTypeName = p.PropertyType.Name;
			var vmTypeName=dataTypeName.Replace("Data", "")+"VM";
			Console.WriteLine("\t\t\t" + "{0} = RegisterChild(new {1}() {{", p.Name, vmTypeName);
			Console.WriteLine("\t\t\t\t" + "MemberName=\"{0}\",", p.Name);
			Console.WriteLine("\t\t\t\t" + "Metadata = new ViewModelMetadata {");
			Console.WriteLine("\t\t\t\t\t" + "DataProvider = new CustomDataProvider<{1}>(() => this.Data.{0}, value => this.Data.{0} = value)", p.Name, dataTypeName);
			Console.WriteLine("\t\t\t\t" + "}");
			Console.WriteLine("\t\t\t" + "});");
		}

		private static void ConstructorExpressionːValue(PropertyInfo p) {
			var propertyType = p.PropertyType.Name;
			var vmPropertyType = propertyType+"VM";
			if(vmPropertyType=="BooleanVM") vmPropertyType = "BoolVM";
			WriteLine(3, @"{0} = RegisterChild(""{0}"", new {1}{{Metadata = new ViewModelMetadata{{DataProvider = new MappedDataProvider<{3},{2}>(""{0}"")}}}});",
				p.Name,vmPropertyType,propertyType,ThisTData
			);
		}

		private static void SetHeader(List<PropertyInfo> missingBusinessProbs) {
			Console.WriteLine("\t\t"+"private void SetHeaders() {");
			Console.WriteLine("\t\t\tEnumVM.UpdateEnums(this);");
			//SR.FlatRoofVM_SampleObject_Header);
			foreach (var p in missingBusinessProbs) {
				SetHeaderExpression(p);
			}
			Console.WriteLine("\t\t\t" + "/* PLACEHOLDER {Next entry} */");
			Console.WriteLine("\t\t" + "}");
		}

		private static void SetHeaderExpression(PropertyInfo p) {
			Console.WriteLine("\t\t\t"+"{0}.PropertyLabel = \"{0}\";",p.Name);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private static void OnBusinessObjectChanged(List<PropertyInfo> missingBusinessProbs) {
			Console.WriteLine("\t\tprivate void OnBusinessObjectChanged(object sender, DataChangedEventArgs e) {");
			foreach (var p in missingBusinessProbs) {
				OnBusinessObjectChangedExpression(p);
			}
			Console.WriteLine("\t\t\t" + "/* PLACEHOLDER {Next entry} */");
			Console.WriteLine("\t\t}");
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private static void OnBusinessObjectChangedExpression(PropertyInfo p) {
			if(p.PropertyType.BaseType.IsGenericType && p.PropertyType.BaseType.GetGenericTypeDefinition()==typeof(ValueBM<>))
				Console.WriteLine("\t\t\t"+"((BusinessValueDataProvider<{0}>) {1}.Metadata.DataProvider).ValueBM = data.{1};	// V",
					p.PropertyType.BaseType.GetGenericArguments()[0].Name,
					p.Name
				);
			else 
				Console.WriteLine("\t\t\t"+"{0}.Metadata.DataProvider .Data = data.{0};	// O",
					p.Name
				);
		}

		private static void Properties(List<PropertyInfo> properties) {
			WriteLine(2,@"#region Properties");
			foreach (var p in properties) {Property(p);}
			WriteLine(2,@"/* PLACEHOLDER {Next entry} */");
			WriteLine(2,@"#endregion");
		}

		private static void Property(PropertyInfo p) {
			switch(GetVMType(p)) {
				case VMType.ListVM  : PropertyːList(p);break;
				case VMType.EnumVM  : PropertyːEnum(p);break;
				case VMType.CustomVM: PropertyːCustom(p);break;
				case VMType.ValueVM : PropertyːValue(p);break;
			}
		}

		private static void PropertyːList(PropertyInfo p) {
			var elementType = GetElementType(p).NameT();
			var vmElementType = elementType.Replace("Data", "")+"VM"; if(vmElementType=="BooleanVM") vmElementType = "BoolVM";
			WriteLine();
			WriteLine(2,@"
				/// <summary> Gets the view model that wraps the <see cref=""{2}.{1}""/> property of <see cref=""{2}""/>.
				/// </summary>
				public ListVM<{0}> {1}{{get;private set;}}
				",
				/*0*/vmElementType,
				/*1*/p.Name,
				/*2*/ThisTData
			);
		}

		private static void PropertyːEnum(PropertyInfo p) {
			WriteLine();
			WriteLine(2, @"
				/// <summary> Gets the view model that wraps the <see cref=""{2}.{1}""/> property of <see cref=""{2}""/>.
				/// </summary>
				public EnumVM<{0}> {1}{{get;private set;}}
				",
				/*0*/p.PropertyType.Name,
				/*1*/p.Name,
				/*2*/ThisTData
			);
		}

		private static void PropertyːCustom(PropertyInfo p) {
			WriteLine();
			WriteLine(2, @"
				/// <summary> Gets the view model that wraps the <see cref=""{2}.{1}""/> property of <see cref=""{2}""/>.
				/// </summary>
				public {0} {1}{{get;private set;}}
				",
				/*0*/p.PropertyType.Name.Replace("Data", "")+"VM",
				/*1*/p.Name,
				/*2*/ThisTData
			);
		}

		private static void PropertyːValue(PropertyInfo p) {
			var propertyType = p.PropertyType.Name;
			var vmPropertyType = propertyType+"VM";
			if(vmPropertyType=="BooleanVM") vmPropertyType = "BoolVM";

			WriteLine(2,@"
				/// <summary> Gets the view model that wraps the <see cref=""{2}.{1}""/> property of <see cref=""{2}""/>.
				/// </summary>
				public {0} {1}{{get;private set;}}
				",
				/*0*/vmPropertyType,
				/*1*/p.Name,
				/*2*/ThisTData
			);
		}

	}

	public class ViewModelCodeGenerator4BusinessObjects:CodeGeneratorBase
	{

		public static void GenarateMissingProperties(Type businessType, Type viewmodelType) {
			var properties=GetProperties(businessType,viewmodelType,true);
			FilterProperties(properties);
			Genarate(properties, businessType, viewmodelType);
		}

		public static void GenarateAllProperties(Type businessType, Type viewmodelType) {
			var properties=GetProperties(businessType,viewmodelType,false);
			FilterProperties(properties);
			Genarate(properties, businessType, viewmodelType);
		}

		private static void FilterProperties(List<PropertyInfo> properties) {
			var excludeNames = new List<string> {"Parent", "Children", "MemberName", "MemberPath", "Metadata", "IsApplicable"};
			for (int i = 0; i < properties.Count; i++) {
				if(excludeNames.Contains(properties[i].Name)) {
					properties.RemoveAt(i);
					i--;
				}
			}
		}

		public static void Genarate(List<PropertyInfo> properties, Type businessType, Type viewmodelType) {
			Constructor(viewmodelType,properties);
//			DataAlias(dataType,businessType);
			SetHeader(properties);
			OnBusinessObjectChanged(properties,businessType);
			Properties(properties);
		}

		public static void Constructor(Type viewmodelType, List<PropertyInfo> properties) {
			WriteLine(2,@"
				public {0}() {{
			",viewmodelType.Name);
			foreach (var p in properties) {
				ConstructorExpression(p);
			}
			WriteLine(2, @"
					/* PLACEHOLDER {{Next entry}} */

					SetHeaders(); AppVM.LanguageChanged += (sender, args) => SetHeaders();
				}}
			",null);
		}

		private static void ConstructorExpression(PropertyInfo p) {
			if(p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition()==typeof(ListBM<>)) {
				//ListBM<>
				var dd = p.PropertyType.GetGenericArguments()[0].Name;
				var vd = (dd.EndsWith("BM")?dd.Substring(0,dd.Length-2):dd) +"VM";if(vd=="BooleanVM")vd="BoolVM";
				Console.WriteLine("\t\t\t"+"{0} = RegisterChild(new ListVM<{1}> {{MemberName=\"{0}\", Metadata = new BusinessListMetadata<BM.{1}>()}});",
					p.Name,
					vd
				);
			} else if(p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition()==typeof(EnumBM<>)) {
				//EnumBM<>
				Console.WriteLine("\t\t\t"+"{0} = RegisterChild(new EnumVM<DM.{1}>{{MemberName=\"{0}\", Metadata = new BusinessValueMetadata <DM.{1}>()}});",
					p.Name,
					p.PropertyType.BaseType.GetGenericArguments()[0].Name
				);
			} else if(p.PropertyType.BaseType.IsGenericType && p.PropertyType.BaseType.GetGenericTypeDefinition()==typeof(ValueBM<>)) {
				//ValueBM<>
				var dd = p.PropertyType.BaseType.GetGenericArguments()[0].Name;
				var vd = dd +"VM"; if(vd=="BooleanVM")vd="BoolVM";
				Console.WriteLine("\t\t\t"+"{0} = RegisterChild(new {1}{{MemberName=\"{0}\", Metadata = new BusinessValueMetadata <{2}>()}});",
					p.Name,
					vd,
					dd
				);
			} else {
				var dd = p.PropertyType.Name;
				var vd = (dd.EndsWith("BM")?dd.Substring(0,dd.Length-2):dd) +"VM"; 
				Console.WriteLine("\t\t\t"+"{0} = RegisterChild(new {1}{{MemberName=\"{0}\", Metadata = new BusinessObjectMetadata<BM.{2}>()}});",
					p.Name,
					vd,
					dd
				);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private static void DataAlias(Type dataType,Type businessType) {
			
			WriteLine(2,@"
				private {0} Data {{
					get{{return ({0}) (({1})Metadata.DataProvider.Data).Metadata.DataProvider.Data;}}
					set{{(({1}) Metadata.DataProvider.Data).Metadata.DataProvider.Data = value;}}
				}}
			",
			 "DM."+dataType.Name,
			 "BM."+businessType.Name
			 );
		}

		public static void SetHeader(List<PropertyInfo> properties) {
			Console.WriteLine("\t\t"+"private void SetHeaders() {");
			Console.WriteLine("\t\t\tEnumVM.UpdateEnums(this);");
			//SR.FlatRoofVM_SampleObject_Header);
			foreach (var p in properties) {
				SetHeaderExpression(p);
			}
			Console.WriteLine("\t\t\t" + "/* PLACEHOLDER {Next entry} */");
			Console.WriteLine("\t\t" + "}");
		}

		public static void SetHeaderExpression(PropertyInfo p) {
			Console.WriteLine("\t\t\t"+"{0}.PropertyLabel = \"{0}\";",p.Name);
		}

		public static void OnBusinessObjectChanged(List<PropertyInfo> properties,Type businessType) {
			Console.WriteLine();
			WriteLine(2, @"
				protected override void OnMetadataChanged() {{
					Metadata.DataProvider.DataChanged+=OnBusinessObjectChanged;
					base.OnMetadataChanged();
				}}
			", "");
			Console.WriteLine();
			Console.WriteLine("\t\tprivate void OnBusinessObjectChanged(object sender, DataChangedEventArgs e) {");
			Console.WriteLine("\t\t\tvar data = (BM.{0})Metadata.DataProvider.Data;", businessType.Name);
			foreach (var p in properties) {
				OnBusinessObjectChangedExpression(p);
			}
			Console.WriteLine("\t\t\t" + "/* PLACEHOLDER {Next entry} */");
			Console.WriteLine("\t\t}");
		}

		public static void OnBusinessObjectChangedExpression(PropertyInfo p) {
			if(p.PropertyType.BaseType.IsGenericType && p.PropertyType.BaseType.GetGenericTypeDefinition()==typeof(ValueBM<>))
				Console.WriteLine("\t\t\t"+"((BusinessValueDataProvider<{0}>) {1}.Metadata.DataProvider).BusinessValue = data.{1};	// V",
					p.PropertyType.BaseType.GetGenericArguments()[0].Name,
					p.Name
				);
			else 
				Console.WriteLine("\t\t\t"+"{0}.Metadata.DataProvider .Data = data.{0};	// O",
					p.Name
				);
		}

		public static void Properties(List<PropertyInfo> missingBusinessProbs) {
			Console.WriteLine("\t\t"+"#region Properties");
			foreach (var p in missingBusinessProbs) {
				Property(p);
			}
			Console.WriteLine("\t\t" + "/* PLACEHOLDER {Next entry} */");
			Console.WriteLine("\t\t" + "#endregion");
		}

		public static void Property(PropertyInfo p) {
			if(p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition()==typeof(ListBM<>)) {
				//ListBM<>
				var dd = p.PropertyType.GetGenericArguments()[0].Name;
				var vd = (dd.EndsWith("BM")?dd.Substring(0,dd.Length-2):dd) +"VM";if(vd=="BooleanVM")vd="BoolVM";
				Console.WriteLine("\t\t"+"public ListVM<{0}> {1}{{get;private set;}}",
					vd,
					p.Name
				);
			} else if(p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition()==typeof(EnumBM<>)) {
				//EnumBM<>
				Console.WriteLine("\t\t"+"public EnumVM<{0}> {1}{{get;private set;}}",
					p.PropertyType.GetGenericArguments()[0].Name,
					p.Name
				);
			} else if(p.PropertyType.BaseType.IsGenericType && p.PropertyType.BaseType.GetGenericTypeDefinition()==typeof(ValueBM<>)) {
				var dd = p.PropertyType.BaseType.GetGenericArguments()[0].Name;
				var vd = p.PropertyType.BaseType.GetGenericArguments()[0].Name+"VM"; if(vd=="BooleanVM")vd="BoolVM";
				Console.WriteLine("\t\t"+"public {0} {1}{{get;private set;}}",
					vd,
					p.Name
				);
			} else {
				var dd = p.PropertyType.Name;
				var vd = (dd.EndsWith("BM")?dd.Substring(0,dd.Length-2):dd) +"VM"; 
				Console.WriteLine("\t\t"+"public {0} {1}{{get;private set;}}",
					vd,
					p.Name
				);
			}
		}

	}

	/// <summary> Code generator to generate business classes from data classes
	/// </summary>
	public class BusinessCodeGenerator4DataObjects:CodeGeneratorBase
	{
//		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]//In use if code missing
//		private static void ClassHelper_ListDataProperties() { BusinessCodeGenerator4DataObjects.GetMissingProperties(typeof(GeoInfoData), typeof(GeoInfo)); }
//
//		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]//In use if code missing
//		private static void ClassHelper_ListAllProperties() { BusinessCodeGenerator4DataObjects.GetAllDataProperties(typeof(GeoInfoData), typeof(GeoInfo)); }

//		private static void GenerateModel() {BusinessCodeGenerator4DataObjects.GenerateMissingProperties(typeof(ColorValueData),typeof(ColorValueVM));}

		public static void GenarateMissingProperties(Type dataType, Type businessType) {
			var missingBusinessProbs=GetMissingProperties(dataType,businessType);

			string businessName = businessType.Name;

			WriteLine("### BEGIN CODE ###\r\n");
			Console.WriteLine("");

			Console.WriteLine("\t\t/// <summary> Alias for Metadata.DataProvider.Data");
			Console.WriteLine("\t\t/// </summary>");
			Console.WriteLine("\t\tinternal DM.{0} Data {{get {{ return (DM.{0})Metadata.DataProvider.Data; }} }} ", dataType.Name);
			Console.WriteLine("");

//			Console.WriteLine("\t\t/// <summary>");
//			Console.WriteLine("\t\t/// Current undo mangager from the current root object project");
//			Console.WriteLine("\t\t/// </summary>");
//			Console.WriteLine("\t\tpublic UndoManager UndoManager {{ get {{ return (({0})BusinessObjectTreeHelper.GetRoot(this)).UndoManager; }} }}", businessType.Name);
//			Console.WriteLine("");

			BusinessCodeGenerator4DataObjects.Constructor(missingBusinessProbs, businessName);
			//BusinessCodeGenerator4DataObjects.OnListDataChanged(properties);
			BusinessCodeGenerator4DataObjects.Properties(missingBusinessProbs, dataType);
		}

		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public static List<PropertyInfo> GetMissingProperties(Type dataType, Type businessType) {
			var dataProps = dataType.GetProperties();
			var businessProps = businessType.GetProperties();
			var missingDataProps = new List<PropertyInfo>();

			Console.WriteLine("Properties to implement in business layer:");
			foreach (var dataProp in dataProps) {
				PropertyInfo prop = dataProp;
				if (businessProps.Any(x => x.Name == prop.Name))
					continue;
				missingDataProps.Add(dataProp);
				Console.WriteLine("{0,-32} {1}", dataProp.Name, dataProp.PropertyType.Name);
			}
			
			Console.WriteLine("Properties to control in business layer:"); //TODO: what do this mean?
			foreach (var businessProp in businessProps) {
				if (dataProps.Any(x => x.Name == businessProp.Name)) continue;
				if (businessProp.PropertyType.BaseType != null && businessProp.PropertyType.BaseType.BaseType != null && businessProp.PropertyType.BaseType.BaseType.Name == "BusinessObject" ||
					businessProp.PropertyType.BaseType != null && businessProp.PropertyType.BaseType.Name == "BusinessObject")
					Console.WriteLine("{0,-32} {1}", businessProp.Name, businessProp.PropertyType.Name);
			}

			return missingDataProps;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "dataName"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "businessName"), SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public static List<PropertyInfo> GetAllDataProperties(Type dataType, MemberInfo businessType) {
			var dataProps = dataType.GetProperties();
			var missingDataProps = new List<PropertyInfo>();
			string dataName = dataType.Name;
			string businessName = businessType.Name;
			Console.WriteLine("All Properties:");
			foreach (var dataProp in dataProps) {
				missingDataProps.Add(dataProp);
				Console.WriteLine("{0,-32} {1}", dataProp.Name, dataProp.PropertyType.Name);
			}

			return missingDataProps;
		}

		private static void Constructor(List<PropertyInfo> missingDataProbs, string businessName) {
			Console.WriteLine("\t\t" + "public {0}() {{", businessName);
			foreach (var p in missingDataProbs) {
				ConstructorExpression(p);
			}
			Console.WriteLine();
			Console.WriteLine("\t\t\t" + "/* PLACEHOLDER {Next entry} */");
			Console.WriteLine("\t\t" + "}");
		}

		public static void ConstructorExpression(PropertyInfo p) {
			if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
				var propertyName      = p.Name;
				var listTypeParamName = p.PropertyType.GetGenericArguments()[0].Name.Replace("Data", "") + "BM";
				var dataListParamName = "DM."+p.PropertyType.GetGenericArguments()[0].Name;
				
				Console.WriteLine("\t\t\t" + "{0} = RegisterChild(new ListBM<{1}> {{", propertyName, listTypeParamName);
				Console.WriteLine("\t\t\t\t" + "MemberName=\"{0}\",", propertyName);
				Console.WriteLine("\t\t\t\t" + "Metadata = new BusinessListMetadata {");
				Console.WriteLine("\t\t\t\t\t" + "DataProvider = new CustomDataProvider<List<{1}>>(() => this.Data.{0}, value => this.Data.{0} = value)", propertyName, dataListParamName);
				Console.WriteLine("\t\t\t\t" + "}");
				Console.WriteLine("\t\t\t" + "});");
			}
			else if (p.PropertyType.BaseType == typeof(Enum)) {
				//EnumBM<> Console.WriteLine("EnumBM: " + p.Name);
				Console.WriteLine("\t\t\t" + "{0} = RegisterChild(new EnumBM<{1}>() {{", p.Name, p.PropertyType.Name);
				Console.WriteLine("\t\t\t\t" + "MemberName=\"{0}\",", p.Name);
				Console.WriteLine("\t\t\t\t" + "Metadata = (BusinessValueMetadata)EnumBM<{1}>.CreateDefaultMetadataS(new CustomDataProvider<{1}>(() => this.Data.{0}, value => this.Data.{0} = value)),", p.Name, p.PropertyType.Name);
				Console.WriteLine("\t\t\t" + "});");
			}
			else if (p.PropertyType.BaseType == typeof(Object) && p.PropertyType.Name != "String") {
				//BusinessObject<> Console.WriteLine("BusinessObject: " + p.Name);
				Console.WriteLine("\t\t\t" + "{0} = RegisterChild(new {1}() {{", p.Name, p.PropertyType.Name.Replace("Data", "")+"BM");
				Console.WriteLine("\t\t\t\t" + "MemberName=\"{0}\",", p.Name);
				Console.WriteLine("\t\t\t\t" + "Metadata = new BusinessObjectMetadata {");
				Console.WriteLine("\t\t\t\t\t" + "DataProvider = new CustomDataProvider<DM.{1}>(() => this.Data.{0}, value => this.Data.{0} = value)", p.Name, p.PropertyType.Name);
				Console.WriteLine("\t\t\t\t" + "}");
				Console.WriteLine("\t\t\t" + "});");
			}
			else {
				//ValueBM<> Console.WriteLine("ValueBM: " + p.Name);
				Console.WriteLine("\t\t\t" + "{0} = RegisterChild(new {1}BM() {{", p.Name, p.PropertyType.Name);
				Console.WriteLine("\t\t\t\t" + "MemberName=\"{0}\",", p.Name);
				Console.WriteLine("\t\t\t\t" + "Metadata = new BusinessValueMetadata {");
				Console.WriteLine("\t\t\t\t\t" + "Settings = new ValueSettings<{0}> {{ AllowNull = true }},", p.PropertyType.Name);
				Console.WriteLine("\t\t\t\t\t" + "DataProvider = new CustomDataProvider<{1}>(() => this.Data.{0}, value => this.Data.{0} = value)", p.Name, p.PropertyType.Name);
				Console.WriteLine("\t\t\t\t" + "}");
				Console.WriteLine("\t\t\t" + "});");
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public static void OnListDataChanged(List<PropertyInfo> missingDataProbs) {
			bool hasList = false;
			foreach (var p in missingDataProbs) {
				if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
					hasList = true;break;
				}
			}
			if(!hasList) return;
			Console.WriteLine("");
//			Console.WriteLine("\t\t/// <summary>");
//			Console.WriteLine("\t\t/// Trigger to set the DataChangeCallback");
//			Console.WriteLine("\t\t/// </summary>");
			Console.WriteLine("\t\tprotected override void OnMetadataChanged() {");
			Console.WriteLine("\t\t\tbase.OnMetadataChanged();");
			Console.WriteLine("\t\t\tMetadata.DataProvider.DataChanged += AtDataChanged;");
			Console.WriteLine("\t\t}");
			Console.WriteLine("");

			Console.WriteLine("\t\tprivate void AtDataChanged(object sender, DataChangedEventArgs e) {");
			Console.WriteLine("\t\t\tusing (BusinessObjectTreeHelper.StopTreeChangedEvents()) {");

			foreach (var p in missingDataProbs) {
				OnListDataChangedExpression(p);
			}
			Console.WriteLine("\t\t\t}");
			Console.WriteLine("\t\t}");
			Console.WriteLine("");
			Console.WriteLine("\t\t" + "/* PLACEHOLDER {Next entry} */");
		}

		public static void OnListDataChangedExpression(PropertyInfo p) {
			if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
				Console.WriteLine("\t\t\t\t" + "{0}.Metadata.DataProvider.NotifyDataChanged();", p.Name);
			}

		}

		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public static void Properties(List<PropertyInfo> missingDataProbs, Type dataType) {
			foreach (var p in missingDataProbs) {
				Property(p,dataType);
			}
			Console.WriteLine("\t\t" + "/* PLACEHOLDER {Next entry} */");
		}

		public static void Property(PropertyInfo p, Type dataType) {
			Console.WriteLine("");
			Console.WriteLine("\t\t/// <summary> Wrapper for the corresponding DataModel property <see cref=\"DM.{0}.{1}\"/>", dataType.Name, p.Name);
			Console.WriteLine("\t\t/// </summary>");
			if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
				//ListBM<>
				Console.WriteLine("\t\t" + "public ListBM<{0}> {1}{{get;private set;}}",
					p.PropertyType.GetGenericArguments()[0].Name.Replace("Data", "")+"BM",
					p.Name
				);
			}
			else if (p.PropertyType.BaseType == typeof(Enum)) {
				//EnumBM<>
				Console.WriteLine("\t\t" + "public EnumBM<{0}> {1}{{get;private set;}}",
					p.PropertyType.Name,
					p.Name
				);
			}
			else if (p.PropertyType.BaseType == typeof(Object) && p.PropertyType.Name != "String") {
				//BusinessObject<>	
				Console.WriteLine("\t\t" + "public {0} {1}{{get;private set;}}",
					p.PropertyType.Name.Replace("Data", "")+"BM",
					p.Name
				);
			}
			else {
				//ValueBM<>
				Console.WriteLine("\t\t" + "public {0}BM {1}{{get;private set;}}",
					p.PropertyType.Name,
					p.Name
				);
			}
		}
	}
	// ReSharper enable LocalizableElement

	
}

/* 
 * ː
*/