using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using KsWare.Presentation.Core;
using KsWare.Presentation.Core.Patterns;
using KsWare.Presentation.ViewModelFramework.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	partial class ObjectVM /*: IHierarchical<IObjectVM> */ {

		private readonly List<IObjectVM> m_Children = new List<IObjectVM>();
		private IObjectVM m_Parent;
		private string m_MemberName;

		private void InitPartHierarchy() {
			
		}

		/// <summary> Gets the name of the property in parent object.
		/// </summary>
		/// <value>The name of the property.</value>
		/// <remarks>
		/// This means <see cref="Parent"/>.GetNamedProperty[<see cref="MemberName"/>] returns this object.
		/// </remarks>
		[Localizable(false)]
		public string MemberName {
			get {return m_MemberName;}
			set {
				//TODO validations
				//MemberAccessUtil.DemandNotNull(value,null,this,"PropertyName","{162F86E9-3E41-41B6-A6BA-6D3165487771}");
				//MemberAccessUtil.DemandWriteOnce(this.memberName==null,null,this,"PropertyName","{A026C8CD-5974-4C67-B1DF-52CA9FA6A87B}");
				m_MemberName = value;
			}
		}

		/// <summary> Gets the parent of this object.
		/// </summary>
		/// <value>The parent of this object</value>
		/// <remarks>
		/// In default case the <see cref="Parent"/> should be set only by the parent itself!
		/// </remarks>
		public IObjectVM Parent {
			get {return m_Parent;}
			set {
				if(value==m_Parent) return;
				var oldParent = m_Parent;
				SetParentPattern.Execute(ref m_Parent, value,"Parent");
				OnParentChanged(oldParent, value);
				OnPropertyChanged("Parent");
			}
		}

		/// <summary> Occurs when <see cref="Parent"/> property has been changed.
		/// </summary>
		/// <remarks>
		/// In default case the event is raised only one time during initialization.
		/// </remarks>
		public event EventHandler ParentChanged;

		public IEventSource<EventHandler> ParentChangedEvent {
			get { return EventSources.Get<EventHandler>("ParentChangedEvent"); }
		}

		/// <summary> Raises the <see cref="ParentChanged"/> event.
		/// </summary>
		/// <param name="oldParent"></param>
		/// <param name="newParent"></param>
		protected virtual void OnParentChanged(object oldParent, object newParent) {
			if (SuppressAnyEvents == 0) {
				EventUtil.Raise(ParentChanged, this, EventArgs.Empty, "{F53C5FB8-A3F6-4EA1-B51A-1B5481C22E1E}");
				EventManager.Raise<EventHandler,EventArgs>(LazyWeakEventStore,"ParentChangedEvent", EventArgs.Empty);
			}
		}

		IObjectVM IHierarchical<IObjectVM>.Parent { get { return Parent; } set { Parent = (IObjectVM)value; } }

		/// <summary> Gets the children (IObjectVMs which are registered als children).
		/// </summary>
		/// <value>The children.</value>
		/// <remarks></remarks>
		public ICollection<IObjectVM> Children {get{return this.m_Children.AsReadOnly();}}
		
		ICollection<IObjectVM> IHierarchical<IObjectVM>.Children {get {return Children;}}

		/// <summary> Registers a child view model
		/// </summary>
		/// <param name="child">The child view model to register</param>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		protected TChild RegisterChild<TChild>([NotNull] TChild child) where TChild:class,IObjectVM {
//			if (child == null) throw new ArgumentNullException("child");
//			var childHierarchical = (IHierarchical<IObjectVM>) child;
//			if (childHierarchical.Parent!=null) throw new InvalidOperationException("Child is already in a hierarchy!");
//
//			if (string.IsNullOrWhiteSpace(childHierarchical.MemberName)) throw new InvalidOperationException("MemberName not specified");
//#if DEBUG   //### Slow
//			var propertyInfo = GetType().GetProperty(childHierarchical.MemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//			if(propertyInfo==null) {
//				var stackFrame = new StackFrame(1, true);
//				var msg = "Property not implemented!"+
//						  "\n\t"+"Type: "+(stackFrame.GetMethod().DeclaringType??GetType()).FullName+
//						  "\n\t"+"Property: "+childHierarchical.MemberName+
//					 "\n"+stackFrame.GetFileName()+"("+stackFrame.GetFileLineNumber()+","+stackFrame.GetFileColumnNumber()+")";
//				DebuggerːBreak(msg); 
//				throw new InvalidOperationException(msg);
//			}
//#endif
//			m_Children.Add(child);
//			SetParent(child, this);
//			return child;
			return ːːInterfaceHelperːRegisterChild(child, m_Children, this);
		}

		// [PREPARED]
		private static TChild ːːInterfaceHelperːRegisterChild<TChild>([NotNull] TChild child, List<IObjectVM> m_Children,IObjectVM @this) where TChild : class, IObjectVM {
			if (child == null) throw new ArgumentNullException("child");
			var childHierarchical = (IHierarchical<IObjectVM>) child;
			if (childHierarchical.Parent!=null) throw new InvalidOperationException("Child is already in a hierarchy!");

			if (string.IsNullOrWhiteSpace(childHierarchical.MemberName)) throw new InvalidOperationException("MemberName not specified");
#if DEBUG   //### Slow
			var propertyInfo = @this.GetType().GetProperty(childHierarchical.MemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if(propertyInfo==null) {
				var stackFrame = new StackFrame(1, true);
				var msg = "Property not implemented!"+
						  "\n\t"+"Type: "+(stackFrame.GetMethod().DeclaringType??@this.GetType()).FullName+
						  "\n\t"+"Property: "+childHierarchical.MemberName+
					 "\n"+stackFrame.GetFileName()+"("+stackFrame.GetFileLineNumber()+","+stackFrame.GetFileColumnNumber()+")";
				DebuggerːBreak(msg); 
				throw new InvalidOperationException(msg);
			}
#endif
			m_Children.Add(child);
			SetParent(child, @this);
			return child;
		}

		/// <summary> Registers a child view model and assigns the property Name
		/// </summary>
		/// <param name="child">The child view model to register</param>
		/// <param name="propertyName"></param>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[Obsolete("Use other method",true)]
		protected TChild RegisterChild<TChild>([NotNull] TChild child, [Localizable(false)] string propertyName) where TChild:class,IObjectVM{
			throw new NotSupportedException("Obsolete!\nErrorID:{03BD0D3F-738D-49DC-9C03-528E7ABD0A6F}");
		}

		/// <summary> Registers a child view model and assigns the property Name
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="child">The child view model to register</param>
		/// <seealso cref="RegisterChildren"/>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		protected TChild RegisterChild<TChild>([Localizable(false)] string propertyName, [NotNull] TChild child) where TChild:class,IObjectVM {
			return RegisterChildInternal(propertyName, child);
		}

		/// <summary> Registers a child view model and assigns the property
		/// </summary>
		/// <typeparam name="TChild"></typeparam>
		/// <param name="propertyExpression"><c>()=>MyProperty</c></param>
		/// <param name="child">The child view model to register</param>
		/// <example><code>MyProperty = RegisterChild(_=>MyProperty, new MyPropertyVM{/*...*/});</code></example>
		/// <seealso cref="RegisterChildren"/>
		protected TChild RegisterChild<TChild>([NotNull] Expression<Func<object, TChild>> propertyExpression, [NotNull] TChild child) where TChild : class, IObjectVM {
			var memberName = MemberNameUtil.GetPropertyName(propertyExpression);
			return RegisterChildInternal(memberName, child);
		}

		/// <summary> Registers a child view model and assigns the property
		/// </summary>
		/// <typeparam name="TChild"></typeparam>
		/// <param name="propertyExpression"><c>()=>MyProperty</c></param>
		/// <param name="child">The child view model to register</param>
		/// <example><code>MyProperty = RegisterChild(_=>MyProperty, new MyPropertyVM{/*...*/});</code></example>
		/// <seealso cref="RegisterChildren"/>
		protected TChild RegisterChild<TChild>([NotNull] Expression<Func<TChild>> propertyExpression, [NotNull] TChild child) where TChild : class, IObjectVM {
			var memberName = MemberNameUtil.GetPropertyName(propertyExpression);
			return RegisterChildInternal(memberName, child);
		}

		/// <summary> Registers a child view model and assigns the property
		/// </summary>
		/// <typeparam name="TViewModel"></typeparam>
		/// <typeparam name="TChild"></typeparam>
		/// <param name="this"><c>this</c></param>
		/// <param name="propertyExpression"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		/// <example><code>MyProperty = RegisterChild(this,vm=>vm.MyProperty, new MyPropertyVM{/*...*/});</code></example>
		protected TChild RegisterChild<TViewModel,TChild>([NotNull] TViewModel @this, [NotNull] Expression<Func<TViewModel, TChild>> propertyExpression, [NotNull] TChild child) 
			where TChild : class, IObjectVM 
			where TViewModel:class, IObjectVM 
		{
			var memberName = MemberNameUtil.GetPropertyName(propertyExpression);
			return RegisterChildInternal(memberName, child);
		}

		protected TChild SetChild<TChild>([NotNull] Expression<Func<TChild>> propertyExpression, TChild child) where TChild : class, IObjectVM {
			var memberName = MemberNameUtil.GetPropertyName(propertyExpression);
			return RegisterChildInternal(memberName, child,replaceExisting:true);
		}

		private TChild RegisterChildInternal<TChild>([Localizable(false)] string propertyName, [NotNull] TChild child,bool replaceExisting = false) where TChild : class, IObjectVM {
			return ːːInterfaceHelperːRegisterChildInternal(propertyName, child,replaceExisting, m_Children, this);
		}

		// [PREPARED]
		private static TChild ːːInterfaceHelperːRegisterChildInternal<TChild>([Localizable(false)] string propertyName, [NotNull] TChild child, bool replaceExisting, List<IObjectVM> m_Children,IObjectVM @this) where TChild : class, IObjectVM {
			if (child == null) throw new ArgumentNullException("child");
//			if(!(child is IHierarchical) throw new ArgumentException("Type of children not supported!");

			var hvm = (IHierarchical<IObjectVM>)child;
			if (string.IsNullOrEmpty(hvm.MemberName) && string.IsNullOrEmpty(propertyName)) throw new InvalidOperationException("ViewModel must have a PropertyName!");
			if (!string.IsNullOrEmpty(hvm.MemberName) && hvm.MemberName != propertyName)throw new InvalidOperationException("ViewModel PropertyName cannot be changed!");
// ReSharper disable RedundantCheckBeforeAssignment
			if(hvm.MemberName!=propertyName) hvm.MemberName = propertyName;
// ReSharper restore RedundantCheckBeforeAssignment

			PropertyInfo propertyInfo;
			PropertyInfo propertyInfo2 = null;
			try {
//				propertyInfo = GetType().GetProperty(hvm.MemberName, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
				propertyInfo = @this.GetType().GetProperty(hvm.MemberName, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,child.GetType(),new Type[0],null );
				if(propertyInfo==null)
					propertyInfo2 = @this.GetType().GetProperty(hvm.MemberName, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
			} 
			catch(AmbiguousMatchException ex) {
				throw new InvalidOperationException(
				"Registered property not unique!"+
				"\n\t"+"Property: "+hvm.MemberName +
				"\n\t"+"Type: "+DebugUtil.FormatTypeName(@this)+
				"\n\t"+"ErrorID: {0088B310-F009-4A5F-A0B7-AE01FD1BC512}",ex);
			}
			if(propertyInfo==null && propertyInfo2==null) throw new InvalidOperationException(
				"Registered property not implemented!"+
				"\n\t"+"Property: "+hvm.MemberName +
				"\n\t"+"Type: "+DebugUtil.FormatTypeName(@this)+
				"\n\t"+"ErrorID: {377D83B5-E045-49D6-80DB-9CD41E8BC918}");
			if(propertyInfo2!=null && !propertyInfo2.PropertyType.IsAssignableFrom(child.GetType())) throw new InvalidOperationException(
				"Registered property has invalid type!"+
				"\n\t"+"Property: "+hvm.MemberName +
				"\n\t"+"Type: "+DebugUtil.FormatTypeName(@this)+
				"\n\t"+"Registered Type: "+DebugUtil.FormatTypeName(child)+
				"\n\t"+"Property Type  : "+DebugUtil.FormatTypeName(propertyInfo2.PropertyType)+
				"\n\t"+"ErrorID: {377D83B5-E045-49D6-80DB-9CD41E8BC918}");

			if (replaceExisting) {
				var index = m_Children.FindIndex(x=>x.MemberName==propertyName);
				if (index >= 0) {
					//??? m_Children[index].Dispose();
					//??? m_Children[index].Parent = null;
					m_Children[index] = child;
				} else { m_Children.Add(child);}
			} else {
				m_Children.Add(child);
			}
			SetParent(child, @this);

			return child;
		}

		static readonly Dictionary<Type,ReflectedPropertyInfo[]>  RegisterChildren_PropertyInfos=new Dictionary<Type, ReflectedPropertyInfo[]>();

		/// <summary> Registers all children of this view model. 
		/// Use <see cref="HierarchyAttribute"/> for specific properties to control the behavior.
		/// </summary>
		/// <param name="this">Always use: <c>_=>this</c></param>
		/// <remarks>
		/// Only members declared at the level of the supplied type's hierarchy should be considered. Inherited members are not considered.
		/// <p>Use <see cref="HierarchyAttribute"/> to control the behavoir for each property.</p>
		/// <p> All properties which have NOT HierarchyType.Children are skipped.</p>
		/// </remarks>
		/// <example><code>
		/// public Constructor() {
		///    // ... custom registrations here ...
		///	   // SendAction = RegisterChild(_=>SendAction, new ActionVM{/*...*/});
		/// 
		///    RegisterChildren(_=>this);
		/// }
		/// </code></example>
		/// <seealso cref="HierarchyAttribute"/>
		/// <seealso cref="ViewModelMetadataAttribute"/>
		/// <seealso cref="ActionMetadataAttribute"/>
		/// <seealso cref="ValueMetadataAttribute"/>
		/// <seealso cref="ListMetadataAttribute"/>
		protected void RegisterChildren(Func<object, IObjectVM> @this) {
			var o = this;
			var t = @this.Method.DeclaringType; if (t == null) throw new InvalidOperationException("ErrorID: {069CE654-981A-48F4-8A0A-9F6E4CA530B9}");
			RegisterChildren(t, o);
		}
		/// <summary> Registers all children of this view model. 
		/// Use <see cref="HierarchyAttribute"/> for specific properties to control the behavior.
		/// </summary>
		/// <param name="this">Always use: <c>_=>this</c></param>
		/// <remarks>
		/// Only members declared at the level of the supplied type's hierarchy should be considered. Inherited members are not considered.
		/// <p>Use <see cref="HierarchyAttribute"/> to control the behavoir for each property.</p>
		/// <p> All properties which have NOT HierarchyType.Children are skipped.</p>
		/// </remarks>
		/// <example><code>
		/// public Constructor() {
		///    // ... custom registrations here ...
		///	   // SendAction = RegisterChild(_=>SendAction, new ActionVM{/*...*/});
		/// 
		///    RegisterChildren(_=>this);
		/// }
		/// </code></example>
		/// <seealso cref="HierarchyAttribute"/>
		/// <seealso cref="ViewModelMetadataAttribute"/>
		/// <seealso cref="ActionMetadataAttribute"/>
		/// <seealso cref="ValueMetadataAttribute"/>
		/// <seealso cref="ListMetadataAttribute"/>
		protected void RegisterChildren(Func<IObjectVM> @this) {
			var o = this;
			var t = @this.Method.DeclaringType; if (t == null) throw new InvalidOperationException("ErrorID: {C475AD16-8D34-4B9E-B028-D83187D38287}");
			RegisterChildren(t, o);
		}

		private void RegisterChildren(Type t, ObjectVM o) {
			ReflectedPropertyInfo[] propertyInfos;
			if (!RegisterChildren_PropertyInfos.ContainsKey(t)) {
				var tempPropertyInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Select(x=>new ReflectedPropertyInfo(null,x)).ToArray();
				var propertyInfosFiltered = new List<ReflectedPropertyInfo>();
				foreach (var p in tempPropertyInfos) {
					if (p.PropertyInfo.GetSetMethod(true) == null) continue; //no setter
					if (p.PropertyInfo.GetIndexParameters().Length > 0) continue; // indexer
					var type = p.PropertyInfo.PropertyType;
					if (!typeof (IObjectVM).IsAssignableFrom(type)) continue; // no IObjectVM
					p.HierarchyAttributes = p.PropertyInfo.GetCustomAttributes(typeof (HierarchyAttribute), false).Cast<HierarchyAttribute>().ToArray();
					if (p.HierarchyAttributes.Length > 0 && p.HierarchyAttributes.All(x => x.ItemType != HierarchyType.Child)) continue;
					propertyInfosFiltered.Add(p);
				}
				RegisterChildren_PropertyInfos.Add(t, propertyInfosFiltered.ToArray());
				propertyInfos = propertyInfosFiltered.ToArray();
			} else { propertyInfos = RegisterChildren_PropertyInfos[t]; }

			foreach (var p in propertyInfos) {
				var type = p.PropertyInfo.PropertyType;
				var name = p.PropertyInfo.Name;
				var currentValue = p.PropertyInfo.GetValue(this, null);
				if (currentValue != null) continue; // skip already initialized property
				if(p.HierarchyAttributes.Any(x=>x.CreateInstance==false)) continue; // skip because CreateInstance=false specified
				
				var child = (IObjectVM) Activator.CreateInstance(type);
				((ObjectVM) child).Reflection = p.ToInstance(this);
				RegisterChildInternal(name, child);
				p.PropertyInfo.SetValue(o, child, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
			}

		}

		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		private static void SetParent([NotNull] IObjectVM child, IObjectVM parent) {
			// MemberAccessUtil.Demand.Parameter(child, "child").Not.Null;
			if (child == null) throw new ArgumentNullException("child");
			//if(child is ObjectVM) ((ObjectVM) child).Parent = this;
			child.Parent = parent;
//			if (child is IHierarchical<IObjectVM>) ((IHierarchical<IObjectVM>)child).Parent = parent;
//			else throw new InvalidOperationException("Child does not implement IHirarchical");
//			else {
//				const BindingFlags bf = BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
//				var propertyInfo = child.GetType().GetProperty("Parent",bf);
//				if(propertyInfo==null) {throw new MissingMethodException("Parent property not found!");}
//				var setter = propertyInfo.GetSetMethod(true);
//				if(setter==null) {throw new MissingMethodException("Parent property has no setter!");}
//				setter.Invoke(child, bf, null, new object[] {parent}, null);
//			}
		}

		/// <summary> Gets the hierarchical path information for this object. For debug purposes.
		/// </summary>
		public string MemberPath {
			get {
				var path = new StringBuilder();
				IHierarchical<IObjectVM> h = this;
				while (h!=null) {
					if(path.Length>0) path.Insert(0, ".");
					path.Insert(0, h.MemberName ?? DebugUtil.FormatTypeName(h,encloseInCurlyBrackets:true));
					h = h.Parent;
				}
				return path.ToString();
			}
		}

		protected virtual void OnHierarchyValueChanged(IValueVM value) {
			var objectVM = value.Parent as ObjectVM;
			if(objectVM!=null) objectVM.OnHierarchyValueChanged(value);
		}

	}


}
