using DSharpPlus;
using System;
using System.Text;

namespace CraftBot.Helper
{
    public class MaterialEmbedSection
    {
        #region Private Fields

        private readonly DiscordClient client;

        #endregion Private Fields

        #region Public Constructors

        public MaterialEmbedSection(string title = null, string icon = null)
        {
            if (title != null)
            {
                _ = this.WithTitle(title, icon);
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public string Text { get; set; }
        public string Title { get; set; }

        #endregion Public Properties

        #region Public Methods

        public MaterialEmbedSection AddListTile(MaterialEmbedListTile listTile) => this.AddListTile(this.client, listTile);

        public MaterialEmbedSection AddListTile(DiscordClient client, MaterialEmbedListTile listTile)
        {
            this.Text += listTile.ToString(client);
            return this;
        }

        public MaterialEmbedSection AddListTile(object icon, string title, string description = null) => this.AddListTile(this.client, icon, title, description);

        public MaterialEmbedSection AddListTile(DiscordClient client, object icon, string title, string description = null) => AddListTile(client, new MaterialEmbedListTile(icon, title, description));

        public MaterialEmbedSection AddText(string text)
        {
            this.Text += text;
            return this;
        }

        public MaterialEmbedSection AddTo(MaterialEmbedBuilder builder)
        {
            _ = builder.embedBuilder.AddField(Title, Text);
            return this;
        }

        public MaterialEmbedSection WithText(string text)
        {
            this.Text = text;
            return this;
        }

        public MaterialEmbedSection WithTitle(string title, string icon = null) => this.WithTitle(this.client, title, icon);

        public MaterialEmbedSection WithTitle(DiscordClient client, string title, string icon = null)
        {
            //Checks if icon is present
            this.Title = icon != null ? icon.GetEmojiString(client) + ' ' + title : title;
            return this;
        }

        #endregion Public Methods
    }
}