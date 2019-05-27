using CraftBot.Localisation;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CraftBot.Profiles
{
    public static class LanguageExtensions
    {
        public static Language GetLanguage(this DiscordUser user) => new Profile(user).Language;

        public static Language GetLanguage(DiscordGuild guild, DiscordUser user) => guild.LanguageOverride() != null ? guild.LanguageOverride() : new Profile(user).Language;

        public static string GetString(this DiscordUser user, string key) => new Profile(user).Language.GetString(key);

        public static Language GetLanguage(this MessageCreateEventArgs eventArgs) => GetLanguage(eventArgs.Guild, eventArgs.Author);

        public static Language GetLanguage(this CommandContext context) => GetLanguage(context.Guild, context.User);

        public static string GetLocalisedLanguageName(this Language language, Language nameLanguage) => language.GetString("Language_" + nameLanguage.EnglishName);

        public static string GetString(this CommandContext context, string key) => context.GetLanguage().GetString(key);

        public static string GetString(this CommandContext context, string key, params string[] args) => string.Format(GetString(context, key), args);

        public static string GetString(this MessageCreateEventArgs eventArgs, string key) => eventArgs.GetLanguage().GetString(key);
    }
}