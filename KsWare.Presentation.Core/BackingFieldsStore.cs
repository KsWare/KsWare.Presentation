using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

namespace KsWare.Presentation {

	public sealed partial class BackingFieldsStore:IDisposable {

		private static readonly Dictionary<Type,Dictionary<string, Type>> s_TypesDic = new Dictionary<Type,Dictionary<string, Type>>();

		private int m_IsDisposed;
		private object m_Owner;
		private Action<string> m_PropertyChangedCallback;
		private Dictionary<string, BackingFieldInfo> m_Fields = new Dictionary<string, BackingFieldInfo>();
		private bool m_ReadOnlyCollection = false;

		public BackingFieldsStore([NotNull] object owner, Action<string> propertyChangedCallback) {
			if(owner==null) throw new ArgumentNullException("owner");

			m_Owner = owner;
			m_PropertyChangedCallback = propertyChangedCallback;

			var ownerType = m_Owner.GetType();

			if (!s_TypesDic.ContainsKey(owner.GetType())) {
				var propertyInfos = m_Owner.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
				var typeDic = new Dictionary<string, Type>();
				foreach (var propertyInfo in propertyInfos) {
					if(propertyInfo.GetIndexParameters().Length>0) continue; //exclude indexer
					if(GetLazyAttribute(propertyInfo)!=null) continue; //exclude lazy

					var type = propertyInfo.PropertyType;
					var name = propertyInfo.Name;

					if (typeDic.ContainsKey(name)) continue;
					typeDic.Add(name,type);
					
				}
				s_TypesDic.Add(ownerType,typeDic);
			}
			{
				var typeDic = s_TypesDic[ownerType];
				foreach (var entry in typeDic) {
					var name = entry.Key;
					var type = entry.Value;
					var value = type.IsValueType ? Activator.CreateInstance(type) : null;
					m_Fields.Add(name, new BackingFieldInfo {
						Value = value,
						Type = type,
					});
				}
			}
		}

		public object Owner { get { return m_Owner; } }

		public BackingFieldInfo this[Expression<Func<object, object>> memberExpression] {
			get { return m_Fields[MemberNameUtil.GetPropertyName(memberExpression)]; }
		}

		public BackingFieldInfo this[Expression<Func<object>> memberExpression] {
			get { return m_Fields[MemberNameUtil.GetPropertyName(memberExpression)]; }
		}

		public BackingFieldInfo this[string name] {
			get { return m_Fields[name]; }
		}

		public void Add<T>(string name, T value=default(T)) { AddCore<T>(name); }

		public void Add<T>(Expression<Func<object, T>> memberExpression, T value=default(T)) {
			Add(MemberNameUtil.GetPropertyName(memberExpression),value);
		}
		public void Add<T>(Expression<Func<T>> memberExpression, T value=default(T)) {
			Add(MemberNameUtil.GetPropertyName(memberExpression),value);
		}

		private void AddCore<T>(string name, T value = default(T)) {
			if(m_ReadOnlyCollection) throw new InvalidOperationException("The BackingFieldStore is read-only!");
			if(m_Fields.ContainsKey(name)) throw new InvalidOperationException("The BackingFieldStore already contains a item with name'"+name+"'!");
			m_Fields.Add(name, new BackingFieldInfo{Value = value,Type = typeof(T)});
			OnPropertyChanged(name,default(T),value);
		}

		public void Set<T>(string name, T value) { SetInternal(name,value); }
//		public void Set<T>(T value,[CallerMemberName] string name=null ) { SetInternal(name,value); }

//		Search:		Fields\.Set\((\(\)|_)\s*=>\s*(\w+),
//		Replace:	Fields.Set("$2",

//		public void Set<T>(Expression<Func<object, T>> memberExpression, T value) { SetInternal(MemberNameUtil.GetPropertyName(memberExpression),value);}
//		public void Set<T>(Expression<Func<T>> memberExpression, T value) { SetInternal(MemberNameUtil.GetPropertyName(memberExpression),value);}

