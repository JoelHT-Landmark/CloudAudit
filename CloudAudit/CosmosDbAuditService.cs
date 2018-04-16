namespace CloudAudit
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Security;
    using System.Threading.Tasks;

    using CloudAudit.Client.Model;

    using LiteGuard;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    /// <summary>
    /// Implementation of <see cref="IAuditService"/> that persists 
    /// <see cref="AuditEvent"/> instances to CosmosDB
    /// </summary>
    /// <seealso cref="CloudAudit.IAuditService" />
    public class CosmosDbAuditService : IAuditService
    {
        private const string DatabaseId = "MyTenant";
        private const string CollectionId = "Audit";
        private const int DefaultThroughput = 400;

        private string accountEndpoint;
        private string accountKey;

        public CosmosDbAuditService()
        {
            this.accountEndpoint = Environment.GetEnvironmentVariable("CosmosDbAccountEndpoint", EnvironmentVariableTarget.Process);
            this.accountKey = Environment.GetEnvironmentVariable("CosmosDbAccountKey", EnvironmentVariableTarget.Process);

        }

        /// <summary>
        /// Create audit record in asynchronous mode
        /// </summary>
        /// <param name="auditDocument"></param>
        /// <returns></returns>
        public async Task AuditAsync(AuditEvent auditEvent)
        {
            Contract.Requires(auditEvent != null);
            Guard.AgainstNullArgument(nameof(auditEvent), auditEvent);
            Contract.EndContractBlock();

            var auditDocument = new AuditDocument(auditEvent);

            try
            {
                using (var client = new DocumentClient(new Uri(this.accountEndpoint), this.accountKey))
                {
                    var collectionLink = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
                    try
                    {
                        await client.CreateDocumentAsync(collectionLink, auditDocument, null, true);
                    }
                    catch (DocumentClientException e)
                    {
                        if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            await CreateAuditCollectionIfNotExist();

                            await client.CreateDocumentAsync(collectionLink, auditDocument, null, true);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            catch (DocumentClientException docEx) when (docEx.Error.Code == "Conflict")
            {
                return;
            }
            catch (DocumentClientException docEx)
            {
                ////For delaying the service bus recall.
                await Task.Delay(3000);
                throw;
            }
        }

        /// <summary>
        /// Creates the audit collection if it does not already exist.
        /// </summary>
        /// <returns></returns>
        private async Task CreateAuditCollectionIfNotExist()
        {
            using (var client = new DocumentClient(new Uri(this.accountEndpoint), this.accountKey))
            {
                var database = new Database() { Id = DatabaseId };

                await client.CreateDatabaseIfNotExistsAsync(database);

                var docDefinition = new DocumentCollection();

                var rangeIndex = new RangeIndex(DataType.String) { Precision = -1 };

                docDefinition.Id = CollectionId;

                docDefinition.PartitionKey.Paths.Add("/PartitionKey");

                docDefinition.IndexingPolicy = new IndexingPolicy(rangeIndex);

                docDefinition.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/Data/*" });

                var options = new RequestOptions() { OfferThroughput = DefaultThroughput };

                var dbUri = UriFactory.CreateDatabaseUri(DatabaseId);

                await client.CreateDocumentCollectionIfNotExistsAsync(dbUri, docDefinition, options);
            }
        }
    }
}
