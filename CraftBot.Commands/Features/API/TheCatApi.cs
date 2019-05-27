using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace CraftBot.Commands.Features
{
    public static class TheCatApi
    {
        #region Private Base.Program

        private static readonly HttpClient client = new HttpClient();

        #endregion Private Base.Program

        #region Public Methods

        public static async Task<TheCatApiResponse[]> GetRandomAsync()
        {
            string response = await client.GetStringAsync("https://api.thecatapi.com/v1/images/search");
            return JsonConvert.DeserializeObject<TheCatApiResponse[]>(response);
        }

        #endregion Public Methods
    }

    public partial class MainCommands : BaseCommandModule
    {
        #region Public Methods

        [Command("randomcat")]
        public async Task RandomCat(CommandContext ctx)
        {
            TheCatApiResponse resp = (await TheCatApi.GetRandomAsync())[0];
            _ = await ctx.Message.RespondAsync(embed: new DiscordEmbedBuilder()
            {
                ImageUrl = resp.Url,
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = "The Cat API",
                    Url = "https://thecatapi.com/"
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    IconUrl = ctx.User.AvatarUrl,
                    Text = ctx.User.Username + " • " + resp.Id
                }
            });
        }

        #endregion Public Methods
    }

    public class TheCatApiResponse
    {
        #region Public Properties

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        #endregion Public Properties
    }
}