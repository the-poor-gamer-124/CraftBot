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
        #region Public Base.Program

        public const string DelayDeleteRegex = "T([0-9]+)";

        #endregion Public Base.Program

        #region Public Methods

        public static async Task DiscordClient_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.DelayDeleteEnabled().Value)
            {
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
        }

        //Fake property
        public static bool? DelayDeleteEnabled(this DiscordUser user, bool? setValue = null)
        {
            if (setValue == null)
            {
                return (bool)user.GetValue("delayDelete", false);
            }
            else
            {
                user.SetValue("delayDelete", setValue);
                return null;
            }
        }

        #endregion Public Methods
    }
}