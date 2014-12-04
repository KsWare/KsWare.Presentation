//OBSOLETE because System.Windows.Interactivity
// C:\Program Files (x86)\Microsoft SDKs\Expression\Blend\.NETFramework\v4.0\Libraries\System.Windows.Interactivity.dll

//using System.Collections.Generic;
//using System.Windows;
//using System.Windows.Interactivity;
//using TriggerBase = System.Windows.Interactivity.TriggerBase;
//
//namespace KsWare.Presentation.ViewFramework
//{
//	public static class Interaction 
//	{
//		public static readonly DependencyProperty BehaviorsProperty	 =
//			DependencyProperty.RegisterAttached("Behaviors", typeof (IList<Behavior>), typeof (Interaction), new PropertyMetadata(null));
//		public static readonly DependencyProperty TriggersProperty	 =
//			DependencyProperty.RegisterAttached("Triggers", typeof (IList<TriggerBase>), typeof (Interaction), new PropertyMetadata(null));
//
//		
//
//		public static IList<Behavior> GetBehaviors(this DependencyObject obj) {
//			return System.Windows.Interactivity.Interaction.GetBehaviors(obj);
//		}
//
//		public static void SetBehaviors(this DependencyObject obj, IList<Behavior> value) {
//			foreach (var v in value) {
//				System.Windows.Interactivity.Interaction.GetBehaviors(obj).Add(v);
//			}
//		}
//
//		public static IList<TriggerBase> GetTriggers(this DependencyObject obj) {
//			return System.Windows.Interactivity.Interaction.GetTriggers(obj);
//		}
//		public static void  SetTriggers(this DependencyObject obj, IList<TriggerBase> value) {
//			foreach (var v in value) {
//				System.Windows.Interactivity.Interaction.GetTriggers(obj).Add(v);
//			}
//		}
//	}
//
//	public class BehaviorCollection:List<Behavior>{}
//	public class TriggerCollection:List<TriggerBase>{}
//}