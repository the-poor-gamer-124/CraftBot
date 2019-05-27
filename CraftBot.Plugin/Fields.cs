using DSharpPlus;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;

namespace CraftBot.Plugin
{
    public class Fields
    {
        public InteractivityExtension Interactivity { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public DiscordClient DiscordClient { get; set; }
        public Random Random { get; set; }
        public Fields(InteractivityExtension interactivity, Dictionary<string, string> tokens, DiscordClient client, Random random)
        {
            this.Interactivity = interactivity;
            this.Tokens = tokens;
            this.DiscordClient = client;
            this.Random = random;
        }
    }
}