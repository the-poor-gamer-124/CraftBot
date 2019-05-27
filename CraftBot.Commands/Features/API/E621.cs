using CraftBot.Base.Plugins;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands
    {
        [Command("e621")]
        [RequireNsfw]
        public async Task E621(CommandContext ctx, params string[] tags)
        {
            API.E612Entry[] entries = await API.E621.GetEntries(tags);
            API.E612Entry entry = entries[Plugin.Random.Next(0, entries.Length - 1)];

            _ = await ctx.RespondAsync(embed: new DiscordEmbedBuilder()
            {
                Description = entry.Description,
                ImageUrl = entry.file_url,
                Title = entry.id.ToString()
            });
        }
    }
}

namespace CraftBot.Commands.Features.API
{
    public static class E621
    {
        public static async Task<E612Entry[]> GetEntries(string[] tags)
        {
            using (var webClient = new WebClient())
            {
                string url = $"https://e621.net/post/index.json?tags={string.Join("+", tags)}&limit=100";
                webClient.Headers.Set("User-Agent", "CraftBot/1.0 (Craftplacer)");
                string json = await Plugin.WebClient.DownloadStringTaskAsync(url);
                return JsonConvert.DeserializeObject<E612Entry[]>(json);
            }
        }
    }

    public class E612Entry
    {
        public string Description;
        public string file_url;
        public int id;
    }
}