using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using KsWare.Presentation.Documentation;

namespace KsWare.Presentation{

	public static class SaveNullCheck {

		// safe null-check.

//		/// <summary> Implementation of Groovy's safe dereferencing operator ?.
//		/// </summary>
//		/// <typeparam name="TIn"></typeparam>
//		/// <typeparam name="TOut"></typeparam>
//		/// <param name="v"></param>
//		/// <param name="f"></param>
//		/// <returns></returns>
//		/// <example><c>this.PropertyA.PropertyB.PropertyC</c> replaced by <br/>
//		/// <c>this.ʘ(o=>o.PropertyA).ʘ(o=>o.PropertyB).ʘ(o=>o.PropertyC)</c></example>
//		public static TOut ʘ<TIn, TOut>(this TIn v, Func<TIn, TOut> f) where TIn : class  where TOut: class {
//			if (v == null) return null; 
//			return f(v); 
//		}


		/// <summary> Implementation of Groovy's <see cref="docːFeatured.safeˑdereferencingˑoperator"/> ?.
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="v"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		/// <example><c>this.PropertyA.PropertyB.PropertyC</c> replaced by <br/>
		/// <c>this.ʘ(o=>o.PropertyA).ʘ(o=>o.PropertyB).ʘ(o=>o.PropertyC)</c></example>
		public static TOut ʘ<TIn, TOut>(this TIn v, Func<TIn, TOut> f) where TIn : class /* where TOut: class */{
			if (v == null) return default(TOut); 
			return f(v); 
		}

		/// <summary> Implementation of Groovy's <see cref="docːFeatured.safeˑdereferencingˑoperator"/> ?.
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="v"></param>
		/// <param name="f"></param>
		/// <param name="defaultValue"> </param>
		/// <returns></returns>
		/// <example><c>this.PropertyA.PropertyB.PropertyC</c> replaced by <br/>
		/// <c>this.ʘ(o=>o.PropertyA).ʘ(o=>o.PropertyB).ʘ(o=>o.PropertyC)</c></example>
		public static TOut ʘ<TIn, TOut>(this TIn v, Func<TIn, TOut> f, TOut defaultValue) where TIn : class {
			if (v == null) return defaultValue; 
			return f(v); 
		}

		/// <summary> Implementation of Groovy's safe dereferencing operator ?.
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="v"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		/// <example><c>this.PropertyA.PropertyB.PropertyC</c> replaced by <br/>
		/// <c>this.ʘ(o=>o.PropertyA).ʘ(o=>o.PropertyB).ʘ(o=>o.PropertyC)</c></example>
		public static TOut SafeRef<TIn, TOut>(this TIn v, Func<TIn, TOut> f) where TIn : class  where TOut: class {
			if (v == null) return null; 
			return f(v); 
		}

		/// <summary> Example: this.ℝ(x => x.Parent.Parent)
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="v"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static TOut ℝ<TIn, TOut>(this TIn v, Expression<Func<TIn, TOut>> f) where TIn : class  where TOut: class {
			var path = MemberNameUtil.SplitPropertyPath(f).ToList();
			var o = (object) v;
			path.RemoveAt(0);
			while (path.Count>0) {
				if (o == null) return null;
				var m = path[0];path.RemoveAt(0);
				var propertyInfo = o.GetType().GetProperty(m);
				o = propertyInfo.GetValue(o, null);
			}
			return (TOut)o;

//			var compiledExpression = f.Compile();
//			var result = compiledExpression(v);
//
//			return result;
		}

		//static int ÑŘǋℕℜℝ;

		

		private static void Test() {
			//if (Application.Current == null) {var dummy = new Application();}
			var store = new TestClass();
			var item1 = new TestClass();item1.Parent = store;
			var item2 = new TestClass();item2.Parent = item1;
			if(item2.Store==null) throw new Exception("Test failed! UID:{8DFA67B7-31BB-4671-A762-4A04C32C53FA}");
			var dummy = item1.Store;//No exception
			dummy = store.Store;//No exception

			var start = DateTime.Now;
			for (int i = 0; i < 100000; i++) { dummy = item2.Store; }
			var time1 = DateTime.Now.Subtract(start);
			start = DateTime.Now;
			for (int i = 0; i < 100000; i++) { dummy = item2.Store2; }
			var time2 = DateTime.Now.Subtract(start);
			start = DateTime.Now;
			for (int i = 0; i < 100000; i++) { dummy = item2.Store3; }
			var time3 = DateTime.Now.Subtract(start);
			Debug.WriteLine(time1);
			Debug.WriteLine(time2);
			Debug.WriteLine(time3);
			Debug.WriteLine(time1.Ticks/time3.Ticks);
			Debug.WriteLine(((double)time2.Ticks/time3.Ticks).ToString("N2"));
		}

		private class TestClass {
			public TestClass Parent { get; set; }
			public TestClass Store { get { return (TestClass)this.ℝ(x => x.Parent.Parent); } }
			public TestClass Store2 { get { return this.Parent == null ? null : this.Parent.Parent; } }
			public TestClass Store3 { get { return (TestClass)this.Parent.Parent; } }
		}
	}
}
