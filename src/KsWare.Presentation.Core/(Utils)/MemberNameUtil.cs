using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace KsWare.Presentation {

	public static class MemberNameUtil {

		/// <summary> propertyname cache
		/// </summary>
		private static readonly ICache<LambdaExpression, string> PropertyCache =
			new MemoizingMRUCache2b<LambdaExpression, string>(GetNameFunc, 25);

		internal static long StatisticsːRaiseːGetCount;
		internal static int StatisticsːCount { get { return PropertyCache.Count; } }

		private static string GetNameFunc(LambdaExpression expression) {
//			MemberExpression body;
//			var unaryExpression = expression.Body as UnaryExpression;
//			if (unaryExpression != null) body = (MemberExpression) unaryExpression.Operand;
//			else body = (MemberExpression) expression.Body;
//			return body.Member.Name;

			MemberExpression body;
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression != null) body = (MemberExpression) expression.Body;
			else body = (MemberExpression) ((UnaryExpression) expression.Body).Operand;
			return body.Member.Name;
		}

//		private static readonly ICache<string, string> PropertyCache =
//			new MemoizingMRUCache2<string, string>((expression, _) => {
//				MemberExpression body;
//				if (expression.Body is UnaryExpression) body = (MemberExpression) ((UnaryExpression) expression.Body).Operand;
//				else body = (MemberExpression) expression.Body;
//				return body.Member.Name;
//			}, 25);

		/// <summary> Typesafe way of getting the name of a property.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		public static string GetPropertyName<T>(Expression<Func<T>> expression) {
			Interlocked.Increment(ref StatisticsːRaiseːGetCount);
			//return PropertyCache.Get(expression);
			return GetNameFunc(expression);
		}

		/// <summary> Typesafe way of getting the name of a property.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		public static string GetPropertyName<T>(Expression<Func<T, object>> expression) {
			Interlocked.Increment(ref StatisticsːRaiseːGetCount);
			//return PropertyCache.Get(expression);
			return GetNameFunc(expression);
		}


		/// <summary> Typesafe way of getting the name of a property without the need of an instance.
		/// </summary>
		/// <typeparam name="TObj">The type of the object.</typeparam>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		public static string GetPropertyName<TObj, TProperty>(Expression<Func<TObj, TProperty>> expression) {
			Interlocked.Increment(ref StatisticsːRaiseːGetCount);
			//return PropertyCache.Get(expression);
			return GetNameFunc(expression);
		}

		public static Type GetPropertyType<T>(Expression<Func<T, object>> expression) {
			MemberExpression body;
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression != null) body = (MemberExpression) expression.Body;
			else body = (MemberExpression) ((UnaryExpression) expression.Body).Operand;

			var type = body.Member.ReflectedType??body.Member.DeclaringType??typeof(T);
			switch (body.Member.MemberType) {
				case MemberTypes.Property:
					return type.GetProperty(body.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).PropertyType;
				case MemberTypes.Field:
					return type.GetField(body.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FieldType;
				case MemberTypes.Method:
					return type.GetMethod(body.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ReturnType;
				default:
					return null;
			}		
		}

/*
//          ??? "Convert(.CollectCount)"
			var body = propertyExpression.Body.ToString();
			if (body.StartsWith("Convert(") && body.EndsWith(")")) body = body.Substring(8, body.Length - 9);
			var memberName = body.Split(new char[] {'.'}).Last();
 */

//		private static string propertyCache_Get(LambdaExpression expression) {
//			MemberExpression body;
//
//			if (expression.Body is UnaryExpression) body = (MemberExpression) ((UnaryExpression) expression.Body).Operand;
//			else body = (MemberExpression) expression.Body;
//
//			return body.Member.Name;
//		}

		private interface ICache<TParam, TVal> {
			TVal Get(TParam key, object context = null);
			int Count { get; }
		}

		private class MemoizingMRUCache3<TParam, TVal>:ICache<TParam, TVal> {
//			private Dictionary<TParam,TVal> _CacheEntries=new Dictionary<TParam, TVal>();
			private readonly Func<TParam, object, TVal> _CalculationFunction;

			public MemoizingMRUCache3(Func<TParam, object, TVal> calculationFunc, int maxSize, Action<TVal> onRelease = null) {
				_CalculationFunction=calculationFunc;
			}

			public TVal Get(TParam key, object context = null) {
//				if (_CacheEntries.ContainsKey(key)) {
//					var found = _CacheEntries[key];
//					return found;
//				}
				var result = _CalculationFunction(key, context);
//				_CacheEntries.Add(key,result);
				return result;
			}

			public int Count { get { return 0; } }

		}

		private class MemoizingMRUCache2b<TParam, TVal>:ICache<TParam, TVal> {
			private Dictionary<int,TVal> _CacheEntries=new Dictionary<int, TVal>();
			private readonly Func<TParam, TVal> _CalculationFunction;

			public MemoizingMRUCache2b(Func<TParam, TVal> calculationFunc, int maxSize, Action<TVal> onRelease = null) {
				_CalculationFunction=calculationFunc;
			}

			public TVal Get(TParam key, object context = null) {
				var hash=key.GetHashCode();
//				var hash = key.ToString().GetHashCode(); //DO NOT CALL ToString() because this will call the ToString from current object
				Debug.WriteLine(String.Format("{0:X8} {1}",hash, ((LambdaExpression)(object)key).Body));
				TVal result;
				if (_CacheEntries.TryGetValue(hash, out result)) return result;
				result = _CalculationFunction(key);
				_CacheEntries.Add(hash,result);
				return result;
			}

			public int Count { get { return _CacheEntries.Count; } }
		}

		private class MemoizingMRUCache2<TParam, TVal>:ICache<TParam, TVal> {
			private Dictionary<TParam,TVal> _CacheEntries=new Dictionary<TParam, TVal>();
			private readonly Func<TParam, TVal> _CalculationFunction;

			public MemoizingMRUCache2(Func<TParam, TVal> calculationFunc, int maxSize, Action<TVal> onRelease = null) {
				_CalculationFunction=calculationFunc;
			}

			public TVal Get(TParam key, object context = null) {
				TVal result;
				if (_CacheEntries.TryGetValue(key, out result)) return result;
				result = _CalculationFunction(key);
				_CacheEntries.Add(key,result);
				return result;
			}

			public int Count { get { return _CacheEntries.Count; } }
		}

		// https://github.com/reactiveui/ReactiveUI/blob/master/ReactiveUI/MemoizingMRUCache.cs

		/// <summary>
		/// This data structure is a representation of a memoizing cache - i.e. a
		/// class that will evaluate a function, but keep a cache of recently
		/// evaluated parameters.
		///
		/// Since this is a memoizing cache, it is important that this function be a
		/// "pure" function in the mathematical sense - that a key *always* maps to
		/// a corresponding return value.
		/// </summary>
		/// <typeparam name="TParam">The type of the parameter to the calculation function.</typeparam>
		/// <typeparam name="TVal">The type of the value returned by the calculation
		/// function.</typeparam>
		private class MemoizingMRUCache<TParam, TVal> {
			private readonly Func<TParam, object, TVal> _calculationFunction;
			private readonly Action<TVal> _releaseFunction;
			private readonly int _maxCacheSize;

			private LinkedList<TParam> _cacheMruList;
			private Dictionary<TParam, Tuple<LinkedListNode<TParam>, TVal>> _cacheEntries;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="calculationFunc">The function whose results you want to cache,
			/// which is provided the key value, and an Tag object that is
			/// user-defined</param>
			/// <param name="maxSize">The size of the cache to maintain, after which old
			/// items will start to be thrown out.</param>
			/// <param name="onRelease">A function to call when a result gets
			/// evicted from the cache (i.e. because Invalidate was called or the
			/// cache is full)</param>
			public MemoizingMRUCache(Func<TParam, object, TVal> calculationFunc, int maxSize, Action<TVal> onRelease = null) {
				Contract.Requires(calculationFunc != null);
				Contract.Requires(maxSize > 0);

				_calculationFunction = calculationFunc;
				_releaseFunction = onRelease;
				_maxCacheSize = maxSize;
				InvalidateAll();
			}

			public TVal Get(TParam key) { return Get(key, null); }

			/// <summary>
			/// Evaluates the function provided, returning the cached value if possible
			/// </summary>
			/// <param name="key">The value to pass to the calculation function.</param>
			/// <param name="context">An additional optional user-specific parameter.</param>
			/// <returns></returns>
			public TVal Get(TParam key, object context = null) {
				Contract.Requires(key != null);

				if (_cacheEntries.ContainsKey(key)) {
					var found = _cacheEntries[key];
					_cacheMruList.Remove(found.Item1);
					_cacheMruList.AddFirst(found.Item1);
					return found.Item2;
				}

//				this.Log().Debug("Cache miss: {0}", key);
				var result = _calculationFunction(key, context);

				var node = new LinkedListNode<TParam>(key);
				_cacheMruList.AddFirst(node);
				_cacheEntries[key] = new Tuple<LinkedListNode<TParam>, TVal>(node, result);
				maintainCache();

				return result;
			}

			public bool TryGet(TParam key, out TVal result) {
				Contract.Requires(key != null);

				Tuple<LinkedListNode<TParam>, TVal> output;
				var ret = _cacheEntries.TryGetValue(key, out output);
				if (ret && output != null) {
					_cacheMruList.Remove(output.Item1);
					_cacheMruList.AddFirst(output.Item1);
					result = output.Item2;
				}
				else {
//					this.Log().Debug("Cache miss: {0}", key);
					result = default(TVal);
				}
				return ret;
			}

			/// <summary>
			/// Ensure that the next time this key is queried, the calculation
			/// function will be called.
			/// </summary>
			public void Invalidate(TParam key) {
				Contract.Requires(key != null);

				if (!_cacheEntries.ContainsKey(key)) return;

				var to_remove = _cacheEntries[key];
				if (_releaseFunction != null) _releaseFunction(to_remove.Item2);

				_cacheMruList.Remove(to_remove.Item1);
				_cacheEntries.Remove(key);
			}

			/// <summary>
			/// Invalidate all items in the cache
			/// </summary>
			public void InvalidateAll() {
				if (_releaseFunction == null || _cacheEntries == null) {
					_cacheMruList = new LinkedList<TParam>();
					_cacheEntries = new Dictionary<TParam, Tuple<LinkedListNode<TParam>, TVal>>();
					return;
				}

				if (_cacheEntries.Count == 0) return;

				/* We have to remove them one-by-one to call the release function
* We ToArray() this so we don't get a "modifying collection while
* enumerating" exception. */
				foreach (var v in _cacheEntries.Keys.ToArray()) { Invalidate(v); }
			}

			/// <summary>
			/// Returns all values currently in the cache
			/// </summary>
			/// <returns></returns>
			public IEnumerable<TVal> CachedValues() { return _cacheEntries.Select(x => x.Value.Item2); }

			private void maintainCache() {
				while (_cacheMruList.Count > _maxCacheSize) {
					var to_remove = _cacheMruList.Last.Value;
					if (_releaseFunction != null) _releaseFunction(_cacheEntries[to_remove].Item2);

//					this.Log().Debug("Evicting {0}", to_remove);
					_cacheEntries.Remove(_cacheMruList.Last.Value);
					_cacheMruList.RemoveLast();
				}
			}

			[ContractInvariantMethod]
			private void Invariants() {
				Contract.Invariant(_cacheEntries.Count == _cacheMruList.Count);
				Contract.Invariant(_cacheEntries.Count <= _maxCacheSize);
			}

			public int Count { get { return _cacheEntries.Count; } }
		}
		
//		/// <summary>
//		/// x => x.Prop1.Prop2
//		/// </summary>
//		/// <param name="expression"></param>
//		/// <returns></returns>
//		public static string[] SplitPropertyPath(Expression<Func<object, object>> expression) { return SplitPropertyPath(expression); }

		public static string[] SplitPropertyPath(LambdaExpression expression) {
			MemberExpression body;
			if (expression.Body is UnaryExpression) {
				body = (MemberExpression) ((UnaryExpression) expression.Body).Operand;
				return new[] {body.Member.Name};
			}
			else {
				body = (MemberExpression) expression.Body;
				// "x.Prop1.Prop2"
				var path = expression.Body.ToString().Split('.').ToArray();
				return path;
			}
		}
	}
}
