using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace CraftBot.Helper
{
    public sealed class RequireTeamMemberAttribute : CheckBaseAttribute
    {
        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            DiscordUser owner = ctx.Client.CurrentApplication.Owner;
            if (IsTeamUser(owner))
            {
                using (var httpClient = new HttpClient())
                {
                    DiscordConfiguration configuration = GetDiscordConfiguration(ctx.Client);
                    string token = GetToken(configuration);
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                    var response = await httpClient.GetAsync($"https://discordapp.com/api/teams/{owner.Id}/members");
                    

                    string json = await response.Content.ReadAsStringAsync();
                    var jArray = (JArray)JsonConvert.DeserializeObject(json);
                    foreach (JToken item in jArray)
                    {
                        ulong id = (ulong)item["user"]["id"];
                        if (id == ctx.User.Id)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            else
            {
                return owner.Id == ctx.User.Id; 
            }
        }

        /// <summary>
        /// Checks if <paramref name="user"/> is a fake Discord user, aka team account
        /// </summary>
        private bool IsTeamUser(DiscordUser user)
        {
            if (user.Username != $"team{user.Id}")
            {
                return false;
            }

            return true;
        }

        private static DiscordConfiguration GetDiscordConfiguration(BaseDiscordClient baseClient)
        {
            var property = baseClient.GetType().GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
            return (DiscordConfiguration)property.GetValue(baseClient);
        }
        private static string GetToken(DiscordConfiguration discordConfiguration)
        {
            var field = discordConfiguration.GetType().GetField("_token", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)field.GetValue(discordConfiguration);
        }
    }
}
