using Newtonsoft.Json;

using System;
using System.Text.RegularExpressions;

namespace CraftBot.ServerSpecial.MessengerGeek
{
    public class Post
    {
        public int Id;
        public string Title;

        [JsonIgnore]
        public string Uncooked
        {
            get
            {
                string uncooked = Cooked;
                uncooked = Regex.Replace(uncooked, "<.*?>", string.Empty);
                uncooked = uncooked.Replace("â€™", "'");
                return uncooked;
            }
        }
        public string Cooked;

        public DateTime Created_At;
    }
}
