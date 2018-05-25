namespace CloudAudit.Client
{
    using System.Collections.Concurrent;

    public static class AuditContext
    {
        private static ConcurrentDictionary<string, string> persistentData = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Gets the ApplicationName
        /// </summary>
        public static string ApplicationName { get; internal set; }

        /// <summary>
        /// Gets the current CorrelationKey
        /// </summary>
        public static string CorrelationKey { get; internal set; }

        /// <summary>
        /// Gets the named persistent data item or null (default) if no data has been persisted
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetPersistentDataOrDefault(string key)
        {
            string result = null;
            if (persistentData.ContainsKey(key) && persistentData.TryGetValue(key, out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Sets the current CorrelationKey
        /// </summary>
        /// <param name="correlationKey"></param>
        public static void SetCorrelationKey(string correlationKey)
        {
            CorrelationKey = correlationKey;
        }

        /// <summary>
        /// Adds or updates a persistent data item
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdatePersistentData(string key, string value)
        {
            persistentData.AddOrUpdate(key, value, (oldValue, newValue) => newValue);
        }
    }
}
