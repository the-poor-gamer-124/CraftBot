using DSharpPlus;
using DSharpPlus.Entities;

using System;
using System.Text;

namespace CraftBot.Helper
{
    public class MaterialEmbedListTile
    {
        private readonly DiscordClient client;
        public object Icon;
        public string Text;
        public string Title;

        public MaterialEmbedListTile(object icon, string title, string text = null)
        {
            Icon = icon;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Text = text;
        }

        public override string ToString() => this.ToString(this.client);

        public string ToString(DiscordClient client)
        {
            var listTile = new StringBuilder();

            _ = listTile.Append(IconHelper.GetString(client, Icon));
            _ = listTile.AppendLine($" **{Title}**");

            if (!string.IsNullOrWhiteSpace(Text))
            {
                _ = listTile.AppendLine($"{"blank".GetEmojiString(client)} {Text}");
            }

            return listTile.ToString();
        }
    }
}