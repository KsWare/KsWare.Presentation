using System;

namespace KsWare.Presentation {

	public static class DateTimeExtension {

		public static DateTime NextFullHour(this DateTime value) {
			var t0 = value;
			var t1=t0.AddHours(1).AddMinutes(-t0.Minute).AddSeconds(-t0.Second).AddMilliseconds(-t0.Millisecond);
			return t1;
		}

		public static TimeSpan NextFullHourDiff(this DateTime value) {
			var d = NextFullHour(value).Subtract(value);
			return d;
		}
	}
}
