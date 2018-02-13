using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Reports.Models.Validators.Report
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class UniqueIDAttribute : ValidationAttribute
	{
		private string errorMessage = "";

		public override bool IsValid(object value)
		{
			var fields = value as Field[][];

			if (fields == null)
			{
				errorMessage = "{0} must be of type {1} and cannot be null.";
				return false;
			}

			if (fields.Length == 0)
			{
				errorMessage = "{0} must have at least one item.";
				return false;
			}

			if (fields.Any(x => x.Length != fields[0].Length))
			{
				errorMessage = "All children of {0} must have an equal number of items.";
				return false;
			}

			foreach (var x in fields[0])
			for (var i = 0; i < fields.Length; i++)
			{
				var y = fields[i];
				var idMatch = y.Count(z => z.ID == x.ID);

				if (idMatch == 0)
				{
					errorMessage = $"{{0}}[{i}] must contain a {nameof(Field)} with an {nameof(x.ID)} of: {x.ID}.";
					return false;
				}
				
				if (idMatch > 1)
				{
					errorMessage = $"{{0}}[{i}] contains {idMatch} {nameof(Field)}s with matching {nameof(x.ID)}s.";
					return false;
				}
			}

			return true;
		}

		public override string FormatErrorMessage(string name)
		{
			return String.Format(errorMessage, name, typeof(Field[][]).Name);
		}
	}
}
