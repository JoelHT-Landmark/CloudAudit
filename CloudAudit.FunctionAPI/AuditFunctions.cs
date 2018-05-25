namespace CloudAudit.FunctionAPI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
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
        public const string SubscriptionName = "CloudAudit";

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
            [HttpTrigger(
                AuthorizationLevel.Anonymous, 
                "post", 
                Route = AuditHttpClient.AuditAsyncRoute)]
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

        /// <summary>
        /// Azure function for reading the audit records by page , without any search condition
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <param name="targetType"></param>
        /// <param name="targetId"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [FunctionName("GetAuditListFunction")]
        public static async Task<HttpResponseMessage> GetAuditItemsList(
            [HttpTrigger(
                AuthorizationLevel.Anonymous, 
                "get", 
                Route = "audit/{targetType}/{targetId}/{pageSize}")]
            HttpRequestMessage req, 
            TraceWriter log, 
            string targetType, 
            string targetId, 
            int pageSize)
        {
            log.Info("Fetching the audit records from targetType: " + targetType + " and Targetid: " + targetId);

            // parse query parameter
            string searchTerm = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "searchTerm", true) == 0)
                .Value;

            IEnumerable<string> headers;
            req.Headers.TryGetValues("ContinuationToken", out headers);

            var continuationToken = string.Empty;
            if (headers != null)
            {
                continuationToken = headers.FirstOrDefault();
            }

            var service = new CosmosDbAuditService();
            var result = await service.RetrieveAuditListAsync(
                targetType, 
                targetId, 
                searchTerm, 
                pageSize, 
                continuationToken);

            return req.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
