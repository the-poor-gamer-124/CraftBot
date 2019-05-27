using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CraftBot.Commands
{
    public static class Helpers
    {
        public static string Get(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static async Task<DiscordChannel> GetChannel(string name, CommandContext ctx)
        {
            var matches = ctx.Guild.Channels.Values.Where(channel => channel.Name.ToLower().Contains(name)).ToList();
            if (matches.Count() == 0)
            {
                _ = await ctx.Message.RespondAsync($"No channels containing {name} have been found");
                return null;
            }
            else if (matches.Count() == 1)
            {
                return matches[0];
            }
            else
            {
                string channels = "";
                for (int i = 0; i < matches.Count() - 1; i++)
                {
                    channels += $"**{i}.** {matches[i]}\n";
                }

                _ = await ctx.RespondAsync(embed: new DiscordEmbedBuilder()
                   .WithAuthor($"{ctx.Guild.Name}", null, ctx.Guild.IconUrl)
                   .AddField("Following channels have been found: ", channels));
                string resp = (await Base.Program.Interactivity.WaitForMessageAsync(msg => Regex.IsMatch(msg.Content, "^[0-9]+$") && msg.Author.Id == ctx.User.Id)).Result.Content;
                if (int.TryParse(resp, out int r))
                {
                    if (r >= 0 && r <= matches.Count() - 1)
                    {
                        return matches[r];
                    }
                    else
                    {
                        _ = await ctx.Message.RespondAsync($"{ctx.User.Username}, your selection is out of range.");
                        return null;
                    }
                }
                else
                {
                    _ = await ctx.Message.RespondAsync($"{ctx.User.Username}, your selection is invalid. (not a number)");
                    return null;
                }
            }
        }

        public static DiscordEmoji GetCheck(DiscordClient c, bool Check) => Check ? DiscordEmoji.FromGuildEmote(Base.Program.Client, 408327774990893056) : DiscordEmoji.FromGuildEmote(c, 408327774869258257);

        public static string GetRandomString()
        {
            var g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            return GuidString;
        }

        public static string MakeListTile(object icon, string title, string description = null)
        {
            var listTile = new StringBuilder();

            switch (icon)
            {
                case string iconString:
                    _ = listTile.Append(iconString.GetEmojiString());
                    break;

                case bool iconBool:
                    _ = listTile.Append(iconBool.GetEmojiString());
                    break;

                case DiscordEmoji iconEmoji:
                    _ = listTile.Append(iconEmoji.ToString());
                    break;

                default:
                    listTile.Append("blank".GetEmojiString());
                    break;
            }

            _ = listTile.AppendLine($" **{title}**");

            if (!string.IsNullOrWhiteSpace(description))
            {
                _ = listTile.AppendLine($"{"blank".GetEmojiString()} {description}");
            }

            return listTile.ToString();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static async Task<bool?> WaitForConfirmation(this InteractivityExtension interactivity, TimeSpan? timeoutoverride = null)
        {
            string[] positive = new string[]
            {
                "yes",
                "y",
                "yeah",
                "✅",
                "✔",
            };
            string[] negative = new string[]
            {
                "no",
                "n",
                "nope",
                "❌",
                "✖",
                "❎"
            };
            InteractivityResult<DiscordMessage> interactivityResult = await interactivity.WaitForMessageAsync(msg => positive.Any(w => msg.Content.ToLower().Contains(w)) ||
                                                                        negative.Any(w => msg.Content.ToLower().Contains(w)),
                                                                        timeoutoverride);
            return positive.Any(w => interactivityResult.Result.Content.ToLower().Contains(w))
                ? true
                : negative.Any(w => interactivityResult.Result.Content.ToLower().Contains(w)) ? false : (bool?)null;
        }
    }
}