namespace CloudAudit.Client.Encryption
{
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// Interface defining a message encryption helper
    /// </summary>
    public interface IMessageEncryption
    {
        /// <summary>
        /// Encrypts the message body.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        BrokeredMessage EncryptMessageBody<TMessage>(TMessage message, string key) where TMessage : class;

        /// <summary>
        /// Decrypts the message body.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        TMessage DecryptMessageBody<TMessage>(BrokeredMessage message, string key) where TMessage : class;
    }
}