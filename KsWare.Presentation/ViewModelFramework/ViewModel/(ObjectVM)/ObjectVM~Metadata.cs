using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework.Providers;
using KsWare.Presentation.Core;
using KsWare.Presentation.Core.Providers;

namespace KsWare.Presentation.ViewModelFramework {

	/* Implements the "Metadata" functionality for ObjectVM
	 *
	 * {ObjectVM}
	 *  ├▻ HasMetadata
	 *  ├↯ MetadataChanged
	 *  ├↯ MetadataChangedEvent
	 *  └▻ Metadata {ViewModelMetadata}
	 *      │         ├╴{ActionMetadata}        in ActionVM
	 *      │         ├╴{ListViewModelMetadata} in ListVM
	 *      │         ├╴... (more)
	 *      │        
	 *      ├▻ DataProvider         {IDataProvider}
	 *      ├▻ DisplayValueProvider {IDisplayValueProvider}
	 *      ├▻ ... (more)
	 */

	public partial interface IObjectVM /*Part: Metadata*/ {

		/// <summary> Gets or sets the metadata for this object.
		/// </summary>
		/// <value>The metadata.</value>
		/// <remarks>The metadata provides additional configuration for this view model object</remarks>
		ViewModelMetadata Metadata{get; set; }

		/// <summary> Gets a value indicating whether this instance has metadata.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has metadata; otherwise, <c>false</c>.
		/// </value>
		bool HasMetadata { get; }

		/// <summary> Occurs when <see cref="Metadata"/> has been changed.
		/// </summary>
		/// <remarks></remarks>
		event EventHandler<ValueChangedEventArgs<ViewModelMetadata>> MetadataChanged;

		IEventSource<EventHandler<ValueChangedEventArgs<ViewModelMetadata>>>  MetadataChangedEvent { get;}
	}

	public partial class ObjectVM /*Part: Metadata*/ {

		private ViewModelMetadata m_Metadata;
		private object m_PreviousData=DBNull.Value;

		private void InitPartMetadata() {
			
		}

		/// <summary> Gets a value indicating whether this instance has metadata.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has metadata; otherwise, <c>false</c>.
		/// </value>
		public bool HasMetadata { get { return m_Metadata != null; }}

		/// <summary> Gets or sets the metadata.
		/// </summary>
		/// <value>The metadata.</value>
		/// <remarks>The metadata provides additional configuration for this view model object
		/// <p>If no Metadata is specified the first get operation will automatically initialize the Metadata.</p>
		/// <p>To avoid an auto init use <see cref="HasMetadata"/> instead of comparing with null (<c>Metadata==null</c>) </p></remarks>
		public ViewModelMetadata Metadata {
			[NotNull]
			get {
				if(DebuggerFlags.Breakpoints.MetadataGet)DebuggerːBreak(this,"DebuggerFlags.Breakpoints.MetadataGet");
				if (m_Metadata == null) {
					if(DebuggerFlags.Breakpoints.MetadataSet)DebuggerːBreak(this,"DebuggerFlags.Breakpoints.MetadataSet");
					m_Metadata = CreateMetadata();
					m_Metadata.Parent = this;
					OnMetadataChanged(new ValueChangedEventArgs<ViewModelMetadata>(null,m_Metadata));
				}
				return m_Metadata;
			}
			[NotNull]
			set {
				if(DebuggerFlags.Breakpoints.MetadataGet)DebuggerːBreak(this,"DebuggerFlags.Breakpoints.MetadataSet");
				MemberAccessUtil.DemandNotNull(value,null,this,"Metadata","{3EAD4816-D4B5-4B68-B157-218C610E3553}");
				MemberAccessUtil.DemandWriteOnce(m_Metadata==null,"Cannot set a metadata property once it is applied to a view model object operation.",this,"Metadata","{16E8F3EE-2629-4244-9D62-F7FD2AD40592}");
				var oldMetadata = m_Metadata;
				m_Metadata = value;
				m_Metadata.Parent = this;
				OnMetadataChanged(new ValueChangedEventArgs<ViewModelMetadata>(oldMetadata,m_Metadata));
			}
		}

