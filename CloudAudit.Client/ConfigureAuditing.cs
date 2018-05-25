namespace CloudAudit.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Security.Claims;

    using CloudAudit.Client.Behaviours;

    using LiteGuard;

    public static class ConfigureAuditing
    {
        /// <summary>
        /// Configures the Auditing framework to use the <see cref="ClaimsPrincipal.Current">current</see>
        /// <see cref="ClaimsPrincipal"/> as the current Actor source each time auditing occurs
        /// </summary>
        /// <returns>The configured <see cref="IAuditConfiguration"/> instance.</returns>
        public static IAuditConfiguration ForCurrentSignedInUser()
        {
            return AuditConfiguration.Current.ForCurrentSignedInUser();
        }

        /// <summary>
        /// Configures the Auditing framework to use the <see cref="ClaimsPrincipal.Current">current</see>
        /// <see cref="ClaimsPrincipal"/> as the current Actor source each time auditing occurs
        /// </summary>
        /// <param name="config">The <see cref="IAuditConfiguration" /> instance to configure.</param>
        /// <returns>The configured <see cref="IAuditConfiguration"/> instance.</returns>
        public static IAuditConfiguration ForCurrentSignedInUser(this IAuditConfiguration config)
        {
            Contract.Requires(config != null);
            Guard.AgainstNullArgument(nameof(config), config);
            Contract.EndContractBlock();

            config.AddBehaviour(AuditBehaviours.SetUserDataToCurrentlySignedInUser);

            return config;
        }

        /// <summary>
        /// Configures the Auditing framework to use the specified <see cref="ClaimsPrincipal"/> factory
        /// as the current Actor source each time auditing occurs.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>The configured <see cref="IAuditConfiguration"/> instance.</returns>
        public static IAuditConfiguration UsingClaimsPrincipalFactory(Func<ClaimsPrincipal> factory)
        {
            return AuditConfiguration.Current.UsingClaimsPrincipalFactory(factory);
        }

        /// <summary>
        /// Configures the Auditing framework to use the specified <see cref="ClaimsPrincipal"/> factory
        /// as the current Actor source each time auditing occurs.
        /// </summary>
        /// <param name="config">The <see cref="IAuditConfiguration" /> instance to configure.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>The configured <see cref="IAuditConfiguration"/> instance.</returns>
        public static IAuditConfiguration UsingClaimsPrincipalFactory(this IAuditConfiguration config, Func<ClaimsPrincipal> factory)
        {
            Contract.Requires(config != null);
            Contract.Requires(factory != null);
            Guard.AgainstNullArgument(nameof(config), config);
            Guard.AgainstNullArgument(nameof(factory), factory);
            Contract.EndContractBlock();

            config.AddBehaviour(auditEvent => AuditBehaviours.SetUserDataUsingClaimsPrincipalFactory(auditEvent, factory));

            return config;
        }

        /// <summary>
        /// Configures the Auditing framework to use the <see cref="ClaimsPrincipal"/> flowed via the
        /// <see cref="LandmarkHttpClient"/> as x-landmark-xxx headers.
        /// </summary>
        /// <returns></returns>
        public static IAuditConfiguration ForFlowedUserData()
        {
            return AuditConfiguration.Current.ForFlowedUserData();
        }

        /// <summary>
        /// Configures the Auditing framework to use the <see cref="ClaimsPrincipal"/> flowed via the
        /// <see cref="LandmarkHttpClient"/> as x-landmark-xxx headers.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public static IAuditConfiguration ForFlowedUserData(this IAuditConfiguration config)
        {
            Contract.Requires(config != null);
            Guard.AgainstNullArgument(nameof(config), config);
            Contract.EndContractBlock();

            config.AddBehaviour(AuditBehaviours.SetUserDataToFlowedUser);

            return config;
        }
    }
}
