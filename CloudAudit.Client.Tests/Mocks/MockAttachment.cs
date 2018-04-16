namespace CloudAudit.Client.Tests.Mocks
{
    using System;

    public class MockAttachment
    {
        public long Id { get; internal set; } = GetRandomAttachmentId();
        public string Filename { get; set; }

        public static long GetRandomAttachmentId()
        {
            return new Random(DateTime.Now.Millisecond).Next(999999);
        }
    }
}
