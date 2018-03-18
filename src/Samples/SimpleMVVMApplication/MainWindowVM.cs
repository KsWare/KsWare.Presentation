using System;
using KsWare.Presentation.ViewModelFramework;

namespace SimpleMVVMApplication {

	public class MainWindowVM : WindowVM {

		public MainWindowVM() {
			RegisterChildren(() => this);

			DayInput.ValueChangedEvent.add = (sender, args) => {
				Date = DateTime.Today.AddDays(DayInput.Value);
			};
		}

		public DateTime Date { get => Fields.GetValue<DateTime>(); private set => Fields.SetValue(value); }

		public Int32VM DayInput { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to FillDate
		/// </summary>
		/// <seealso cref="DoFillDate"/>
		public ActionVM FillDateAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="FillDateAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoFillDate(object parameter) {
			var days = (int) Convert.ChangeType(parameter ?? 0, typeof(int));
			Date = DateTime.Today.AddDays(days);
		}
	}

}
