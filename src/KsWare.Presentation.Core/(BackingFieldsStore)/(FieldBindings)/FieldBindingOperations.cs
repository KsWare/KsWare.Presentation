﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KsWare.Presentation {

	/// <summary>
	/// [EXPERIMENTAL] Class FieldBindingOperations.
	/// </summary>
	/// <autogeneratedoc />
	public static class FieldBindingOperations {
		
		//TODO optimize store. maybe use BackingFieldStore.Bindings
		private static readonly List<FieldBinding> Bindings = new List<FieldBinding>();

		/// <summary>
		/// Sets the binding between to fields.
		/// </summary>
		/// <param name="target">The target field.</param>
		/// <param name="binding">The binding.</param>
		public static void SetBinding(BackingFieldsStore.IBackingFieldInfo target, FieldBinding binding) {
			//BindingOperations.SetBinding(target, BackingFieldsStore.BackingFieldInfo.ValueProperty, binding);

			if (target == null) throw new ArgumentNullException(nameof(target));
			if (binding == null) throw new ArgumentNullException(nameof(binding));
			
			if (binding.Converter == null) binding.Converter = DataTypeConverter.Default;
			var existingBinding = GetBindings(binding.Source, target);
			if (existingBinding != null) throw new InvalidOperationException("Allready binding configured!"); // TODO concept: silent overwrite synchronization?

			binding.Target = target;
			switch (binding.Mode) {
				case BindingMode.TwoWay:
				case BindingMode.OneTime:
				case BindingMode.OneWay:
					target.Value = binding.Converter.Convert(binding.Source.Value, target.Type, null, CultureInfo.CurrentCulture);
					break;
				case BindingMode.OneWayToSource:
					binding.Source.Value = binding.Converter.ConvertBack(target.Value, binding.Source.Type, null, CultureInfo.CurrentCulture);
					break;
				default: goto case BindingMode.TwoWay;
			}
			switch (binding.Mode) {
				case BindingMode.TwoWay:
					binding.Add(
						(s, e) => target.Value = binding.Converter.Convert(binding.Source.Value, target.Type, null, CultureInfo.CurrentCulture),
						(s, e) => binding.Source.Value = binding.Converter.ConvertBack(target.Value, binding.Source.Type, null, CultureInfo.CurrentCulture));
					break;
				case BindingMode.OneWay:
					binding.Add(
						(s, e) => target.Value = binding.Converter.Convert(binding.Source.Value, target.Type, null, CultureInfo.CurrentCulture), null);
					break;
				case BindingMode.OneWayToSource:
					binding.Add(null,
						(s, e) => binding.Source.Value = binding.Converter.ConvertBack(target.Value, binding.Source.Type, null, CultureInfo.CurrentCulture));
					break;
				default: goto case BindingMode.TwoWay;
			}
			if (binding.HasEvents) Bindings.Add(binding);
		}

		private static FieldBinding GetBindings(BackingFieldsStore.IBackingFieldInfo source, BackingFieldsStore.IBackingFieldInfo target) {
			return Bindings.FirstOrDefault(f => f.Source == source && f.Target == target || f.Source == target && f.Target == source);
		}
	}
}
