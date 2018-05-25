namespace CloudAudit.Client
{
    using System;
    using System.Diagnostics.Contracts;

    using CloudAudit.Client.Model;

    using LiteGuard;

    public class AuditRequest
    {
        private static Func<DateTime> timeProviderFunction = () => DateTime.UtcNow;

        protected AuditRequest(OperationType operationType, Type targetType, string targetId)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            this.OperationType = operationType;
            this.TargetType = targetType.Name;
            this.TargetId = targetId;
            this.Timestamp = timeProviderFunction();
        }

        /// <summary>
        /// Operation type of the activity
        /// </summary>
        public OperationType OperationType { get; }

        /// <summary>
        /// Event type 
        /// </summary>
        public string EventType { get; protected internal set; }

        /// <summary>
        /// Gets or sets the event created UTC date/time.
        /// </summary>
        public DateTime Timestamp { get; protected internal set; }

        /// <summary>
        /// Target id of the activity, for example, case ref no
        /// </summary>
        public string TargetId { get; }

        /// <summary>
        /// Target type
        /// </summary>
        public string TargetType { get; }

        /// <summary>
        /// Description of the action 
        /// </summary>
        public string Description { get; protected internal set; }

        /// <summary>
        /// Data type of the data passed
        /// </summary>
        public string DataType { get; protected internal set; }

        /// <summary>
        /// Data Id 
        /// </summary>
        public string DataId { get; protected internal set; }

        /// <summary>
        /// Actual data modified with the action
        /// </summary>
        public dynamic Data { get; protected internal set; }

        protected internal static Func<DateTime> TimeProvider
        {
            get { return timeProviderFunction; }
            set { timeProviderFunction = value; }
        }

        public static AuditRequest AsViewOf(Type targetType, string targetId)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            return new AuditRequest(OperationType.View, targetType, targetId);
        }

        public static AuditRequest AsViewOf<T>(T target, Func<T, string> targetIdAccessor)
            where T : class
        {
            Contract.Requires(target != null);
            Contract.Requires(targetIdAccessor != null);
            Guard.AgainstNullArgument(nameof(target), target);
            Guard.AgainstNullArgument(nameof(targetIdAccessor), targetIdAccessor);
            Contract.EndContractBlock();

            var request = new AuditRequest(OperationType.View, typeof(T), targetIdAccessor(target));
            return request;
        }

        public static AuditRequest AsChangeTo(Type targetType, string targetId)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            return new AuditRequest(OperationType.Change, targetType, targetId);
        }

        public static AuditRequest AsChangeTo<T>(T target, Func<T, string> targetIdAccessor)
            where T : class
        {
            Contract.Requires(target != null);
            Contract.Requires(targetIdAccessor != null);
            Guard.AgainstNullArgument(nameof(target), target);
            Guard.AgainstNullArgument(nameof(targetIdAccessor), targetIdAccessor);
            Contract.EndContractBlock();

            var request = new AuditRequest(OperationType.Change, typeof(T), targetIdAccessor(target));
            return request;
        }

        public static AuditRequest AsActionOn(Type targetType, string targetId)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            return new AuditRequest(OperationType.Action, targetType, targetId);
        }

        public static AuditRequest AsActionOn<T>(T target, Func<T, string> targetIdAccessor)
            where T : class
        {
            Contract.Requires(target != null);
            Contract.Requires(targetIdAccessor != null);
            Guard.AgainstNullArgument(nameof(target), target);
            Guard.AgainstNullArgument(nameof(targetIdAccessor), targetIdAccessor);
            Contract.EndContractBlock();

            var request = new AuditRequest(OperationType.Action, typeof(T), targetIdAccessor(target));
            return request;
        }

        public static AuditRequest AsStatementAbout(Type targetType, string targetId)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            return new AuditRequest(OperationType.Statement, targetType, targetId);
        }

        public static AuditRequest AsStatementAbout<T>(T target, Func<T, string> targetIdAccessor)
            where T : class
        {
            Contract.Requires(target != null);
            Contract.Requires(targetIdAccessor != null);
            Guard.AgainstNullArgument(nameof(target), target);
            Guard.AgainstNullArgument(nameof(targetIdAccessor), targetIdAccessor);
            Contract.EndContractBlock();

            var request = new AuditRequest(OperationType.Statement, typeof(T), targetIdAccessor(target));
            return request;
        }
    }
}
