using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interactivity;
using KsWare.Presentation.ViewFramework.Behaviors.DragDrop;

namespace KsWare.Presentation.ViewFramework.Behaviors {

	public class FrameworkElementDropBehavior : Behavior<FrameworkElement> {

		private Type m_DataType; //the type of the data that can be dropped into this control
		private FrameworkElementAdorner m_Adorner;
		private Type m_AdornerType;

		public FrameworkElementDropBehavior() {
			AdornerType = typeof (FrameworkElementAdorner);
		}

		public Thickness DropRegion { get; set; }

		public Type AdornerType {
			get { return m_AdornerType; }
			set {
				if(!typeof(FrameworkElementAdorner).IsAssignableFrom(value)) throw new InvalidOperationException("Invalid type of adorner. Type of FrameworkElementAdorner expected.");
				m_AdornerType = value;
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
			if (m_DataType != null) {
				//if the data type can be dropped 
				if (e.Data.GetDataPresent(m_DataType)) {
					var pos=UIHelper.GetDropPosition(AssociatedObject, e.GetPosition(AssociatedObject), DropRegion);

					//drop the data
					var target = AssociatedObject.DataContext as IDropable;
					target.Drop(e.Data.GetData(m_DataType),-1,pos);

					//remove the data from the source
					var source = e.Data.GetData(m_DataType) as IDragable;
					source.Remove(e.Data.GetData(m_DataType));
				}
			}
			if (m_Adorner != null)m_Adorner.Remove();

			e.Handled = true;
		}


		private void AtDragEnter(object sender, DragEventArgs e) {
			//if the DataContext implements IDropable, record the data type that can be dropped
			if (m_DataType == null) {
				if (AssociatedObject.DataContext != null) {
					var dropObject = AssociatedObject.DataContext as IDropable;
					if (dropObject != null) {
						m_DataType = dropObject.DataType;
					}
				}
			}

			if (m_Adorner == null) m_Adorner = (FrameworkElementAdorner)Activator.CreateInstance(AdornerType,sender);
			e.Handled = true;
		}

		private void AtDragOver(object sender, DragEventArgs e) {
			if (m_DataType != null) {
				//if item can be dropped
				if (e.Data.GetDataPresent(m_DataType)) {
					var pos=UIHelper.GetDropPosition(AssociatedObject, e.GetPosition(AssociatedObject), DropRegion);

					var target = AssociatedObject.DataContext as IDropable;
					var canDrop=target.CanDrop(e.Data.GetData(m_DataType),-1,pos);

					//give mouse effect
					if(!canDrop) e.Effects=DragDropEffects.None;
					else e.Effects=DragDropEffects.Move;

					//draw the dots
					if (m_Adorner != null) {
						m_Adorner.DropEffect = canDrop ? DragDropEffects.Move : DragDropEffects.None;
						m_Adorner.Position = pos;
						m_Adorner.Update();
					}
				}
			}
			e.Handled = true;
		}

		private void AtDragLeave(object sender, DragEventArgs e) {
			if (m_Adorner != null) m_Adorner.Remove();
			e.Handled = true;
		}

		/// <summary> Provides feedback on if the data can be dropped
		/// </summary>
		/// <param name="e"></param>
		private void SetDragDropEffects(DragEventArgs e) {
			e.Effects = DragDropEffects.None; //default to None

			//if the data type can be dropped 
			if (e.Data.GetDataPresent(m_DataType)) {
				e.Effects = DragDropEffects.Move;
			}
		}

	}

}