using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using KsWare.Presentation.Core;

namespace KsWare.Presentation {

	/// <summary> Provides UserFeedbackEventArgs for the standard MessageDialog showing an exception
	/// </summary>
	public class ExceptionFeedbackEventArgs : DetailMessageFeedbackEventArgs {

		/// <summary> Initializes a new instance of the <see cref="ExceptionFeedbackEventArgs"/> class.
		/// </summary>
		/// <param name="exception">The exception</param>
		/// <param name="caption">The caption.</param>
		/// <param name="button">The button.</param>
		/// <param name="icon">The icon.</param>
		/// <param name="defaultResult">The default result.</param>
		/// <param name="options">The options.</param>
		public ExceptionFeedbackEventArgs(Exception exception, string caption = "Unhandled Exception", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Error, MessageBoxResult defaultResult = MessageBoxResult.OK, MessageBoxOptions options = MessageBoxOptions.None)
			: base(FormatException(exception),FormatExceptionDetails(exception), caption, button, icon, defaultResult, options) {
			Exception = exception;
		}

		/// <summary> Initializes a new instance of the <see cref="ExceptionFeedbackEventArgs"/> class.
		/// </summary>
		/// <param name="exception">The exception</param>
		/// <param name="message">The message</param>
		/// <param name="detailMessage"></param>
		/// <param name="caption">The caption.</param>
		/// <param name="button">The button.</param>
		/// <param name="icon">The icon.</param>
		/// <param name="defaultResult">The default result.</param>
		/// <param name="options">The options.</param>
		public ExceptionFeedbackEventArgs(Exception exception, string message, string detailMessage, string caption = "Unhandled Exception", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Error, MessageBoxResult defaultResult = MessageBoxResult.OK, MessageBoxOptions options = MessageBoxOptions.None)
			: base(message, detailMessage, caption, button, icon, defaultResult, options) {
			Exception = exception;
		}

		public override FeedbackType FeedbackType { get{return FeedbackType.Exception;} }

		/// <summary> Gets or sets the Exception.
		/// </summary>
		/// <value>The Exception.</value>
		public Exception Exception { get; set; }

		private static string FormatException(Exception ex) {
			var e = ex;
			var s = new List<string>();
			while (e!=null) {
				s.Add(e.GetType().Name+": "+e.Message);
				e = e.InnerException;
			}
			//return ex.Message;
			return string.Join(" -->\n", s);
		}

		private static string FormatExceptionDetails(Exception ex) {
			var sb = new StringBuilder();

			// Messages for all InnerExceptions
			var e = ex;
			var s = new List<string>();
			while (e!=null) {
				s.Add(e.GetType().Name+": "+e.Message);
				e = e.InnerException;
			}
			sb.Append(string.Join(" -->\n", s));
			sb.AppendLine();sb.AppendLine("StackTrace:");
			sb.AppendLine(ex.StackTrace);

			if (ex.InnerException != null) {
				sb.AppendLine();sb.AppendLine("--- InnerExceptions --- ");

				e = ex.InnerException;
				while (e!=null) {
					sb.AppendLine(e.GetType().Name+": "+e.Message);
					sb.AppendLine();
					sb.AppendLine(ex.StackTrace);
					e=e.InnerException;
				}				
			}

			return sb.ToString();
		}
	}
}