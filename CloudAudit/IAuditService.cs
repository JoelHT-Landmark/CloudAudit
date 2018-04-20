namespace CloudAudit
{
    using System.Threading.Tasks;

    using CloudAudit.Client.Model;

    public interface IAuditService
    {
        /// <summary>
        /// Saves the specified <paramref name="auditEvent"/> to CosmosDB
        /// </summary>
        /// <param name="auditDocument"></param>
        /// <returns></returns>
        Task AuditAsync(AuditEvent auditEvent);

        /// <summary>
        /// Fetch audit records from the document database matching the search term
        /// </summary>
        /// <param name="targetType">audit target type</param>
        /// <param name="targetId">audit target id</param>
        /// <param name="searchTerm">search term </param>
        /// <param name="pageSize">number of records to return</param>
        /// <param name="continuationToken">Continuous token string for the next page.</param>
        /// <returns>return AuditList object</returns>
        Task<AuditList> RetrieveAuditListAsync(
            string targetType,
            string targetId,
            string searchTerm,
            int pageSize,
            string continuationToken
            );
    }
}
