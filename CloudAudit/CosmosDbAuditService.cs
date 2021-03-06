﻿namespace CloudAudit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using CloudAudit.Client.Model;

    using LiteGuard;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

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
                            await this.CreateAuditCollectionIfNotExist();

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
            catch (DocumentClientException)
            {
                ////For delaying the service bus recall.
                await Task.Delay(3000);
                throw;
            }
        }

        /// <summary>
        /// Fetch audit records from the document database matching the search term
        /// </summary>
        /// <param name="targetType">audit target type</param>
        /// <param name="targetId">audit target id</param>
        /// <param name="searchTerm">search term </param>
        /// <param name="pageSize">number of records to return</param>
        /// <param name="continuationToken">Continuous token string for the next page.</param>
        /// <returns>return AuditList object</returns>
        public async Task<AuditList> RetrieveAuditListAsync(
            string targetType, 
            string targetId, 
            string searchTerm, 
            int pageSize, 
            string continuationToken)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(targetType));
            Contract.Requires(!string.IsNullOrWhiteSpace(targetId));
            Guard.AgainstNullArgument(nameof(targetType), targetType);
            Guard.AgainstNullArgument(nameof(targetId), targetId);
            Contract.EndContractBlock();

            var paramterCollection = new SqlParameterCollection(
                new SqlParameter[]
                {
                    new SqlParameter { Name = "@TargetType", Value = targetType },
                    new SqlParameter { Name = "@TargetId", Value = targetId }
                });
            AuditList auditList = new AuditList();

            var sqlQuery = "SELECT a.id, a.EventType, a.Timestamp, a.DataType, a.Description, a.UserId, " +
                    "a.UserName, a.UserEmail, a.OperationType  FROM audits a WHERE  a.PartitionKey = CONCAT(@TargetType,'-', @TargetId)";

                // If the search term is not null add the search term also in the where 
                if (searchTerm != null)
                {
                    sqlQuery += "  AND (CONTAINS(UPPER(a.UserName), UPPER(@SearchTerm)) OR CONTAINS(UPPER(a.EventType), UPPER(@SearchTerm)) )";
                    paramterCollection.Add(new SqlParameter { Name = "@SearchTerm", Value = searchTerm });
                }

            sqlQuery += " ORDER BY a.Timestamp DESC";
            var querySpec = new SqlQuerySpec(sqlQuery, paramterCollection);

            var collectionLink = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var results = new List<AuditRecord>();
            var nextContinuationToken = string.Empty;

            using (var client = new DocumentClient(new Uri(this.accountEndpoint), this.accountKey))
            {
                IDocumentQuery<AuditRecord> query = client.CreateDocumentQuery<AuditRecord>(
                    collectionLink,
                    querySpec,
                    new FeedOptions
                    {
                        MaxItemCount = pageSize,
                        RequestContinuation = continuationToken,
                        PartitionKey = new PartitionKey(targetType + "-" + targetId)
                    }).AsDocumentQuery();

                if (query.HasMoreResults)
                {
                    var result = await query.ExecuteNextAsync<AuditRecord>();
                    nextContinuationToken = result.ResponseContinuation;
                    results.AddRange(result);
                }

                auditList.ItemsList = results;
                auditList.ContinuationToken = nextContinuationToken;
            }

            return auditList;
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

                var cosmosDbUri = UriFactory.CreateDatabaseUri(DatabaseId);

                await client.CreateDocumentCollectionIfNotExistsAsync(cosmosDbUri, docDefinition, options);
            }
        }
    }
}