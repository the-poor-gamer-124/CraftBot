using DSharpPlus.Entities;
using CraftBot.Base.Plugins;

namespace CraftBot.Localisation
{
    public static class Extensions
    {
        //Fake property
        public static Language LanguageOverride(this DiscordGuild guild, Language setValue = null)
        {
            if (setValue == null)
            {
                return (Language)guild.GetValue("languageOverride", null);
            }
            else
            {
                guild.SetValue("languageOverride", setValue);
                return null;
            }
        }
    }
}