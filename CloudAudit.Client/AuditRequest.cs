namespace CloudAudit.Client
{
    using System;
    using System.Diagnostics.Contracts;

    using CloudAudit.Client.Model;

    using LiteGuard;

    /// <summary>
    /// Defines an abstract Audit Request that is used to create the
    /// <see cref="AuditEvent"/> that goes over the wire.
    /// </summary>
    /// <remarks>
    /// This really is just to support the "fluent API" - preventing a
    /// developer from using <see cref="AuditEvent"/> instances directly.
    /// </remarks>
    public class AuditRequest
    {
        private static Func<DateTime> timeProviderFunction = () => DateTime.UtcNow;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditRequest"/> class.
        /// </summary>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetId">The target identifier.</param>
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

        /// <summary>
        /// Gets or sets the time provider.
        /// </summary>
        /// <remarks>
        /// Used for tests only
        /// </remarks>
        /// <value>
        /// The time provider.
        /// </value>
        protected internal static Func<DateTime> TimeProvider
        {
            get { return timeProviderFunction; }
            set { timeProviderFunction = value; }
        }

        /// <summary>
        /// Returns an <see cref="AuditRequest"/> representing a view of
        /// the target object
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetId">The target identifier.</param>
        /// <returns>The <see cref="AuditRequest"/></returns>
        public static AuditRequest AsViewOf(Type targetType, string targetId)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            return new AuditRequest(OperationType.View, targetType, targetId);
        }

        /// <summary>
        /// Returns an <see cref="AuditRequest" /> representing a view of
        /// the target object
        /// </summary>
        /// <typeparam name="T">The type of the target</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="targetIdAccessor">The target identifier accessor.</param>
        /// <returns>
        /// The <see cref="AuditRequest" />
        /// </returns>
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

        /// <summary>
        /// Returns an <see cref="AuditRequest"/> representing a change to
        /// the target object
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetId">The target identifier.</param>
        /// <returns>The <see cref="AuditRequest"/></returns>
        public static AuditRequest AsChangeTo(Type targetType, string targetId)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            return new AuditRequest(OperationType.Change, targetType, targetId);
        }

        /// <summary>
        /// Returns an <see cref="AuditRequest" /> representing a change to
        /// the target object
        /// </summary>
        /// <typeparam name="T">The type of the target</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="targetIdAccessor">The target identifier accessor.</param>
        /// <returns>
        /// The <see cref="AuditRequest" />
        /// </returns>
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

        /// <summary>
        /// Returns an <see cref="AuditRequest"/> representing an action on
        /// the target object
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetId">The target identifier.</param>
        /// <returns>The <see cref="AuditRequest"/></returns>
        public static AuditRequest AsActionOn(Type targetType, string targetId)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            return new AuditRequest(OperationType.Action, targetType, targetId);
        }

        /// <summary>
        /// Returns an <see cref="AuditRequest" /> representing an action on
        /// the target object
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="targetIdAccessor">The target identifier accessor.</param>
        /// <returns>
        /// The <see cref="AuditRequest" />
        /// </returns>
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

        /// <summary>
        /// Returns an <see cref="AuditRequest"/> representing a statement about
        /// the target object
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetId">The target identifier.</param>
        /// <returns>The <see cref="AuditRequest"/></returns>
        public static AuditRequest AsStatementAbout(Type targetType, string targetId)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            return new AuditRequest(OperationType.Statement, targetType, targetId);
        }

        /// <summary>
        /// Returns an <see cref="AuditRequest" /> representing a statement about
        /// the target object
        /// </summary>
        /// <typeparam name="T">The type of the target</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="targetIdAccessor">The target identifier accessor.</param>
        /// <returns>
        /// The <see cref="AuditRequest" />
        /// </returns>
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
