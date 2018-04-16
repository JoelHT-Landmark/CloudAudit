namespace CloudAudit.Client.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using CloudAudit.Client.Extensions;

    using LiteGuard;

    /// <summary>
    /// Validation attribute indicating that when the decorated property is non-null, the
    /// associated property must also be non-null (and vice versa)
    /// </summary>
    /// <remarks>
    /// Never quite got this working - but it's a useful documentation decorator
    /// anyway. See <see cref="AuditEventValidatorAttribute"/>
    /// </remarks>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments",
        Justification = "Not required for a validation attribute")]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RequiredTogetherWithAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredTogetherWithAttribute"/> class.
        /// </summary>
        /// <param name="otherFieldName">Name of the other field.</param>
        public RequiredTogetherWithAttribute(string otherFieldName)
            : base()
        {
            Guard.AgainstNullArgument(nameof(otherFieldName), otherFieldName);
            this.OtherField = otherFieldName;
        }

        /// <summary>
        /// Gets the name of the "other" field.
        /// </summary>
        /// <value>
        /// The other field.
        /// </value>
        public string OtherField { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether the attribute requires validation context.
        /// </summary>
        public override bool RequiresValidationContext
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.
        /// </returns>
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
