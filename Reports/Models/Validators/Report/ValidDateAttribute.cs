using System;
using System.ComponentModel.DataAnnotations;

namespace Reports.Models.Validators.Report
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal class ValidDateAttribute : ValidationAttribute
	{
		private string errorMessage = "";

		public override bool IsValid(object value)
		{
			var d = value as DateTime?;

			if (d == null)
			{
				errorMessage = "{0} must be of type {1} (preferably ISO 8601 format) and cannot be null.";
				return false;
			}

			if (d > DateTime.UtcNow.AddYears(1))
			{
				errorMessage = "{0} cannot be further than one year away from the current UTC date.";
				return false;
			}

			if (d < DateTime.UtcNow)
			{
				errorMessage = "{0} must be a date in the future.";
				return false;
			}

			return true;
		}

		public override string FormatErrorMessage(string name) =>
			String.Format(errorMessage, name, nameof(DateTime));
	}
}
