namespace CloudAudit.Client.Tests.Mocks
{
    using System;

    public class MockCase
    {
        public string SysRef { get; set; } = GetRandomSysRef();

        public static string GetRandomSysRef()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(8);
        }
    }
}
