/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\ 
 * Author: ks@ksware.de
 * 
 * OriginalFileName : ViewModelProperty.cs
 * OriginalNamespace: KsWare.Presentation.ViewModelFramework
\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Reflection;
using System.Windows;
using JetBrains.Annotations;

namespace KsWare.Presentation.ViewModelFramework {

	/// <summary> [DRAFT] Represents a view model property that is registered with the ViewModel (VM) property system. 
	/// View model properties provide support for value expressions, property invalidation and dependent-value coercion, default values, inheritance, data binding, animation, property change notification, and styling.
	/// </summary>
	/// <remarks>This class imitats and extends the <see cref="DependencyProperty"/> behavior</remarks>
	public abstract class ViewModelProperty {

		private static int __lastGlobalIndex;

		/// <summary> Registers a view model property.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="ownerType">Type of the owner.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static ViewModelProperty Register(string name, Type propertyType, Type ownerType) { return Register(name, propertyType, ownerType, null, null); }

		/// <summary> Registers a view model property.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="typeMetadata">The type metadata.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static ViewModelProperty Register(string name, Type propertyType, Type ownerType, ViewModelMetadata typeMetadata){ return Register(name, propertyType, ownerType, typeMetadata, null); }

		/// <summary> Registers a view model property.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="typeMetadata">The type metadata.</param>
		/// <param name="validateValueCallback">The validate value callback.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static ViewModelProperty Register(string name, Type propertyType, Type ownerType, ViewModelMetadata typeMetadata, ValidateValueCallback validateValueCallback) {
			var property = new ModelProperty(name, propertyType, ownerType, typeMetadata, validateValueCallback,false);
			PropertyCache.Register(property);
			return property;
		}

		/// <summary> Registers a read-only view model property.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="typeMetadata">The type metadata.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static ViewModelPropertyKey RegisterReadOnly(string name, Type propertyType, Type ownerType, ViewModelMetadata typeMetadata) { return RegisterReadOnly(name, propertyType, ownerType, typeMetadata, null); }

