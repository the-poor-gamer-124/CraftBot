using CraftBot.Helper;
using CraftBot.Profiles;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands : BaseCommandModule
    {
        #region Public Classes

        [Group("bot")]
        public partial class BotCommands : BaseCommandModule
        {
            #region Public Methods

            [Command("info")]
            public async Task Info(CommandContext context)
            {
                var builder = new MaterialEmbedBuilder(context.Client);

                _ = builder.WithTitle(context.Client.CurrentUser.Username + " / Bot Info", context.Client.CurrentUser.AvatarUrl);

                builder.AddSection(null, "General",
                    new MaterialEmbedListTile(
                        "discord",
                        $"[{await context.GetStringAsync("Bot_Info_DSharpPlusVersion")}](https://github.com/DSharpPlus/DSharpPlus/)",
                        context.Client.VersionString),

                    new MaterialEmbedListTile(
                        "account-star",
                        await context.GetStringAsync("Bot_Info_Creator"),
                        "<@!194891941509332992>"),

                    new MaterialEmbedListTile(
                        "help-circle",
                        await context.GetStringAsync("Bot_Info_HelpCommand"),
                        $"`{context.Prefix}help`")
                );

                builder.AddSection(null, await context.GetStringAsync("Bot_Info_Statistics"),
                    new MaterialEmbedListTile(
                        "clock-fast",
                        await context.GetStringAsync("Bot_Info_Uptime"),
                        (DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()).ToConversationalString()),

                    new MaterialEmbedListTile(
                        "blank",
                        await context.GetStringAsync("Bot_Info_Commands"),
                        context.Client.GetExtension<CommandsNextExtension>().RegisteredCommands.Count.ToString()),

                    new MaterialEmbedListTile(
                        "blank",
                        await context.GetStringAsync("Bot_Info_Servers"),
                        context.Client.Guilds.Count.ToString())
                    );

                builder.WithText($"[{"github-circle".GetEmojiString()}](https://github.com/Craftplacer/CraftBot) [{"earth".GetEmojiString()}](https://craftplacer.trexion.com/projects/craftbot)");

                _ = await context.Message.RespondAsync(embed: builder.Build());
            }

            [RequireUserGroup]
            [Command("optimizedb")]
            [Description("Deletes empty database entries to improve transferring itself.")]
            public async Task OptimizeDatabase(CommandContext ctx)
            {
                int deleted = 0;
                int total = 0;

                await ctx.TriggerTypingAsync();

                foreach (string paths in new[] { PathHelper.GuildDataDirectory, PathHelper.ChannelDataDirectory, PathHelper.UserDataDirectory })
                {
                    foreach (string file in Directory.GetFiles(paths, "*.json"))
                    {
                        total++;
                        if (new FileInfo(file).Length == 2)
                        {
                            File.Delete(file);
                            deleted++;
                        }
                    }
                }

                _ = await ctx.RespondAsync($"Done, {deleted} out of {total} files have been deleted. ({deleted/total}%)");
            }

            [Description("Patches plugins either to update the bot or something else")]
            [RequireUserGroup]
            [Command("patch")]
            public async Task Patch(CommandContext ctx)
            {
                DiscordAttachment attachment;
                DiscordMessage message;

                if (ctx.Message.Attachments.Count == 0)
                {
                    ulong botOwnerId = (await ctx.Client.GetCurrentApplicationAsync()).Owner.Id;
                    message = await ctx.RespondAsync("Waiting for patch...");
                    InteractivityResult<DiscordMessage> interactivityResult = await Base.Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == botOwnerId && m.ChannelId == m.Channel.Id && m.Attachments.Count != 0);
                    attachment = interactivityResult.Result.Attachments[0];
                }
                else
                {
                    attachment = ctx.Message.Attachments[0];
                    message = await ctx.RespondAsync("Checking...");
                }

                using (var webClient = new WebClient())
                {
                    if (attachment.FileName == "patch.zip")
                    {
                        string tempFile = Path.GetTempFileName();

                        _ = await message.ModifyAsync("Downloading patch...");
                        await webClient.DownloadFileTaskAsync(attachment.Url, tempFile);

                        _ = await message.ModifyAsync("Deleting backup...");
                        foreach (string file in Directory.GetFiles("plugins", "*.old"))
                        {
                            File.Delete(file);
                        }

                        ZipArchive patch = ZipFile.Open(tempFile, ZipArchiveMode.Update);

                        _ = await message.ModifyAsync("Comparing versions...");
                        foreach (ZipArchiveEntry entry in patch.Entries)
                        {
                            string path = Path.Combine("plugins", entry.Name);
                            if (File.Exists(path))
                            {
                                File.Move(path, path + ".old");
                            }
                        }

                        _ = await message.ModifyAsync("Extracting patch...");
                        patch.ExtractToDirectory("plugins");

                        patch.Dispose();

                        _ = await message.ModifyAsync("Deleting patch...");
                        File.Delete(tempFile);
                    }
                    else if (Path.GetExtension(attachment.FileName) == ".dll")
                    {
                        string fileName = Path.GetFileNameWithoutExtension(attachment.FileName);
                        string path = Path.Combine("plugins", fileName);

                        _ = await message.ModifyAsync("Deleting backup...");
                        if (File.Exists(path + ".old"))
                        {
                            File.Delete(path + ".old");
                        }

                        _ = await message.ModifyAsync("Renaming file...");
                        if (File.Exists(path))
                        {
                            File.Move(path, path + ".old");
                        }

                        _ = await message.ModifyAsync("Downloading patch...");
                        await webClient.DownloadFileTaskAsync(attachment.Url, path);
                    }
                    else
                    {
                        _ = await message.ModifyAsync("Patch file rejected");
                        return;
                    }
                }

                await message.DeleteAsync();
                await this.Restart(ctx);
            }

            [Description("Restarts the bot")]
            [Command("restart")]
            [RequireUserGroup]
            public async Task Restart(CommandContext ctx)
            {
                DiscordMessage msg = await ctx.RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithAuthor("CraftBot / Restart", iconUrl: ctx.Client.CurrentUser.AvatarUrl)
                    .WithTitle("**Status** Restarting...")
                    .WithColor(DiscordColor.Orange));

                File.WriteAllText("restart", msg.Channel.Id.ToString() + "\r" + msg.Id.ToString());

                await ctx.Client.UpdateStatusAsync(new DiscordActivity("Restarting..."), UserStatus.DoNotDisturb);
                await ctx.Client.DisconnectAsync();

                try
                {
                    _ = Process.Start(Assembly.GetEntryAssembly().Location);
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    async Task Client_Ready(DSharpPlus.EventArgs.ReadyEventArgs e)
                    {
                        ctx.Client.Ready -= Client_Ready;
                        await (await Plugin.GetRestartStatusMessageAsync(e.Client)).ModifyAsync(embed: new DiscordEmbedBuilder()
                            .WithAuthor("CraftBot / Restart", iconUrl: e.Client.CurrentUser.AvatarUrl)
                            .WithTitle("**Status** Failed to restart")
                            .WithDescription($"```{ex.ToString()}```")
                            .WithColor(DiscordColor.IndianRed).Build());
                    }

                    ctx.Client.Ready += Client_Ready;
                    await ctx.Client.ConnectAsync();
                }
                
            }

            [Description("Stops the bot")]
            [Command("stop")]
            [Aliases("shutdown")]
            public async Task Shutdown(CommandContext context)
            {
                _ = await context.RespondAsync(embed: new DiscordEmbedBuilder().WithTitle("Shutting down..."));

                Base.Helpers.Shutdown(context.Client);
            }

            #endregion Public Methods

            [Command("exportcommands")]
            [Aliases("export")]
            [RequireUserGroup]
            public async Task ExportCommands(CommandContext context)
            {
                //var jsonResolver = new IgnorableSerializerContractResolver();
                //jsonResolver.Ignore(typeof(CommandGroup), "Parent");

                var jsonSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

                string json = JsonConvert.SerializeObject(context.CommandsNext.RegisteredCommands, jsonSettings);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    await context.RespondWithFileAsync("commands.json", stream);
                }
            }

            [Group("plugins")]
            [Aliases("plugin", "pl")]
            public partial class PluginCommands : BaseCommandModule
            {
                [Command("list")]
                [GroupCommand]
                public async Task ListPlugins(CommandContext context)
                {
                    var builder = new MaterialEmbedBuilder(context.Client);

                    _ = builder.WithTitle(context.Client.CurrentUser.Username + " / Plugins", context.Client.CurrentUser.AvatarUrl);

                    foreach (Base.Plugins.Plugin plugin in Base.PluginLoader.Plugins)
                    {
                        builder.AddListTile("puzzle", $"{plugin.Name} by {plugin.Author}");
                    }

                    _ = await context.Message.RespondAsync(embed: builder.Build());
                }
            }

            [Command("progtest")]
            public async Task ProgressBarTest(CommandContext context, double progress, int length) => _ = await context.RespondAsync(progress.MakeProgressBar(context.Client, length));
        }

        #endregion Public Classes
    }
}