using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace KsWare.Presentation.DataVirtualization {

	public abstract class DataRefBase<T> : ICustomTypeDescriptor, INotifyPropertyChanged where T : class {

		private static readonly IDictionary<PropertyDescriptor, PropertyDescriptor> _PropertyMap;
		internal static readonly PropertyDescriptorCollection PropertyDescriptorCollection;

		static DataRefBase() {
			PropertyDescriptorCollection = new PropertyDescriptorCollection(null);
			var propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof (T));
			_PropertyMap = new Dictionary<PropertyDescriptor, PropertyDescriptor>(propertyDescriptorCollection.Count);
			foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection) {
				var mappedPropertyDescriptor = new DataRefPropertyDescriptor(propertyDescriptor);
				_PropertyMap.Add(propertyDescriptor, mappedPropertyDescriptor);
				PropertyDescriptorCollection.Add(mappedPropertyDescriptor);
			}
			// create an artificial read-only property for the referenced instance
			//var instancePropertyDescriptor = TypeDescriptor.CreateProperty(typeof(DataRefBase<T>), "__DATA__", typeof(T));
			//var mappedInstancePropertyDescriptor = new InstancePropertyDescriptor(instancePropertyDescriptor);
			//m_PropertyMap.Add(instancePropertyDescriptor, mappedInstancePropertyDescriptor);
			//PropertyDescriptorCollection.Add(mappedInstancePropertyDescriptor);
		}


		public abstract T Data { get; }

		private void SetValue(DataRefPropertyDescriptor propertyDescriptor, object value) {
			var data = Data;
			if (data != null) {
				propertyDescriptor.SetValue(data, value);
				NotifyPropertyChanged(propertyDescriptor);
			}
		}

		private object GetValue(PropertyDescriptor propertyDescriptor) {
			var data = Data;
			if (data != null) return propertyDescriptor.GetValue(data);
			else return null;
		}

		private void NotifyPropertyChanged(DataRefPropertyDescriptor propertyDescriptor) { if (PropertyChanged != null) PropertyChanged(this, propertyDescriptor.PropertyChangedEventArgs); }

		protected void NotifyAllPropertiesChanged() { foreach (DataRefPropertyDescriptor propertyDescriptor in PropertyDescriptorCollection) NotifyPropertyChanged(propertyDescriptor); }

		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(typeof (T)); }

		public string GetClassName() { return TypeDescriptor.GetClassName(typeof (T)); }

		public string GetComponentName() { return TypeDescriptor.GetComponentName(this); }

		public System.ComponentModel.TypeConverter GetConverter() { return TypeDescriptor.GetConverter(typeof (T)); }

		public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(typeof (T)); }

		public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(typeof (T)); }

		public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(typeof (T), editorBaseType); }

		public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(typeof (T), attributes); }

		public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(typeof (T)); }

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			var mappedCollection = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(typeof (T), attributes)) {
				Debug.Assert(_PropertyMap.ContainsKey(propertyDescriptor));
				mappedCollection.Add(_PropertyMap[propertyDescriptor]);
			}
			return mappedCollection;
		}


		public PropertyDescriptorCollection GetProperties() { return PropertyDescriptorCollection; }

		public object GetPropertyOwner(PropertyDescriptor pd) { return this; }

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private class DataRefPropertyDescriptor : PropertyDescriptor {

			private readonly PropertyDescriptor _PropertyDescriptor;
			public readonly PropertyChangedEventArgs PropertyChangedEventArgs;

			public DataRefPropertyDescriptor(PropertyDescriptor propertyDescriptor)
				: base(propertyDescriptor) {
				_PropertyDescriptor = propertyDescriptor;
				PropertyChangedEventArgs = new PropertyChangedEventArgs(propertyDescriptor.Name);
			}


			public override bool CanResetValue(object component) { return false; }

			public override Type ComponentType => typeof (DataRefBase<T>);

			public override object GetValue(object component) { return ((DataRefBase<T>) component).GetValue(_PropertyDescriptor); }

			public override bool IsReadOnly => _PropertyDescriptor.IsReadOnly;

			public override Type PropertyType => _PropertyDescriptor.PropertyType;

			public override void ResetValue(object component) { throw new NotImplementedException(); }

			public override void SetValue(object component, object value) { ((DataRefBase<T>) component).SetValue(this, value); }

			public override bool ShouldSerializeValue(object component) { return _PropertyDescriptor.ShouldSerializeValue(component); }

		}

		private class InstancePropertyDescriptor : DataRefPropertyDescriptor {

			public InstancePropertyDescriptor(PropertyDescriptor propertyDescriptor)
				: base(propertyDescriptor) { }

			public override bool CanResetValue(object component) { return false; }

			public override object GetValue(object component) { return ((DataRefBase<T>) component).Data; }

			public override Type PropertyType => typeof (T);

			public override void ResetValue(object component) { throw new NotImplementedException(); }

			public override void SetValue(object component, object value) { throw new NotImplementedException(); }

			public override bool ShouldSerializeValue(object component) { return false; }

		}

	}

}