using System;
using System.Collections.Generic;
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
		[Required]
		public string ID;

		public string Name;

		[Required]
		public string Value;

		public Type Type;
	}

	public class Record
	{
		[Required]
		public ICollection<Field> Fields;
	}

	public class Report
	{
		[RegularExpression("^[a-zA-Z0-9]{6}")]
		public string ID;

		[Required, RegularExpression("^[a-zA-Z0-9_ ]{3,25}")]
		public string AppName;

		public string AppURL;

		public DateTime ExpiresAt;

		[Required]
		public ICollection<Record> Records;
	}
}
