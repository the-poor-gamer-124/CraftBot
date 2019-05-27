using CraftBot.Base;

namespace CraftBot.IrcBridge
{
    public class Plugin : Base.Plugins.Plugin
    {
        public Plugin()
        {
            this.PluginRegistered += (s, ea) =>
            {
                Program.Client.MessageCreated += IrcGateway.DiscordClient_MessageCreated;
                Program.Client.MessageUpdated += IrcGateway.DiscordClient_MessageUpdated;
                Program.Client.Ready += IrcGateway.DiscordClient_Ready;
            };
        }

        public override string Author => "Craftplacer";
        public override string Name => "IRC Bridge";
    }
}