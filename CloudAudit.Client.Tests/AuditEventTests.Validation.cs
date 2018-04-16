namespace CloudAudit.Client.Tests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using CloudAudit.Client.Model;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public partial class AuditEventTests
    {
        [TestMethod]
        public void AuditEventIsNotValidWithUnspecifiedOperationType()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.OperationType = OperationType.Unspecified;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.OperationType)));
        }

        [TestMethod]
        public void AuditEventIsValidWithOtherSpecifiedOperationTypes()
        {
            var operationTypes = ((OperationType[])Enum.GetValues(typeof(OperationType)))
                .Where(v => v != OperationType.Unspecified);

            var auditEvent = GetFullyPopulatedAuditEvent();
            foreach (var operationType in operationTypes)
            {
                auditEvent.OperationType = operationType;

                Action act = () => auditEvent.EnsureValid();
                act.Should().NotThrow<ValidationException>();
            }
        }

        [TestMethod]
        public void AuditEventIsNotValidWithInvalidOperationType()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.OperationType = (OperationType)int.MaxValue;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.OperationType)));
        }

        [TestMethod]
        public void AuditEventIsNotValidWithEmptyTargetType()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.TargetType = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.TargetType)));

            auditEvent.TargetType = string.Empty;
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.TargetType)));

            auditEvent.TargetType = "    ";
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.TargetType)));
        }

        [TestMethod]
        public void AuditEventIsNotValidWithEmptyTargetId()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.TargetId = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.TargetId)));

            auditEvent.TargetId = string.Empty;
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.TargetId)));

            auditEvent.TargetId = "    ";
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.TargetId)));
        }

        [TestMethod]
        public void AuditEventIsNotValidWithEmptyDataType()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.DataType = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.DataType)));

            auditEvent.DataType = string.Empty;
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.DataType)));

            auditEvent.DataType = "    ";
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.DataType)));
        }

        [TestMethod]
        public void AuditEventIsNotValidWithEmptyDataId()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.DataId = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.DataId)));

            auditEvent.DataId = string.Empty;
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.DataId)));

            auditEvent.DataId = "    ";
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.DataId)));
        }

        [TestMethod]
        public void AuditEventIsNotValidWithEmptyDataIfDataIdAndTypeAreSpecified()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.Data = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.Data)));
        }

        [TestMethod]
        public void AuditEventIsValidWithAllNullDataFields()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.DataType = null;
            auditEvent.DataId = null;
            auditEvent.Data = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditEventIsNotValidWithEmptySessionId()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.SessionId = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.SessionId)));

            auditEvent.SessionId = string.Empty;
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.SessionId)));

            auditEvent.SessionId = "    ";
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.SessionId)));
        }

        [TestMethod]
        public void AuditEventIsValidWithAllUserFieldsEmpty()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.UserId = null;
            auditEvent.UserName = null;
            auditEvent.UserEmail = null;
            auditEvent.UserIdentity = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().NotThrow();

            auditEvent.UserId = string.Empty;
            auditEvent.UserName = string.Empty;
            auditEvent.UserEmail = string.Empty;
            auditEvent.UserIdentity = string.Empty;
            act.Should().NotThrow();

            auditEvent.UserId = "    ";
            auditEvent.UserName = "    ";
            auditEvent.UserEmail = "    ";
            auditEvent.UserIdentity = "    ";

            act.Should().NotThrow();
        }

        [TestMethod]
        public void AuditEventIsNotValidWithAllUserIdEmptyButOtherUserFieldsPopulated()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.UserId = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserId)));

            auditEvent.UserId = string.Empty;
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserId)));

            auditEvent.UserId = "    ";
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserId)));
        }

        [TestMethod]
        public void AuditEventIsNotValidWithAllUserNameEmptyButOtherUserFieldsPopulated()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.UserName = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserName)));

            auditEvent.UserName = string.Empty;
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserName)));

            auditEvent.UserName = "    ";
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserName)));
        }

        [TestMethod]
        public void AuditEventIsNotValidWithAllUserEmailEmptyButOtherUserFieldsPopulated()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.UserEmail = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserEmail)));

            auditEvent.UserEmail = string.Empty;
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserEmail)));

            auditEvent.UserEmail = "    ";
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserEmail)));
        }

        [TestMethod]
        public void AuditEventIsNotValidWithAllUserIdentityEmptyButOtherUserFieldsPopulated()
        {
            var auditEvent = GetFullyPopulatedAuditEvent();
            auditEvent.UserIdentity = null;

            Action act = () => auditEvent.EnsureValid();
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserIdentity)));

            auditEvent.UserIdentity = string.Empty;
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserIdentity)));

            auditEvent.UserIdentity = "    ";
            act.Should().Throw<ValidationException>()
                .Where(e => e.ValidationResult.MemberNames.Contains(nameof(AuditEvent.UserIdentity)));
        }

        private static AuditEvent GetFullyPopulatedAuditEvent()
        {
            var auditEvent = new AuditEvent()
            {
                Data = Guid.NewGuid(),
                DataType = typeof(Guid).Name,
                DataId = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                EventType = Guid.NewGuid().ToString(),
                OperationType = OperationType.View,
                SessionId = Guid.NewGuid().ToString(),
                TargetId = Guid.NewGuid().ToString(),
                TargetType = typeof(Guid).Name,

                UserId = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString(),
                UserEmail = Guid.NewGuid().ToString(),
                UserIdentity = Guid.NewGuid().ToString(),
            };

            return auditEvent;
        }
    }
}
