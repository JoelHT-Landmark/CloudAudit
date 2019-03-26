namespace CloudAudit.Client
{
    using System;
    using System.Collections.Generic;

    using CloudAudit.Client.Behaviours;
    using CloudAudit.Client.Model;

    /// <summary>
    /// Implemenation of the <see cref="IAuditConfiguration"/> interface
    /// </summary>
    /// <remarks>
    /// Also provides a static instance via <see cref="AuditConfiguration.Current"/> which is what
    /// you would normally use
    /// </remarks>
    /// <seealso cref="CloudAudit.Client.IAuditConfiguration" />
    public sealed class AuditConfiguration : IAuditConfiguration
    {
        private readonly List<Action<AuditEvent>> behaviours = new List<Action<AuditEvent>>()
        {
            AuditBehaviours.SetApplicationNameToLoggingApplicationName,
            AuditBehaviours.SetCorrelationKeyToFlowedCorrelationKeyOrDefault,
            AuditBehaviours.SetMachineNameToEnvironmentMachineName
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditConfiguration"/> class.
        /// </summary>
        internal AuditConfiguration()
        {
        }

        /// <summary>
        /// Gets the current static <see cref="AuditConfiguration"/> instance
        /// </summary>
        /// <value>
        /// The current static <see cref="AuditConfiguration"/> instance
        /// </value>
        public static AuditConfiguration Current { get; internal set; } = new AuditConfiguration();

        /// <summary>
        /// Gets the behaviours.
        /// </summary>
        /// <value>
        /// The behaviours.
        /// </value>
        internal List<Action<AuditEvent>> Behaviours => this.behaviours;

        /// <summary>
        /// Applies the configured behaviours to the specified <see cref="AuditEvent"/>
        /// </summary>
        /// <param name="auditEvent">The audit event.</param>
        public void ApplyTo(AuditEvent auditEvent)
        {
            this.behaviours.ForEach(behaviour => behaviour(auditEvent));
        }

        /// <summary>
        /// Adds the behaviour.
        /// </summary>
        /// <param name="behaviour">The behaviour.</param>
        /// <returns></returns>
        public IAuditConfiguration AddBehaviour(Action<AuditEvent> behaviour)
        {
            this.behaviours.Add(behaviour);
            return this;
        }

        /// <summary>
        /// Resets the static <see cref="AuditConfiguration"/> instance
        /// </summary>
        /// <remarks>
        /// Used for testing only
        /// </remarks>
        internal static void Reset()
        {
            Current = new AuditConfiguration();
        }
    }
}
