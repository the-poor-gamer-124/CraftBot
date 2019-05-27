using CraftBot.Helper;

using DSharpPlus;
using DSharpPlus.Entities;

using System;
using System.Drawing;

using static CraftBot.Helper.CustomEmoji;

namespace CraftBot.Commands
{
    public static class Extensions
    {
        #region DiscordEmbedBuilder

        public static DiscordEmbedBuilder WithColor(this DiscordEmbedBuilder builder, Color color) => builder.WithColor(new DiscordColor(color.R, color.G, color.B));

        #endregion DiscordEmbedBuilder

        #region DiscordMessage

        public static DiscordEmbed ToEmbed(this DiscordMessage message, DiscordClient client)
        {
            return new DiscordEmbedBuilder()
                .WithAuthor(message.Author.Username, null, message.Author.AvatarUrl)
                .WithDescription($"{message.Content} [{"message-up".GetEmojiString(client)}](https://discordapp.com/channels/{message.Channel.Guild.Id}/{message.Channel.Id}/{message.Id}/)")
                .WithTimestamp(message.CreationTimestamp)
                .WithColor(DiscordColor.Blurple)
                .Build();
        }

        #endregion DiscordMessage

        #region TimeSpan

        public static string ToConversationalString(this TimeSpan timeSpan)
        {
            var text = new System.Text.StringBuilder();
            text.Append((timeSpan.Seconds == 0) ? string.Empty : $"{timeSpan.Seconds} seconds");
            text.Append((timeSpan.Minutes == 0) ? string.Empty : $", {timeSpan.Minutes} minutes");
            text.Append((timeSpan.Hours == 0) ? string.Empty : $", {timeSpan.Hours} hours");
            text.Append((timeSpan.Days == 0) ? string.Empty : $", {timeSpan.Days} days");
            return text.ToString();
        }

        #endregion TimeSpan

        #region Boolean

        public static DiscordEmoji GetEmoji(this bool boolean) => GetEmoji(boolean ? "checkbox-marked" : "checkbox-blank-outline");

        public static string GetEmojiString(this bool boolean) => boolean.GetEmoji().ToString();

        #endregion Boolean

        #region String

        public static DiscordEmoji GetEmoji(this ulong id) => id.GetEmoji(Base.Program.Client);

        public static DiscordEmoji GetEmoji(this string name) => CustomEmoji.GetEmojiId(name).GetEmoji();

        public static string GetEmojiString(this string name) => name.GetEmoji().ToString();

        #endregion String
    }
}