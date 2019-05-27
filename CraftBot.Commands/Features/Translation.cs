using CraftBot.Base;
using CraftBot.Helper;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CraftBot.Commands.Features
{
    public static class Translation
    {
        public static async Task<TranslationResult> TranslateAsync(this string input, string targetLanguage, string sourceLanguage = "auto", TranslationEngine translationEngine = null)
        {
            if (translationEngine == null)
            {
                translationEngine = new GoogleTranslate();
            }
            return new TranslationResult(input, targetLanguage, await translationEngine.TranslateAsync(input, targetLanguage, sourceLanguage));
        }
    }

    public struct TranslationEngineResult
    {
        public string TranslatedText;
        public string SourceLanguage;

        public TranslationEngineResult(string sourceLanguage, string translatedText)
        {
            SourceLanguage = sourceLanguage ?? throw new ArgumentNullException(nameof(sourceLanguage));
            TranslatedText = translatedText ?? throw new ArgumentNullException(nameof(translatedText));
        }
    }

    public struct TranslationResult
    {
        public string TranslatedText;
        public string Input;
        public string SourceLanguage;
        public string TargetLanguage;

        public TranslationResult(string input, string targetLanguage, TranslationEngineResult result)
        {
            TranslatedText = result.TranslatedText ?? throw new ArgumentNullException(nameof(result.TranslatedText));
            Input = input ?? throw new ArgumentNullException(nameof(input));
            SourceLanguage = result.SourceLanguage ?? throw new ArgumentNullException(nameof(result.SourceLanguage));
            TargetLanguage = targetLanguage ?? throw new ArgumentNullException(nameof(targetLanguage));
        }

        public TranslationResult(string translatedText, string input, string sourceLanguage, string targetLanguage)
        {
            TranslatedText = translatedText ?? throw new ArgumentNullException(nameof(translatedText));
            Input = input ?? throw new ArgumentNullException(nameof(input));
            SourceLanguage = sourceLanguage ?? throw new ArgumentNullException(nameof(sourceLanguage));
            TargetLanguage = targetLanguage ?? throw new ArgumentNullException(nameof(targetLanguage));
        }
    }

    public abstract class TranslationEngine
    {
        public abstract string Name { get; }

        public abstract Task<TranslationEngineResult> TranslateAsync(string input, string targetLanguage, string sourceLanguage);
    }

    public class GoogleTranslate : TranslationEngine
    {
        public override string Name => "Google Translate";

        public override async Task<TranslationEngineResult> TranslateAsync(string input, string targetLanguage, string sourceLanguage)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");

                string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLanguage}&tl={targetLanguage}&dt=t&q={HttpUtility.UrlEncode(input, Encoding.UTF8)}";
                string response = await client.GetStringAsync(url);

                JArray json = JsonConvert.DeserializeObject<JArray>(response);

                string translatedText = json[0][0][0].ToObject<string>();
                string detectedLanguage = json[2].ToObject<string>();

                return new TranslationEngineResult(detectedLanguage, translatedText);
            }
        }
    }

    public partial class MainCommands : BaseCommandModule
    {
        [Command("translate")]
        [Aliases("t")]
        public async Task Translate(CommandContext context, string input, string target = "en")
        {
            if (Program.Flags["translate"])
            {
                var result = await Translation.TranslateAsync(input, target);

                var builder = new MaterialEmbedBuilder(context.Client);
                builder.WithTitle("Translator", "translate");
                builder.WithFooter($"Google Translate • {result.SourceLanguage} -> {result.TargetLanguage}", "https://i.imgur.com/hOZBVvU.png");
                builder.WithText(result.TranslatedText);
                await context.RespondAsync(embed: builder.Build());
            }
            else
            {
                await context.RespondAsync("The translation debug flag has not been enabled.");
            }
        }
    }
}