using Newtonsoft.Json.Linq;

namespace CraftBot.ServerSpecial.MessengerGeek
{
    public class TopicsResponse
    {
        public TopicsResponse(JObject jObject)
        {
            this.Topics = jObject["topic_list"]["topics"].ToObject<Topic[]>();
            this.Users = jObject["users"].ToObject<User[]>();
        }

        public Topic[] Topics;

        public User[] Users;
    }
}
