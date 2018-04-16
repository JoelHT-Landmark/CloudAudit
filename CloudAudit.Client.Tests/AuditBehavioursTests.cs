namespace CloudAudit.Client.Tests
{
    using System;
    using System.Security.Claims;

    using CloudAudit.Client.Behaviours;
    using CloudAudit.Client.Model;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AuditBehavioursTests
    {
        private static readonly object Padlock = new object();

        [TestMethod]
        public void SetMachineNameToEnvironmentMachineNameSetsMachineName()
        {
            var auditEvent = new AuditEvent();
            auditEvent.MachineName.Should().BeNull();

            AuditBehaviours.SetMachineNameToEnvironmentMachineName(auditEvent);

            auditEvent.MachineName.Should().Be(Environment.MachineName);
        }

        [TestMethod]
        public void SetApplicationNameToLoggingApplicationNameSetsApplicationName()
        {
            var auditEvent = new AuditEvent();
            auditEvent.ApplicationName.Should().BeNull();

            AuditBehaviours.SetApplicationNameToLoggingApplicationName(auditEvent);

            auditEvent.ApplicationName.Should().Be(AuditContext.ApplicationName);
        }

        [TestMethod]
        public void SetCorrelationKeyToFlowedCorrelationKeyOrDefaultSetsCorrelationKeyWhenAvailable()
        {
            lock (Padlock)
            {
                var expected = Guid.NewGuid().ToString();
                AuditContext.SetCorrelationKey(expected);

                var auditEvent = new AuditEvent();
                auditEvent.CorrelationKey.Should().BeNull();

                AuditBehaviours.SetCorrelationKeyToFlowedCorrelationKeyOrDefault(auditEvent);

                auditEvent.CorrelationKey.Should().Be(expected);
            }
        }

        [TestMethod]
        public void SetCorrelationKeyToFlowedCorrelationKeyOrDefaultSetsCorrelationKeyWhenNoneAvailable()
        {
            lock (Padlock)
            {
                AuditContext.SetCorrelationKey(null);

                var auditEvent = new AuditEvent();
                auditEvent.CorrelationKey.Should().BeNull();

                AuditBehaviours.SetCorrelationKeyToFlowedCorrelationKeyOrDefault(auditEvent);

                auditEvent.CorrelationKey.Should().NotBeNullOrWhiteSpace();
            }
        }

        [TestMethod]
        public void SetUserDataUsingToFlowedUserSetsUserData()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = $"JohnSmith_{userId}";
            var userEmail = $"{userName}@email.com";
            var userIdentity = $"Fake|{userName}";

            var auditEvent = new AuditEvent();
            auditEvent.UserId.Should().BeNull();
            auditEvent.UserName.Should().BeNull();
            auditEvent.UserEmail.Should().BeNull();
            auditEvent.UserIdentity.Should().BeNull();

            lock (Padlock)
            {
                AuditContext.AddOrUpdatePersistentData("UserId", userId);
                AuditContext.AddOrUpdatePersistentData("UserName", userName);
                AuditContext.AddOrUpdatePersistentData("UserEmail", userEmail);
                AuditContext.AddOrUpdatePersistentData("UserIdentity", userIdentity);

                AuditBehaviours.SetUserDataToFlowedUser(auditEvent);

                auditEvent.UserId.Should().Be(userId);
                auditEvent.UserName.Should().Be(userName);
                auditEvent.UserEmail.Should().Be(userEmail);
            }
        }

        [TestMethod]
        public void SetUserDataUsingClaimsPrincipalFactorySetsUserDataFromFactory()
        {
            var userName = $"John Smith";
            var userEmail = $"john.smith@email.com";
            var userIdentity = Guid.NewGuid().ToString();
            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, userName),
                        new Claim(ClaimTypes.Email, userEmail),
                        new Claim(ClaimTypes.NameIdentifier, userIdentity)
                    },
                    "Mocked"));

            lock (Padlock)
            {
                var auditEvent = new AuditEvent();
                auditEvent.UserId.Should().BeNull();
                auditEvent.UserName.Should().BeNull();
                auditEvent.UserEmail.Should().BeNull();
                auditEvent.UserIdentity.Should().BeNull();

                AuditBehaviours.SetUserDataUsingClaimsPrincipalFactory(auditEvent, () => principal);

                auditEvent.UserId.Should().Be(userEmail);
                auditEvent.UserName.Should().Be(userName);
                auditEvent.UserEmail.Should().Be(userEmail);
                auditEvent.UserIdentity.Should().Be(userIdentity);
            }
        }
    }
}
