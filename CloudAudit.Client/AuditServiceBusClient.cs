namespace CloudAudit.Client
{
    using System;
    using System.Threading.Tasks;

    public class AuditServiceBusClient : IAuditClient
    {
        public const string ServiceBusTopicName = @"Audit";

        public void Audit(AuditRequest auditRequest)
        {
            throw new NotImplementedException();
        }

        public Task AuditAsync(AuditRequest auditRequest)
        {
            throw new NotImplementedException();
        }
    }
}
