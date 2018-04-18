namespace CloudAudit.Client
{
    using System;

    using CloudAudit.Client.Model;

    public interface IAuditConfiguration
    {
        IAuditConfiguration AddBehaviour(Action<AuditEvent> behaviour);
    }
}
