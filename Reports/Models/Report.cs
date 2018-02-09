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
		[Required(AllowEmptyStrings = false), RegularExpression("^[a-zA-Z0-9_\\- ]{1,25}")]
		public string ID { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string Name { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string Value { get; set; }

		public Type Type { get; set; }
	}

	public class Report
	{
		public string ID { get; set; }

		[Required(AllowEmptyStrings = false), RegularExpression("^[a-zA-Z0-9_\\- ]{3,25}")]
		public string AppName { get; set; }

		[RegularExpression("^https?://.*")]
		public string AppURL { get; set; }

		public DateTime ExpiresAt { get; set; }

		[Required]
		public Field[][] Fields { get; set; }
	}
}
