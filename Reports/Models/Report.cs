using System;
using System.ComponentModel.DataAnnotations;
using Reports.Models.Validators.Report;

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
		[UniqueID]
		public Field[][] Fields { get; set; }
	}
}
