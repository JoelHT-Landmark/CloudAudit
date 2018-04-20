namespace CloudAudit.Client.Model
{
    using System.Collections.Generic;

    public class AuditList
    {
        public IEnumerable<AuditRecord> ItemsList { get; set; }

        public string ContinuationToken { get; set; }
    }
}

