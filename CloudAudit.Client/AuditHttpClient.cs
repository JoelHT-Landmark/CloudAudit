namespace CloudAudit.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading.Tasks;

    using CloudAudit.Client.Extensions;

    using LiteGuard;

    /// <summary>
    /// Implementation of <see cref="IAuditClient"/> that uses direct
    /// HTTP calls to the Azure Function
    /// </summary>
    /// <seealso cref="CloudAudit.Client.IAuditClient" />
    /// <seealso cref="System.IDisposable" />
    public class AuditHttpClient : IAuditClient, IDisposable
    {
        public const string AuditAsyncRoute = "audit";

        private HttpClient httpClient;
        private bool disposed;
        private string serviceBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditHttpClient"/> class.
        /// </summary>
        /// <param name="serviceBase">The service base.</param>
        public AuditHttpClient(string serviceBase)
        {
            Contract.Requires(serviceBase != null);
            Guard.AgainstNullArgument(nameof(serviceBase), serviceBase);
            Contract.EndContractBlock();

            this.httpClient = new HttpClient();
            this.serviceBase = serviceBase;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                this.httpClient.Dispose();
                this.httpClient = null;
            }

            disposed = true;
        }

        /// <summary>
        /// Audits your DTO synchronously.
        /// </summary>
        /// <param name="auditRequest">The audit request.</param>
        public void Audit(AuditRequest auditRequest)
        {
            var auditEvent = auditRequest.AsAuditEvent();

            //// AuditConfiguration.Current.ApplyTo(auditEvent);
            auditEvent.EnsureValid();

            var url = $"{this.serviceBase}/api/{AuditAsyncRoute}";
            this.httpClient.PostAsJson(url, auditEvent);
        }

        /// <summary>
        /// Audits your DTO asynchronously.
        /// </summary>
        /// <param name="auditRequest">The audit request.</param>
        /// <returns></returns>
        public async Task AuditAsync(AuditRequest auditRequest)
        {
            var auditEvent = auditRequest.AsAuditEvent();

            //AuditConfiguration.Current.ApplyTo(auditEvent);
            auditEvent.EnsureValid();

            var url = $"{this.serviceBase}/api/{AuditAsyncRoute}";
            await this.httpClient.PostAsJsonAsync(url, auditEvent);
        }
    }
}
