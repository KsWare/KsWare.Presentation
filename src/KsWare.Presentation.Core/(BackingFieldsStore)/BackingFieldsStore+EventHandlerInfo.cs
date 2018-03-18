using System;

namespace KsWare.Presentation {

	public sealed partial class BackingFieldsStore {
		internal class EventHandlerInfo:IDisposable {

			private BackingFieldInfo _fieldInfo;

			public EventHandlerInfo(BackingFieldInfo fieldInfo, EventHandler<ValueChangedEventArgs> propertyChangedEventHandler) {
				_fieldInfo = fieldInfo;
				PropertyChangedEventHandler = propertyChangedEventHandler;
			}

			public EventHandler<ValueChangedEventArgs> PropertyChangedEventHandler { get; private set; }

			public void Dispose() {
				lock (_fieldInfo.EventHandlers) {
					_fieldInfo.EventHandlers.Remove(this);
				}
				_fieldInfo = null;
			}
		}
	}

}