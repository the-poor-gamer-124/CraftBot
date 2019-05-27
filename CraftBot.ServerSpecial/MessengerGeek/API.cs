using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CraftBot.ServerSpecial.MessengerGeek
{
    public static class API
    {
        public static TopicsResponse LastResponse;
        internal static WebClient WebClient { get; } = new WebClient();

        public static async Task<List<Topic>> GetLatestTopics()
        {
            string json = await WebClient.DownloadStringTaskAsync("http://wink.messengergeek.com/latest.json?order=created");
            LastResponse = new TopicsResponse((JObject)JsonConvert.DeserializeObject(json));
            return LastResponse.Topics.OrderBy(t => t.CreatedAt).ToList();
        }
    }
}
