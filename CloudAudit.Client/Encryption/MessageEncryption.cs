namespace CloudAudit.Client.Encryption
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    using CloudAudit.Client.ServiceBus;

    using Microsoft.ServiceBus.Messaging;

    using Newtonsoft.Json;

    /// <summary>
    /// Implementation of a message encryptor / decryptor that uses the
    /// <see cref="RijndaelManaged"/> encryption scheme
    /// </summary>
    /// <seealso cref="CloudAudit.Client.Encryption.IMessageEncryption" />
    public class MessageEncryption : IMessageEncryption
    {
        private static readonly int KeySizeBits = 256;
        private static readonly int KeySizeBytes = 32;

        /// <summary>
        /// Encrypts the message body.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="MessageEncryptionException">Could not encrypt message</exception>
        public BrokeredMessage EncryptMessageBody<TMessage>(TMessage message, string key) where TMessage : class
        {
            BrokeredMessage encryptedMessage;
            var byteKey = GetKeyFromString(key);

            try
            {
                var jsonMessage = JsonConvert.SerializeObject(message);

                using (var rijndael = new RijndaelManaged())
                {
                    SetupRijndael(rijndael);
                    rijndael.GenerateIV();
                    var initVector = rijndael.IV;
                    var encryptor = rijndael.CreateEncryptor(byteKey, initVector);

                    using (var memoryStream = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    using (var streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(jsonMessage);
                        streamWriter.Flush();
                        cryptoStream.FlushFinalBlock();
                        encryptedMessage = new BrokeredMessage(Convert.ToBase64String(memoryStream.ToArray()));
                        encryptedMessage.Properties[MessageProperties.KeySalt] = Convert.ToBase64String(initVector);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MessageEncryptionException("Could not encrypt message", ex);
            }

            return encryptedMessage;
        }

        /// <summary>
        /// Decrypts the message body.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="MessageEncryptionException">Could not decrypt message</exception>
        public TMessage DecryptMessageBody<TMessage>(BrokeredMessage message, string key) where TMessage : class
        {
            TMessage decryptedMessage;
            var byteKey = GetKeyFromString(key);

            try
            {
                var bodyString = message.GetBody<string>();
                var messagebody = Convert.FromBase64CharArray(bodyString.ToCharArray(), 0, bodyString.Length);
                var initVectorProperty = message.Properties[MessageProperties.KeySalt].ToString().ToCharArray();
                var initVector = Convert.FromBase64CharArray(initVectorProperty, 0, initVectorProperty.Length);

                using (var rijndael = new RijndaelManaged())
                {
                    SetupRijndael(rijndael);
                    var decryptor = rijndael.CreateDecryptor(byteKey, initVector);

                    using (var memoryStream = new MemoryStream(messagebody))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        var jsonMessage = streamReader.ReadToEnd();
                        decryptedMessage = JsonConvert.DeserializeObject<TMessage>(jsonMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MessageEncryptionException("Could not decrypt message", ex);
            }

            return decryptedMessage;
        }

        /// <summary>
        /// Gets the key from string.
        /// </summary>
        /// <param name="keyString">The key string.</param>
        /// <returns></returns>
        /// <exception cref="MessageEncryptionException">
        /// Invalid key
        /// </exception>
        private byte[] GetKeyFromString(string keyString)
        {
            byte[] key;
            try
            {
                var charKey = keyString.ToCharArray();
                key = Convert.FromBase64CharArray(charKey, 0, charKey.Length);
                if (key.Length != KeySizeBytes)
                {
                    throw new MessageEncryptionException(
                        $"Wrong key size, key must be {KeySizeBits} bits. Key provided was {key.Length * 8} bits.");
                }
            }
            catch (Exception ex)
            {
                throw new MessageEncryptionException("Invalid key", ex);
            }
            return key;
        }

        /// <summary>
        /// Setups the rijndael.
        /// </summary>
        /// <param name="rijndael">The rijndael.</param>
        private void SetupRijndael(RijndaelManaged rijndael)
        {
            rijndael.KeySize = KeySizeBits;
            rijndael.BlockSize = 128;
            rijndael.Padding = PaddingMode.Zeros;
        }
    }
}