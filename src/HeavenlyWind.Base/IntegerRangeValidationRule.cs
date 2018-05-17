using System.Globalization;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze
{
    public class IntegerRangeValidationRule : ValidationRule
    {
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }

        public bool AllowEmpty { get; set; }

        public override ValidationResult Validate(object rpValue, CultureInfo rpCultureInfo)
        {
            var rInput = (string)rpValue;

            if (rInput.IsNullOrEmpty() && AllowEmpty)
                return ValidationResult.ValidResult;

            int rNumber;
            if (!int.TryParse(rInput, out rNumber))
                return new ValidationResult(false, StringResources.Instance.Main.ValidationRule_PleaseInputNumber);

            if (Minimum.HasValue && rNumber < Minimum)
                return new ValidationResult(false, string.Format(StringResources.Instance.Main.ValidationRule_NumberGreaterThanMaximum, Minimum));
            if (Maximum.HasValue && rNumber > Maximum)
                return new ValidationResult(false, string.Format(StringResources.Instance.Main.ValidationRule_NumberLessThanMinimum, Maximum));

            return ValidationResult.ValidResult;
        }
    }
}
