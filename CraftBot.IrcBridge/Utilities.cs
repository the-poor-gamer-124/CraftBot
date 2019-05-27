using DSharpPlus.Entities;

using System.Linq;
using System.Threading.Tasks;

namespace CraftBot.IrcBridge
{
    public static class Utilities
    {
        public static DiscordMember GetDiscordMember(string username, DiscordGuild guild) => guild.Members.FirstOrDefault(m => m.Value.Username == username).Value;

        public static async Task SendIrcMessageAsync(DiscordWebhook webhook, string nickname, string message)
        {
            DiscordGuild discordGuild = await Base.Program.Client.GetGuildAsync(webhook.GuildId);
            DiscordMember discordMember = GetDiscordMember(nickname, discordGuild);

            await webhook.ExecuteAsync(
                await MessageConverter.ToDiscordMessgeAsync(message, discordGuild),
                discordMember == null ? nickname : discordMember.Username,
                discordMember == null ? IrcGateway.DefaultAvatar : discordMember.AvatarUrl,
                false,
                null,
                null);
        }
    }
}