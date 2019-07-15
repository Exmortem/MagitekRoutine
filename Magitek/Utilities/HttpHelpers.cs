using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Magitek.Utilities
{
    internal static class HttpHelpers
    {
        private static readonly HttpClient Client = new HttpClient();

        public static async Task<HttpResult> Post(string url, object payload)
        {
            var result = new HttpResult();

            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await Client.PostAsync(url, requestContent))
                {
                    result.Code = response.StatusCode;
                    if (response.Content == null) { return result; }
                    result.Content = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Code = HttpStatusCode.Ambiguous;
                result.Content = e.Message;
                return result;
            }
        }

        public static async Task<HttpResult> Put(string url, object payload)
        {
            var result = new HttpResult();

            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await Client.PutAsync(url, requestContent))
                {
                    result.Code = response.StatusCode;
                    if (response.Content == null) { return result; }
                    result.Content = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Code = HttpStatusCode.Ambiguous;
                result.Content = e.Message;
                return result;
            }
        }

        public static async Task<HttpResult> PostWithCancel(string url, object payload)
        {
            var result = new HttpResult();

            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await Client.PostAsync(url, requestContent))
                {
                    result.Code = response.StatusCode;
                    if (response.Content == null) { return result; }
                    result.Content = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Code = HttpStatusCode.Ambiguous;
                result.Content = e.Message;
                return result;
            }
        }

        public struct HttpResult
        {
            public HttpStatusCode Code;
            public string Content;
        }
    }
}
