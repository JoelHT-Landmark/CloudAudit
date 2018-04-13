using CloudAudit.Client.Model;
using LiteGuard;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudAudit.Client
{
    public static class AuditRequestApi
    {
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
