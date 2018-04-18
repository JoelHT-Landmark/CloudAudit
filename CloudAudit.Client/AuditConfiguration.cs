namespace CloudAudit.Client
{
    using System;
    using System.Collections.Generic;

    using CloudAudit.Client.Behaviours;
    using CloudAudit.Client.Model;

    public sealed class AuditConfiguration : IAuditConfiguration
    {
        private readonly List<Action<AuditEvent>> behaviours = new List<Action<AuditEvent>>()
        {
            AuditBehaviours.SetApplicationNameToLoggingApplicationName,
            AuditBehaviours.SetCorrelationKeyToFlowedCorrelationKeyOrDefault,
            AuditBehaviours.SetMachineNameToEnvironmentMachineName
        };

        internal AuditConfiguration()
        {
        }

        public static AuditConfiguration Current { get; internal set; } = new AuditConfiguration();

        internal List<Action<AuditEvent>> Behaviours => this.behaviours;

        public void ApplyTo(AuditEvent auditEvent)
        {
            behaviours.ForEach(behaviour => behaviour(auditEvent));
        }

        public IAuditConfiguration AddBehaviour(Action<AuditEvent> behaviour)
        {
            behaviours.Add(behaviour);
            return this;
        }

        internal static void Reset()
        {
            Current = new AuditConfiguration();
        }
    }
}
