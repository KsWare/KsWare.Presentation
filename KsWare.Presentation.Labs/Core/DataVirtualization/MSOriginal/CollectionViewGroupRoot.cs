// Type: MS.Internal.Data.CollectionViewGroupRoot
// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0F086D49-4CAD-43AD-A3E2-A5268BD16302
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationFramework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime;
using System.Windows;
using System.Windows.Data;
using MS.Internal;
using MS.Internal.Data;

namespace KsWare.Presentation.DataVirtualization.MSOriginal {

	internal class CollectionViewGroupRoot : CollectionViewGroupInternal, INotifyCollectionChanged {

		private static readonly object UseAsItemDirectly = (object) new NamedObject("UseAsItemDirectly");
		private ObservableCollection<GroupDescription> _groupBy = new ObservableCollection<GroupDescription>();
		private CollectionView _view;
		private IComparer _comparer;
		private bool _isDataInGroupOrder;
		private GroupDescriptionSelectorCallback _groupBySelector;
		private static GroupDescription _topLevelGroupDescription;

		public virtual ObservableCollection<GroupDescription> GroupDescriptions {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._groupBy; }
		}

		public virtual GroupDescriptionSelectorCallback GroupBySelector {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._groupBySelector; }
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set { this._groupBySelector = value; }
		}

		internal IComparer ActiveComparer {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._comparer; }
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set { this._comparer = value; }
		}

		internal CultureInfo Culture { get { return this._view.Culture; } }

		internal bool IsDataInGroupOrder {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._isDataInGroupOrder; }
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set { this._isDataInGroupOrder = value; }
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		internal event EventHandler GroupDescriptionChanged;

		static CollectionViewGroupRoot() { }

		internal CollectionViewGroupRoot(CollectionViewOriginal view)
			: base((object) "Root", (CollectionViewGroupInternal) null) { this._view = view; }

		public void OnCollectionChanged(NotifyCollectionChangedEventArgs args) {
			if (args == null) throw new ArgumentNullException("args");
			if (this.CollectionChanged == null) return;
			this.CollectionChanged((object) this, args);
		}

		protected override void OnGroupByChanged() {
			if (this.GroupDescriptionChanged == null) return;
			this.GroupDescriptionChanged((object) this, EventArgs.Empty);
		}

		internal void Initialize() {
			if (CollectionViewGroupRoot._topLevelGroupDescription == null) CollectionViewGroupRoot._topLevelGroupDescription = (GroupDescription) new CollectionViewGroupRoot.TopLevelGroupDescription();
			this.InitializeGroup((CollectionViewGroupInternal) this, CollectionViewGroupRoot._topLevelGroupDescription, 0);
		}

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		internal void AddToSubgroups(object item, LiveShapingItem lsi, bool loading) { this.AddToSubgroups(item, lsi, (CollectionViewGroupInternal) this, 0, loading); }

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		internal bool RemoveFromSubgroups(object item) { return this.RemoveFromSubgroups(item, (CollectionViewGroupInternal) this, 0); }

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		internal void RemoveItemFromSubgroupsByExhaustiveSearch(object item) { this.RemoveItemFromSubgroupsByExhaustiveSearch((CollectionViewGroupInternal) this, item); }

		internal void InsertSpecialItem(int index, object item, bool loading) {
			this.ChangeCounts(item, 1);
			this.ProtectedItems.Insert(index, item);
			if (loading) return;
			int index1 = this.LeafIndexFromItem(item, index);
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index1));
		}

		internal void RemoveSpecialItem(int index, object item, bool loading) {
			int index1 = -1;
			if (!loading) index1 = this.LeafIndexFromItem(item, index);
			this.ChangeCounts(item, -1);
			this.ProtectedItems.RemoveAt(index);
			if (loading) return;
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index1));
		}

		internal void MoveWithinSubgroups(object item, LiveShapingItem lsi, IList list, int oldIndex, int newIndex) {
			if (lsi == null) { this.MoveWithinSubgroups(item, (CollectionViewGroupInternal) this, 0, list, oldIndex, newIndex); } else {
				CollectionViewGroupInternal parentGroup = lsi.ParentGroup;
				if (parentGroup != null) { this.MoveWithinSubgroup(item, parentGroup, list, oldIndex, newIndex); } else { foreach (CollectionViewGroupInternal group in lsi.ParentGroups) this.MoveWithinSubgroup(item, group, list, oldIndex, newIndex); }
			}
		}

		protected override int FindIndex(object item, object seed, IComparer comparer, int low, int high) {
			IEditableCollectionView editableCollectionView = this._view as IEditableCollectionView;
			if (editableCollectionView != null) {
				if (editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) {
					++low;
					if (editableCollectionView.IsAddingNew) ++low;
				} else {
					if (editableCollectionView.IsAddingNew) --high;
					if (editableCollectionView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd) --high;
				}
			}
			return base.FindIndex(item, seed, comparer, low, high);
		}

		internal void RestoreGrouping(LiveShapingItem lsi, List<AbandonedGroupItem> deleteList) {
			CollectionViewGroupRoot.GroupTreeNode node = this.BuildGroupTree(lsi);
			node.ContainsItem = true;
			this.RestoreGrouping(lsi, node, 0, deleteList);
		}

		internal void DeleteAbandonedGroupItems(List<AbandonedGroupItem> deleteList) {
			foreach (AbandonedGroupItem abandonedGroupItem in deleteList) {
				this.RemoveFromGroupDirectly(abandonedGroupItem.Group, abandonedGroupItem.Item.Item);
				abandonedGroupItem.Item.RemoveParentGroup(abandonedGroupItem.Group);
			}
		}

		private void AddToSubgroups(object item, LiveShapingItem lsi, CollectionViewGroupInternal group, int level, bool loading) {
			object groupName = this.GetGroupName(item, group.GroupBy, level);
			if (groupName == CollectionViewGroupRoot.UseAsItemDirectly) {
				if (lsi != null) lsi.AddParentGroup(group);
				if (loading) { group.Add(item); } else {
					int index1 = group.Insert(item, item, this.ActiveComparer);
					int index2 = group.LeafIndexFromItem(item, index1);
					this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index2));
				}
			} else {
				ICollection collection;
				if ((collection = groupName as ICollection) == null) { this.AddToSubgroup(item, lsi, group, level, groupName, loading); } else { foreach (object name in (IEnumerable) collection) this.AddToSubgroup(item, lsi, group, level, name, loading); }
			}
		}

		private bool RemoveFromSubgroups(object item, CollectionViewGroupInternal group, int level) {
			bool flag = false;
			object groupName = this.GetGroupName(item, group.GroupBy, level);
			if (groupName == CollectionViewGroupRoot.UseAsItemDirectly) { flag = this.RemoveFromGroupDirectly(group, item); } else {
				ICollection collection;
				if ((collection = groupName as ICollection) == null) { if (this.RemoveFromSubgroup(item, group, level, groupName)) flag = true; } else { foreach (object name in (IEnumerable) collection) { if (this.RemoveFromSubgroup(item, group, level, name)) flag = true; } }
			}
			return flag;
		}

		private void RemoveItemFromSubgroupsByExhaustiveSearch(CollectionViewGroupInternal group, object item) {
			if (!this.RemoveFromGroupDirectly(group, item)) return;
			for (int index = group.Items.Count - 1; index >= 0; --index) {
				CollectionViewGroupInternal group1 = group.Items[index] as CollectionViewGroupInternal;
				if (group1 != null) this.RemoveItemFromSubgroupsByExhaustiveSearch(group1, item);
			}
		}

		private void RestoreGrouping(LiveShapingItem lsi, CollectionViewGroupRoot.GroupTreeNode node, int level, List<AbandonedGroupItem> deleteList) {
			if (node.ContainsItem) {
				object obj = this.GetGroupName(lsi.Item, node.Group.GroupBy, level);
				if (obj != CollectionViewGroupRoot.UseAsItemDirectly) {
					ICollection c = obj as ICollection;
					ArrayList arrayList = c == null ? (ArrayList) null : new ArrayList(c);
					for (CollectionViewGroupRoot.GroupTreeNode groupTreeNode = node.FirstChild; groupTreeNode != null; groupTreeNode = groupTreeNode.Sibling) {
						if (arrayList == null) {
							if (object.Equals(obj, groupTreeNode.Group.Name)) {
								groupTreeNode.ContainsItem = true;
								obj = DependencyProperty.UnsetValue;
								break;
							}
						} else if (arrayList.Contains(groupTreeNode.Group.Name)) {
							groupTreeNode.ContainsItem = true;
							arrayList.Remove(groupTreeNode.Group.Name);
						}
					}
					if (arrayList == null) { if (obj != DependencyProperty.UnsetValue) this.AddToSubgroup(lsi.Item, lsi, node.Group, level, obj, false); } else { foreach (object name in arrayList) this.AddToSubgroup(lsi.Item, lsi, node.Group, level, name, false); }
				}
			} else if (node.ContainsItemDirectly) deleteList.Add(new AbandonedGroupItem(lsi, node.Group));
			for (CollectionViewGroupRoot.GroupTreeNode node1 = node.FirstChild; node1 != null; node1 = node1.Sibling) this.RestoreGrouping(lsi, node1, level + 1, deleteList);
		}

		private CollectionViewGroupRoot.GroupTreeNode BuildGroupTree(LiveShapingItem lsi) {
			CollectionViewGroupInternal viewGroupInternal1 = lsi.ParentGroup;
			if (viewGroupInternal1 != null) {
				CollectionViewGroupRoot.GroupTreeNode groupTreeNode = new CollectionViewGroupRoot.GroupTreeNode() {
					Group = viewGroupInternal1,
					ContainsItemDirectly = true
				};
				while (true) {
					viewGroupInternal1 = viewGroupInternal1.Parent;
					if (viewGroupInternal1 != null)
						groupTreeNode = new CollectionViewGroupRoot.GroupTreeNode() {
							Group = viewGroupInternal1,
							FirstChild = groupTreeNode
						};
					else break;
				}
				return groupTreeNode;
			} else {
				List<CollectionViewGroupInternal> parentGroups = lsi.ParentGroups;
				List<CollectionViewGroupRoot.GroupTreeNode> list = new List<CollectionViewGroupRoot.GroupTreeNode>(parentGroups.Count + 1);
				CollectionViewGroupRoot.GroupTreeNode groupTreeNode1 = (CollectionViewGroupRoot.GroupTreeNode) null;
				foreach (CollectionViewGroupInternal viewGroupInternal2 in parentGroups) {
					CollectionViewGroupRoot.GroupTreeNode groupTreeNode2 = new CollectionViewGroupRoot.GroupTreeNode() {
						Group = viewGroupInternal2,
						ContainsItemDirectly = true
					};
					list.Add(groupTreeNode2);
				}
				for (int index1 = 0; index1 < list.Count; ++index1) {
					CollectionViewGroupRoot.GroupTreeNode groupTreeNode2 = list[index1];
					CollectionViewGroupInternal parent = groupTreeNode2.Group.Parent;
					CollectionViewGroupRoot.GroupTreeNode groupTreeNode3 = (CollectionViewGroupRoot.GroupTreeNode) null;
					if (parent == null) { groupTreeNode1 = groupTreeNode2; } else {
						for (int index2 = list.Count - 1; index2 >= 0; --index2) {
							if (list[index2].Group == parent) {
								groupTreeNode3 = list[index2];
								break;
							}
						}
						if (groupTreeNode3 == null) {
							CollectionViewGroupRoot.GroupTreeNode groupTreeNode4 = new CollectionViewGroupRoot.GroupTreeNode() {
								Group = parent,
								FirstChild = groupTreeNode2
							};
							list.Add(groupTreeNode4);
						} else {
							groupTreeNode2.Sibling = groupTreeNode3.FirstChild;
							groupTreeNode3.FirstChild = groupTreeNode2;
						}
					}
				}
				return groupTreeNode1;
			}
		}

		private void InitializeGroup(CollectionViewGroupInternal group, GroupDescription parentDescription, int level) {
			GroupDescription groupDescription = this.GetGroupDescription((CollectionViewGroup) group, parentDescription, level);
			group.GroupBy = groupDescription;
			ObservableCollection<object> observableCollection = groupDescription != null ? groupDescription.GroupNames : (ObservableCollection<object>) null;
			if (observableCollection != null) {
				int index = 0;
				for (int count = observableCollection.Count; index < count; ++index) {
					CollectionViewGroupInternal group1 = new CollectionViewGroupInternal(observableCollection[index], group);
					this.InitializeGroup(group1, groupDescription, level + 1);
					group.Add((object) group1);
				}
			}
			group.LastIndex = 0;
		}

		private GroupDescription GetGroupDescription(CollectionViewGroup group, GroupDescription parentDescription, int level) {
			GroupDescription groupDescription = (GroupDescription) null;
			if (group == this) group = (CollectionViewGroup) null;
			if (groupDescription == null && this.GroupBySelector != null) groupDescription = this.GroupBySelector(group, level);
			if (groupDescription == null && level < this.GroupDescriptions.Count) groupDescription = this.GroupDescriptions[level];
			return groupDescription;
		}

		private void AddToSubgroup(object item, LiveShapingItem lsi, CollectionViewGroupInternal group, int level, object name, bool loading) {
			int index = !loading || !this.IsDataInGroupOrder ? 0 : group.LastIndex;
			object groupNameKey = this.GetGroupNameKey(name, group);
			CollectionViewGroupInternal subgroupFromMap;
			if ((subgroupFromMap = group.GetSubgroupFromMap(groupNameKey)) != null && group.GroupBy.NamesMatch(subgroupFromMap.Name, name)) {
				group.LastIndex = group.Items[index] == subgroupFromMap ? index : 0;
				this.AddToSubgroups(item, lsi, subgroupFromMap, level + 1, loading);
			} else {
				for (int count = group.Items.Count; index < count; ++index) {
					CollectionViewGroupInternal viewGroupInternal = group.Items[index] as CollectionViewGroupInternal;
					if (viewGroupInternal != null && group.GroupBy.NamesMatch(viewGroupInternal.Name, name)) {
						group.LastIndex = index;
						group.AddSubgroupToMap(groupNameKey, viewGroupInternal);
						this.AddToSubgroups(item, lsi, viewGroupInternal, level + 1, loading);
						return;
					}
				}
				CollectionViewGroupInternal viewGroupInternal1 = new CollectionViewGroupInternal(name, group);
				this.InitializeGroup(viewGroupInternal1, group.GroupBy, level + 1);
				if (loading) {
					group.Add((object) viewGroupInternal1);
					group.LastIndex = index;
				} else group.Insert((object) viewGroupInternal1, item, this.ActiveComparer);
				group.AddSubgroupToMap(groupNameKey, viewGroupInternal1);
				this.AddToSubgroups(item, lsi, viewGroupInternal1, level + 1, loading);
			}
		}

		private void MoveWithinSubgroups(object item, CollectionViewGroupInternal group, int level, IList list, int oldIndex, int newIndex) {
			object groupName = this.GetGroupName(item, group.GroupBy, level);
			if (groupName == CollectionViewGroupRoot.UseAsItemDirectly) { this.MoveWithinSubgroup(item, group, list, oldIndex, newIndex); } else {
				ICollection collection;
				if ((collection = groupName as ICollection) == null) { this.MoveWithinSubgroup(item, group, level, groupName, list, oldIndex, newIndex); } else { foreach (object name in (IEnumerable) collection) this.MoveWithinSubgroup(item, group, level, name, list, oldIndex, newIndex); }
			}
		}

		private void MoveWithinSubgroup(object item, CollectionViewGroupInternal group, int level, object name, IList list, int oldIndex, int newIndex) {
			object groupNameKey = this.GetGroupNameKey(name, group);
			CollectionViewGroupInternal subgroupFromMap;
			if ((subgroupFromMap = group.GetSubgroupFromMap(groupNameKey)) != null && group.GroupBy.NamesMatch(subgroupFromMap.Name, name)) { this.MoveWithinSubgroups(item, subgroupFromMap, level + 1, list, oldIndex, newIndex); } else {
				int index = 0;
				for (int count = group.Items.Count; index < count; ++index) {
					CollectionViewGroupInternal viewGroupInternal = group.Items[index] as CollectionViewGroupInternal;
					if (viewGroupInternal != null && group.GroupBy.NamesMatch(viewGroupInternal.Name, name)) {
						group.AddSubgroupToMap(groupNameKey, viewGroupInternal);
						this.MoveWithinSubgroups(item, viewGroupInternal, level + 1, list, oldIndex, newIndex);
						break;
					}
				}
			}
		}

		private void MoveWithinSubgroup(object item, CollectionViewGroupInternal group, IList list, int oldIndex, int newIndex) {
			if (!group.Move(item, list, ref oldIndex, ref newIndex)) return;
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
		}

		private object GetGroupNameKey(object name, CollectionViewGroupInternal group) {
			object obj = name;
			PropertyGroupDescription groupDescription = group.GroupBy as PropertyGroupDescription;
			if (groupDescription != null) {
				string str = name as string;
				if (str != null) {
					if (groupDescription.StringComparison == StringComparison.OrdinalIgnoreCase || groupDescription.StringComparison == StringComparison.InvariantCultureIgnoreCase) str = str.ToUpperInvariant();
					else if (groupDescription.StringComparison == StringComparison.CurrentCultureIgnoreCase) str = str.ToUpper(CultureInfo.CurrentCulture);
					obj = (object) str;
				}
			}
			return obj;
		}

		private bool RemoveFromSubgroup(object item, CollectionViewGroupInternal group, int level, object name) {
			object groupNameKey = this.GetGroupNameKey(name, group);
			CollectionViewGroupInternal subgroupFromMap;
			if ((subgroupFromMap = group.GetSubgroupFromMap(groupNameKey)) != null && group.GroupBy.NamesMatch(subgroupFromMap.Name, name)) return this.RemoveFromSubgroups(item, subgroupFromMap, level + 1);
			int index = 0;
			for (int count = group.Items.Count; index < count; ++index) {
				CollectionViewGroupInternal group1 = group.Items[index] as CollectionViewGroupInternal;
				if (group1 != null && group.GroupBy.NamesMatch(group1.Name, name)) return this.RemoveFromSubgroups(item, group1, level + 1);
			}
			return true;
		}

		private bool RemoveFromGroupDirectly(CollectionViewGroupInternal group, object item) {
			int index = group.Remove(item, true);
			if (index < 0) return true;
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
			return false;
		}

		private object GetGroupName(object item, GroupDescription groupDescription, int level) {
			if (groupDescription != null) return groupDescription.GroupNameFromItem(item, level, this.Culture);
			else return CollectionViewGroupRoot.UseAsItemDirectly;
		}

		private class GroupTreeNode {

			public CollectionViewGroupRoot.GroupTreeNode FirstChild { get; set; }

			public CollectionViewGroupRoot.GroupTreeNode Sibling { get; set; }

			public CollectionViewGroupInternal Group { get; set; }

			public bool ContainsItem { get; set; }

			public bool ContainsItemDirectly { get; set; }

		}

		private class TopLevelGroupDescription : GroupDescription {

			public override object GroupNameFromItem(object item, int level, CultureInfo culture) { throw new NotSupportedException(); }

		}

	}

}