using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	public sealed partial class BackingFieldsStore:IDisposable {

		// TODO Binding to a Field is not working if the field does not yet exist because _fields Dictionary is not observable.

		private static readonly Dictionary<Type,Dictionary<string, Type>> TypesDictionary = new Dictionary<Type,Dictionary<string, Type>>();

		private int _isDisposed;
		private object _owner;
		private Action<string> _propertyChangedCallback;
		private Dictionary<string, BackingFieldInfo> _fields = new Dictionary<string, BackingFieldInfo>();
		private bool _readOnlyCollection = false;

		public BackingFieldsStore([NotNull] object owner, Action<string> propertyChangedCallback) {
			_owner = owner ?? throw new ArgumentNullException(nameof(owner));

			_propertyChangedCallback = propertyChangedCallback;

			var ownerType = _owner.GetType();

			if (!TypesDictionary.ContainsKey(owner.GetType())) {
				var propertyInfos = _owner.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
				var typeDic = new Dictionary<string, Type>();
				foreach (var propertyInfo in propertyInfos) {
					if(propertyInfo.GetIndexParameters().Length>0) continue; //exclude indexer
					if(GetLazyAttribute(propertyInfo)!=null) continue; //exclude lazy

					var type = propertyInfo.PropertyType;
					var name = propertyInfo.Name;

					if (typeDic.ContainsKey(name)) continue;
					typeDic.Add(name,type);
					
				}
				TypesDictionary.Add(ownerType,typeDic);
			}
			{
				var typeDic = TypesDictionary[ownerType];
				foreach (var entry in typeDic) {
					var name = entry.Key;
					var type = entry.Value;
					var value = type.IsValueType ? Activator.CreateInstance(type) : null;
					var field = new BackingFieldInfo(name, type, value, this);
					_fields.Add(name, field);
				}
			}
		}

		/// <summary>
		/// Gets the owner of this instance.
		/// </summary>
		/// <value>The owner.</value>
		/// <remarks>If used in KsWare.PresentationFramework is it the ObjectVM or ObjectBM.</remarks>
		public object Owner => _owner;

		[Obsolete("Use overload.",true)]
		public IBackingFieldInfo this[Expression<Func<object, object>> memberExpression] => _fields[MemberNameUtil.GetPropertyName(memberExpression)];

		[Obsolete("Use overload.", true)]
		public IBackingFieldInfo this[Expression<Func<object>> memberExpression] => _fields[MemberNameUtil.GetPropertyName(memberExpression)];

		/// <summary>
		/// Gets the <see cref="BackingFieldInfo"/> with the specified name.
		/// </summary>
		/// <param name="fieldName">The name of the field</param>
		/// <returns>The <see cref="BackingFieldInfo"/>.</returns>
		/// <example>
		/// Gets the MyBool field. 
		/// <code language="C#">
		/// Fields[nameof(MyBool)]
		/// </code>
		/// </example>
		public IBackingFieldInfo this[string fieldName] => _fields[fieldName];

		/// <summary>
		/// Adds a field with the specified name.
		/// </summary>
		/// <typeparam name="T">The type of the field.</typeparam>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="value">The value.</param>
		public void Add<T>(string fieldName, T value=default(T)) { AddCore<T>(fieldName); }

		[Obsolete("Use overload.", true)]
		public void Add<T>(Expression<Func<object, T>> memberExpression, T value=default(T)) {
			Add(MemberNameUtil.GetPropertyName(memberExpression),value);
		}

		[Obsolete("Use overload.", true)]
		public void Add<T>(Expression<Func<T>> memberExpression, T value=default(T)) {
			Add(MemberNameUtil.GetPropertyName(memberExpression),value);
		}

		private void AddCore<T>(string name, T value = default(T)) {
			if(_readOnlyCollection) throw new InvalidOperationException("The BackingFieldStore is read-only!");
			if(_fields.ContainsKey(name)) throw new InvalidOperationException("The BackingFieldStore already contains a item with name'"+name+"'!");
			var field = new BackingFieldInfo(name, typeof(T), value, this);
			_fields.Add(name, field);
			OnPropertyChanged(name,default(T),value);
		}

		[Obsolete("Use SetValue", true)]
		public void Set<T>(string name, T value) { SetValueInternal(name,value); }


		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <typeparam name="T">The type of the field</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="name">The field name.</param>
		public void SetValue<T>(T value, [CallerMemberName] string name=null) { SetValueInternal(name,value); }

//		Search:		Fields\.Set\((\(\)|_)\s*=>\s*(\w+),
//		Replace:	Fields.Set("$2",

//		public void Set<T>(Expression<Func<object, T>> memberExpression, T value) { SetInternal(MemberNameUtil.GetPropertyName(memberExpression),value);}
//		public void Set<T>(Expression<Func<T>> memberExpression, T value) { SetInternal(MemberNameUtil.GetPropertyName(memberExpression),value);}

		public void SetAndRaise<T>(string propertyName, T value, Action<T> changedCallback) {
			if (SetValueInternal(propertyName, value)) 
				changedCallback(value);
		}

//		public void SetAndRaise<T>(Expression<Func<object, T>> memberExpression, T value, Action<T> changedCallback) {
//			if (SetInternal(MemberNameUtil.GetPropertyName(memberExpression), value)) 
//				changedCallback(value);
//		}

//		public void SetAndRaise<T>(Expression<Func<T>> memberExpression, T value, Action<T> changedCallback) {
//			if (SetInternal(MemberNameUtil.GetPropertyName(memberExpression), value)) 
//				changedCallback(value);
//		}

		private bool SetValueInternal<T>(string name, T value) {
			BackingFieldInfo field;
			if (!_fields.ContainsKey(name)) {
				field = new BackingFieldInfo(name, typeof(T), this);
				_fields.Add(name, field);
			}
			else {
				field = _fields[name];
			}
			var previousValue = field.Value;
			if(!field.SetValue(this, value)) return false;
			OnPropertyChanged(name, previousValue, value);
			return true;
		}

		/// <summary>
		/// [SPECIAL] Called by Field.Value {set;}
		/// </summary>
		private void SetValueInternal<T>(BackingFieldInfo field, T value, T previousValue) {
			if (!field.SetValue(this, value)) return;
			OnPropertyChanged(field.Name, previousValue, value);
		}

		/// <summary> Gets the value for the property with the specified name.
		/// </summary>
		/// <typeparam name="TRet">The type of the value.</typeparam>
		/// <param name="name">The name of the property.</param>
		/// <returns>The value</returns>
		[Obsolete("Use GetValue",true)]
		public TRet Get<TRet>(string name) {return GetInternal(name, default(TRet)); }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <typeparam name="TRet">The type of the field.</typeparam>
		/// <param name="name">The field name.</param>
		/// <returns>The value.</returns>
		public TRet GetValue<TRet>([CallerMemberName] string name=null) { return GetInternal<TRet>(name,default(TRet)); }

		/// <summary> Gets the value for the property with the specified name.
		/// </summary>
		/// <typeparam name="TRet">The type of the value.</typeparam>
		/// <param name="name">The name of the property.</param>
		/// <param name="defaultValue">The default value </param>
		/// <returns>The value</returns>
		[Obsolete("Use GetValue",true)]
		public TRet Get<TRet>(string name, TRet defaultValue) { return GetInternal(name, defaultValue); }
		//TODO logic to use CallerMemberName
//		public TRet Get<TRet>(TRet defaultValue, [CallerMemberName] string name=null) { return GetInternal<TRet>(name,defaultValue); }

//		Search:		Fields\.Get\((\(\)|_)\s*=>\s*(\w+)\)
//		Replace:	Fields.Get<>("$2")

//		public TRet Get<TRet>(Expression<Func<object,TRet>> propertyExpression) {
//			var name = MemberNameUtil.GetPropertyName(propertyExpression);
//			return GetInternal<TRet>(name);
//		}

//		public TRet Get<TRet>(Expression<Func<TRet>> propertyExpression) {
//			var name = MemberNameUtil.GetPropertyName(propertyExpression);
//			return GetInternal<TRet>(name);
//		}

		private TRet GetInternal<TRet>(string name, TRet defaultValue) {
			if (!_fields.ContainsKey(name)) {
				if (_readOnlyCollection) throw new KeyNotFoundException();
				return defaultValue;
			}
			return (TRet)_fields[name].Value;			
		}

//		public bool Exists(string name) { return _Fields.ContainsKey(name); }
//
//		public bool Remove(string name) {
//			if (!_Fields.ContainsKey(name)) return false;
//			return _Fields.Remove(name);
//		}

		private void OnPropertyChanged(string propertyName, object oldValue, object newValue) {
			_propertyChangedCallback?.Invoke(propertyName);

			var fieldInfo = _fields[propertyName];
			var eventHandlerInfos = fieldInfo.EventHandlers;
			var ea=new ValueChangedEventArgs(newValue, oldValue);
			foreach (var eventHandlerInfo in eventHandlerInfos) { eventHandlerInfo.PropertyChangedEventHandler(_owner, ea); }
			EventManager.Raise<EventHandler<ValueChangedEventArgs>,ValueChangedEventArgs>(fieldInfo.LazyWeakEventProperties,"ValueChangedEvent",ea);
		}
		
//		[Obsolete("Use indexer",true)]
//		public void RegisterPropertyChangedHandler<TProperty>(Expression<Func<object, TProperty>> viewModelProperty, EventHandler<ValueChangedEventArgs> propertyChangedEventHandler) {
//			string name = MemberNameUtil.GetPropertyName(viewModelProperty);
//			RegisterPropertyChangedHandler(name, propertyChangedEventHandler);
//		}

//		[Obsolete("Use indexer",true)]
//		public IDisposable RegisterPropertyChangedHandler(string name, EventHandler<ValueChangedEventArgs> propertyChangedEventHandler) {
//			BackingFieldInfo fieldInfo;
//			if(!_Fields.TryGetValue(name,out fieldInfo)){
//				if (_ReadOnlyCollection) throw new KeyNotFoundException();
//				throw new KeyNotFoundException();
//			}
//			var item = new EventHandlerInfo(fieldInfo, propertyChangedEventHandler);
//			lock (fieldInfo.EventHandlers) {
//				fieldInfo.EventHandlers.Add(item);
//			}
//			return item;
//		}

//		[Obsolete("Use indexer",true)]
//		public void ReleasePropertyChangedHandler<TProperty>(Expression<Func<object, TProperty>> viewModelProperty, EventHandler<ValueChangedEventArgs> propertyChangedEventHandler) {
//			string name = MemberNameUtil.GetPropertyName(viewModelProperty);
//			ReleasePropertyChangedHandler(name,propertyChangedEventHandler);
//		}
//
//		[Obsolete("Use indexer",true)]
//		public void ReleasePropertyChangedHandler(string name, EventHandler<ValueChangedEventArgs> propertyChangedEventHandler) {
//			if (!_Fields.ContainsKey(name)) {
//				throw new KeyNotFoundException();
//			}
//			var item = _Fields[name].EventHandlers.First(x=>x.PropertyChangedEventHandler==propertyChangedEventHandler);
//			item.Dispose();
//		}

		private void Dispose(bool explicitDispose ) {
			if (explicitDispose) {
				if(Interlocked.Exchange(ref _isDisposed,1)!=0) return;
				foreach (var field in _fields.Values) field.Dispose();
				_fields = null;
				_propertyChangedCallback = null;
				_owner = null;				
			}

		}

		public void Dispose() {
			Dispose(true);
			//not required: GC.SuppressFinalize(this);
		}
	}

}