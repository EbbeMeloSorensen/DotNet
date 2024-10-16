using System.Net.Http;
using System.Net.Http.Headers;

namespace PR.Persistence.APIClient
{
    public static class ApiHelper
    {
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            //ApiClient.BaseAddress = new Uri("http://xkcd.com/"); // We don't do this, because we want to use the same client for two different addresses
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