		/// <summary> Registers a read-only view model property.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="typeMetadata">The type metadata.</param>
		/// <param name="validateValueCallback">The validate value callback.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static ViewModelPropertyKey RegisterReadOnly(string name, Type propertyType, Type ownerType, ViewModelMetadata typeMetadata, ValidateValueCallback validateValueCallback) {
			var property = new ModelProperty(name, propertyType, ownerType, typeMetadata, validateValueCallback,true);
			var key = new ViewModelPropertyKey(property);
			PropertyCache.Register(property);
			return key;
		}

		private readonly string _Name;
		private readonly Type _PropertyType;
		private readonly Type _OwnerType;
		private readonly ValidateValueCallback _ValidateValueCallback;
		private readonly ViewModelMetadata _DefaultMetadata;
		private readonly bool _ReadOnly;
		private readonly int _GlobalIndex=++__lastGlobalIndex;

		/// <summary> Initializes a new instance of the <see cref="ViewModelProperty"/> class.
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="typeMetadata">The type metadata.</param>
		/// <param name="validateValueCallback">The validate value callback.</param>
		/// <param name="readOnly">if set to <c>true</c> [read only].</param>
		/// <remarks></remarks>
		protected ViewModelProperty (string name, Type propertyType, Type ownerType, ViewModelMetadata typeMetadata, ValidateValueCallback validateValueCallback, bool readOnly) {
			_Name                  = name                 ;
			_PropertyType          = propertyType         ;
			_OwnerType             = ownerType            ;
			_DefaultMetadata       = typeMetadata         ;
			_ValidateValueCallback = validateValueCallback;
			_ReadOnly              = readOnly             ;
		}

		/// <summary> Gets the name of the dependency property. 
		/// </summary>
		/// <value>The name of the property.</value>
		public string Name {get {return this._Name;}}

		/// <summary> Gets the type that the dependency property uses for its value.
		/// </summary>
		/// <value>The <see cref="Type"/> of the property value.</value>
		public Type PropertyType {get {return this._PropertyType;}}

		/// <summary> Gets the type of the object that registered the property with the property system, or added itself as owner of the property. 
		/// </summary>
		/// <value>The type of the object that registered the property or added itself as owner of the property.</value>
		public Type OwnerType {get {return this._OwnerType;}}

		/// <summary> Gets the value validation callback for the dependency property.
		/// </summary>
		/// <value>The value validation callback for this property, as provided for the validateValueCallback parameter in the original property registration.</value>
		public ValidateValueCallback ValidateValueCallback {get {return this._ValidateValueCallback;}}

		/// <summary> Gets a value that indicates whether the property identified by this <see cref="ViewModelProperty"/> instance is a read-only property.
		/// </summary>
		/// <value>true if the dependency property is read-only; otherwise, false.</value>
		public bool ReadOnly { get {return _ReadOnly;} }

		/// <summary> Gets an internally generated value that uniquely identifies the property.
		/// </summary>
		/// <value>A unique numeric identifier.</value>
		public int GlobalIndex { get {return _GlobalIndex;} }

		/// <summary> Gets the default metadata of the property. 
		/// </summary>
		/// <value>The default metadata of the property.</value>
		public ViewModelMetadata DefaultMetadata {get {return _DefaultMetadata;}}

		/// <summary> [NOT IMPLEMENTED] Adds the owner for this property
		/// </summary>
		/// <param name="ownerType">Type of the owner.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "ownerType")]
		public ViewModelProperty AddOwner(Type ownerType) {
			throw new NotImplementedException("{6DD703EC-CD06-42CB-B822-A7D3112A9166}");
		}

		/// <summary> [NOT IMPLEMENTED] Adds the owner for this property
		/// </summary>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="typeMetadata">The type metadata.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "typeMetadata"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "ownerType")]
		public ViewModelProperty AddOwner(Type ownerType, ViewModelMetadata typeMetadata) {
			throw new NotImplementedException("{035AB271-7899-4FB0-B3BD-AA99425E7995}");
		}

		/// <summary> [NOT IMPLEMENTED] Gets the metadata.
		/// </summary>
		/// <param name="forType">For type.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "forType"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public ViewModelMetadata GetMetadata(Type forType) {
			throw new NotImplementedException("{EC57D178-7682-4C33-ADB9-5FB0F006E695}");
		}

		/// <summary> [NOT IMPLEMENTED] Gets the metadata.
		/// </summary>
		/// <param name="viewModelObject">The view model object.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "viewModelObject")]
		public ViewModelMetadata GetMetadata(ObjectVM viewModelObject) {
			throw new NotImplementedException("{68B5216F-B46B-42BC-B3F8-A9A75F49411B}");
		}


		/// <summary> [DRAFT] 
		/// </summary>
		internal class ModelProperty:ViewModelProperty {
			internal ModelProperty(string name, Type propertyType, Type ownerType, ViewModelMetadata typeMetadata, ValidateValueCallback validateValueCallback, bool readOnly): 
				base(name, propertyType, ownerType, typeMetadata, validateValueCallback, readOnly) {}
		}

		/// <summary> [DRAFT] 
		/// </summary>
		internal sealed class RuntimeProperty: ViewModelProperty {
			#region static helper

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "ownerType")]
			private static string GetPropertyName(string name, Type ownerType) {
				if(name=="Item[]") return "Item";
				if(name.Contains("[")) throw new NotImplementedException("{CE0B9A55-BC62-4AB0-A833-50E6E62AEEE5}");
				return name;
			}

			private static PropertyInfo GetProperty([NotNull] string name, [NotNull] Type ownerType) {
				#region parameter verification
				if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
				if(ownerType==null) throw new ArgumentNullException(nameof(ownerType));
				#endregion

				var n=GetPropertyName(name, ownerType);
				var property = (PropertyInfo) null;
				try{property= ownerType.GetProperty(n);} catch(AmbiguousMatchException){}
				var ownerType1 = ownerType;
				while (property==null && ownerType1!=null) {
					property = ownerType1.GetProperty(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
					if (property == null) ownerType1 = ownerType1.BaseType;
				}
				//if(property==null) throw new MissingMemberException("Property not found or property not public! \""+n+"\" in "+ownerType.NameT());
				return property;
			}

			private static bool GetReadOnly(string name, Type ownerType) { 
				var property = GetProperty(name, ownerType);
				return property.GetSetMethod(false) != null;
			}

			private static Type GetPropertyType([NotNull] string name, [NotNull] Type ownerType) {
				var property = GetProperty(name, ownerType);
				if(property==null) throw new MissingMemberException("Property not found or property not public! \""+name+"\" in "+ownerType.NameT());
				return property.PropertyType;
			}
			#endregion

			readonly PropertyInfo _PropertyInfo;

			/// <summary> Initializes a new instance of the <see cref="ViewModelProperty.RuntimeProperty"/> class.
			/// </summary>
			/// <param name="name">The String containing the name of the property.</param>
			/// <param name="propertyType">Type of the property.</param>
			/// <param name="ownerType">The owner type that is registering the property.</param>
			/// <param name="typeMetadata">Property metadata for the property.</param>
			/// <param name="validateValueCallback">A reference to a callback that should perform any custom validation of the dependency property value beyond typical type validation.</param>
			/// <param name="readOnly"></param>
			internal RuntimeProperty(string name, Type propertyType, Type ownerType, ViewModelMetadata typeMetadata, ValidateValueCallback validateValueCallback, bool readOnly): 
				base(name, propertyType, ownerType, typeMetadata, validateValueCallback, readOnly) {
				_PropertyInfo = GetProperty(name,ownerType);
			}

			/// <summary> Initializes a new instance of the <see cref="ViewModelProperty.RuntimeProperty"/> class.
			/// </summary>
			/// <param name="name">The String containing the name of the property.</param>
			/// <param name="ownerType">The owner type that is registering the property.</param>
			internal RuntimeProperty(string name, Type ownerType)
				: this(GetPropertyName(name,ownerType), GetPropertyType(name,ownerType), ownerType, null, null, GetReadOnly(name,ownerType)) {
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			public PropertyInfo PropertyInfo {
				get {return _PropertyInfo;}
			}
		}

	}


	/// <summary> [DRAFT] Provides a view model property identifier for limited write access to a read-only view model property. )
	/// </summary>
	/// <remarks>This class imitats the <see cref="DependencyPropertyKey"/> behavior for view model</remarks>
	public class ViewModelPropertyKey {
		private readonly ViewModelProperty viewModelProperty;

		/// <summary> Initializes a new instance of the <see cref="ViewModelPropertyKey"/> class.
		/// </summary>
		/// <param name="viewModelProperty">The view model property.</param>
		/// <remarks></remarks>
		public ViewModelPropertyKey(ViewModelProperty viewModelProperty) {
			this.viewModelProperty = viewModelProperty;
		}

		/// <summary> Gets the view model property identifier associated with this specialized read-only view model property identifier.
		/// </summary>
		/// <remarks></remarks>
		public ViewModelProperty ViewModelProperty {
			get {return viewModelProperty;}
		}
	}
}
