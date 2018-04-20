namespace CloudAudit.Client.Model
{
    using System;

    public class AuditRecord
    {
        /// <summary>
        /// Document id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Operation type of the activity
        /// </summary>
        public OperationType OperationType { get; set; }

        /// <summary>
        /// Event type 
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Data type of the storing data
        /// </summary>
        /// <value>
        /// ValuationHub.Core.Domain.Case, ValuationHub.Core.Domain
        /// </value>
        public string DataType { get; set; }

        /// <summary>
        /// Gets or sets the event created UTC date/time.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Description of the action 
        /// </summary>
        public string Description { get; set; }

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
    }
}