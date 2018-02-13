using System;
using System.ComponentModel.DataAnnotations;

namespace Reports.Models
{
	public enum Type
	{
		Default,
		Link,
		Date,
		Answers
	}

	public class Field
	{
		[Required(AllowEmptyStrings = false)]
		[RegularExpression("^[a-zA-Z0-9_\\- ]{1,25}")]
		public string ID { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string Name { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string Value { get; set; }

		public Type Type { get; set; }
	}

	public class Report
	{
		private const string appNamePattenErrorMessage = "'AppName' must be between 3 to 25 characters long (inclusive), and can only contain: alphanumerical characters, underscores, dashes (-), and spaces.";

		public string ID { get; set; }

		[Required(AllowEmptyStrings = false)]
		[RegularExpression("^[a-zA-Z0-9_\\- ]{3,25}", ErrorMessage = appNamePattenErrorMessage)]
		public string AppName { get; set; }

		[RegularExpression("^https?://.*")]
		public string AppURL { get; set; }

		[ValidDate]
		public DateTime ExpiresAt { get; set; }

		[Required]
		public Field[][] Fields { get; set; }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal class ValidDateAttribute : ValidationAttribute
	{
		private string errorMessage = "";

		public override bool IsValid(object value)
		{
			var d = value as DateTime?;

			if (d == null)
			{
				errorMessage = "{0} must be of type " + nameof(DateTime) + " (preferably ISO 8601 format) and cannot be null.";
				return false;
			}

			if (d > DateTime.UtcNow.AddYears(1))
			{
				errorMessage = "{0} cannot be further than one year away from the current UTC date.";
				return false;
			}
			else if (d < DateTime.UtcNow)
			{
				errorMessage = "{0} must be a date in the future.";
				return false;
			}

			return true;
		}

		public override string FormatErrorMessage(string name) => String.Format(errorMessage, name);
	}
}
