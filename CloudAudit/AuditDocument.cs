namespace CloudAudit
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Globalization;

    using CloudAudit.Client.Model;

    using Newtonsoft.Json;

    internal class AuditDocument
    {
        [JsonConstructor]
        internal AuditDocument()
        {
        }

        internal AuditDocument(AuditEvent auditEvent)
        {
            ////Make the id matching the timestamp value if specified - or NOW
            this.id = (auditEvent.Timestamp != default(DateTime)) ? 
                auditEvent.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture)
                : DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture);

            this.PartitionKey = auditEvent.TargetType + "-" + auditEvent.TargetId;
            this.ApplicationName = auditEvent.ApplicationName;
            this.CorrelationKey = auditEvent.CorrelationKey;

            this.DataId = auditEvent.DataId;
            this.DataType = auditEvent.DataType;
            if (!string.IsNullOrWhiteSpace(auditEvent.DataType))
            {
                var serializedData = JsonConvert.SerializeObject(auditEvent.Data);
                this.Data = JsonConvert.DeserializeObject<ExpandoObject>(serializedData);
            }

            this.Description = auditEvent.Description;
            this.EventType = auditEvent.EventType;
            this.MachineName = auditEvent.MachineName;
            this.OperationType = (int)auditEvent.OperationType;
            this.TargetId = auditEvent.TargetId;
            this.TargetType = auditEvent.TargetType;
            this.Timestamp = auditEvent.Timestamp;
            this.UserEmail = auditEvent.UserEmail;
            this.UserId = auditEvent.UserId;
            this.UserIdentity = auditEvent.UserIdentity;
            this.UserName = auditEvent.UserName;
        }

        /// <summary>
        /// Document Id will be Timestamp
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", MessageId = "id", Justification = "https://github.com/Azure/azure-webjobs-sdk/issues/1214  A known issue with azure webjob sdk")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "id", Justification = "https://github.com/Azure/azure-webjobs-sdk/issues/1214  A known issue with azure webjob sdk")]
        public string id { get; set; }

        /// <summary>
        /// PartitionKey will be {TargetType - TargetId}
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// Operation type of the activity
        /// </summary>
        public int OperationType { get; set; }

        /// <summary>
        /// Event type 
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the event created UTC date/time.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Target id of the activity, for example, case ref no
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Target type
        /// </summary>
        public string TargetType { get; set; }

        /// <summary>
        /// Description of the action 
        /// </summary>
        public string Description { get; set; }

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
        public string UserId { get; set; }

        /// <summary>
        /// User name 
        /// </summary>
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
        /// Data type of the data passed
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Data Id 
        /// </summary>
        public string DataId { get; set; }

        /// <summary>
        /// Actual data modified with the action
        /// </summary>
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "It's not a collection, it's just data")]
        public ExpandoObject Data { get; set; }
    }
}