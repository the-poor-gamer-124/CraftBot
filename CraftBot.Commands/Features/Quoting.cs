using CraftBot.Base.Plugins;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public static class Quoting
    {
        #region Public Base.Program

        public const string ReplyRegex = "(>>([0-9]{18}))+";

        #endregion Public Base.Program

        #region Public Methods

        public static async Task DiscordClient_MessageCreated(MessageCreateEventArgs e)
        {
            MatchCollection matches = Regex.Matches(e.Message.Content, ReplyRegex);
            if (await e.Author.IsQuotingEnabledAsync() && matches.Count != 0)
            {
                foreach (Match match in matches)
                {
                    ulong id = ulong.Parse(match.Value.Remove(0, 2));
                    try
                    {
                        DiscordMessage message = await e.Channel.GetMessageAsync(id);
                        _ = await e.Channel.SendMessageAsync(embed: message.ToEmbed(e.Client));
                    }
                    catch (NotFoundException)
                    {
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":x:"));
                    }
                }
            }
        }

        public static async Task<bool> IsQuotingEnabledAsync(this DiscordUser user) => await user.GetAsync<bool>("quoting", false);

        public static async Task SetQuotingAsync(this DiscordUser user, bool enable) => await user.SetAsync("quoting", enable);

        #endregion Public Methods
    }
}