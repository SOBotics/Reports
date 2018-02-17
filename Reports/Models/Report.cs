using System;
using System.ComponentModel.DataAnnotations;
using Reports.Models.Validators.Report;
using ZeroFormatter;

namespace Reports.Models
{
	public enum Type
	{
		Default,
		Link,
		Date,
		Answers
	}

	[ZeroFormattable]
	public class Field
	{
		[Index(0)]
		[Required(AllowEmptyStrings = false)]
		[RegularExpression("^[a-zA-Z0-9_\\- ]{1,25}")]
		public virtual string ID { get; set; }

		[Index(1)]
		[Required(AllowEmptyStrings = false)]
		public virtual string Name { get; set; }

		[Index(2)]
		[Required(AllowEmptyStrings = false)]
		public virtual string Value { get; set; }

		[Index(3)]
		public virtual Type Type { get; set; }
	}

	[ZeroFormattable]
	public class Report
	{
		private const string appNamePattenErrorMessage = "'AppName' must be between 3 to 25 characters long (inclusive), and can only contain: alphanumerical characters, underscores, dashes (-), and spaces.";

		[Index(0)]
		public virtual string ID { get; set; }

		[Index(1)]
		[Required(AllowEmptyStrings = false)]
		[RegularExpression("^[a-zA-Z0-9_\\- ]{3,25}", ErrorMessage = appNamePattenErrorMessage)]
		public virtual string AppName { get; set; }

		[Index(2)]
		[RegularExpression("^https?://.*")]
		public virtual string AppURL { get; set; }

		[Index(3)]
		[ValidDate]
		public virtual DateTime ExpiresAt { get; set; }

		[Index(4)]
		[Required]
		[UniqueID]
		public virtual Field[][] Fields { get; set; }
	}
}
