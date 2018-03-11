namespace KsWare.Presentation.BusinessFramework {

	/// <summary> Provides a metric business value
	/// </summary>
	public class MetricValueBM: DoubleBM {

		private const double FactorM = 1000000.0;
		private const double FactorK = 1000.0;
		private const double FactorH = 100.0;
		private const double FactorD = 10.0;

		private Unit _unit;

#pragma warning disable 1591 //TODO document this
		public double Micro{get {return GetValue(FactorM);}set {SetValue(value,FactorM);}}
		public double Milli{get {return GetValue(FactorK);}set {SetValue(value,FactorK);}}
		public double Centi{get {return GetValue(FactorH);}set {SetValue(value,FactorH);}}
		public double Deci {get {return GetValue(FactorD);}set {SetValue(value,FactorD);}}
		public double Deca {get {return GetValue(1/FactorD);}set {SetValue(value,1/FactorD);}}
		public double Hecto{get {return GetValue(1/FactorH);}set {SetValue(value,1/FactorH);}}
		public double Kilo {get {return GetValue(1/FactorK);}set {SetValue(value,1/FactorK);}}
		public double Mega {get {return GetValue(1/FactorM);}set {SetValue(value,1/FactorM);}}
#pragma warning restore 1591

		/// <summary> Gets or sets the _Unit of measure.
		/// </summary>
		/// <value>The _Unit of measure.</value>
		public Unit Unit {
			get {return _unit;}
			set {
				MemberAccessUtil.DemandWriteOnce(_unit==Unit.None,null,this,nameof(Unit),"{554C3318-8CF8-4990-A3C6-51023ECA2D16}");
				_unit = value;
			}
		}

		private double Factor(double baseFactor) {
			switch (Unit) {
				case Unit.QubicMeter: return System.Math.Pow(baseFactor, 3.0);
				case Unit.SquareMeter: return System.Math.Pow(baseFactor, 2.0);
				default:
					return baseFactor;
			}
		}

		private double GetValue(double baseFactor) {
			return Value*Factor(baseFactor);
		}

		private void SetValue(double value, double baseFactor) {
			Value = value/Factor(baseFactor);
		}
	}

	/// <summary> Unit of measure
	/// </summary>
	public enum Unit {

		/// <summary> No unit </summary>
		None,

		/// <summary> <i>m</i>, Metric unit of length </summary>
		Meter,

		/// <summary> <i>m²</i>, Metric unit of area </summary>
		SquareMeter,

		/// <summary> <i>m³</i>, Metric unit of volume </summary>
		QubicMeter,

		/// <summary><i>l</i>, Metric unit of capacity for liquids </summary>
		Litre, // Liter (Brit.)

		/// <summary><i>V</i>,  unit of electrical potential difference </summary>
		Volt,

		/// <summary> <i>A</i>, unit of electric current  </summary>
		Ampere,

		/// <summary> <i>W</i>, Metric unit of electrical measurement, unit of electrical power </summary>
		Watt,

		/// <summary> <i>Wh</i>, unit of energy which is equal to the amount of energy expended by one watt in one hour  </summary>
		WattHour,

		/// <summary> <i>VA</i> VA, </summary>
		Voltampere,

		/// <summary> <i>°C</i>, Degrees measured on a thermometer on which 0 degrees is the freezing point and 100 degrees is the boiling point of water  </summary>
		DegreesCentigrade,

		/// <summary> <i>°</i>, Unit of measurement of temperature and angles </summary>
		Degree,


		/// <summary> <i>'</i>/<i>s</i> </summary>
		Second,

		/*
		Minute,
		Hour,
		Day,
		Week
		Month,
		Quarter,
		Year,
		*/
	}
}