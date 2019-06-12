using CraftBot.Base.Plugins;
using CraftBot.Helper;
using CraftBot.Profiles;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CraftBot.ServerSpecial
{
    public static class EmojiLeaderBoard
    {
        public const ulong MessengerGeekDiscordId = 573623720766078996;
        public static bool Enable = true;
        public static Dictionary<ulong, int> Leaderboard = new Dictionary<ulong, int>();

        static EmojiLeaderBoard() => LoadLeaderboardAsync();

        public static async void LoadLeaderboardAsync()
        {
            var value = await Base.Program.Database.GetAsync("guild", MessengerGeekDiscordId, "elb", JObject.FromObject(new Dictionary<ulong, int>()));
            foreach (KeyValuePair<string, JToken> item in value)
            {
                ulong id = ulong.Parse(item.Key);
                int first = (int)item.Value;
                Leaderboard[id] = first;
            }
        }

        public static async Task MessageCreated(MessageCreateEventArgs e)
        {
            if (Enable && !e.Author.IsBot && e.Guild != null && e.Guild.Id == MessengerGeekDiscordId)
            {
                string msg = e.Message.Content;

                bool containsDiscordEmoji = msg.Contains("😛");
                bool containsMsnEmoji = msg.Contains("<:msn_tongue:484159452807823370>");
                bool containsEmoji = containsDiscordEmoji || containsMsnEmoji;

                if (containsEmoji)
                {
                    if (!Leaderboard.ContainsKey(e.Author.Id))
                    {
                        Leaderboard.Add(e.Author.Id, 0);
                    }
                    Leaderboard[e.Author.Id]++;

                    int newRank = Leaderboard[e.Author.Id];

                    string nulls = new string('0', newRank.ToString().Length - 1);
                    if (!string.IsNullOrWhiteSpace(nulls))
                    {
                        bool hitMilestone = newRank.ToString().EndsWith(nulls);
                        if (hitMilestone)
                        {
                            _ = await e.Message.RespondAsync(string.Format(await e.GetStringAsync("ELB_MilestoneHit"), e.Author.Mention, newRank));
                        }
                    }

                    SaveLeaderboardAsync();
                }
            }
        }

        public static async Task SaveLeaderboardAsync() => await Base.Program.Database.SetAsync("guild", MessengerGeekDiscordId, "elb", Leaderboard);

    }

    [CommandClass]
    public partial class MainCommands : BaseCommandModule
    {
        [Hidden]
        [RequireGuild]
        [Group("emojileaderboard")]
        [Aliases("elb")]
        public class LeaderboardCommands : BaseCommandModule
        {
            [Command("enable")]
            [RequireUserGroup]
            public async Task Enable(CommandContext ctx, bool enable)
            {
                EmojiLeaderBoard.Enable = enable;
                _ = await ctx.RespondAsync($"Leaderboard has been {(enable ? "en" : "dis")}abled.");
            }

            [Command("purge")]
            [RequireUserGroup]
            public async Task Purge(CommandContext ctx)
            {
                EmojiLeaderBoard.Leaderboard.Clear();
                _ = await ctx.RespondAsync("Leaderboard purged.");
            }

            [Command("reload")]
            [RequireUserGroup]
            public async Task Reload(CommandContext ctx)
            {
                EmojiLeaderBoard.LoadLeaderboardAsync();
                _ = await ctx.RespondAsync("Leaderboard reloaded from file.");
            }

            [Command("save")]
            [RequireUserGroup]
            public async Task Save(CommandContext ctx)
            {
                EmojiLeaderBoard.SaveLeaderboardAsync();
                _ = await ctx.RespondAsync("Leaderboard has been saved as file.");
            }

            [Command("view")]
            [GroupCommand]
            public async Task View(CommandContext context)
            {
                if (context.Guild.Id != EmojiLeaderBoard.MessengerGeekDiscordId)
                {
                    await context.Message.RespondAsync(await context.GetStringAsync("ELB_RestrictionMessage"));
                    return;
                }

                var builder = new DiscordEmbedBuilder();
                builder.WithAuthor(await context.GetStringAsync("ELB_Title"), iconUrl: "https://github.com/twitter/twemoji/raw/gh-pages/72x72/1f61b.png");

                string list = "";
                var sorted = EmojiLeaderBoard.Leaderboard.OrderByDescending(a => a.Value);
                var top10 = sorted.Take(10).ToList();

                for (int i = 0; i < top10.Count(); i++)
                {
                    KeyValuePair<ulong, int> item = top10[i];
                    string prefix = new[] { ":one:", ":two:", ":three:", ":three:", ":four:", ":five:", ":six:", ":seven:", ":eight:", ":nine:", ":keycap_ten:" }[i];
                    list += $"{prefix} {(await context.Client.GetUserAsync(item.Key)).Username} `{item.Value}`\n";
                }

                int leftOver = sorted.Count() - top10.Count();
                if (leftOver != 0)
                {
                    list += $"\n*and {leftOver} more...*";
                }

                builder.AddField(await context.GetStringAsync("ELB_Ranking"), list);

                if (!EmojiLeaderBoard.Enable)
                {
                    builder.WithFooter(await context.GetStringAsync("ELB_DisabledNotice"));
                }

                await context.RespondAsync(embed: builder.Build());
            }
        }
    }
}