using CraftBot.Base;

namespace CraftBot.ServerSpecial
{
    public class Plugin : Base.Plugins.Plugin
    {
        public Plugin()
        {
            this.PluginRegistered += (s, e) =>
            {
                Program.Client.MessageCreated += EmojiLeaderBoard.MessageCreated;
                Program.Client.Ready += MessengerGeekFeed.DiscordClient_Ready;
            };
        }

        public override string Author => "Craftplacer";
        public override string Name => "Server Specials";
    }
}