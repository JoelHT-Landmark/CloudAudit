namespace CloudAudit.FunctionAPI
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using CloudAudit.Client;
    using CloudAudit.Client.Model;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;

    public static class AuditFunctions
    {
        const string SubscriptionName = "CloudAudit";
        static AuditFunctions()
        {
        }

        [FunctionName(nameof(AuditAsyncViaServiceBus))]
        public static async Task AuditAsyncViaServiceBus(
            [ServiceBusTrigger(AuditServiceBusClient.ServiceBusTopicName, SubscriptionName, 
                               AccessRights.Listen, Connection = "Audit.ServiceBus")]
        BrokeredMessage message, TraceWriter log)
        {
            var payload = message.GetBody<string>();
            var auditEvent = JsonConvert.DeserializeObject<AuditEvent>(payload);
            var service = new CosmosDbAuditService();
            await service.AuditAsync(auditEvent);
        }

        [FunctionName(nameof(AuditAsyncViaHttp))]
        public static async Task<HttpResponseMessage> AuditAsyncViaHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = AuditHttpClient.AuditAsyncRoute)]
            HttpRequestMessage req, TraceWriter log)
        {
            var body = await req.Content.ReadAsStringAsync();

            var auditEvent = JsonConvert.DeserializeObject<AuditEvent>(body);
            try
            {
                auditEvent.EnsureValid();
            }
            catch (ValidationException valex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, valex.Message);
            }

            var service = new CosmosDbAuditService();
            await service.AuditAsync(auditEvent);

            return req.CreateResponse(HttpStatusCode.OK, "OK");
        }
    }
}
