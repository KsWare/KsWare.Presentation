using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using KsWare.Presentation.Collections;
using NUnit.Framework;

namespace KsWare.Presentation.Core.Tests.Collections {

	[TestFixture()]
	public class ObservableDictionaryTests {

		[Test()]
		public void ObservableDictionaryTest() {
			var dic = new ObservableDictionary<int,string>();
		}

		[Test()]
		public void ObservableDictionaryTest1() {
			var dic0 = new Dictionary<string, string>() {{"a", "1"}};
			var dic = new ObservableDictionary<string, string>(dic0);
			dic.ContainsKey("a").Should().BeTrue();
		}

		[Test()]
		public void ObservableDictionaryTest2() {
			var dic0 = new Dictionary<string, string>() {{"a", "1"}};
			var dic = new ObservableDictionary<string, string>(dic0, StringComparer.OrdinalIgnoreCase);
			dic.ContainsKey("A").Should().BeTrue();
		}

		[Test()]
		public void ObservableDictionaryTest3() {
			var dic = new ObservableDictionary<string, string>(StringComparer.OrdinalIgnoreCase) {{"a", "1"}};
			dic.ContainsKey("A").Should().BeTrue();
		}

		[Test()]
		public void ObservableDictionaryTest4() {
			var dic = new ObservableDictionary<string, string>(1,StringComparer.OrdinalIgnoreCase) {{"a", "1"}};
			dic.ContainsKey("A").Should().BeTrue();
		}

		[Test()]
		public void ObservableDictionaryTest5() {
			var dic = new ObservableDictionary<int, string>(1);
			dic.Count.Should().Be(0);
		}

		[Test()]
		public void AddTest() {
			var dic = new ObservableDictionary<int, string>();
			List<NotifyCollectionChangedEventArgs> collectionsEvents = new List<NotifyCollectionChangedEventArgs>();
			List<PropertyChangedEventArgs> propertyEvents = new List<PropertyChangedEventArgs>();
			((INotifyCollectionChanged) dic).CollectionChanged += (s, e) => collectionsEvents.Add(e);
			((INotifyPropertyChanged) dic).PropertyChanged += (s, e) => propertyEvents.Add(e);

			dic.Add(1,"1");
			collectionsEvents[0].Action.Should().Be(NotifyCollectionChangedAction.Add);
			propertyEvents.First(e => e.PropertyName == "Count").Should().NotBeNull();
			dic.Count.Should().Be(1);
		}

		[Test()]
		public void AddTest1() {
			var                                    dic               = new ObservableDictionary<int, string>();
			List<NotifyCollectionChangedEventArgs> collectionsEvents = new List<NotifyCollectionChangedEventArgs>();
			List<PropertyChangedEventArgs>         propertyEvents    = new List<PropertyChangedEventArgs>();
			var                                    entry             = new KeyValuePair<int, string>(1, "1");
			((INotifyCollectionChanged) dic).CollectionChanged += (s, e) => collectionsEvents.Add(e);
			((INotifyPropertyChanged) dic).PropertyChanged     += (s, e) => propertyEvents.Add(e);

			dic.Add(entry);
			collectionsEvents[0].Action.Should().Be(NotifyCollectionChangedAction.Add);
			propertyEvents.First(e => e.PropertyName == "Count").Should().NotBeNull();
			dic.Count.Should().Be(1);
		}

		[Test()]
		public void ContainsKeyTest() {
			var dic = new ObservableDictionary<int, string>();
			dic.Add(1,"1");
			dic.ContainsKey(1).Should().BeTrue();
		}

