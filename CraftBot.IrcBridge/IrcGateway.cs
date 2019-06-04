using CraftBot.Base.Plugins;
using CraftBot.Helper;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;

using Meebey.SmartIrc4net;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CraftBot.IrcBridge
{
    public static class IrcGateway
    {
        public const string ErrorAvatar = "https://i.imgur.com/F7GUi1C.png";
        public const string DefaultAvatar = "https://i.imgur.com/ZnsEicL.png";
        public const string IrcGatewayName = "IRC Gateway";

        private const string IrcNickname = "CraftBot";

        public static readonly Dictionary<string[], Tuple<string, int>> HostOverrides = new Dictionary<string[], Tuple<string, int>>()
        {
            { new[] { "irc.freenode.net", "chat.freenode.net", "freenode.net", "webchat.freenode.net" }, Tuple.Create("94.125.182.252", 6697) },
        };

        public static List<IrcEntry> IrcEntries { get; } = new List<IrcEntry>();

        public static async Task CheckChannelForIRCAsync(ulong channelId)
        {
            Dictionary<string, object> db = Database.GetDatabase(DatabaseType.Channel, channelId);
            if ((bool)Database.GetValue(db, "irc.enabled", false))
            {
                string host = (string)Database.GetValue(db, "irc.host", null);
                int port = int.Parse(Database.GetValue(db, "irc.port", null).ToString());

                Tuple<string, int> hostOverride = HostOverrides.FirstOrDefault(ho => ho.Key.Contains(host.ToLower())).Value;

                if (hostOverride != null)
                {
                    LogManager.GetCurrentClassLogger().Info($"Overriding {host}:{port} to {hostOverride.Item1}:{hostOverride.Item2}");
                    host = hostOverride.Item1;
                    port = hostOverride.Item2;
                }

                if (!IPAddress.TryParse(host, out _))
                {
                    IEnumerable<IPAddress> addresses = Dns.GetHostEntry(host).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                    IPAddress address = addresses.Last();
                    host = address.ToString();
                }

                string channel = (string)Database.GetValue(db, "irc.channel", null);
                ulong webhookId = ulong.Parse(Database.GetValue(db, "irc.webhook", null).ToString());

                try
                {
                    DiscordWebhook webhook = await Base.Program.Client.GetWebhookAsync(webhookId);

                    if (webhook.ChannelId != channelId)
                    {
                        return;
                    }

                    IrcEntry entry = IrcEntries.FirstOrDefault(a => a.ServerHost == host && a.ServerPort == port);

                    if (entry == null)
                    {
                        var tcs = new TaskCompletionSource<bool>();

                        entry = new IrcEntry(host, port, new List<ChannelLink> { new ChannelLink(webhook, channel) });
                        entry.IrcClient.Encoding = Encoding.UTF8;
                        entry.IrcClient.EnableUTF8Recode = true;
                        entry.IrcClient.ActiveChannelSyncing = true;
                        entry.IrcClient.OnConnectionError += (s, e) => tcs.SetResult(false);
                        entry.IrcClient.OnConnected += (s, e) => tcs.SetResult(true);
                        entry.IrcClient.OnConnecting += IrcClient_OnConnecting;
                        entry.IrcClient.OnJoin += IrcClient_OnJoinAsync;
                        entry.IrcClient.OnError += IrcClient_OnError;
                        entry.IrcClient.OnPart += IrcClient_OnPartAsync;
                        entry.IrcClient.OnQuit += IrcClient_OnQuitAsync;

                        IrcEntries.Add(entry);

                        new Thread(() =>
                        {
                            try
                            {
                                entry.IrcClient.Connect(new[] { entry.ServerHost }, entry.ServerPort);
                            }
                            catch
                            {
                                tcs.SetResult(false);
                            }
                        })
                        { Name = "Connection Thread" }.Start();

                        bool success = await tcs.Task;

                        if (!success)
                        {
                            var channelsAffected = new List<ulong>();
                            foreach (ChannelLink channelLink in entry.ChannelLinks)
                            {
                                ulong channelId2 = channelLink.Webhook.ChannelId;
                                if (!channelsAffected.Contains(channelId2))
                                {
                                    await channelLink.Webhook.ExecuteAsync("Connection to IRC server failed! Please make sure the server is accessible.", IrcGatewayName, ErrorAvatar, false, null, null);
                                    channelsAffected.Add(channelId2);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!entry.ChannelLinks.Any(cl => cl.Webhook.ChannelId == channelId && cl.IrcChannel == channel))
                        {
                            entry.ChannelLinks.Add(new ChannelLink(webhook, channel));
                        }

                        if (!entry.IrcClient.IsConnected)
                        {
                            entry.IrcClient.Connect(new[] { entry.ServerHost }, entry.ServerPort);
                        }
                    }
                }
                catch (NotFoundException)
                {
                    Database.SetValue(DatabaseType.Channel, channelId, "irc.enabled", false);
                    return;
                }
            }
        }

        private static async void IrcClient_OnQuitAsync(object sender, QuitEventArgs e)
        {
            if (e.Who == IrcNickname)
            {
                return;
            }

            if (sender is IrcClient ircClient)
            {
                IrcEntry entry = IrcEntries.First(a => a.IrcClient == ircClient);

                foreach (ChannelLink channelLink in entry.ChannelLinks.FindAll(c => c.IrcChannel == e.Data.Channel))
                {
                    await channelLink.Webhook.ExecuteAsync(
                        null,
                        IrcGatewayName,
                        DefaultAvatar,
                        false,
                        new List<DiscordEmbed>() { new MaterialEmbedBuilder(Base.Program.Client).WithTitle(e.Who, "leave").Build() },
                        null);
                }
            }
        }

        private static async void IrcClient_OnPartAsync(object sender, PartEventArgs e)
        {
            if (e.Who == IrcNickname)
            {
                return;
            }

            if (sender is IrcClient ircClient)
            {
                IrcEntry entry = IrcEntries.First(a => a.IrcClient == ircClient);

                foreach (ChannelLink channelLink in entry.ChannelLinks.FindAll(c => c.IrcChannel == e.Data.Channel))
                {
                    await channelLink.Webhook.ExecuteAsync(
                        null,
                        IrcGatewayName,
                        DefaultAvatar,
                        false,
                        new List<DiscordEmbed>() { new MaterialEmbedBuilder(Base.Program.Client).WithTitle(e.Who, "leave").Build() },
                        null);
                }
            }
        }

        public static async Task CheckChannelsAsync()
        {
            foreach (ulong channelId in Database.GetChannels())
            {
                await CheckChannelForIRCAsync(channelId);
            }
        }

        #region Discord Events

        public static async Task DiscordClient_MessageCreated(MessageCreateEventArgs e) => await SendMessage(e.Message);

        public static async Task DiscordClient_MessageUpdated(MessageUpdateEventArgs e) => await SendMessage(e.Message);

        public static async Task DiscordClient_Ready(ReadyEventArgs e) => await CheckChannelsAsync();

        #endregion Discord Events

        #region IRC Events

        private static void IrcClient_OnConnecting(object sender, EventArgs e)
        {
            if (sender is IrcClient ircClient)
            {
                LogManager.GetCurrentClassLogger().Info($"Connecting to {ircClient.Address}");
            }
            else
            {
                throw new ArgumentException($"{nameof(sender)} is not an {nameof(IrcClient)}", nameof(sender));
            }
        }

        private static void IrcClient_OnError(object sender, ErrorEventArgs e) => LogManager.GetCurrentClassLogger().Error(e.ErrorMessage);

        private static async void IrcClient_OnJoinAsync(object sender, JoinEventArgs e)
        {
            if (sender is IrcClient ircClient)
            {
                if (e.Who == IrcNickname)
                {
                    await Task.Delay(5000);

                    IrcEntry entry = IrcEntries.First(a => a.IrcClient == ircClient);
                    ChannelLink link = entry.ChannelLinks.First(cl => cl.IrcChannel == e.Channel);
                    Channel channel = ircClient.GetChannel(e.Channel);

                    if (channel == null)
                    {
                        await link.Webhook.ExecuteAsync($"Channel, {e.Channel}, seems wrong.", IrcGatewayName, ErrorAvatar, false, null, null);
                        return;
                    }
                    else
                    {
                        foreach (DictionaryEntry dictionaryEntry in channel.Users)
                        {
                            if (dictionaryEntry.Value is ChannelUser user && user.Nick == IrcNickname && !ircClient.IsMe(user.Nick))
                            {
                                LogManager.GetCurrentClassLogger().Info($"Leaving {e.Channel} because of duplicate bot user");
                                ircClient.RfcPart(e.Channel);
                                await link.Webhook.ExecuteAsync($"Linked channel, {e.Channel}, has already an user name of CraftBot.", IrcGatewayName, ErrorAvatar, false, null, null);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    IrcEntry entry = IrcEntries.First(a => a.IrcClient == ircClient);

                    foreach (ChannelLink channelLink in entry.ChannelLinks.FindAll(c => c.IrcChannel == e.Data.Channel))
                    {
                        await channelLink.Webhook.ExecuteAsync(
                            null,
                            IrcGatewayName,
                            DefaultAvatar,
                            false,
                            new List<DiscordEmbed>() { new MaterialEmbedBuilder(Base.Program.Client).WithTitle(e.Who, "join").Build() },
                            null);
                    }
                }
            }
        }

        public static async void IrcClient_ConnectedAsync(object sender, EventArgs e)
        {
            if (sender is IrcClient client)
            {
                IrcEntry entry = IrcEntries.First(a => a.IrcClient == client);

                #region Authentication + Verification

                LogManager.GetCurrentClassLogger().Info($"Logging in as {IrcNickname}");
                client.Login(IrcNickname, IrcNickname);

                string tokenKey = $"irc:{entry.ServerHost}:{entry.ServerPort}";
                if (Base.Program.Config.Tokens.ContainsKey(tokenKey))
                {
                    LogManager.GetCurrentClassLogger().Info($"Identifying over NickServ on {client.Address} as {IrcNickname}");
                    client.SendMessage(SendType.Message, "NickServ", "IDENTIFY " + Base.Program.Config.Tokens[tokenKey]);
                }

                LogManager.GetCurrentClassLogger().Info("Setting mode to +B");
                client.RfcMode(IrcNickname, "+B");

                #endregion Authentication + Verification

                #region Join Channels

                foreach (ChannelLink link in entry.ChannelLinks)
                {
                    string channelName = link.IrcChannel;

                    LogManager.GetCurrentClassLogger().Info("Joining channel: " + channelName);
                    client.RfcJoin(channelName);
                }

                #endregion Join Channels

                new Thread(() => client.Listen(true))
                {
                    Name = $"ListenThread ({entry.ServerHost}:{entry.ServerPort})"
                }.Start();
            }
            else
            {
                throw new ArgumentException($"{nameof(sender)} is not an {nameof(IrcClient)}", nameof(sender));
            }
        }

        public static async void IrcClient_OnMessageAsync(object sender, IrcEventArgs e)
        {
            if (sender is IrcClient ircClient)
            {
                IrcEntry entry = IrcEntries.First(a => a.IrcClient == ircClient);

                foreach (ChannelLink channelLink in entry.ChannelLinks.FindAll(c => c.IrcChannel == e.Data.Channel))
                {
                    try
                    {
                        await Utilities.SendIrcMessageAsync(channelLink.Webhook, e.Data.Nick, e.Data.Message);
                    }
                    catch
                    { }
                }
            }
            else
            {
                throw new ArgumentException($"{nameof(sender)} is not an {nameof(IrcClient)}", nameof(sender));
            }
        }

        #endregion IRC Events

        public static async Task SendMessage(DiscordMessage message)
        {
            if (message.WebhookMessage)
            {
                return;
            }

            if (!(bool)message.Channel.GetValue("irc.enabled", false))
            {
                return;
            }

            ulong channelId = message.Channel.Id;
            List<IrcEntry> entries = IrcEntries.FindAll(entry => entry.ChannelLinks.Any(cl => cl.Webhook.ChannelId == channelId));

            foreach (IrcEntry entry in entries)
            {
                foreach (ChannelLink channelLink in entry.ChannelLinks.FindAll(cl => cl.Webhook.ChannelId == channelId))
                {
                    if (MessageConverter.ToIrcMessage(message) is string ircMessage)
                    {
                        entry.IrcClient.SendMessage(SendType.Message, channelLink.IrcChannel, ircMessage);
                    }
                }
            }
        }
    }
}