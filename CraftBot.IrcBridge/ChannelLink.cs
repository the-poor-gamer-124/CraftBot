using DSharpPlus.Entities;
using System;

namespace CraftBot.IrcBridge
{
    public class ChannelLink
    {
        #region Public Base.Program

        public string IrcChannel;
        public DiscordWebhook Webhook;

        public ChannelLink(DiscordWebhook webhook, string ircChannel)
        {
            Webhook = webhook;
            IrcChannel = ircChannel ?? throw new ArgumentNullException(nameof(ircChannel));
        }

        #endregion Public Base.Program
    }
}