		public void SetAndRaise<T>(string propertyName, T value, Action<T> changedCallback) {
			if (SetInternal(propertyName, value)) 
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

		private bool SetInternal<T>(string name, T value) {
			const bool bChanged=true;const bool bNotChanged = false;
			if (!m_Fields.ContainsKey(name)) {
				m_Fields.Add(name, new BackingFieldInfo{Value = value,Type = typeof(T)});
				OnPropertyChanged(name,default(T),value);
				return bChanged;
			} 
			if (Equals(m_Fields[name].Value, value)) return bNotChanged;
			var prev = m_Fields[name].Value;
			m_Fields[name].Value = value;
			OnPropertyChanged(name,prev,value);
			return bChanged;
		}


		/// <summary> Gets the value for the property with the specified name.
		/// </summary>
		/// <typeparam name="TRet">The type of the value.</typeparam>
		/// <param name="name">The name of the property.</param>
		/// <returns>The value</returns>
		public TRet Get<TRet>(string name) {return GetInternal(name, default(TRet)); }
		//TODO logic to use CallerMemberName
//		public TRet Get<TRet>([CallerMemberName] string name=null) { return GetInternal<TRet>(name,default(TRet)); }

		/// <summary> Gets the value for the property with the specified name.
		/// </summary>
		/// <typeparam name="TRet">The type of the value.</typeparam>
		/// <param name="name">The name of the property.</param>
		/// <param name="defaultValue">The default value </param>
		/// <returns>The value</returns>
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
			if (!m_Fields.ContainsKey(name)) {
				if (m_ReadOnlyCollection) throw new KeyNotFoundException();
				return defaultValue;
			}
			return (TRet)m_Fields[name].Value;			
		}

//		public bool Exists(string name) { return m_Fields.ContainsKey(name); }
//
//		public bool Remove(string name) {
//			if (!m_Fields.ContainsKey(name)) return false;
//			return m_Fields.Remove(name);
//		}

		private void OnPropertyChanged(string propertyName, object oldValue, object newValue) {
			m_PropertyChangedCallback(propertyName);

			var fieldInfo = m_Fields[propertyName];
			var eventHandlerInfos = fieldInfo.EventHandlers;
			var ea=new ValueChangedEventArgs(oldValue,newValue);
			foreach (var eventHandlerInfo in eventHandlerInfos) { eventHandlerInfo.PropertyChangedEventHandler(m_Owner, ea); }
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
//			if(!m_Fields.TryGetValue(name,out fieldInfo)){
//				if (m_ReadOnlyCollection) throw new KeyNotFoundException();
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
//			if (!m_Fields.ContainsKey(name)) {
//				throw new KeyNotFoundException();
//			}
//			var item = m_Fields[name].EventHandlers.First(x=>x.PropertyChangedEventHandler==propertyChangedEventHandler);
//			item.Dispose();
//		}


		private void Dispose(bool explicitDispose ) {
			if (explicitDispose) {
				if(Interlocked.Exchange(ref m_IsDisposed,1)!=0) return;
				foreach (var field in m_Fields.Values) field.Dispose();
				m_Fields = null;
				m_PropertyChangedCallback = null;
				m_Owner = null;				
			}

		}

		public void Dispose() {
			Dispose(true);
			//not required: GC.SuppressFinalize(this);
		}

		// ###

		public class BackingFieldInfo:IDisposable {
//			public string Name { get; set; }
			public Type Type { get; set; }
//			public object DefaultValue { get; set; }
//			public bool CanWrite { get; set; }
			public object Value { get; set; }
			internal List<EventHandlerInfo> EventHandlers=new List<EventHandlerInfo>();
			internal Lazy<EventSourceStore> LazyWeakEventProperties;

			public BackingFieldInfo() {
				LazyWeakEventProperties = new Lazy<EventSourceStore>(() => new EventSourceStore(this));
			}

			public event EventHandler<ValueChangedEventArgs> ValueChanged {
				add {lock (EventHandlers) {EventHandlers.Add(new EventHandlerInfo(this, value));}}
				remove {lock (EventHandlers) {var item = EventHandlers.First(x=>x.PropertyChangedEventHandler==value);item.Dispose();}}
			}

			public IEventSource<EventHandler<ValueChangedEventArgs>> ValueChangedEvent {
				get { return LazyWeakEventProperties.Value.Get<EventHandler<ValueChangedEventArgs>>("ValueChangedEvent"); }
			}

			public void Dispose() {
				foreach (var eventHandlerInfo in EventHandlers) eventHandlerInfo.Dispose();
				EventHandlers = null;
				if (LazyWeakEventProperties.IsValueCreated) LazyWeakEventProperties.Value.Dispose();
				LazyWeakEventProperties = null;
			}
		}

		internal class EventHandlerInfo:IDisposable {

			private BackingFieldInfo m_FieldInfo;

			public EventHandlerInfo(BackingFieldInfo fieldInfo, EventHandler<ValueChangedEventArgs> propertyChangedEventHandler) {
				m_FieldInfo = fieldInfo;
				PropertyChangedEventHandler = propertyChangedEventHandler;
			}

			public EventHandler<ValueChangedEventArgs> PropertyChangedEventHandler { get; private set; }

			public void Dispose() {
				lock (m_FieldInfo.EventHandlers) {
					m_FieldInfo.EventHandlers.Remove(this);
				}
				m_FieldInfo = null;
			}
		}
	}

