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
    }
}