		[Test()]
		public void RemoveTest() {
			var                                    dic               = new ObservableDictionary<int, string>();
			List<NotifyCollectionChangedEventArgs> collectionsEvents = new List<NotifyCollectionChangedEventArgs>();
			List<PropertyChangedEventArgs>         propertyEvents    = new List<PropertyChangedEventArgs>();
			dic.Add(1, "1");
			((INotifyCollectionChanged) dic).CollectionChanged += (s, e) => collectionsEvents.Add(e);
			((INotifyPropertyChanged) dic).PropertyChanged     += (s, e) => propertyEvents.Add(e);

			dic.Remove(1);
			collectionsEvents[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
			propertyEvents.First(e => e.PropertyName == "Count").Should().NotBeNull();
			dic.Count.Should().Be(0);
		}

		[Test()]
		public void RemoveTest1() {
			var                                    dic               = new ObservableDictionary<int, string>();
			List<NotifyCollectionChangedEventArgs> collectionsEvents = new List<NotifyCollectionChangedEventArgs>();
			List<PropertyChangedEventArgs>         propertyEvents    = new List<PropertyChangedEventArgs>();
			var entry=new KeyValuePair<int,string>(1,"1");
			dic.Add(entry);
			((INotifyCollectionChanged) dic).CollectionChanged += (s, e) => collectionsEvents.Add(e);
			((INotifyPropertyChanged) dic).PropertyChanged     += (s, e) => propertyEvents.Add(e);

			dic.Remove(entry);
			collectionsEvents[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
			propertyEvents.First(e => e.PropertyName == "Count").Should().NotBeNull();
			dic.Count.Should().Be(0);
		}

		[Test()]
		public void TryGetValueTest() {
			var dic = new ObservableDictionary<int, string>();
			string value;
			dic.TryGetValue(1,out value).Should().BeFalse();
			dic.Add(1, "1");
			dic.Add(2, "2");
			dic.TryGetValue(1,out value).Should().BeTrue();
			value.Should().Be("1");
		}

		[Test()]
		public void ClearTest() {
			var                                    dic               = new ObservableDictionary<int, string>();
			List<NotifyCollectionChangedEventArgs> collectionsEvents = new List<NotifyCollectionChangedEventArgs>();
			List<PropertyChangedEventArgs>         propertyEvents    = new List<PropertyChangedEventArgs>();
			dic.Add(1, "1");
			((INotifyCollectionChanged) dic).CollectionChanged += (s, e) => collectionsEvents.Add(e);
			((INotifyPropertyChanged) dic).PropertyChanged     += (s, e) => propertyEvents.Add(e);

			dic.Clear();
			collectionsEvents[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
			propertyEvents.First(e => e.PropertyName == "Count").Should().NotBeNull();
			dic.Count.Should().Be(0);
		}

		[Test()]
		public void ContainsTest() {
			var    dic = new ObservableDictionary<int, string>();
			dic.Contains(new KeyValuePair<int, string>(1, null)).Should().BeFalse();
			dic.Add(1, "1"); dic.Add(2, "2");
			dic.Contains(new KeyValuePair<int, string>(1, "1")).Should().BeTrue();
		}

		[Test()]
		public void CopyToTest() {
			var dic = new ObservableDictionary<int, string>();
			dic.Add(1, "1");
			var array=new KeyValuePair<int, string>[1];
			dic.CopyTo(array,0);
			array[0].Key.Should().Be(1);
			array[0].Value.Should().Be("1");
		}

		[Test()]
		public void GetEnumeratorTest() {
			var dic = new ObservableDictionary<int, string>();
			dic.Add(1, "1"); 
			using (var enumerator = dic.GetEnumerator()) {
				enumerator.Reset();
				enumerator.MoveNext().Should().BeTrue();
				enumerator.Current.Key.Should().Be(1);
				enumerator.Current.Value.Should().Be("1");
				enumerator.MoveNext().Should().BeFalse();
			}

		}

		[Test()]
		public void AddRangeTest() {
			var                                    dic               = new ObservableDictionary<int, string>();
			List<NotifyCollectionChangedEventArgs> collectionsEvents = new List<NotifyCollectionChangedEventArgs>();
			List<PropertyChangedEventArgs>         propertyEvents    = new List<PropertyChangedEventArgs>();
			((INotifyCollectionChanged) dic).CollectionChanged += (s, e) => collectionsEvents.Add(e);
			((INotifyPropertyChanged) dic).PropertyChanged     += (s, e) => propertyEvents.Add(e);

			dic.AddRange(new Dictionary<int, string>(){{1,"1"},{2, "2"}});

			collectionsEvents[0].Action.Should().Be(NotifyCollectionChangedAction.Add);
			propertyEvents.First(e => e.PropertyName == "Count").Should().NotBeNull();
			dic.Count.Should().Be(2);
		}
	}
}