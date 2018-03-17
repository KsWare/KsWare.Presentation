namespace KsWare.Presentation.ViewModelFramework {

	partial class WindowVM /* Part: Overlays*/ {

		/// <summary>[EXPERIMENTAL] Gets or sets a value indicating whether a user feedback uses overlays.
		/// </summary>
		/// <value><c>true</c> if user feedback uses overlays; otherwise, <c>false</c>.</value>
		/// <remarks>
		/// If <c>true</c> a call to <see cref="RequestUserFeedback"/> adds the matching  <see cref="UserFeedbackVM{T}"/> to the <see cref="Overlays"/> collection.
		/// </remarks>
		public bool UserFeedbackUsesOverlays { get; set; }

		/// <summary> [EXPERIMENTAL] Gets the overlays.
		/// </summary>
		/// <value>The overlays.</value>
		/// <example>
		/// Usage in Window/Page:
		/// <code>
		/// &lt;ItemsControl ItemsSource="{Binding Overlays}" d:IsHidden="True"&gt;
		///     &lt;ItemsControl.ItemsPanel&gt;
		///         &lt;ItemsPanelTemplate&gt;
		///             &lt;Grid IsItemsHost="True"/&gt;
		///         &lt;/ItemsPanelTemplate&gt;
		///     &lt;/ItemsControl.ItemsPanel&gt;
		///     &lt;ItemsControl.ItemTemplateSelector&gt;
		///         &lt;vf:TypeNameDataTemplateSelector DeclaringType="{x:Type mainWindow:MainWindowVM}"/&gt;
		///     &lt;/ItemsControl.ItemTemplateSelector&gt;
		/// &lt;/ItemsControl&gt;
		/// </code></example>
		public OverlayCollection Overlays { get; private set; }

		/// <summary> [EXPERIMENTAL] override RequestUserFeedbackCore for overlays
		/// </summary>
		/// <param name="args">The <see cref="UserFeedbackEventArgs" /> instance containing the event data.</param>
		protected override void RequestUserFeedbackCore(UserFeedbackEventArgs args) {
			if(!HandleUserFeedbackRequest(args)) base.RequestUserFeedbackCore(args);
		}

		private bool HandleUserFeedbackRequest(UserFeedbackEventArgs args) {
			if (UserFeedbackUsesOverlays) {
				if     (args is ExceptionFeedbackEventArgs    ) Feedback(new ExceptionFeedbackVM    (),(ExceptionFeedbackEventArgs    ) args);
				else if(args is DetailMessageFeedbackEventArgs) Feedback(new DetailMessageFeedbackVM(),(DetailMessageFeedbackEventArgs) args);
				else if(args is MessageFeedbackEventArgs      ) Feedback(new MessageFeedbackVM      (),(MessageFeedbackEventArgs      ) args);
				else if(args is InputFeedbackEventArgs        ) Feedback(new InputFeedbackVM        (),(InputFeedbackEventArgs        ) args);
	//			else if(args is OpenFileFeedbackEventArgs     ) Feedback(new OpenFileFeedbackVM     (),(OpenFileFeedbackEventArgs     ) args);
	//			else if(args is SaveFileFeedbackEventArgs     ) Feedback(new SaveFileFeedbackVM     (),(SaveFileFeedbackEventArgs     ) args);
				else return false;
			}
			return false;
		}

		/// <summary> [EXPERIMENTAL] Enqueues the feedback into the <see cref="Overlays"/> 
		/// </summary>
		/// <typeparam name="TViewModel">The type of the view model.</typeparam>
		/// <typeparam name="TFeedbackEventArgs">The type of the feedback event arguments.</typeparam>
		/// <param name="viewModel">The feedback view model.</param>
		/// <param name="args">The <see cref="TFeedbackEventArgs"/> instance containing the event data.</param>
		private void Feedback<TViewModel, TFeedbackEventArgs>(TViewModel viewModel, TFeedbackEventArgs args)
			where TViewModel : UserFeedbackVM<TFeedbackEventArgs>
			where TFeedbackEventArgs : UserFeedbackEventArgs {

			args.AsyncHandled = true;
			viewModel.Data = args;
			Overlays.Insert(0,viewModel);
			viewModel.IsOpen = true;
		}

		public void HandleApplicationUserFeedbackRequests(UserFeedbackEventArgs args) { HandleUserFeedbackRequest(args); }

	}


	/// <summary> [EXPERIMENTAL] Provides a collection of view models used as overlays
	/// </summary>
	public class OverlayCollection:ListVM<IObjectVM> {}

}
