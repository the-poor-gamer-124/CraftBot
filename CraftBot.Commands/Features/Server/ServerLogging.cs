using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CraftBot.Helper;
using CraftBot.Base.Plugins;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace CraftBot.Commands.Features
{
    public static class ServerLogging
    {
        public static async Task DiscordClient_MessageCreated(MessageCreateEventArgs e)
        {
            await Task.Delay(0);
            try
            {
                if (e.Guild.Logging().Value)
                {
                    string filename = Path.Combine(PathHelper.GuildDataDirectory, e.Guild.Id + ".log.json");
                    if (!File.Exists(filename))
                    {
                        e.Client.DebugLogger.LogMessage(LogLevel.Debug, "Logger", "Creating new logging list for " + e.Guild.Id, DateTime.Now);
                        File.WriteAllText(filename, JsonConvert.SerializeObject(new List<DiscordMessage>()));
                    }
                    if (File.Exists(filename))
                    {
                        List<DiscordMessage> msgs = JsonConvert.DeserializeObject<List<DiscordMessage>>(File.ReadAllText(filename));
                        msgs.Add(e.Message);
                        File.WriteAllText(filename, JsonConvert.SerializeObject(msgs));
                    }
                    else
                    {
                        e.Client.DebugLogger.LogMessage(LogLevel.Warning, "Logger", "Unusual Case: Logging file not found after creation/check.", DateTime.Now);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        //Fake property
        public static bool? Logging(this DiscordGuild guild, bool? setValue = null)
        {
            if (setValue == null)
            {
                return (bool)guild.GetValue("logmsg", false);
            }
            else
            {
                guild.SetValue("logmsg", setValue);
                return null;
            }
        }
    }

    public partial class MainCommands : BaseCommandModule
    {
        public partial class ServerCommands : BaseCommandModule
        {
            [Command("logging")]
            public async Task Logging(CommandContext ctx)
            {
                bool enabled = ctx.Guild.Logging().Value;

                var builder = new DiscordEmbedBuilder();

                string description = string.Empty;
                description += $"{enabled.GetEmoji().ToString()} Enabled\n";
                string filename = Path.Combine(PathHelper.GuildDataDirectory, ctx.Guild.Id + ".log.json");
                if (enabled && File.Exists(filename))
                {
                    int messageCount = JsonConvert.DeserializeObject<List<DiscordMessage>>(File.ReadAllText(filename)).Count();
                    description += $"{"file-document-box".GetEmojiString()} {messageCount} messages\n";
                }

                _ = await ctx.RespondAsync(embed: builder
                    .WithAuthor($"{ctx.Guild.Name} / Logging", null, ctx.Guild.IconUrl)
                    .WithDescription(description)
                    .Build()
                    );
            }

            [Command("logging")]
            [RequirePermissions(Permissions.Administrator)]
            public async Task Logging(CommandContext ctx, bool enable)
            {
                _ = ctx.Guild.Logging(enable);
                _ = await ctx.Message.RespondAsync($"Logging has been {(enable ? "enabled" : "disabled")}.");
            }
        }
    }
}