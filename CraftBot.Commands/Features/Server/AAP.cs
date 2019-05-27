using CraftBot.Commands.Features.Server;
using CraftBot.Base.Plugins;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WordsMatching;
using DSharpPlus.Interactivity;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands : BaseCommandModule
    {
        public partial class ServerCommands : BaseCommandModule
        {
            [Command("aap")]
            public async Task AAP(CommandContext ctx)
            {
                bool enabled = ctx.Guild.AAP().Value;

                var builder = new DiscordEmbedBuilder();

                string description = string.Empty;
                description += $"{enabled.GetEmoji().ToString()} Enabled\n";

                _ = await ctx.RespondAsync(embed: builder
                    .WithAuthor($"{ctx.Guild.Name} / Alternate Account Protection", null, ctx.Guild.IconUrl)
                    .WithDescription(description)
                    .Build()
                    );
            }

            [Command("aap")]
            [RequirePermissions(Permissions.Administrator)]
            public async Task AAP(CommandContext ctx, bool enable)
            {
                _ = ctx.Guild.AAP(enable);
                _ = await ctx.Message.RespondAsync($"Alternate Account Protection has been {(enable ? "enabled" : "disabled")}.");
            }

            [Command("aap")]
            [RequirePermissions(Permissions.Administrator)]
            public async Task AAP(CommandContext ctx, DiscordChannel channel)
            {
                _ = ctx.Guild.AAPDestination(channel.Id);
                _ = await ctx.Message.RespondAsync($"Alternate Account Protection's announcement channel has been set to {channel.Mention}.");
            }
        }
    }
}

namespace CraftBot.Commands.Features.Server
{
    public static class AlternateAccountProtection
    {
        public static Leven Leven = new Leven();

        //Fake property
        public static bool? AAP(this DiscordGuild guild, bool? setValue = null)
        {
            if (setValue == null)
            {
                return (bool)guild.GetValue("aap", false);
            }
            else
            {
                guild.SetValue("aap", setValue);
                return null;
            }
        }

        //Fake property
        public static ulong? AAPDestination(this DiscordGuild guild, ulong? setValue = null)
        {
            if (setValue == null)
            {
                string value = guild.GetValue("aap-dest", ulong.MinValue).ToString();
                return ulong.Parse(value);
            }
            else
            {
                guild.SetValue("aap-dest", setValue);
                return null;
            }
        }

        public static async Task DiscordClient_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            if (e.Guild.AAP().Value)
            {
                IReadOnlyList<DiscordBan> banList = await e.Guild.GetBansAsync();
                foreach (DiscordBan ban in banList)
                {
                    float similarity = Leven.GetSimilarity(ban.User.Username.ToLower(), e.Member.Username.ToLower());
                    if (similarity < 1f)
                    {
                        DiscordChannel announcementChannel = e.Guild.GetChannel(e.Guild.AAPDestination().Value);
                        DiscordMessage confirmationMessage = await announcementChannel.SendMessageAsync(embed: new DiscordEmbedBuilder()
                            .WithTitle($"{"help-circle".GetEmojiString()} Confirm Ban")
                            .AddField("Member", e.Member.Mention)
                            .AddField("Match", ban.User.Mention)
                            .AddField("Matching", similarity.ToString())
                            );

                        const string check = ":white_check_mark:";
                        const string cross = ":negative_squared_cross_mark:";

                        await confirmationMessage.CreateReactionAsync(DiscordEmoji.FromName(e.Client, check));
                        await confirmationMessage.CreateReactionAsync(DiscordEmoji.FromName(e.Client, cross));

                        string reason = "Alternate Account Protection: Username matched by " + similarity.ToString();
                        bool confirmed = false;

                        while (!confirmed)
                        {
                            InteractivityResult<MessageReactionAddEventArgs> interactivityResult = await Base.Program.Interactivity.WaitForReactionAsync(args => args.Emoji.GetDiscordName() == check || args.Emoji.GetDiscordName() == cross);

                            if (!interactivityResult.TimedOut)
                            {
                                if (interactivityResult.Result.Message.Id == confirmationMessage.Id)
                                {
                                    DiscordMember member = await interactivityResult.Result.Channel.Guild.GetMemberAsync(interactivityResult.Result.User.Id);
                                    if (member.Roles.Any(r => r.CheckPermission(Permissions.BanMembers) == PermissionLevel.Allowed))
                                    {
                                        if (interactivityResult.Result.Emoji.GetDiscordName() == check)
                                        {
                                            await e.Member.BanAsync(7, reason);
                                            confirmed = true;
                                            break;
                                        }
                                        else if (interactivityResult.Result.Emoji.GetDiscordName() == cross)
                                        {
                                            confirmed = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (confirmed)
                        {
                            _ = await announcementChannel.SendMessageAsync(embed: new DiscordEmbedBuilder()
                                .WithTitle($"{"check".GetEmojiString()} Ban succeeded")
                                .AddField("Ban issuer", e.Client.CurrentUser.Mention, true)
                                .AddField("Ban receiver", e.Member.Mention, true)
                                .AddField("Reason", reason)
                                .WithTimestamp(DateTime.Now));
                        }
                    }
                }
            }
        }
    }
}