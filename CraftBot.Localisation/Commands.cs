using CraftBot.Base.Plugins;
using CraftBot.Helper;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CraftBot.Localisation
{
    [CommandClass]
    [Group("localisation")]
    [Aliases("local", "localization")]
    public class LocalisationCommands : BaseCommandModule
    {
        [Command("getstring")]
        [Aliases("g", "get")]
        public async Task GetString(CommandContext context, string languageName, string key)
        {
            var language = Language.GetLanguage(languageName);
            await context.RespondAsync(embed: new DiscordEmbedBuilder()
                .WithTitle(key)
                .WithDescription(language.GetString(key))
                .AddField("Language", $"{language.NativeName} ({language.EnglishName}) [{language.TwoLetterCode}/{language.ThreeLetterCode}]"));
        }

        [RequireUserGroup]
        [Command("load")]
        [Aliases("l")]
        public async Task LoadFromAttachment(CommandContext context)
        {
            if (context.Message.Attachments.Count == 0)
            {
                await context.RespondAsync("Attachment missing");
            }
            else
            {
                await context.TriggerTypingAsync();
                await LocalisationTable.LoadFromWeb(context.Message.Attachments[0].Url);
                await context.RespondAsync("Localisation table has been loaded");
            }
        }

        [Command("missing")]
        [Aliases("m")]
        public async Task MissingStrings(CommandContext context, string languageName)
        {
            await context.TriggerTypingAsync();

            var language = Language.GetLanguage(languageName);

            if (language == null)
            {
                await context.RespondAsync(languageName + " is an unrecognized language");
                return;
            }

            var builder = new MaterialEmbedBuilder(context.Client);

            var missingKeys = new List<string>();
            foreach (string key in LocalisationTable.GetKeys())
            {
                if (!language.HasKey(key) && missingKeys.Count < 30)
                {
                    missingKeys.Add(key);
                }
            }

            string body = string.Empty;
            if (missingKeys.Count == 0)
            {
                body = "*no entries present*";
            }
            else
            {
                foreach (string key in missingKeys)
                {
                    body += $"- `{key}`\n";
                }
            }

            builder.WithTitle($"{language.EnglishName} / Missing Strings");
            builder.WithText(body);

            await context.RespondAsync(embed: builder.Build());
        }

        [Cooldown(3, 10, CooldownBucketType.Global)]
        [Command("reload")]
        [Aliases("r")]
        public async Task ReloadFromWeb(CommandContext context)
        {
            await LocalisationTable.LoadFromWeb();
            await context.RespondAsync("Localisation table has been reloaded");
        }
    }
}