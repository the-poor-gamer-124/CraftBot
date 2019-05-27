using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using System;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features.Protection
{
    public static class SexBotProtection
    {
        public const string SexUrl = "amazingsexdating";

        public static async Task MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Message.Content.Contains(SexUrl, StringComparison.InvariantCulture))
            {
                DiscordGuild userServer = await e.Client.GetGuildAsync(448179260167946243);
                DiscordMember member = await userServer.GetMemberAsync(194891941509332992);
                _ = await member.SendMessageAsync(embed: new DiscordEmbedBuilder()
                {
                    Title = "Message matched",
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        IconUrl = e.Author.AvatarUrl,
                        Name = $"{e.Author.Username}#{e.Author.Discriminator} ({e.Author.Id})"
                    },
                    Description = e.Message.Content,
                    Footer = new DiscordEmbedBuilder.EmbedFooter()
                    {
                        IconUrl = e.Guild.IconUrl,
                        Text = $"{e.Guild.Name} ({e.Guild.Id}) - {e.Message.Id}"
                    }
                }
                );
            }
        }
    }
}
