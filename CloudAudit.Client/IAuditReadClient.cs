namespace CloudAudit.Client
{
    using System.Threading.Tasks;

    using CloudAudit.Client.Model;

    public interface IAuditReadClient
    {
        /// <summary>
         /// Function for retrieving the audit list from the Cosmos DB
         /// </summary>
         /// <param name="targetType"></param>
         /// <param name="targetId"></param>
         /// <param name="pageSize"></param>
         /// <param name="continuationToken"></param>
         /// <returns></returns>
        Task<AuditList> GetAuditItemsListAsync(string targetType, string targetId, int pageSize, string searchTerm, string continuationToken);

        /// <summary>
        /// Function for retrieving the audit list from the Cosmos DB
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="targetId"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<AuditList> GetAuditItemsListAsync(string targetType, string targetId, int pageSize, string searchTerm);
    }
}
