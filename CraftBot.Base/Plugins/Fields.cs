using DSharpPlus;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;

namespace CraftBot.Base.Plugins
{
    public class Fields
    {
        public Fields(InteractivityExtension interactivity, Dictionary<string, string> tokens, DiscordClient client, Random random)
        {
            this.Interactivity = interactivity;
            this.Tokens = tokens;
            this.DiscordClient = client;
            this.Random = random;
        }

        public DiscordClient DiscordClient { get; set; }
        public InteractivityExtension Interactivity { get; set; }
        public Random Random { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}