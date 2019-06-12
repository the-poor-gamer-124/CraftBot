using CraftBot.Base;
using CraftBot.Base.Plugins;
using CraftBot.Commands.Features;
using CraftBot.Commands.Features.Protection;
using CraftBot.Commands.Features.Server;
using CraftBot.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CraftBot.Commands
{
    public class Plugin : Base.Plugins.Plugin
    {
        public Plugin()
        {
            this.PluginRegistered += (s, ea) =>
            {
                Program.Client.MessageCreated += Quoting.DiscordClient_MessageCreated;
                Program.Client.MessageCreated += Shitposting.DiscordClient_MessageCreated;
                Program.Client.MessageCreated += DelayDelete.DiscordClient_MessageCreated;
                Program.Client.MessageCreated += SexBotProtection.MessageCreated;
                Program.Client.Ready += async (e) =>
                {
                    #region Bot Emojis

                    await Task.Run(
                        async delegate
                        {
                            LogManager.GetCurrentClassLogger().Info("Retrieving bot emojis...");

                            try
                            {
                                DiscordGuild guild = await e.Client.GetGuildAsync(356532339020660749);
                                foreach (DiscordEmoji emoji in await guild.GetEmojisAsync())
                                {
                                    CustomEmoji.EmojiDictionary[emoji.Name] = emoji.Id;
                                }
                                LogManager.GetCurrentClassLogger().Info("Finished setting up emojis");
                            }
                            catch (Exception ex)
                            {
                                LogManager.GetCurrentClassLogger().Error(ex, "Failed to retrieve the emojis");
                            }
                        });

                    #endregion Bot Emojis

                    if (File.Exists("restart"))
                    {
                        await (await GetRestartStatusMessageAsync(e.Client)).ModifyAsync(embed: new DiscordEmbedBuilder()
                            .WithAuthor("CraftBot / Restart", iconUrl: e.Client.CurrentUser.AvatarUrl)
                            .WithTitle("**Status** Restarted")
                            .WithColor(DiscordColor.Green).Build());
                    }
                }; //Bot Emojis
            };
        }

        public static async Task<DiscordMessage> GetRestartStatusMessageAsync(DiscordClient client)
        {
            string[] lines = File.ReadAllLines("restart");

            DiscordChannel channel = await client.GetChannelAsync(ulong.Parse(lines[0]));
            DiscordMessage message = await channel.GetMessageAsync(ulong.Parse(lines[1]));

            File.Delete("restart");

            return message;
        }

        public static Random Random { get; } = new Random();
        public static WebClient WebClient { get; } = new WebClient();
        public override string Author => "Craftplacer";
        public override string Name => "Base";
    }
}