	// Lazy support
	partial class BackingFieldsStore {


		public void AddLazy<TValue>(string propertyName, Lazy<TValue> lazy) {
			AddCore(propertyName,lazy);
		}

//		public void AddLazy<T>(Expression<Func<object, T>> propertyExpression, Lazy<object> lazy) {
//			var name = MemberNameUtil.GetPropertyName(propertyExpression);
//			AddCore(name,lazy);
//		}
//
//		public void AddLazy<T>(Expression<Func<T>> propertyExpression, Lazy<object> lazy) {
//			var name = MemberNameUtil.GetPropertyName(propertyExpression);
//			AddCore(name,lazy);
//		}

		public TValue GetLazy<TValue>(string propertyName) {
			return (TValue)GetInternal<Lazy<TValue>>(propertyName,default(Lazy<TValue>)).Value;
		}

//		public TValue GetLazy<TValue>(Expression<Func<object,TValue>> propertyExpression) {
//			var name = MemberNameUtil.GetPropertyName(propertyExpression);
//			return (TValue)GetInternal<Lazy<object>>(name,default(TValue)).Value;
//		}
//
//		public TValue GetLazy<TValue>(Expression<Func<TValue>> propertyExpression) {
//			var name = MemberNameUtil.GetPropertyName(propertyExpression);
//			return (TValue)GetInternal<Lazy<object>>(name).Value;
//		}

		/// <summary> Gets the specified property. When lazy initialization occurs, the specified initialization function is used.
		/// </summary>
		/// <typeparam name="TRet">The type of the return value.</typeparam>
		/// <param name="propertyExpression">The property expression.</param>
		/// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
		/// <returns>The value</returns>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
		public TRet Get<TRet>([NotNull]Expression<Func<object,TRet>> propertyExpression, [NotNull]Func<TRet> valueFactory) {
			if (valueFactory == null) throw new ArgumentNullException("valueFactory");
			var name = MemberNameUtil.GetPropertyName(propertyExpression);
			if (!m_Fields.ContainsKey(name)) {
				if (m_ReadOnlyCollection) throw new KeyNotFoundException();
				var value = valueFactory();
				AddCore(name,value);
				if(Equals(value,default(TRet))) OnPropertyChanged(name,default(TRet),value);
			}
			return (TRet)m_Fields[name].Value;	
		}

		/// <summary> Gets the specified property. When lazy initialization occurs, the specified initialization function is used.
		/// </summary>
		/// <typeparam name="TRet">The type of the return value.</typeparam>
		/// <param name="propertyExpression">The property expression.</param>
		/// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
		/// <returns>The value</returns>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
		public TRet Get<TRet>([NotNull]Expression<Func<TRet>> propertyExpression, [NotNull]Func<TRet> valueFactory) {
			if (valueFactory == null) throw new ArgumentNullException("valueFactory");
			var name = MemberNameUtil.GetPropertyName(propertyExpression);
			if (!m_Fields.ContainsKey(name)) {
				if (m_ReadOnlyCollection) throw new KeyNotFoundException();
				var value = valueFactory();
				AddCore(name,value);
				if(Equals(value,default(TRet))) OnPropertyChanged(name,default(TRet),value);
			}
			return (TRet)m_Fields[name].Value;	
		}


		private LazyAttribute GetLazyAttribute(PropertyInfo propertyInfo) { return propertyInfo.GetCustomAttributes(typeof (LazyAttribute), false).Cast<LazyAttribute>().FirstOrDefault(); }

		public sealed class LazyAttribute : Attribute {

			public LazyAttribute(Type valueFactory) {
				ValueFactory = valueFactory;
			}

			public Type ValueFactory { get; set; }
		}

		public interface IValueFactory {
			object CreateInstance(Type type, BackingFieldsStore store);
		}
	}
	
}