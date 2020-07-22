using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GoldTracker
{
    internal class Api
    {
        public static HttpClient ClientConfig(string key)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.guildwars2.com/v2/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            return client;
        }
    }
}