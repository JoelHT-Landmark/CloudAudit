namespace CloudAudit.Client.Tests
{
    using System;
    using System.Security.Claims;
    using CloudAudit.Client.Behaviours;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConfigureAuditingTests
    {
        [TestMethod]
        public void DefaultBehavioursArePreconfigured()
        {
            var configuration = new AuditConfiguration();
            configuration.Behaviours.Should().Contain(AuditBehaviours.SetMachineNameToEnvironmentMachineName);
            configuration.Behaviours.Should().Contain(AuditBehaviours.SetApplicationNameToLoggingApplicationName);
            configuration.Behaviours.Should().Contain(AuditBehaviours.SetCorrelationKeyToFlowedCorrelationKeyOrDefault);
        }

        [TestMethod]
        public void ForCurrentSignedInUserAddsCorrectBehavior()
        {
            var configuration = new AuditConfiguration();
            configuration.Behaviours.Should().NotContain(AuditBehaviours.SetUserDataToCurrentlySignedInUser);

            configuration.ForCurrentSignedInUser();

            configuration.Behaviours.Should().Contain(AuditBehaviours.SetUserDataToCurrentlySignedInUser);
        }

        [TestMethod]
        public void ForFlowedUserDataAddsCorrectBehavior()
        {
            var configuration = new AuditConfiguration();
            configuration.Behaviours.Should().NotContain(AuditBehaviours.SetUserDataToFlowedUser);

            configuration.ForFlowedUserData();

            configuration.Behaviours.Should().Contain(AuditBehaviours.SetUserDataToFlowedUser);
        }

        [TestMethod]
        public void UsingClaimsPrincipalFactoryAddsBehavior()
        {
            Func<ClaimsPrincipal> principalFactory = () => new ClaimsPrincipal();

            var configuration = new AuditConfiguration();
            var defaultCount = configuration.Behaviours.Count;

            configuration.UsingClaimsPrincipalFactory(principalFactory);

            // How the heck to test for a custom generated action being added to the list?
            configuration.Behaviours.Count.Should().Be(defaultCount + 1);
        }
    }
}
