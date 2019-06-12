using CraftBot.Base.Plugins;

using DSharpPlus.Entities;

using System.Threading.Tasks;

namespace CraftBot.Localisation
{
    public static class Extensions
    {
        public static async Task<Language> GetLanguageOverrideAsync(this DiscordGuild guild) => await guild.GetAsync<Language>("languageOverride", null);

        public static async Task SetLanguageOverrideAsync(this DiscordGuild guild, Language language) => await guild.SetAsync("languageOverride", language);
    }
}