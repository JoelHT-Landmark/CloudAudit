namespace CloudAudit.Client.Tests.Mocks
{
    using System;

    public class MockEmailNotification
    {
        public long Id { get; internal set; } = GetRandomEmailNotificationId();

        public static long GetRandomEmailNotificationId()
        {
            return new Random(DateTime.Now.Millisecond).Next(999999);
        }
    }
}
