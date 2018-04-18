namespace CloudAudit.Client.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;

    using CloudAudit.Client.Model;

    using LiteGuard;

    /// <summary>
    /// Custom validation class for the <see cref="AuditEvent"/> DTO
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class AuditEventValidatorAttribute : ValidationAttribute
    {
        /// <summary>
        /// Returns true if the <see cref="AuditEvent"/> DTO is valid.
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

            var auditEvent = value as AuditEvent;
            if (auditEvent == null)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "AuditEventValidatorAttribute does not support {0}", value.GetType());
                return new ValidationResult(message);
            }

            // Replace whitespace with null
            auditEvent.TargetType = SanitiseWhitespaceAsNull(auditEvent.TargetType);
            auditEvent.TargetId = SanitiseWhitespaceAsNull(auditEvent.TargetId);
            auditEvent.DataType = SanitiseWhitespaceAsNull(auditEvent.DataType);
            auditEvent.DataId = SanitiseWhitespaceAsNull(auditEvent.DataId);
            auditEvent.SessionId = SanitiseWhitespaceAsNull(auditEvent.SessionId);
            auditEvent.UserId = SanitiseWhitespaceAsNull(auditEvent.UserId);
            auditEvent.UserName = SanitiseWhitespaceAsNull(auditEvent.UserName);
            auditEvent.UserEmail = SanitiseWhitespaceAsNull(auditEvent.UserEmail);
            auditEvent.UserIdentity = SanitiseWhitespaceAsNull(auditEvent.UserIdentity);

            // Ensure OperationType is valid
            var validOperationTypes = ((OperationType[])Enum.GetValues(typeof(OperationType)))
                .Where(v => v != OperationType.Unspecified);
            if (!validOperationTypes.Contains(auditEvent.OperationType))
            {
                validationContext.Items.Add(nameof(AuditEvent.OperationType), auditEvent.OperationType + " is not a supported OperationType");
            }

            // Ensure we have a TargetId if we've a TargetType (and vice versa)
            if (!IsValidRequiredSet(auditEvent.TargetType, auditEvent.TargetId))
            {
                validationContext.Items.Add(nameof(AuditEvent.TargetType), "TargetType and TargetId are required together.");
                validationContext.Items.Add(nameof(AuditEvent.TargetId), "TargetType and TargetId are required together.");
            }

            // Ensure we have a DataId if we've a DataType (and vice versa)
            if (!IsValidRequiredSet(auditEvent.DataType, auditEvent.DataId))
            {
                validationContext.Items.Add(nameof(AuditEvent.DataType), "DataType and DataId are required together.");
                validationContext.Items.Add(nameof(AuditEvent.DataId), "DataType and DataId are required together.");
            }

            // Ensure if we have a DataType / DataId, then we also need Data!
            if ((!string.IsNullOrWhiteSpace(auditEvent.DataType) ||
                    !string.IsNullOrWhiteSpace(auditEvent.DataId)) &&
                auditEvent.Data == default(dynamic))
            {
                validationContext.Items.Add(nameof(AuditEvent.Data), "Data is required if DataType / DataId are specified.");
            }

            // Ensure we have a Session Id
            if (string.IsNullOrWhiteSpace(auditEvent.SessionId))
            {
                validationContext.Items.Add(nameof(AuditEvent.SessionId), "A SessionID is required");
            }

            // Ensure we have a UserId / UserName / UserEmail / UserIdentifier set
            if (!IsValidRequiredSet(auditEvent.UserId, auditEvent.UserName)) ////, auditEvent.UserEmail, auditEvent.UserIdentity))
            {
                validationContext.Items.Add(nameof(AuditEvent.UserId), "UserId, UserName, UserEmail and UserIdentifier are required together.");
                validationContext.Items.Add(nameof(AuditEvent.UserName), "UserId, UserName, UserEmail and UserIdentifier are required together.");
                ////validationContext.Items.Add(nameof(AuditEvent.UserEmail), "UserId, UserName, UserEmail and UserIdentifier are required together.");
                ////validationContext.Items.Add(nameof(AuditEvent.UserIdentity), "UserId, UserName, UserEmail and UserIdentifier are required together.");
            }

            if (validationContext.Items.Any())
            {
                var memberNames = validationContext.Items.Select(i => i.Key.ToString()).ToArray();
                return new ValidationResult("AuditEvent is not valid. See ValidationResult for details.", memberNames);
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Sanitises the whitespace as null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string SanitiseWhitespaceAsNull(string value)
        {
            return !string.IsNullOrWhiteSpace(value) ? value : null;
        }

        /// <summary>
        /// Determines whether all values in the set are non-null.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>
        ///   <c>true</c> if [is valid required set] [the specified values]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsValidRequiredSet(params string[] values)
        {
            return !values.Any(s => string.IsNullOrWhiteSpace(s)) ||
                   values.All(s => string.IsNullOrWhiteSpace(s));
        }
    }
}
