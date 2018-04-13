using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudAudit.Client
{
    public static class AuditContext
    {
        public static string ApplicationName { get; internal set; }
        public static string CorrelationKey { get; internal set; }
        public static ConcurrentDictionary<string, string> PersistentData = new ConcurrentDictionary<string, string>();

        internal static string GetPersistentDataOrDefault(string key)
        {
            string result = null;
            if (PersistentData.ContainsKey(key) && PersistentData.TryGetValue(key, out result))
            {
                return result;
            }

            return null;
        }

        internal static void SetCorrelationKey(string correlationKey)
        {
            CorrelationKey = correlationKey;
        }

        internal static void AddOrUpdatePersistentData(string key, string value)
        {
            PersistentData.AddOrUpdate(key, value, (oldValue, newValue) => newValue);
        }
    }
}
