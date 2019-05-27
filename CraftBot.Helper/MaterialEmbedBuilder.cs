using DSharpPlus;
using DSharpPlus.Entities;

using System.Text;

namespace CraftBot.Helper
{
    /// <summary>
    /// Embed Builder for embeds that try to simulate the Material Design.
    /// </summary>
    public class MaterialEmbedBuilder
    {
        internal readonly DiscordClient client;

        internal DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();

        public MaterialEmbedBuilder(DiscordClient client) => this.client = client;

        public MaterialEmbedBuilder AddListTile(object icon, string title, string text = null)
        {
            embedBuilder.Description += new MaterialEmbedListTile(icon, title, text).ToString(this.client) + '\n';
            return this;
        }

        public MaterialEmbedBuilder AddListTile(MaterialEmbedListTile listTile)
        {
            embedBuilder.Description += listTile.ToString(client) + '\n';
            return this;
        }

        public MaterialEmbedBuilder AddSection(string icon, string title, params MaterialEmbedListTile[] listTiles)
        {
            var section = new MaterialEmbedSection(title, icon);
            foreach (MaterialEmbedListTile listTile in listTiles)
            {
                if (listTile == null)
                {
                    continue;
                }
                section.AddText(listTile.ToString(this.client));
            }
            if (!string.IsNullOrWhiteSpace(section.Text))
            {
                _ = section.AddTo(this);
            }
            return this;
        }

        public MaterialEmbedBuilder AddSection(string icon, string title, string text)
        {
            MaterialEmbedSection section = new MaterialEmbedSection(title, icon).WithText(text);

            if (!string.IsNullOrWhiteSpace(section.Text))
            {
                _ = section.AddTo(this);
            }
            return this;
        }

        public DiscordEmbed Build() => embedBuilder.Build();

        public MaterialEmbedBuilder WithFooter(string text, string iconUrl = null)
        {
            embedBuilder.Footer = new DiscordEmbedBuilder.EmbedFooter()
            {
                Text = text,
                IconUrl = iconUrl,
            };
            return this;
        }

        public MaterialEmbedBuilder WithImage(string imageUrl)
        {
            embedBuilder.ImageUrl = imageUrl;
            return this;
        }

        public MaterialEmbedBuilder WithText(string text)
        {
            embedBuilder.Description = text;
            return this;
        }

        public MaterialEmbedBuilder WithTitle(string title, string icon = null)
        {
            //Checks if icon is present

            if (icon != null)
            {
                //Checks if icon is an emoji or URL.
                if (icon.ToLower().StartsWith("http"))
                {
                    embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        IconUrl = icon,
                        Name = title
                    };
                }
                else
                {
                    embedBuilder.Title = icon.GetEmojiString(client) + ' ' + title;
                }
            }
            else
            {
                embedBuilder.Title = title;
            }

            return this;
        }
    }
}