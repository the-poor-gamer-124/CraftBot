using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands : BaseCommandModule
    {
        [Description("Contains commands for moderating a server")]
        [Aliases("mod")]
        [Group("moderation")]
        public class ModerationCommands : BaseCommandModule
        {

            [Description("Purges the specified channel")]
            [RequirePermissions(Permissions.ManageMessages)]
            [Command("purge")]
            [Aliases("clear", "cls", "clr")]
            public async Task Purge(CommandContext ctx, int amount = 100, DiscordChannel channel = null)
            {
                if (channel == null)
                {
                    channel = ctx.Channel;
                }

                var messages = await channel.GetMessagesBeforeAsync(ctx.Message.Id, amount);

                await channel.DeleteMessagesAsync(messages);

                if (ctx.Channel.Id == channel.Id)
                {
                    await ctx.Message.DeleteAsync();
                    return;
                }

                await ctx.RespondAsync("Channel cleared");
            }

            [RequirePermissions(Permissions.BanMembers)]
            [Command("ban")]
            public async Task Ban(CommandContext ctx, DiscordUser user, string reason = "No reason was provided.")
            {
                await ctx.Guild.BanMemberAsync(user.Id, 7, $"Ban issued by {ctx.User.Mention}: {reason}");

                _ = await ctx.RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithTitle($"{"check".GetEmojiString()} Ban succeeded")
                    .AddField("Ban issuer", ctx.User.Mention, true)
                    .AddField("Ban receiver", user.Mention, true)
                    .AddField("Reason", reason)
                    .WithTimestamp(DateTime.Now)
                    );
            }

            [RequirePermissions(Permissions.BanMembers)]
            [Command("unban")]
            public async Task Unban(CommandContext ctx, DiscordUser user, string reason = "No reason was provided.")
            {
                await ctx.Guild.UnbanMemberAsync(user.Id, $"Unban issued by {ctx.User.Username}: {reason}");

                _ = await ctx.RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithTitle($"{"check".GetEmojiString()} Unban succeeded")
                    .AddField("Unban issuer", ctx.User.Mention, true)
                    .AddField("Unban receiver", user.Mention, true)
                    .AddField("Reason", reason)
                    .WithTimestamp(DateTime.Now)
                    );
            }
        }
    }
}