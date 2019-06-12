using CraftBot.Base.Plugins;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public static class DelayDelete
    {
        public const string DelayDeleteRegex = "T([0-9]+)";

        public static async Task DiscordClient_MessageCreated(MessageCreateEventArgs e)
        {
            if (!await e.Author.GetDelayDeletingAsync())
            {
                return;
            }

            MatchCollection matches = Regex.Matches(e.Message.Content, DelayDeleteRegex);
            if (matches.Count == 1)
            {
                if (int.TryParse(matches[0].Value.Substring(1), out int seconds))
                {
                    if (!e.Channel.PermissionsFor(await e.Guild.GetMemberAsync(e.Client.CurrentUser.Id)).HasFlag(Permissions.ManageMessages))
                    {
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":no_entry_sign:"));
                        return;
                    }

                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":wastebasket:"));
                    await Task.Delay(seconds * 1000);
                    await e.Message.DeleteAsync();
                }
                else
                {
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":x:"));
                }
            }
            else if (matches.Count > 1)
            {
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":question:"));
            }
        }

        //Fake property
        public static async Task SetDelayDeletingAsync(this DiscordUser user, bool enabled) => await user.SetAsync("delayDelete", enabled);

        public static async Task<bool> GetDelayDeletingAsync(this DiscordUser user) => await user.GetAsync<bool>("delayDelete");
    }
}