using System;

namespace CloudAudit.Client.Tests.Mocks
{
    public class MockEmailNotification
    {
        public long Id { get; internal set; } = GetRandomEmailNotificationId();

        public static long GetRandomEmailNotificationId()
        {
            return new Random(DateTime.Now.Millisecond).Next(999999);
        }
    }
}
