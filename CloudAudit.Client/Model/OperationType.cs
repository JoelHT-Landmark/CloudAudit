namespace CloudAudit.Client.Model
{
    /// <summary>
    /// Enumeration for representing possible types of operations performed in the applications to be audited.
    /// </summary>
    public enum OperationType
    {
        Unspecified = 0,
        View = 1,
        Change = 2,
        Action = 3,
        Statement = 4
    }
}
