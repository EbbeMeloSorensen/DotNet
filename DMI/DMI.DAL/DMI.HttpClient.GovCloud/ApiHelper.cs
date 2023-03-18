using System.Net.Http.Headers;

namespace DMI.HttpClient.GovCloud
{
    public static class ApiHelper
    {
        public static System.Net.Http.HttpClient ApiClient { get; set; }

        static ApiHelper()
        {
            ApiClient = new System.Net.Http.HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
