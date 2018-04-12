using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CloudAudit.Client.Extensions;
using LiteGuard;

namespace CloudAudit.Client.Validation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments",
        Justification = "Not required for a validation attribute")]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RequiredTogetherWithAttribute : ValidationAttribute
    {
        public RequiredTogetherWithAttribute(string otherFieldName)
            : base()
        {
            Guard.AgainstNullArgument(nameof(otherFieldName), otherFieldName);
            this.OtherField = otherFieldName;
        }

        public string OtherField { get; private set; }

        public override bool RequiresValidationContext
        {
            get
            {
                return true;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Guard.AgainstNullArgument(nameof(value), value);
            Guard.AgainstNullArgument(nameof(validationContext), validationContext);

            var validationTarget = validationContext.ObjectInstance;

            var objectType = validationTarget.GetType();
            var otherValueProperty = objectType.GetProperty(OtherField);
            var otherValueType = otherValueProperty.PropertyType;

            var otherValue = otherValueProperty.GetValue(this, null);

            var otherValueIsSet = (otherValue != null) &&
                (otherValue != otherValueType.GetDefaultValue());

            var thisValueIsSet = value != null;
            var thisField = validationContext.MemberName;

            if ((thisValueIsSet && !otherValueIsSet) || (!thisValueIsSet && otherValueIsSet))
            {
                var message = string.Format(CultureInfo.InvariantCulture, "{0} is required if {1} is set.", thisField, OtherField);
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }
    }
}
