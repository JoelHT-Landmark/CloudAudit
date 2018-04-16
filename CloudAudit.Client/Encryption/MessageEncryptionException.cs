namespace CloudAudit.Client.Encryption
{
    using System;

    public class MessageEncryptionException : Exception
    {
        public MessageEncryptionException()
        {
        }

        public MessageEncryptionException(string message)
            : base(message)
        {
        }

        public MessageEncryptionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}