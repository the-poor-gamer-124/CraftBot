using DSharpPlus;
using DSharpPlus.Entities;

namespace CraftBot.Helper
{
    public static class IconHelper
    {
        public static string GetString(DiscordClient client, object icon)
        {
            if (client == null)
            {
                throw new System.ArgumentNullException(nameof(client));
            }

            switch (icon)
            {
                case string iconString: return iconString.GetEmojiString(client);
                case bool iconBool: return iconBool.GetEmojiString(client);
                case DiscordEmoji iconEmoji: return iconEmoji.ToString();
                default: return "blank".GetEmojiString(client);
            }
        }
    }
}