using Microsoft.ServiceBus.Messaging;

namespace CloudAudit.Client.Encryption
{
    public interface IMessageEncryption
    {
        BrokeredMessage EncryptMessageBody<TMessage>(TMessage message, string key) where TMessage : class;

        TMessage DecryptyMessageBody<TMessage>(BrokeredMessage message, string key) where TMessage : class;
    }
}