using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudAudit.Client
{
    public interface IAuditClient
    {
        /// <summary>
        /// Audits your DTO asynchronously.
        /// </summary>
        /// <param name="auditRequest">The audit request.</param>
        /// <returns></returns>
        Task AuditAsync(AuditRequest auditRequest);

        /// <summary>
        /// Audits your DTO synchronously.
        /// </summary>
        /// <param name="auditRequest">The audit request.</param>
        /// <returns></returns>
        void Audit(AuditRequest auditRequest);
    }
}
