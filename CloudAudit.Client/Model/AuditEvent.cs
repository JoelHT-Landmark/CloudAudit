using CloudAudit.Client.Extensions;
using CloudAudit.Client.Validation;
using LiteGuard;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace CloudAudit.Client.Model
{
    [AuditEventValidator]
    public class AuditEvent
    {
        protected internal AuditEvent()
        {
        }

        protected internal AuditEvent(AuditRequest source)
        {
            Contract.Requires(source != null);
            Contract.Requires(source.Timestamp != null);
            Contract.Requires(source.Timestamp.Kind != DateTimeKind.Local);
            Guard.AgainstNullArgument(nameof(source), source);
            if (source.Timestamp == null)
            {
                throw new ArgumentException("Source must have a non-null Timestamp", nameof(source));
            }

            if (source.Timestamp.Kind == DateTimeKind.Local)
            {
                throw new ArgumentException("Source must have a UTC Timestamp", nameof(source));
            }

            Contract.EndContractBlock();

            // copy data from source
            this.Data = source.Data;
            this.DataId = source.DataId;
            this.DataType = source.DataType;
            this.Description = source.Description;
            this.EventType = source.EventType;
            this.OperationType = source.OperationType;
            this.TargetId = source.TargetId;
            this.TargetType = source.TargetType;
            this.Timestamp = source.Timestamp.AsUtcDateTime();

            this.SessionId = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", this.TargetType, this.TargetId);
        }

        /// <summary>
        /// Operation type of the activity
        /// </summary>
        [Required]
        public OperationType OperationType { get; set; }

        /// <summary>
        /// Event type 
        /// </summary>
        [Required]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the event created UTC date/time.
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Target id of the activity, for example, case ref no
        /// </summary>
        [Required]
        public string TargetId { get; set; }

        /// <summary>
        /// Target type
        /// </summary>
        [Required]
        public string TargetType { get; set; }

        /// <summary>
        /// Description of the action 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data type of the data passed
        /// </summary>
        [RequiredTogetherWith("DataId")]
        public string DataType { get; set; }

        /// <summary>
        /// Data Id 
        /// </summary>
        [RequiredTogetherWith("DataType")]
        public string DataId { get; set; }

        /// <summary>
        /// Actual data modified with the action
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// Correlation key
        /// </summary>
        public string CorrelationKey { get; set; }

        /// <summary>
        /// Application from where the audit event is initiated.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Machine name from where the audit event is raised.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [RequiredTogetherWith("UserName")]

        public string UserId { get; set; }

        /// <summary>
        /// User name 
        /// </summary>
        [RequiredTogetherWith("UserId")]
        public string UserName { get; set; }

        /// <summary>
        /// User Email 
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// User Identity 
        /// </summary>
        public string UserIdentity { get; set; }

        /// <summary>
        /// The SessionId (for service bus)
        /// </summary>
        public string SessionId { get; set; }

        internal void EnsureValid()
        {
            Validator.ValidateObject(this, new ValidationContext(this));
        }
    }
}
