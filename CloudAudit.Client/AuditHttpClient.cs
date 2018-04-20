namespace CloudAudit.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading.Tasks;

    using CloudAudit.Client.Extensions;
    using CloudAudit.Client.Model;

    using LiteGuard;
    using Newtonsoft.Json;

    /// <summary>
    /// Implementation of <see cref="IAuditClient"/> and
    /// <see cref="IAuditReadClient"/> that uses direct
    /// HTTP calls to the Azure Functions
    /// </summary>
    /// <seealso cref="CloudAudit.Client.IAuditClient" />
    /// <seealso cref="CloudAudit.Client.IAuditReadClient" />
    /// <seealso cref="System.IDisposable" />
    public class AuditHttpClient : IAuditClient, IAuditReadClient, IDisposable
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

            AuditConfiguration.Current.ApplyTo(auditEvent);
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

            AuditConfiguration.Current.ApplyTo(auditEvent);
            auditEvent.EnsureValid();

            var url = $"{this.serviceBase}/api/{AuditAsyncRoute}";
            await this.httpClient.PostAsJsonAsync(url, auditEvent);
        }

        /// <summary>
        /// Function for retrieving the audit list from the Cosmos DB
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="targetId"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTerm"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<AuditList> GetAuditItemsListAsync(
            string targetType, 
            string targetId, 
            int pageSize, 
            string searchTerm)
        {
            return await GetAuditItemsListAsync(targetType, targetId, pageSize, searchTerm, string.Empty);
        }

        /// <summary>
        /// Function for retrieving the audit list from the Cosmos DB
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="targetId"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<AuditList> GetAuditItemsListAsync(
            string targetType, 
            string targetId, 
            int pageSize, 
            string searchTerm, 
            string continuationToken)
        {
            var requestUrl = $"{this.serviceBase}/api/{AuditAsyncRoute}/{targetType}/{targetId}/{pageSize}";
            if (searchTerm != string.Empty)
            {
                requestUrl += "?searchTerm=" + searchTerm;
            }

            if (continuationToken != string.Empty)
            {
                this.httpClient.DefaultRequestHeaders.Add("ContinuationToken", continuationToken);
            }

            var response = await this.httpClient.GetAsync(requestUrl);
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<AuditList>(json);
            }
            else
            {
                throw new Exception(string.Format("Unable to retrieve {0} number of audit items for target id {1} and targetType {2}", pageSize, targetId, targetType));
            }
        }
    }
}
