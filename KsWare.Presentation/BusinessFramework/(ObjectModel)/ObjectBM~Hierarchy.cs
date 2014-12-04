using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using KsWare.Presentation.Providers;
using KsWare.Presentation.Core;
using KsWare.Presentation.Core.Patterns;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Business object
	/// </summary>
	public partial class ObjectBM:IHierarchical<IObjectBM> {

		static readonly Dictionary<Type,List<BusinessPropertyInfo>>  RegisterChildren_PropertyInfos=new Dictionary<Type, List<BusinessPropertyInfo>>();
		
		private IObjectBM m_Parent;
		private readonly List<IObjectBM> m_Children = new List<IObjectBM>();

		private void InitPartHierarchy() {
			
		}

		 #region IHirarchical

		/// <summary> Gets the parent object.
		/// </summary>
		/// <value>The parent object</value>
		/// <remarks>
		/// Only the parent object itself can set this property!
		/// </remarks>
		public virtual IObjectBM Parent {
			get { return m_Parent; }
			set {
				if (Equals(m_Parent,value)) return;
				var prev = m_Parent;
				SetParentPattern.Execute(ref m_Parent, value, "Parent");
				OnParentChanged(prev,value);
			}
		}

		/// <summary> Raises the <see cref="ParentChanged"/> event.
		/// </summary>
		protected virtual void OnParentChanged(object previousParent, object newParent) {
			EventUtil.Raise(ParentChanged, this, EventArgs.Empty, "{351F875B-5BC8-491F-B783-EA2B0D204F74}");
			EventUtil.WeakEventManager.Raise(ParentChangedEvent, EventArgs.Empty);
		}

		/// <summary> Occurs when the <see cref="Parent"/> property has been changed.
		/// </summary>
		public event EventHandler ParentChanged;

		public IWeakEventSource<EventHandler> ParentChangedEvent { get { return WeakEventProperties.Get<EventHandler>("ParentChangedEvent"); } }

		/// <summary> Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		IObjectBM IHierarchical<IObjectBM>.Parent { get { return Parent; } set { Parent = value; } }

		/// <summary> Gets or sets the name of the member.
		/// </summary>
		/// <value>The name of the member.</value>
		public string MemberName { get; set; }

		/// <summary> Gets the children.
		/// </summary>
		/// <value>The children.</value>
		public ICollection<IObjectBM> Children { get { return m_Children.AsReadOnly(); } }

		ICollection<IObjectBM> IHierarchical<IObjectBM>.Children { get { return this.Children; } }

		/// <summary> Registers a child <see cref="IObjectBM"/> in the business object tree
		/// </summary>
		/// <typeparam name="TChild">Type child business object</typeparam>
		/// <param name="child">The child business object to be registered</param>
		/// <returns>The registered child business object</returns>
		protected TChild RegisterChild<TChild>(TChild child) where TChild : class, IObjectBM { return RegisterChildInternal(child); }


		private TChild RegisterChildInternal<TChild>(TChild child) where TChild : class, IObjectBM {
			if (child == null) throw new ArgumentNullException("child");
			var childHierarchical = (IHierarchical<IObjectBM>) child;
			if (childHierarchical.Parent != null) throw new InvalidOperationException("Child is already in a hierarchy!");
			if (string.IsNullOrWhiteSpace(childHierarchical.MemberName)) throw new InvalidOperationException("MemberName not specified");
		   
			var propertyInfo = this.GetType().GetProperty(childHierarchical.MemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (propertyInfo == null)
			    throw new InvalidOperationException(
			        "Property is not implemented!" +
			        "\n\t" + "Type: " + this.GetType().FullName +
			        "\n\t" + "Property: " + childHierarchical.MemberName +
			        "\n\t" + "UniqueID: {377D83B5-E045-49D6-80DB-9CD41E8BC918}");

			m_Children.Add(child);
			childHierarchical.Parent = this;
			return child;
		}

		/// <summary> Registers a disposable object
		/// </summary>
		/// <param name="disposable">A disposable object</param>
		/// <remarks>
		/// When this instance is disposed <see cref="IDisposable.Dispose"/> will be called for each registered object.
		/// </remarks>
		protected void RegisterDispose(IDisposable disposable) { m_DisposableObjects.Add(disposable); }

		/// <summary> Gets the member path.
		/// </summary>
		/// <value>The member path.</value>
		public string MemberPath {
			get {
			    try {
			        var path = new StringBuilder();
			        IHierarchical<IObjectBM> h = this;
			        while (h != null) {
			            if (path.Length > 0) path.Insert(0, ".");
			            path.Insert(0, h.MemberName ?? "?");
			            h = h.Parent;
			        }
			        return path.ToString();
			    }
			    catch (Exception ex) {
			        this.DoNothing(ex);
			        Debug.WriteLine("WARNING: Exception caught and ignored!" +
			                        "\r\n\tin ObjectBM.MemberPath" +
			                        "\r\n\tUniqueID:{22C3698D-3020-4C9E-A84B-E61647C9B2DC}");
			        return "?";
			    }

			}
		}

		#endregion



		/// <summary> Registers all children of this business model. 
		/// Use <see cref="HierarchyAttribute"/> for specific properties to control the behavior.
		/// </summary>
		/// <param name="this">Always use: <c>_=>this</c></param>
		/// <remarks>
		/// Only members declared at the level of the supplied type's hierarchy should be considered. Inherited members are not considered.
		/// <p>Use <see cref="HierarchyAttribute"/> to control the behavior for each property.</p>
		/// <p> All properties which have NOT HierarchyType.Children are skipped.</p>
		/// </remarks>
		/// <example><code>
		/// public Constructor() {
		///    // ... custom registrations here ...
		///	   // SendAction = RegisterChild(_=>SendAction, new ActionBM{/*...*/});
		/// 
		///    RegisterChildren(_=>this);
		/// }
		/// </code></example>
		/// <seealso cref="HierarchyAttribute"/>
//		/// <seealso cref="ViewModelMetadataAttribute"/>
//		/// <seealso cref="ActionMetadataAttribute"/>
//		/// <seealso cref="ValueMetadataAttribute"/>
//		/// <seealso cref="ListMetadataAttribute"/>
		protected void RegisterChildren(Func<object, IObjectBM> @this) {
			var o = this;
			var t = @this.Method.DeclaringType; if (t == null) throw new InvalidOperationException("ErrorID: {6B8E8EDB-5A2E-4AB6-A572-AB54C369555A}");

			List<BusinessPropertyInfo> propertyInfos;
			if (!RegisterChildren_PropertyInfos.ContainsKey(t)) {
				propertyInfos=t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Select(x=>new BusinessPropertyInfo(x)).ToList();

				var propertyInfosFiltered=new List<BusinessPropertyInfo>();
				foreach (var propertyInfo in propertyInfos) {
					if(propertyInfo.PropertyInfo.GetSetMethod(true)==null) continue; //no setter
					if(propertyInfo.PropertyInfo.GetIndexParameters().Length>0) continue; // indexer
					var type = propertyInfo.PropertyInfo.PropertyType;
					if (!typeof (IObjectBM).IsAssignableFrom(type)) continue; // no IObjectBM
					if (propertyInfo.HierarchyAttributes.Length > 0 && propertyInfo.HierarchyAttributes.All(x => x.ItemType != HierarchyType.Child)) continue;
					propertyInfosFiltered.Add(propertyInfo);
				}
				RegisterChildren_PropertyInfos.Add(t,propertyInfosFiltered);
				propertyInfos = propertyInfosFiltered;
			}
			else { propertyInfos = RegisterChildren_PropertyInfos[t]; }
			
			foreach (var propertyInfo in propertyInfos) {
				var type = propertyInfo.Type;
				var name = propertyInfo.Name;
				var currentValue=propertyInfo.PropertyInfo.GetValue(this, null);
				if(currentValue!=null) continue;// skip already initialized property
				var child = (IObjectBM) Activator.CreateInstance(type);
				child.MemberName = name;
				((ObjectBM) child).Reflection=new ReflectedInfo(this,propertyInfo);
				RegisterChildInternal(child);
				propertyInfo.PropertyInfo.SetValue(o,child,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,null,null);
			}

		}

	}
}
