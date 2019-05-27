using Newtonsoft.Json;

namespace CraftBot.ServerSpecial.MessengerGeek
{
    public class Poster
    {
        public string Description;

        [JsonProperty("user_id")]
        public int UserId;
    }
}
