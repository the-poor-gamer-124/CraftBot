using CraftBot.Localisation;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using System.Threading.Tasks;

namespace CraftBot.Profiles
{
    public static class LanguageExtensions
    {
        public static Language GetLanguage(this DiscordUser user) => new Profile(user).Language;

        public static async Task<Language> GetLanguageAsync(DiscordGuild guild, DiscordUser user) => await guild.GetLanguageOverrideAsync() ?? new Profile(user).Language;

        public static string GetString(this DiscordUser user, string key) => new Profile(user).Language.GetString(key);

        public static async Task<Language> GetLanguageAsync(this MessageCreateEventArgs eventArgs) => await GetLanguageAsync(eventArgs.Guild, eventArgs.Author);

        public static async Task<Language> GetLanguageAsync(this CommandContext context) => await GetLanguageAsync(context.Guild, context.User);

        public static string GetLocalisedLanguageName(this Language language, Language nameLanguage) => language.GetString("Language_" + nameLanguage.EnglishName);

        public static async Task<string> GetStringAsync(this CommandContext context, string key) => (await context.GetLanguageAsync()).GetString(key);

        public static async Task<string> GetStringAsync(this CommandContext context, string key, params string[] args) => string.Format(await GetStringAsync(context, key), args);

        public static async Task<string> GetStringAsync(this MessageCreateEventArgs eventArgs, string key) => (await eventArgs.GetLanguageAsync()).GetString(key);
    }
}