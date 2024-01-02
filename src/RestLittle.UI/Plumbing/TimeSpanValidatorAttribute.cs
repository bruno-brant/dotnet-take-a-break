// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TakeABreak.UI.Plumbing
{
	/// <summary>
	/// Declarative validations for a <see cref="TimeSpan"/> field.
	/// </summary>
	public class TimeSpanValidatorAttribute : ValidationAttribute
	{
		private TimeSpan? _minValue;
		private TimeSpan? _maxValue;

		/// <summary>
		/// Gets or sets minimum inclusive allowed value for this TimeSpan.
		/// </summary>
		public string MinValue { get => _minValue.ToString(); set => _minValue = TimeSpan.Parse(value, CultureInfo.CurrentCulture); }

		/// <summary>
		/// Gets or sets maximum inclusive allowed value for this TimeSpan.
		/// </summary>
		public string MaxValue { get => _maxValue.ToString(); set => _maxValue = TimeSpan.Parse(value, CultureInfo.CurrentCulture); }

		/// <inheritdoc/>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
			{
				return ValidationResult.Success;
			}

			if (value is not TimeSpan ts)
			{

				return new ValidationResult("Invalid TimeSpan.", [validationContext?.MemberName]);
			}

			if (_minValue is not null && ts < _minValue)
			{
				var msg = $"TimeSpan value '{value}' must be greater or equal to '{MinValue}'";
				return new ValidationResult(msg, [validationContext?.MemberName]);
			}

			if (_maxValue is not null && ts > _maxValue)
			{
				var msg = $"TimeSpan value '{value}' must be less or equal to '{MaxValue}'";
				return new ValidationResult(msg, [validationContext?.MemberName]);
			}

			return ValidationResult.Success;
		}
	}
}
