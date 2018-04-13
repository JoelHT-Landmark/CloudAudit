﻿using CloudAudit.Client.Encryption;
using CloudAudit.Client.Model;
using CloudAudit.Client.Tests.Mocks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudAudit.Client.Tests
{
    [TestClass]
    public partial class AuditEventTests
    {
        private static string GetRandomFilename()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(8) + ".txt";
        }

        [TestMethod]
        public void AuditEventConstructorBuildsAuditEventFromAuditRequestAsExpected()
        {
            var mockCase = new MockCase();
            var mockAttachment = new MockAttachment();
            var request = AuditRequest.AsChangeTo(mockCase, c => c.SysRef)
                .WithData(mockAttachment, a => a.Id.ToString(CultureInfo.InvariantCulture))
                .AsEvent(nameof(AuditEventConstructorBuildsAuditEventFromAuditRequestAsExpected))
                .WithDescription("Cool Stuff");

            var auditEvent = new AuditEvent(request);
            auditEvent.OperationType.Should().Be(OperationType.Change);
            auditEvent.EventType.Should().Be(nameof(AuditEventConstructorBuildsAuditEventFromAuditRequestAsExpected));
            auditEvent.TargetType.Should().Be(typeof(MockCase).Name);
            auditEvent.TargetId.Should().Be(mockCase.SysRef);
            auditEvent.DataType.Should().Be(typeof(MockAttachment).FullName);
            auditEvent.DataId.Should().Be(mockAttachment.Id.ToString(CultureInfo.InvariantCulture));
            auditEvent.Description.Should().Be("Cool Stuff");
            auditEvent.SessionId.Should().Be($"{typeof(MockCase).Name}: {mockCase.SysRef}");
            auditEvent.Timestamp.Should().BeCloseTo(DateTime.UtcNow, 100);

            dynamic expectedData = mockAttachment;
            string serializedData = JsonConvert.SerializeObject(auditEvent.Data);
            string serializedExpectedData = JsonConvert.SerializeObject(expectedData);

            serializedData.Should().Be(serializedExpectedData);
        }

        [TestMethod]
        public void AuditEventWithDataEncryptsAndDecryptsSuccessfully()
        {
            var mockCase = new MockCase();
            var mockAttachment = new MockAttachment()
            {
                Filename = GetRandomFilename()
            };

            var request = AuditRequest.AsChangeTo(mockCase, c => c.SysRef)
                .WithData(mockAttachment, a => a.Id.ToString(CultureInfo.InvariantCulture))
                .AsEvent(nameof(AuditEventWithDataEncryptsAndDecryptsSuccessfully));
            var auditEvent = new AuditEvent(request);

            var key = "IlgY+s8d2q+QVXNx3ULHaUmNmcjgKQGXZFvxysqF5mA=";

            var crypto = new MessageEncryption();
            var encryptedMessage = crypto.EncryptMessageBody(auditEvent, key);

            var decryptedEvent = crypto.DecryptyMessageBody<AuditEvent>(encryptedMessage, key);
            decryptedEvent.TargetType.Should().Be(typeof(MockCase).Name);
            decryptedEvent.TargetId.Should().Be(mockCase.SysRef);

            decryptedEvent.DataType.Should().Be(typeof(MockAttachment).FullName);
            decryptedEvent.DataId.Should().Be(mockAttachment.Id.ToString(CultureInfo.InvariantCulture));

            Assert.IsFalse(object.ReferenceEquals(null, decryptedEvent.Data));

            string dataAsJson = JsonConvert.SerializeObject(decryptedEvent.Data);
            string attachmentAsJson = JsonConvert.SerializeObject(mockAttachment);
            dataAsJson.Should().Be(attachmentAsJson);
        }

        [TestMethod]
        public void AuditEventWithNoDataEncryptsAndDecryptsSuccessfully()
        {
            var mockCase = new MockCase();
            var mockAttachment = new MockAttachment()
            {
                Filename = GetRandomFilename()
            };

            var request = AuditRequest.AsChangeTo(mockCase, c => c.SysRef)
                .WithNoData()
                .AsEvent(nameof(AuditEventWithNoDataEncryptsAndDecryptsSuccessfully));
            var auditEvent = new AuditEvent(request);

            var key = "IlgY+s8d2q+QVXNx3ULHaUmNmcjgKQGXZFvxysqF5mA=";

            var crypto = new MessageEncryption();
            var encryptedMessage = crypto.EncryptMessageBody(auditEvent, key);

            var decryptedEvent = crypto.DecryptyMessageBody<AuditEvent>(encryptedMessage, key);
            decryptedEvent.TargetType.Should().Be(typeof(MockCase).Name);
            decryptedEvent.TargetId.Should().Be(mockCase.SysRef);

            decryptedEvent.DataType.Should().BeNull();
            decryptedEvent.DataId.Should().BeNull();

            Assert.IsTrue(object.ReferenceEquals(null, decryptedEvent.Data));
        }
    }
}
