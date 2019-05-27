using DSharpPlus.Entities;

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CraftBot.IrcBridge
{
    public static class MessageConverter
    {
        public static string ToIrcMessage(DiscordMessage message)
        {
            if (message.Author == null || string.IsNullOrWhiteSpace(message.Content))
            {
                return null;
            }

            string discordMessage = message.Content;

            if (!string.IsNullOrWhiteSpace(discordMessage))
            {
                //Filter out newlines
                discordMessage = discordMessage.Replace(Environment.NewLine, " ");
                discordMessage = discordMessage.Replace("\r", " ");
                discordMessage = discordMessage.Replace("\n", " ");
            }

            foreach (DiscordUser user in message.MentionedUsers)
            {
                discordMessage = discordMessage.Replace($"<@{user.Id}>", $"@{user.Username}");
                discordMessage = discordMessage.Replace($"<@!{user.Id}>", $"@{user.Username}");
            }

            var ircMessage = new StringBuilder();

            if (message.IsEdited)
            {
                ircMessage.Append("(edited) ");
            }

            ircMessage.Append(message.Author.Username);
            ircMessage.Append('#' + message.Author.Discriminator);
            ircMessage.Append(": " + discordMessage);

            if (message.Attachments.Count != 0)
            {
                string[] urls = new string[message.Attachments.Count];

                for (int i = 0; i < message.Attachments.Count; i++)
                {
                    urls[i] = message.Attachments[i].Url;
                }

                ircMessage.Append($" (attachments: {string.Join(" ", urls)})");
            }

            return ircMessage.ToString();
        }

        /// <summary>
        /// Converts a(n IRC) message to <see cref="DiscordMessage"/> content
        /// </summary>
        /// <param name="message">The original message</param>
        /// <param name="discordGuild">The target guild</param>
        /// <returns>The converted message</returns>
        public static async Task<string> ToDiscordMessgeAsync(string message, DiscordGuild discordGuild)
        {
            foreach (Match match in Regex.Matches(message, "@(\\w+)"))
            {
                string memberName = match.Value.Substring(1);

                DiscordMember mentionedMember = Utilities.GetDiscordMember(memberName, discordGuild);

                if (mentionedMember != null)
                {
                    message = message.Replace(match.Value, mentionedMember.Mention);
                }
            }

            foreach (Match match in Regex.Matches(message, ":([A-Za-z0-9_]+):"))
            {
                string emojiName = match.Value.Substring(1, match.Value.Length - 2);

                var emojis = await discordGuild.GetEmojisAsync();
                var emoji = emojis.FirstOrDefault(em => em.Name.Equals(emojiName, StringComparison.InvariantCultureIgnoreCase));

                if (emoji != null)
                {
                    message = message.Replace(match.Value, emoji.ToString());
                }
            }

            return message;
        }
    }
}
