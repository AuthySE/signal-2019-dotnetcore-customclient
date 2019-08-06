
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace signal_2019_dotnetcore.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<JObject> GetAsJObjectAsync(this HttpClient client, string uri)
        {
            Func<Task<HttpResponseMessage>> caller = () => client.GetAsync(uri);

            return await ProcessAsync(caller);
        }

        public static async Task<JObject> PostFormAsJObjectAsync(this HttpClient client, string uri, Dictionary<string,string> body)
        {
            var content = new FormUrlEncodedContent(body);

            Func<Task<HttpResponseMessage>> caller = () => client.PostAsync(uri, content);

            return await ProcessAsync(caller);
        }

        public static async Task<JObject> PostAsJObjectAsync(this HttpClient client, string uri, Dictionary<string,string> body)
        {
            string request = string.Empty;

            if (body != null)
                request = JsonConvert.SerializeObject(body);

            var content = new StringContent(request, Encoding.UTF8, "application/json");

            Func<Task<HttpResponseMessage>> caller = () => client.PostAsync(uri, content);

            return await ProcessAsync(caller);
        }

        private static async Task<JObject> ProcessAsync(Func<Task<HttpResponseMessage>> httpCall)
        {
            var response = await httpCall();

            try
            {
                if(response.Content == null){
                    throw new Exception("Woah no content bro.");
                }

                var json = await response.Content.ReadAsAsync<JObject>();

                return json;
            }
            catch
            {
                throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase}");
            }
        }
    }
}