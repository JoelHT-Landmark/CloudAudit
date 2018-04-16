namespace CloudAudit.Client.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using LiteGuard;

    using Newtonsoft.Json;

    public static class HelperExtensions
    {
        /// <summary>
        /// Posts an object to the specified <paramref name="requestUrl"/> as serialized JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUrl"></param>
        /// <param name="dataPayload"></param>
        /// <returns></returns>
        public static async Task PostAsJsonAsync<T>(this HttpClient client, string requestUrl, T dataPayload)
        {
            var json = JsonConvert.SerializeObject(dataPayload);
            await client.PostAsync(requestUrl, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        /// <summary>
        /// Posts an object to the specified <paramref name="requestUrl"/> as serialized JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="requestUrl"></param>
        /// <param name="dataPayload"></param>
        /// <returns></returns>
        public static void PostAsJson<T>(this HttpClient client, string requestUrl, T dataPayload)
        {
            PostAsJsonAsync(client, requestUrl, dataPayload).RunSynchronously();
        }

        /// <summary>
        /// Forces a datetime to be UTC - throws if passed a Local
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime AsUtcDateTime(this DateTime source)
        {
            if (source.Kind == DateTimeKind.Local)
            {
                throw new InvalidOperationException("Cannot force a Local datetime to UTC");
            }

            return new DateTime(source.Ticks, DateTimeKind.Utc);
        }

        /// <summary>
        /// Gets the default value of the specified <typeparamref name="T">type</typeparamref>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDefaultValue<T>()
        {
            Expression<Func<T>> e = Expression.Lambda<Func<T>>(
                Expression.Default(typeof(T)));

            return e.Compile()();
        }

        /// <summary>
        /// Gets the default value of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object GetDefaultValue(this Type type)
        {
            Contract.Requires(type != null);
            Guard.AgainstNullArgument(nameof(type), type);
            Contract.EndContractBlock();

            Expression<Func<object>> e = Expression.Lambda<Func<object>>(
                Expression.Convert(Expression.Default(type), typeof(object)));

            return e.Compile()();
        }
    }
}