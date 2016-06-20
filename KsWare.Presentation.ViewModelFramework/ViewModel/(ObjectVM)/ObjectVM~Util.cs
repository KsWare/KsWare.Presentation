using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using KsWare.Presentation.BusinessFramework;

namespace KsWare.Presentation.ViewModelFramework {

	partial class ObjectVM {

		/// <summary> ALIAS CultureInfo.CreateSpecificCulture("en-US")
		/// </summary>
		protected static readonly CultureInfo EnUs = CultureInfo.CreateSpecificCulture("en-US");

		/// <summary> ALIAS for CultureInfo.InvariantCulture
		/// </summary>
		protected static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

		/// <summary> ALIAS for <c>Metadata.DataProvider.Data</c> </summary>
		/// <remarks>
		/// <para>This property does not raise <see cref="INotifyPropertyChanged.PropertyChanged"/>.</para>
		/// </remarks>
		[EditorBrowsable(EditorBrowsableState.Never),Browsable(false)]
		//[Obsolete("Use Data or Metadata.DataProvider.Data",true)]/*enable this to find data accessor/for debug)
		[Bindable(BindableSupport.No)]
		public virtual object MːData {get { return Metadata.DataProvider.Data; }set {  Metadata.DataProvider.Data=value; }}

		/// <summary> [EXPERIMENTAL] Gets or sets the data of the underlying business object </summary>
		/// <remarks>
		/// <para>ALIAS for <c>((IObjectBM)Metadata.DataProvider.Data).Metadata.DataProvider.Data</c> </para>
		/// <para>This property does not raise <see cref="INotifyPropertyChanged.PropertyChanged"/>.</para>
		/// </remarks>
		[Bindable(BindableSupport.No)]
		public object MːDataːData {
			get {
				if (IsInDesignMode) return null; // WORKAROUND because NullReferenceException in designer
				// return ((IObjectBM)MːData).Metadata.DataProvider.Data;
				return (MːData as IObjectBM)?.Metadata?.DataProvider?.Data;
			} set {  ((IObjectBM)MːData).Metadata.DataProvider.Data=value; }
		}

		/// <summary> [EXPERIMENTAL] Gets or sets the data of the underlying business object.
		///  </summary>
		/// <remarks>
		/// <para>ALIAS for <c>((IBusinessObjectVM)Metadata.DataProvider.Data).Metadata.DataProvider.Data</c></para>
		/// <para>This property does not raise <see cref="INotifyPropertyChanged.PropertyChanged"/>.</para>
		/// </remarks>
		[Bindable(BindableSupport.No)]
		public object MːBusinessObjectːData {
			get {
				if (IsInDesignMode) return null; // WORKAROUND because NullReferenceException in designer
				// return ((IBusinessObjectVM)Metadata.DataProvider.Data).Metadata.DataProvider.Data;
				return (Metadata.DataProvider.Data as IBusinessObjectVM)?.Metadata?.DataProvider?.Data;
			}
			set { ((IObjectBM)MːData).Metadata.DataProvider.Data=value; }
		}

		/// <summary> Gets or set the underlying business object (if any)
		/// </summary>
		/// <remarks> ALIAS for <c>(IObjectBM)this.Metadata.DataProvider.Data</c> 
		/// <para>This property does not raise <see cref="INotifyPropertyChanged.PropertyChanged"/>.</para>
		/// </remarks>
		[Bindable(BindableSupport.No)]
		public virtual IObjectBM MːBusinessObject {
			get {
				if (this is IBusinessObjectVM) return ((IBusinessObjectVM) this).BusinessObject;
				else return MːData as IObjectBM;
			}
			set {
				if (this is IBusinessObjectVM) ((IBusinessObjectVM) this).BusinessObject = value;
				else MːData = value;
			}
		}

		protected string NameOf<TRet>(Expression<Func<IObjectVM, TRet>> memberExpression) {
			var memberName = MemberNameUtil.GetPropertyName(memberExpression);
			return memberName;
		}

		/// <summary> TreeHelper
		/// </summary>
		public static class TreeHelper {

			public static T FindAnchor<T>(IObjectVM obj) where  T:IObjectVM {
				return FindAnchor<T>(obj, typeof(T), findSelf: false);
			}

			public static T FindAnchor<T>(IObjectVM obj, Type type) where  T:IObjectVM {
				return FindAnchor<T>(obj, type, findSelf: false);
			}

			public static T FindAnchor<T>(IObjectVM obj, Type type, bool findSelf) {
				IObjectVM p = findSelf?obj:obj.Parent;
				while (true) {
					if (p == null) return default(T);
					if (p.GetType() == type) return (T)p;
					if (type.IsInstanceOfType(p)) return (T)p;
					p = p.Parent;
				}
			}

			public static IObjectVM FindAnchor<T1, T2, T3, T4, T5>(IObjectVM obj) {
				return FindAnchor(obj, new[] {typeof (T1), typeof (T2), typeof (T3), typeof (T4), typeof (T5)}, false);
			}
			public static IObjectVM FindAnchor<T1, T2, T3, T4>(IObjectVM obj) {
				return FindAnchor(obj, new[] {typeof (T1), typeof (T2), typeof (T3), typeof (T4)}, false);
			}
			public static IObjectVM FindAnchor<T1, T2, T3>(IObjectVM obj) {
				return FindAnchor(obj, new[] {typeof (T1), typeof (T2), typeof (T3)}, false);
			}
			public static IObjectVM FindAnchor<T1, T2>(IObjectVM obj) {
				return FindAnchor(obj, new[] {typeof (T1), typeof (T2)}, false);
			}

			public static IObjectVM FindAnchor(IObjectVM obj, params Type[] types) {
				return FindAnchor(obj, types, false);
			}

			public static IObjectVM FindAnchor(IObjectVM obj,Type[] types, bool findSelf=false) {
				IObjectVM p = findSelf?obj:obj.Parent;
				while (true) {
					if (p == null) return null;
					var t = p.GetType();
					if (types.Contains(t)) return p;
					if (types.Any(x => x.IsInstanceOfType(p))) return p;
					p = p.Parent;
				}
			}
		}
	}

}
