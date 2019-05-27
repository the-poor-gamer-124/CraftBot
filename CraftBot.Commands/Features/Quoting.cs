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
            if (e.Author.QuotingEnabled().Value && matches.Count != 0)
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

        //Fake property
        public static bool? QuotingEnabled(this DiscordUser user, bool? setValue = null)
        {
            if (setValue == null)
            {
                return (bool)user.GetValue("quoting", false);
            }
            else
            {
                user.SetValue("quoting", setValue);
                return null;
            }
        }

        #endregion Public Methods
    }
}