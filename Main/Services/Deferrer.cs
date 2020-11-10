using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kraken.Api.Main.Services
{
    /// <summary>
    /// Contains the addresses of other APIs.
    /// </summary>
    public class TargetApi
    {
        /// <value>The address of the sensor API.</value>
        public const string Sensor = "http://kraken_sensor_api:5002/api";
    }

    /// <summary>
    /// Used for deferring HTTP requests to other APIs.
    /// </summary>
    public class Deferrer
    {
        /// <summary>
        /// Defers a GET request to '{api}{target}'.
        /// </summary>
        /// <param name="api">The address of the api to which the request should be deferred.</param>
        /// <param name="target">The target endpoint.</param>
        /// <typeparam name="T">The data model returned by the other api.</typeparam>
        /// <exception cref="HttpRequestException">Thrown if the returned status code did not equal 200.</exception>
        /// <returns>The result of the request.</returns>
        public async Task<T> DeferGetRequest<T>(string api, string target)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback
                    = (sender, certificate, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(handler))
                {
                    HttpResponseMessage response = await client.GetAsync($"{api}{target}");

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string body = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<T>(body);
                    }

                    throw new HttpRequestException($"Received response with status code {response.StatusCode}");
                }
            }
        }
    }
}