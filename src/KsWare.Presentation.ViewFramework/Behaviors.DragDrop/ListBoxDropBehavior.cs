﻿using System;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Documents;
using JetBrains.Annotations;
using KsWare.Presentation.ViewFramework.Behaviors.DragDrop;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	/// <summary> For enabling Drop on ItemsControl
	/// </summary>
	[PublicAPI]
	public class ListBoxDropBehavior : Behavior<ItemsControl> {

		private Type _dataType; //the type of the data that can be dropped into this control
		private ListBoxAdornerManager _insertAdornerManager;

		protected override void OnAttached() {
			base.OnAttached();

			AssociatedObject.AllowDrop = true;
			AssociatedObject.DragEnter += AtDragEnter;
			AssociatedObject.DragOver  += AtDragOver;
			AssociatedObject.DragLeave += AtDragLeave;
			AssociatedObject.Drop      += AtDrop;
		}

		private void AtDrop(object sender, DragEventArgs e) {
			//if the data type can be dropped 
			if (_dataType != null) {
				if (e.Data.GetDataPresent(_dataType)) {
					//first find the UIElement that it was dropped over, then we determine if it's 
					//dropped above or under the UIElement, then insert at the correct index.
					var dropContainer = (ItemsControl)sender;
					//get the UIElement that was dropped over
					var droppedOverItem = UIHelper.GetUIElement(dropContainer, e.GetPosition(dropContainer));
					int dropIndex = -1; //the location where the item will be dropped
					dropIndex = dropContainer.ItemContainerGenerator.IndexFromContainer(droppedOverItem) + 1;
					//find if it was dropped above or below the index item so that we can insert 
					//the item in the correct place
					if (UIHelper.IsPositionAboveElement(droppedOverItem, e.GetPosition(droppedOverItem))){ //if above
						dropIndex = dropIndex - 1; //we insert at the index above it
					}
					//remove the data from the source
					var source = (IDragable)e.Data.GetData(_dataType);
					source.Remove(e.Data.GetData(_dataType));

					//drop the data
					var target = (IDropable)AssociatedObject.DataContext;
					target.Drop(e.Data.GetData(_dataType), dropIndex);
				}
			}
			if (_insertAdornerManager != null)
				_insertAdornerManager.Clear();
			e.Handled = true;
		}

		private void AtDragLeave(object sender, DragEventArgs e) {
			if (_insertAdornerManager != null)
				_insertAdornerManager.Clear();
			e.Handled = true;
		}

		private void AtDragOver(object sender, DragEventArgs e) {
			if (_dataType != null) {
				if (e.Data.GetDataPresent(_dataType)) {
					SetDragDropEffects(e);
					if (_insertAdornerManager != null) {
						var dropContainer = (ItemsControl)sender;
						var droppedOverItem = UIHelper.GetUIElement(dropContainer, e.GetPosition(dropContainer));
						var isAboveElement = UIHelper.IsPositionAboveElement(droppedOverItem, e.GetPosition(droppedOverItem));
						_insertAdornerManager.Update(droppedOverItem, isAboveElement);
					}
				}
			}
			e.Handled = true;
		}

		private void AtDragEnter(object sender, DragEventArgs e) {
			if (_dataType == null) {
				//if the DataContext implements IDropable, record the data type that can be dropped
				if (AssociatedObject.DataContext != null) {
					if (AssociatedObject.DataContext as IDropable != null) {
						_dataType = ((IDropable) AssociatedObject.DataContext).DataType;
					}
				}
			}
			//initialize adorner manager with the adorner layer of the itemsControl
			if (_insertAdornerManager == null)
				_insertAdornerManager = new ListBoxAdornerManager(AdornerLayer.GetAdornerLayer((ItemsControl)sender));

			e.Handled = true;
		}

		/// <summary> Provides feedback on if the data can be dropped
		/// </summary>
		/// <param name="e"></param>
		private void SetDragDropEffects(DragEventArgs e) {
			e.Effects = DragDropEffects.None; //default to None

			//if the data type can be dropped 
			if (e.Data.GetDataPresent(_dataType)) {
				e.Effects = DragDropEffects.Move;
			}
		}
	}
}