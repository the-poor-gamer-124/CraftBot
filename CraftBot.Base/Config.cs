using System.Collections.Generic;

namespace CraftBot.Base
{
    public class Config
    {
        public List<string> Triggers { get; set; } = new List<string>()
        {
            "cb!",
            "craftbot!"
        };

        public Dictionary<string, string> Tokens { get; set; } = new Dictionary<string, string>();

        public DebugFlags DebugFlags { get; set; } = new DebugFlags();
    }
}