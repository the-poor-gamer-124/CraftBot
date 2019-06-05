using CraftBot.Base.Plugins;
using CraftBot.Helper;
using CraftBot.Localisation;
using CraftBot.Profiles;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Meebey.SmartIrc4net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CraftBot.IrcBridge
{
    [CommandClass]
    public class MainCommands : BaseCommandModule
    {
        [Group("irc")]
        public class IrcCommands : BaseCommandModule
        {
            [RequirePermissions(Permissions.Administrator)]
            [Command("link")]
            [Aliases("create")]
            public async Task Link(CommandContext context, DiscordChannel discordChannel, string host, string ircChannel)
            {
                await context.TriggerTypingAsync();
                if (!IrcGateway.IrcEntries.Any(e => e.ServerHost == host && e.ChannelLinks.Any(cl => cl.Webhook.ChannelId == discordChannel.Id && cl.IrcChannel == ircChannel)))
                {
                    DiscordWebhook webhook = await discordChannel.CreateWebhookAsync("IRC Link", reason: $"IRC Link created by {context.User.Username}#{context.User.Discriminator} ({context.User.Id})");

                    discordChannel.SetValue("irc.enabled", true);
                    discordChannel.SetValue("irc.host", host);
                    discordChannel.SetValue("irc.port", 6667);
                    discordChannel.SetValue("irc.channel", ircChannel);
                    discordChannel.SetValue("irc.webhook", webhook.Id);

                    await IrcGateway.CheckChannelForIRCAsync(discordChannel.Id);
                    await context.RespondAsync("kk");
                }
            }

            [Command("unlink")]
            [Aliases("delete", "remove", "delink")]
            public async Task Unlink(CommandContext context, DiscordChannel discordChannel, string host, string ircChannel)
            {
                await context.TriggerTypingAsync();
                IrcEntry entry = IrcGateway.IrcEntries.FirstOrDefault(e => e.ServerHost == host && e.ChannelLinks.Any(cl => cl.Webhook.ChannelId == discordChannel.Id && cl.IrcChannel == ircChannel));
                if (entry != null)
                {
                    DiscordWebhook webhook = await context.Client.GetWebhookAsync(ulong.Parse(discordChannel.GetValue("irc.webhook").ToString()));
                    await webhook.DeleteAsync();

                    discordChannel.SetValue("irc.enabled", false);
                    discordChannel.RemoveKey("irc.host");
                    discordChannel.RemoveKey("irc.port");
                    discordChannel.RemoveKey("irc.channel");
                    discordChannel.RemoveKey("irc.webhook");

                    entry.IrcClient.Disconnect();

                    IrcGateway.IrcEntries.Remove(entry);

                    await IrcGateway.CheckChannelsAsync();
                    await context.RespondAsync("kk");
                }
                else
                {
                    await context.RespondAsync("not found");
                }
            }

            [Command("enable")]
            [GroupCommand]
            public async Task Enable(CommandContext context, bool enable)
            {
                if (!context.Channel.PermissionsFor(context.Member).HasPermission(Permissions.Administrator) && context.User.Id != 194891941509332992)
                {
                    await context.RespondAsync($"You don't have the necessary permissions");
                    return;
                }

                context.Channel.SetValue("irc.enabled", enable);
                await context.RespondAsync($"This channel's IRC Link has been set to {enable}");
            }

            [GroupCommand]
            [Command("view")]
            public async Task View(CommandContext context)
            {
                MaterialEmbedBuilder builder = new MaterialEmbedBuilder(context.Client).WithTitle($"{context.Guild.Name} / IRC Gateway", context.Guild.IconUrl);

                foreach (var item in context.Guild.Channels)
                {
                    DiscordChannel channel = item.Value;
                    IrcEntry entry = IrcGateway.IrcEntries.FirstOrDefault(e => e.ChannelLinks.Any(cl => cl.Webhook.ChannelId == channel.Id));

                    if (entry != null)
                    {
                        string[] channels = new string[entry.ChannelLinks.Count];

                        for (int i = 0; i < entry.ChannelLinks.Count; i++)
                        {
                            channels[i] = entry.ChannelLinks[i].IrcChannel;
                        }

                        builder.AddListTile(
                            "pound",
                            channel.Mention,
                            string.Format(context.GetString("Server_IRC_LinkedWith"), $"`{entry.ServerHost}:{entry.ServerPort}`: {string.Join(", ", channels)}")
                            );
                    }
                }

                await context.RespondAsync(embed: builder.Build());
            }

            [Command("users")]
            public async Task ListUsers(CommandContext context)
            {
                if (!(bool)context.Channel.GetValue("irc.enabled", false))
                {
                    await context.RespondAsync("There's no IRC Link for this channel.");
                    return;
                }
                ulong channelId = context.Channel.Id;

                var builder = new MaterialEmbedBuilder(context.Client);

                List<IrcEntry> entries = IrcGateway.IrcEntries.FindAll(entry => entry.ChannelLinks.Any(cl => cl.Webhook.ChannelId == channelId));
                foreach (IrcEntry entry in entries)
                {
                    foreach (ChannelLink channelLink in entry.ChannelLinks.FindAll(cl => cl.Webhook.ChannelId == channelId))
                    {
                        builder.WithTitle($"Users in {channelLink.IrcChannel}", "account-multiple");

                        foreach (DictionaryEntry item in entry.IrcClient.GetChannel(channelLink.IrcChannel).Users)
                        {
                            string nickname = (string)item.Key;
                            string discordUser = null;

                            if (Utilities.GetDiscordMember(nickname, context.Guild) is DiscordMember member)
                            {
                                discordUser = $"{member.Username}#{member.Discriminator}";
                            }

                            builder.AddListTile("account", nickname, discordUser);
                        }
                    }
                }

                await context.RespondAsync(embed: builder.Build());
            }
        }
    }
}