namespace CloudAudit.Client.Tests
{
    using System;
    using System.Globalization;

    using CloudAudit.Client.Model;
    using CloudAudit.Client.Tests.Mocks;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public partial class AuditRequestTests
    {
        [TestMethod]
        public void AuditingACaseViewCreatesAuditRequestCorrectly()
        {
            var newCase = new MockCase();
            var request = AuditRequest
                .AsViewOf(newCase, c => c.SysRef)
                .WithData(newCase, c => c.SysRef)
                .AsEvent("ReadCase")
                .WithDescription("Read case");

            request.OperationType.Should().Be(OperationType.View);
            request.TargetType.Should().Be(typeof(MockCase).Name);
            request.TargetId.Should().Be(newCase.SysRef);
            request.DataType.Should().Be(typeof(MockCase).FullName);
            request.DataId.Should().Be(newCase.SysRef);
            request.EventType.Should().Be("ReadCase");
            request.Description.Should().Be("Read case");
            request.Timestamp.Should().BeCloseTo(DateTime.UtcNow);

            var auditEvent = request.AsAuditEvent();

            Action act = () => auditEvent.EnsureValid();
            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditingACaseChangeCreatesAuditRequestCorrectly()
        {
            var newCase = new MockCase();
            var request = AuditRequest
                .AsChangeTo(newCase, c => c.SysRef)
                .WithData(newCase, c => c.SysRef)
                .AsEvent("SaveCase")
                .WithDescription("Save case");
            request.OperationType.Should().Be(OperationType.Change);
            request.TargetType.Should().Be(typeof(MockCase).Name);
            request.TargetId.Should().Be(newCase.SysRef);
            request.DataType.Should().Be(typeof(MockCase).FullName);
            request.DataId.Should().Be(newCase.SysRef);
            request.EventType.Should().Be("SaveCase");
            request.Description.Should().Be("Save case");
            request.Timestamp.Should().BeCloseTo(DateTime.UtcNow);

            var auditEvent = request.AsAuditEvent();

            Action act = () => auditEvent.EnsureValid();
            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditingACaseChangeForAnAttachmentCreatesAuditRequestCorrectly()
        {
            var newAttachment = new MockAttachment();

            var request = AuditRequest
                .AsChangeTo(typeof(MockCase), "sysref123")
                .WithData(newAttachment, c => c.Id.ToString(CultureInfo.InvariantCulture))
                .AsEvent("SaveAttachment")
                .WithDescription("Save Attachment");

            request.OperationType.Should().Be(OperationType.Change);
            request.TargetType.Should().Be(typeof(MockCase).Name);
            request.TargetId.Should().Be("sysref123");
            request.DataType.Should().Be(typeof(MockAttachment).FullName);
            request.DataId.Should().Be(newAttachment.Id.ToString(CultureInfo.InvariantCulture));
            request.EventType.Should().Be("SaveAttachment");
            request.Description.Should().Be("Save Attachment");
            request.Timestamp.Should().BeCloseTo(DateTime.UtcNow);

            var auditEvent = request.AsAuditEvent();

            Action act = () => auditEvent.EnsureValid();
            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditingACaseActionCreatesAuditRequestCorrectly()
        {
            var newCase = new MockCase();
            var enqueuedEmailNotification = new MockEmailNotification();

            var request = AuditRequest
                .AsActionOn(newCase, c => c.SysRef)
                .WithData(enqueuedEmailNotification, n => n.Id.ToString(CultureInfo.InvariantCulture))
                .AsEvent("EmailSent")
                .WithDescription("Email sent");

            request.OperationType.Should().Be(OperationType.Action);
            request.TargetType.Should().Be(typeof(MockCase).Name);
            request.TargetId.Should().Be(newCase.SysRef);
            request.DataType.Should().Be(typeof(MockEmailNotification).FullName);
            request.DataId.Should().Be(enqueuedEmailNotification.Id.ToString(CultureInfo.InvariantCulture));
            request.EventType.Should().Be("EmailSent");
            request.Description.Should().Be("Email sent");
            request.Timestamp.Should().BeCloseTo(DateTime.UtcNow);

            var auditEvent = request.AsAuditEvent();

            Action act = () => auditEvent.EnsureValid();
            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditingACaseStatementCreatesAuditRequestCorrectly()
        {
            var newCase = new MockCase();

            var request = AuditRequest
                .AsStatementAbout(newCase, c => c.SysRef)
                .WithNoData()
                .AsEvent("PaymentConfirmationMessageNotSent")
                .WithDescription("Payment confirmation message not sent");

            request.OperationType.Should().Be(OperationType.Statement);
            request.TargetType.Should().Be(typeof(MockCase).Name);
            request.TargetId.Should().Be(newCase.SysRef);
            request.DataType.Should().BeNull();
            request.DataId.Should().BeNull();
            ////request.Data.Should().BeNull();
            request.EventType.Should().Be("PaymentConfirmationMessageNotSent");
            request.Description.Should().Be("Payment confirmation message not sent");
            request.Timestamp.Should().BeCloseTo(DateTime.UtcNow);

            var auditEvent = request.AsAuditEvent();

            Action act = () => auditEvent.EnsureValid();
            act.Should().NotThrow();
        }
    }
}
