namespace CloudAudit.Client
{
    using System;
    using System.Diagnostics.Contracts;

    using CloudAudit.Client.Model;

    using LiteGuard;

    /// <summary>
    /// Extension methods on <see cref="AuditRequest"/> that implement the Fluent API
    /// </summary>
    public static class AuditRequestApi
    {
        /// <summary>
        /// Attaches data to the <paramref name="source"/> <see cref="AuditRequest"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data object</typeparam>
        /// <param name="source">The source <see cref="AuditRequest"/>.</param>
        /// <param name="data">The data object.</param>
        /// <param name="dataIdAccessor">The data identifier accessor.</param>
        /// <returns>The <see cref="AuditRequest"/></returns>
        public static AuditRequest WithData<T>(this AuditRequest source, T data, Func<T, string> dataIdAccessor)
            where T : class
        {
            Contract.Requires(source != null);
            Contract.Requires(data != null);
            Contract.Requires(dataIdAccessor != null);
            Guard.AgainstNullArgument(nameof(source), source);
            Guard.AgainstNullArgument(nameof(data), data);
            Guard.AgainstNullArgument(nameof(dataIdAccessor), dataIdAccessor);
            Contract.EndContractBlock();

            source.Data = (dynamic)data;
            source.DataType = typeof(T).FullName;
            source.DataId = dataIdAccessor(data);

            return source;
        }

        /// <summary>
        /// Indicates that the <paramref name="source"/> <see cref="AuditRequest"/>
        /// has no data payload
        /// </summary>
        /// <param name="source">The source <see cref="AuditRequest"/>.</param>
        /// <returns>The <see cref="AuditRequest"/></returns>
        public static AuditRequest WithNoData(this AuditRequest source)
        {
            Contract.Requires(source != null);
            Guard.AgainstNullArgument(nameof(source), source);
            Contract.EndContractBlock();

            source.DataType = null;
            source.DataId = null;
            source.Data = null;

            return source;
        }

        /// <summary>
        /// Defines the <paramref name="eventName"/> for the <paramref name="source"/> <see cref="AuditRequest"/>
        /// </summary>
        /// <param name="source">The source <see cref="AuditRequest"/>.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>The <see cref="AuditRequest"/></returns>
        public static AuditRequest AsEvent(this AuditRequest source, string eventName)
        {
            Contract.Requires(source != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(eventName));
            Guard.AgainstNullArgument(nameof(source), source);
            Guard.AgainstNullArgument(nameof(eventName), eventName);
            Contract.EndContractBlock();

            source.EventType = eventName;

            return source;
        }

        /// <summary>
        /// Adds a <paramref name="description"/> to the <paramref name="source"/> <see cref="AuditRequest"/>.
        /// </summary>
        /// <param name="source">The source <see cref="AuditRequest"/>.</param>
        /// <param name="description">The description.</param>
        /// <returns>The <see cref="AuditRequest"/></returns>
        public static AuditRequest WithDescription(this AuditRequest source, string description)
        {
            Contract.Requires(source != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(description));
            Guard.AgainstNullArgument(nameof(source), source);
            Guard.AgainstNullArgument(nameof(description), description);
            Contract.EndContractBlock();

            source.Description = description;

            return source;
        }

        /// <summary>
        /// Converts the <paramref name="source"/> <see cref="AuditRequest"/> to
        /// an equivalent <see cref="AuditEvent"/> instance
        /// </summary>
        /// <param name="source">The source <see cref="AuditRequest"/>.</param>
        /// <returns>The <see cref="AuditEvent"/></returns>
        internal static AuditEvent AsAuditEvent(this AuditRequest source)
        {
            Contract.Requires(source != null);
            Guard.AgainstNullArgument(nameof(source), source);
            Contract.EndContractBlock();

            var result = new AuditEvent(source);
            return result;
        }
    }
}
