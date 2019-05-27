using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Net;
using System.Threading.Tasks;

namespace CraftBot.ServerSpecial.MessengerGeek
{
    public class Topic
    {
        public int Id;
        public string Title;

        [JsonProperty("posts_count")]
        public int PostsCount;

        [JsonProperty("reply_count")]
        public int ReplyCount;

        [JsonProperty("like_count")]
        public int LikeCount;

        public int Views;

        [JsonProperty("image_url")]
        public string ImageUrl;

        public bool Pinned;

        [JsonProperty("created_at")]
        public DateTime CreatedAt;

        public Poster[] Posters;

        public async Task<Post> GetPost(int index)
        {
            using (var webClient = new WebClient())
            {
                string json = await webClient.DownloadStringTaskAsync($"http://wink.messengergeek.com/t/{this.Id}.json");
                var jObject = (JObject)JsonConvert.DeserializeObject(json);
                return jObject["post_stream"]["posts"][index].ToObject<Post>();
            }
        }
    }
}