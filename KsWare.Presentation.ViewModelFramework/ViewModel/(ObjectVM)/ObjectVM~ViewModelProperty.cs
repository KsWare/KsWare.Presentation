using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace KsWare.Presentation.ViewModelFramework 
{
	//
	// Implements "ViewModelProperty" functionality for ObjectVM
	// 

	public partial class ObjectVM 
	{
		private readonly List<Tuple<ViewModelProperty,EventHandler>> m_PropertyChangedEventHandlers=new List<Tuple<ViewModelProperty, EventHandler>>();
		private readonly List<Tuple<string,EventHandler>> _propertyChangedEventHandlers2=new List<Tuple<string, EventHandler>>();
		private readonly Dictionary<object,object> _propertyValues=new Dictionary<object, object>();

		/// <summary> Returns the current effective value of a dependency property on this instance of a System.Windows.DependencyObject. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewModelProperty">The <see cref="ViewModelProperty"/> identifier of the property to retrieve the value for.</param>
		/// <returns>Returns the current effective value.</returns>
		public T GetValue<T>(ViewModelProperty viewModelProperty) {
			if(!_propertyValues.ContainsKey(viewModelProperty)) {
				return default(T);
			} else {
				var value = _propertyValues[viewModelProperty];
				return (T) value;
			}
		}

		/// <summary> Sets the local value of a dependency property, specified by its dependency property identifier. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewModelProperty">The identifier of the view model property to set.</param>
		/// <param name="value">The new local value.</param>
		public void SetValue<T>(ViewModelProperty viewModelProperty, T value) {
			if(viewModelProperty.ReadOnly) 
				throw new InvalidOperationException("Property is read-only!");
			var prevValue = GetValue<T>(viewModelProperty);
			if(_propertyValues.ContainsKey(viewModelProperty)) {
				_propertyValues[viewModelProperty]=value;
			} else {
				_propertyValues.Add(viewModelProperty, value);
			}
			if(!Equals(prevValue, value)) 
				OnPropertyChanged(viewModelProperty);
		}

		/// <summary> Sets the local value of a read-only dependency property, specified by the System.Windows.DependencyPropertyKey identifier of the dependency property. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The <see cref="ViewModelPropertyKey"/> identifier of the property to set.</param>
		/// <param name="value"> The new local value.</param>
		public void SetValue<T>(ViewModelPropertyKey key, T value) {
			var prevValue=GetValue<T>(key.ViewModelProperty);
			if(_propertyValues.ContainsKey(key)) {
				_propertyValues[key]=value;
			} else {
				_propertyValues.Add(key, value);
			}
			if(!Equals(prevValue, value)) {
				OnPropertyChanged(key.ViewModelProperty);
			}
		}
		
		/// <summary> Registers a property changed handler.
		/// </summary>
		/// <param name="viewModelProperty">The view model property.</param>
		/// <param name="propertyChangedEventHandler">The property changed event handler.</param>
		public void RegisterPropertyChangedHandler(ViewModelProperty viewModelProperty, EventHandler propertyChangedEventHandler) {
			if(m_PropertyChangedEventHandlers.Any(tuple => tuple.Item1 == viewModelProperty && tuple.Item2 == propertyChangedEventHandler))
				throw new InvalidOperationException("Eventhandler already registered!"+
				"\r\n\tEventHandler: "+propertyChangedEventHandler.Method.GetFullSignatur());
			m_PropertyChangedEventHandlers.Add(new Tuple<ViewModelProperty, EventHandler>(viewModelProperty, propertyChangedEventHandler));
		}


		/// <summary> Releases a property changed handler.
		/// </summary>
		/// <param name="viewModelProperty">The view model property.</param>
		/// <param name="propertyChangedEventHandler">The property changed event handler.</param>
		public void ReleasePropertyChangedHandler(ViewModelProperty viewModelProperty, EventHandler propertyChangedEventHandler) {
			m_PropertyChangedEventHandlers.RemoveAll(tuple => tuple.Item1 == viewModelProperty && tuple.Item2 == propertyChangedEventHandler);
		}


		public void RegisterPropertyChangedHandler<TProperty>(Expression<Func<object, TProperty>> viewModelProperty, EventHandler propertyChangedEventHandler) {
			string name = MemberNameUtil.GetPropertyName(viewModelProperty);
			var prop = PropertyCache.GetProperty(name, this.GetType(), autoRegister: true);
			RegisterPropertyChangedHandler(prop, propertyChangedEventHandler);
		}

		public void ReleasePropertyChangedHandler<TProperty>(Expression<Func<object, TProperty>> viewModelProperty, EventHandler propertyChangedEventHandler) {
			string name = MemberNameUtil.GetPropertyName(viewModelProperty);
			var prop = PropertyCache.GetProperty(name, this.GetType(), autoRegister: false);
			if(prop==null) throw new InvalidOperationException("Property not registered! Name:"+name+". ErrorID:{EC7E3C09-2ED6-4D44-B6F2-4444C4DF7CB3}");
			ReleasePropertyChangedHandler(prop, propertyChangedEventHandler);
		}

	}
}
