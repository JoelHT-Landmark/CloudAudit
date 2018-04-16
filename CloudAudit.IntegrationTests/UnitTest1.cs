using System;
using System.Threading.Tasks;
using CloudAudit.Client;
using CloudAudit.IntegrationTests.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudAudit.IntegrationTests
{
    [TestClass]
    public class UnitTest1
    {
        private IAuditClient auditClient;
        private Case @case;

        [TestInitialize]
        public void Initialize()
        {
            this.auditClient = new AuditHttpClient("https://azureinaction.azurewebsites.net/");
            this.@case = new Case();
        }

        [TestMethod]
        public void AuditAViewWorks()
        {
            var viewCaseRequest = AuditRequest
                .AsViewOf(@case, c => c.SysRef)
                .WithData(@case, c => c.SysRef)
                .AsEvent("ViewCase")
                .WithDescription("View case");

            Func<Task> act = async () =>
            {
                await this.auditClient.AuditAsync(viewCaseRequest);
            };

            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditAChangeWorks()
        {
            var changeCaseRequest = AuditRequest
                        .AsChangeTo(@case, c => c.SysRef)
                        .WithData(@case, c => c.SysRef)
                        .AsEvent("ChangeCase")
                        .WithDescription("Change case");
            Func<Task> act = async () =>
            {
                await this.auditClient.AuditAsync(changeCaseRequest);
            };
            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditAChangeForDifferentDataWorks()
        {
            var newApplicant = new Applicant();
            var addAttachmentRequest = AuditRequest
            .AsChangeTo(@case, c => c.SysRef)
            .WithData(newApplicant, c => Guid.NewGuid().ToString())
            .AsEvent("AddApplicant")
            .WithDescription("Add applicant");

            Func<Task> act = async () =>
            {
                await this.auditClient.AuditAsync(addAttachmentRequest);
            };

            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditAnActionWorks()
        {
            var surveyor = new Surveyor() { Code = "AB123" };
        var surveyorAssignedRequest = AuditRequest
            .AsActionOn(@case, c => c.SysRef)
            .WithData(surveyor, n => n.Code.ToString())
            .AsEvent("SurveyorAssigned")
            .WithDescription("Surveyor assigned");

            Func<Task> act = async () =>
            {
                await this.auditClient.AuditAsync(surveyorAssignedRequest);
            };

            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditAStatementWorks()
        {
            var notSentRequest = AuditRequest
                .AsStatementAbout(@case, c => c.SysRef)
                .WithNoData()
                .AsEvent("PaymentConfirmationMessageNotSent")
                .WithDescription("Payment confirmation message not sent");

            Func<Task> act = async () =>
            {
                await this.auditClient.AuditAsync(notSentRequest);
            };

            act.Should().NotThrow();
        }
    }
}
