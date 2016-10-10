using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace KsWare.Presentation.ViewModelFramework {

	public partial interface IObjectVM {

		/// <summary> Gets a value indicating whether this instance is enabled.
		/// </summary>
		/// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
		bool IsEnabled{get; }

		/// <summary> Occurs when <see cref="IsEnabled"/> has been changed.
		/// </summary>
		/// <remarks></remarks>
		event RoutedPropertyChangedEventHandler<bool> IsEnabledChanged;

	}

	public partial class ObjectVM {

		/// <summary> Gets a value indicating whether this instance is enabled.
		/// </summary>
		/// <remarks>Use <see cref="SetEnabled"/> to change the value.</remarks>
		[DefaultValue(true)]
		public bool IsEnabled{get; private set;}

		/// <summary> Occurs when <see cref="IsEnabled"/> has been changed.
		/// </summary>
		/// <remarks></remarks>
		public event RoutedPropertyChangedEventHandler<bool> IsEnabledChanged;

		/// <summary>  Sets the enabled state.
		/// </summary>
		/// <param name="token">An unique token to change the state back</param>
		/// <param name="value"><see langword="true"/> to enable; else <see langword="false"/> to disabled</param>
		/// <returns><see langword="true"/> if enabled; else <see langword="false"/></returns>
		/// <remarks>
		/// If the <paramref name="value"/> is <see langword="false"/> the objection (with the  <paramref name="token"/> ) will be added.<br/>
		/// If the <paramref name="value"/> is <see langword="true"/> the objection (with the  <paramref name="token"/> ) will be removed.<br/>
		/// As long at least one objection ist pressend, the <see cref="IsEnabled"/> returns <see langword="false"/>. <br/>
		/// Multiple objections with same token does not increase the objection counter.
		/// </remarks>
		public bool SetEnabled(object token, bool value) {
			lock (_enableObjections) {
				bool oldIsEnabled = _enableObjections.Count==0;
				if (value==false) {
					if(!_enableObjections.Contains(token)) // ADDED [xgksc 2013-01-24] to prevent multiple objection with same token
						_enableObjections.Add(token);
				} else {
					//FIX: System.ArgumentOutOfRangeException in List<>.Remove(o) (ObjectVM.SetEnabled)
					var idx = _enableObjections.IndexOf(token);
					if(idx>=0) _enableObjections.RemoveAt(idx);
				}
				bool newIsEnabled = _enableObjections.Count==0;
				if(oldIsEnabled!=newIsEnabled) {
					IsEnabled = newIsEnabled;
					OnPropertyChanged("IsEnabled");
					EventUtil.Raise(IsEnabledChanged,this,new RoutedPropertyChangedEventArgs<bool>(oldIsEnabled, newIsEnabled),"{54239EDA-24CB-4B82-9023-D9E403E0194F}");
				}
				return newIsEnabled;
			}
		}

		public string DebugːEnabledObjections {
			get {
				lock (_enableObjections) {
					return string.Join(" + ", _enableObjections.Select(d => d.ToString()).ToArray());
				}				
			}
		}

	}

}
