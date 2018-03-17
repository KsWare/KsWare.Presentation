using System;
using System.Windows;
using System.Windows.Interactivity;
using KsWare.Presentation.ViewFramework.Behaviors.DragDrop;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	public class FrameworkElementDropBehavior : Behavior<FrameworkElement> {

		private Type _dataType; //the type of the data that can be dropped into this control
		private FrameworkElementAdorner _adorner;
		private Type _adornerType;

		public FrameworkElementDropBehavior() {
			AdornerType = typeof (FrameworkElementAdorner);
		}

		public Thickness DropRegion { get; set; }

		public Type AdornerType {
			get => _adornerType;
			set {
				if(!typeof(FrameworkElementAdorner).IsAssignableFrom(value)) throw new InvalidOperationException("Invalid type of adorner. Type of FrameworkElementAdorner expected.");
				_adornerType = value;
			}
		}

		protected override void OnAttached() {
			base.OnAttached();

			AssociatedObject.AllowDrop = true;
			AssociatedObject.DragEnter += AtDragEnter;
			AssociatedObject.DragOver  += AtDragOver;
			AssociatedObject.DragLeave += AtDragLeave;
			AssociatedObject.Drop      += AtDrop;
		}

		private void AtDrop(object sender, DragEventArgs e) {
			if (_dataType != null) {
				//if the data type can be dropped 
				if (e.Data.GetDataPresent(_dataType)) {
					var pos=UIHelper.GetDropPosition(AssociatedObject, e.GetPosition(AssociatedObject), DropRegion);

					//drop the data
					var target = AssociatedObject.DataContext as IDropable;
					target.Drop(e.Data.GetData(_dataType),-1,pos);

					//remove the data from the source
					var source = e.Data.GetData(_dataType) as IDragable;
					source.Remove(e.Data.GetData(_dataType));
				}
			}
			if (_adorner != null)_adorner.Remove();

			e.Handled = true;
		}


		private void AtDragEnter(object sender, DragEventArgs e) {
			//if the DataContext implements IDropable, record the data type that can be dropped
			if (_dataType == null) {
				if (AssociatedObject.DataContext != null) {
					var dropObject = AssociatedObject.DataContext as IDropable;
					if (dropObject != null) {
						_dataType = dropObject.DataType;
					}
				}
			}

			if (_adorner == null) _adorner = (FrameworkElementAdorner)Activator.CreateInstance(AdornerType,sender);
			e.Handled = true;
		}

		private void AtDragOver(object sender, DragEventArgs e) {
			if (_dataType != null) {
				//if item can be dropped
				if (e.Data.GetDataPresent(_dataType)) {
					var pos=UIHelper.GetDropPosition(AssociatedObject, e.GetPosition(AssociatedObject), DropRegion);

					var target = AssociatedObject.DataContext as IDropable;
					var canDrop=target.CanDrop(e.Data.GetData(_dataType),-1,pos);

					//give mouse effect
					if(!canDrop) e.Effects=DragDropEffects.None;
					else e.Effects=DragDropEffects.Move;

					//draw the dots
					if (_adorner != null) {
						_adorner.DropEffect = canDrop ? DragDropEffects.Move : DragDropEffects.None;
						_adorner.Position = pos;
						_adorner.Update();
					}
				}
			}
			e.Handled = true;
		}

		private void AtDragLeave(object sender, DragEventArgs e) {
			if (_adorner != null) _adorner.Remove();
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