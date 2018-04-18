namespace CloudAudit.Client
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using LiteGuard;

    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;

    public class AuditServiceBusClient : IAuditClient
    {
        public const string ServiceBusTopicName = @"Audit";
        private readonly TopicClient topicClient;
        private bool disposed;

        public AuditServiceBusClient(string connectionString)
        {
            Contract.Requires(connectionString != null);
            Guard.AgainstNullArgument(nameof(connectionString), connectionString);
            Contract.EndContractBlock();

            this.topicClient = TopicClient.CreateFromConnectionString(connectionString, ServiceBusTopicName);
        }

        public void Audit(AuditRequest auditRequest)
        {
            var auditEvent = auditRequest.AsAuditEvent();

            AuditConfiguration.Current.ApplyTo(auditEvent);
            auditEvent.EnsureValid();

            var payload = JsonConvert.SerializeObject(auditEvent);
            var message = new BrokeredMessage(payload);
            this.topicClient.Send(message);
        }

        public async Task AuditAsync(AuditRequest auditRequest)
        {
            var auditEvent = auditRequest.AsAuditEvent();

            AuditConfiguration.Current.ApplyTo(auditEvent);
            auditEvent.EnsureValid();

            var payload = JsonConvert.SerializeObject(auditEvent);
            var message = new BrokeredMessage(payload);
            await this.topicClient.SendAsync(message);
        }
    }
}