		private ViewModelMetadata CreateMetadata() {
			var metadata = CreateDefaultMetadata();
			MemberAccessUtil.DemandNotNull(metadata,null,this,"Metadata","{50EC0095-2360-4666-81B6-8E44A28B82B7}");
			return metadata;
		}

		/// <summary> Creates the default metadata.
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual ViewModelMetadata CreateDefaultMetadata() {
			return new ViewModelMetadata();
		}

		/// <summary> Occurs when Metadata-Property has been changed.
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<ViewModelMetadata>>  MetadataChanged;

		/// <summary> Gets the event source for the event which occurs when Metadata-Property has been changed.
		/// </summary>
		public IEventSource<EventHandler<ValueChangedEventArgs<ViewModelMetadata>>> MetadataChangedEvent {
			get { return EventSources.Get<EventHandler<ValueChangedEventArgs<ViewModelMetadata>>>("MetadataChangedEvent"); }
		}

		/// <summary> Raises the MetadataChanged-event.<br/>
		/// For derived classes: Called when <see cref="Metadata"/> property changes. 
		/// </summary>
		protected virtual void OnMetadataChanged(ValueChangedEventArgs<ViewModelMetadata> e) {
			if (HasMetadata) {
				Metadata.DataProviderChanged+= (o1,e1) => OnDataProviderChanged(e1);
				var oldMetadata = e.PreviousValue;
				var oldDataProvider=oldMetadata!=null ? (oldMetadata.HasDataProvider?oldMetadata.DataProvider:null):null;
				var newDataProvider=Metadata.HasDataProvider?Metadata.DataProvider:null;
				
				if(oldDataProvider!=newDataProvider) OnDataProviderChanged(new ValueChangedEventArgs<IDataProvider>(oldDataProvider,newDataProvider));
			}

			//if (SuppressAnyEvents == 0) ...
			EventUtil.Raise(MetadataChanged, this, e, "{C9E46906-5D20-4D65-8A28-6849E145D786}");
			EventManager.Raise<EventHandler<ValueChangedEventArgs<ViewModelMetadata>>,ValueChangedEventArgs<ViewModelMetadata>>(LazyWeakEventStore, "MetadataChangedEvent", e);
		}


		/// <summary> Called if Metadata.DataProvider changes
		/// </summary>
		/// <remarks>Also called if Metadata changes.</remarks>
		protected virtual void OnDataProviderChanged(ValueChangedEventArgs<IDataProvider> e) {
			if (e.PreviousValue != null) e.PreviousValue.DataChanged -= AtDataChanged;
			if(e.NewValue==null) return;

			var dataProvider = e.NewValue;
			dataProvider.DataChanged+=AtDataChanged;

			Exception exception;
			var data = dataProvider.TryGetData(out exception);
			if (exception == null && !Equals(m_PreviousData, data) && (m_PreviousData!=null || data!=null)) {
				var prev = m_PreviousData;
				m_PreviousData = data;
				OnDataChanged(new DataChangedEventArgs(prev,data){Cause = "ObjectVM.OnDataProviderChanged"});
			}
			if (exception != null && m_PreviousData != DBNull.Value) {
				var prev = m_PreviousData;
				m_PreviousData = DBNull.Value;
				OnDataChanged(new DataChangedEventArgs(prev,DBNull.Value){Cause = "ObjectVM.OnDataProviderChanged"});
			}
		}

		private void AtDataChanged(object sender, DataChangedEventArgs e) {
			//DO THIS NOT!: if (Equals(e.NewData, m_PreviousData)) return;
			m_PreviousData = e.NewData;
			e.Cause += "\n"+"ObjectVM.AtDataChanged";
			OnDataChanged(e);
		}

		/// <summary> Called if Metadata.DataProvider.Data changes
		/// </summary>
		/// <param name="e">The <see cref="DataChangedEventArgs" /> instance containing the event data.</param>
		/// <remarks>
		/// Also called if Metadata and/or DataProvider changes and the new DataProvider returns other as the previous data.
		/// </remarks>
		protected virtual void OnDataChanged(DataChangedEventArgs e) {
			
		}
	}
}
