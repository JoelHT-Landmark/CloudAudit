// <copyright file="Program.cs" company="Landmark Information Group Ltd">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author>Joel Hammond-Turner</author>
// <summary>Console sample for Cloud Audit</summary>
namespace CloudAudit.ConsoleSample
{
    using System;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using CloudAudit.Client;
    using CloudAudit.ConsoleSample.Models;
    using LiteGuard;

    /// <summary>
    /// Console test harness for the <see cref="IAuditClient"/> implementations.
    /// </summary>
    public class Program
    {
        private readonly IAuditClient auditClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        /// <param name="auditClient">The audit client.</param>
        public Program(IAuditClient auditClient)
        {
            Contract.Requires(auditClient != null);
            Guard.AgainstNullArgument(nameof(auditClient), auditClient);
            Contract.EndContractBlock();

            this.auditClient = auditClient;
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public static void Main()
        {
            Console.Write("Press ENTER to start...");
            Console.ReadLine();

            ConfigureAuditing.ForCurrentSignedInUser();

            var client = new AuditHttpClient(ConfigurationManager.AppSettings["Audit.ServiceBase"]);
            ///var client = new AuditServiceBusClient(ConfigurationManager.AppSettings["Audit.ServiceBus"]);

            var program = new Program(client);
            program.Run().Wait();

            Console.Write("Press ENTER to quit...");
            Console.ReadLine();
        }

        /// <summary>
        /// Runs the test sequence.
        /// </summary>
        public async Task Run()
        {
            Console.WriteLine("Writing 'ViewCase' event...");
            var newCase = new Case() { SysRef = "QU123456" };
            var viewCaseRequest = AuditRequest
                .AsViewOf(newCase, c => c.SysRef)
                .WithData(newCase, c => c.SysRef)
                .AsEvent("ViewCase")
                .WithDescription("View case");
            await this.auditClient.AuditAsync(viewCaseRequest);

            Console.WriteLine("Writing 'ChangeCase' event...");
            var changeCaseRequest = AuditRequest
                .AsChangeTo(newCase, c => c.SysRef)
                .WithData(newCase, c => c.SysRef)
                .AsEvent("ChangeCase")
                .WithDescription("Change case");
            await this.auditClient.AuditAsync(changeCaseRequest);

            Console.WriteLine("Writing 'AddApplicant' event...");
            var newApplicant = new Applicant();
            var addAttachmentRequest = AuditRequest
                .AsChangeTo(typeof(Case), "sysref123")
                .WithData(newApplicant, c => Guid.NewGuid().ToString())
                .AsEvent("AddApplicant")
                .WithDescription("Add applicant");
            await this.auditClient.AuditAsync(addAttachmentRequest);

            Console.WriteLine("Writing 'SurveyorAssigned' event...");
            var surveyor = new Surveyor() { Code = "AB123" };
            var surveyorAssignedRequest = AuditRequest
                .AsActionOn(newCase, c => c.SysRef)
                .WithData(surveyor, n => n.Code.ToString())
                .AsEvent("SurveyorAssigned")
                .WithDescription("Surveyor assigned");
            await this.auditClient.AuditAsync(surveyorAssignedRequest);

            Console.WriteLine("Writing 'PaymentConfirmationMessageNotSent' event...");
            var notSentRequest = AuditRequest
                .AsStatementAbout(newCase, c => c.SysRef)
                .WithNoData()
                .AsEvent("PaymentConfirmationMessageNotSent")
                .WithDescription("Payment confirmation message not sent");
            await this.auditClient.AuditAsync(notSentRequest);
        }
    }
}
