namespace CloudAudit.Client.Behaviours
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;

    using CloudAudit.Client.Model;

    using LiteGuard;

    internal static class AuditBehaviours
    {
        internal static void SetMachineNameToEnvironmentMachineName(AuditEvent auditEvent)
        {
            auditEvent.MachineName = Environment.MachineName;
        }

        internal static void SetApplicationNameToLoggingApplicationName(AuditEvent auditEvent)
        {
            auditEvent.ApplicationName = AuditContext.ApplicationName;
        }

        internal static void SetCorrelationKeyToFlowedCorrelationKeyOrDefault(AuditEvent auditEvent)
        {
            auditEvent.CorrelationKey = AuditContext.CorrelationKey ?? Guid.NewGuid().ToString();
        }

        internal static void SetUserDataToCurrentlySignedInUser(AuditEvent auditEvent)
        {
            SetUserDataUsingClaimsPrincipalFactory(auditEvent, () => ClaimsPrincipal.Current ?? Thread.CurrentPrincipal as ClaimsPrincipal);
        }

        internal static void SetUserDataUsingClaimsPrincipalFactory(AuditEvent auditEvent, Func<ClaimsPrincipal> principalFactory)
        {
            Contract.Requires(auditEvent != null);
            Contract.Requires(principalFactory != null);
            Guard.AgainstNullArgument(nameof(auditEvent), auditEvent);
            Guard.AgainstNullArgument(nameof(principalFactory), principalFactory);
            Contract.EndContractBlock();

            var currentPrincipal = principalFactory();

            if (currentPrincipal?.Identity == null)
            {
                return;
            }

            if (currentPrincipal.Identity.IsAuthenticated)
            {
                var userId = currentPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (userId == null)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "User '{0}' is authenticated, but has no '{1}' claim.",
                        currentPrincipal.Identity.Name,
                        ClaimTypes.Name);
                    throw new NotSupportedException(message);
                }

                auditEvent.UserId = userId;
                auditEvent.UserName = currentPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                auditEvent.UserEmail = currentPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                auditEvent.UserIdentity = currentPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            }
        }

        internal static void SetUserDataToFlowedUser(AuditEvent auditEvent)
        {
            Contract.Requires(auditEvent != null);
            Guard.AgainstNullArgument(nameof(auditEvent), auditEvent);
            Contract.EndContractBlock();

            var userId = AuditContext.GetPersistentDataOrDefault("UserId") as string;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InvalidOperationException("No 'UserId' data item found in flowed data.");
            }

            auditEvent.UserId = userId;
            auditEvent.UserName = AuditContext.GetPersistentDataOrDefault("UserName") as string;
            auditEvent.UserEmail = AuditContext.GetPersistentDataOrDefault("UserEmail") as string;
            auditEvent.UserIdentity = AuditContext.GetPersistentDataOrDefault("UserIdentity") as string;
        }
    }
}
