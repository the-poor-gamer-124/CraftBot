using CraftBot.Commands.Features.Server;
using CraftBot.Helper;
using CraftBot.Profiles;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands : BaseCommandModule
    {
        [RequireGuild]
        [Group("server")]
        public partial class ServerCommands : BaseCommandModule
        {
            [RequirePermissions(Permissions.ReadMessageHistory)]
            [Command("archive")]
            [RequireGuild]
            public async Task Archive(CommandContext ctx, string exclude = null)
            {
                if (!(ctx.User.Id == ctx.Guild.Owner.Id || ctx.User.Id == 194891941509332992))
                {
                    _ = await ctx.RespondAsync("You have to be the server owner to do that");
                    return;
                }
                _ = Directory.CreateDirectory(ctx.Guild.Id.ToString());
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder().WithTitle($"0% Archiving begins...");
                DiscordMessage statusMessage = await ctx.Message.RespondAsync(embed: embed.Build());
                IEnumerable<DiscordChannel> channels = ctx.Guild.Channels.Values;
                int progress = 0;
                foreach (DiscordChannel c in channels)
                {
                    progress++;
                    if (c.Type != ChannelType.Text)
                    {
                        continue;
                    }
                    if (c.Name == exclude)
                    {
                        continue;
                    }
                    try
                    {
                        int messageCount = 0;
                        double percentage = (double)((double)progress / (double)channels.Count()) * 100;
                        _ = await statusMessage.ModifyAsync(embed: embed.WithTitle($"{Math.Round(percentage)}% Archiving #{c.Name}").Build());
                        IReadOnlyList<DiscordMessage> lastMessage = await c.GetMessagesAsync(1);
                        if (lastMessage.Count == 0)
                        {
                            continue;
                        }
                        IReadOnlyList<DiscordMessage> messages = await c.GetMessagesBeforeAsync(lastMessage[0].Id);
                        while (messages.Count != 0)
                        {
                            foreach (DiscordMessage msg in messages)
                            {
                                File.AppendAllText(ctx.Guild.Id.ToString() + "\\" + c.Name + ".txt", $"[{msg.CreationTimestamp.ToString()}] {msg.Author.Username}#{msg.Author.Discriminator}: {msg.Content}\r\n");
                            }

                            messageCount += messages.Count;
                            _ = await statusMessage.ModifyAsync(embed: embed.WithDescription(messageCount + " messages archived").Build());
                            messages = await c.GetMessagesBeforeAsync(messages.Last().Id);
                        }
                    }
                    catch { }
                }
            }

            [Command("emojis")]
            [Aliases("emoji")]
            [RequireGuild]
            public async Task Emojis(CommandContext ctx)
            {
                string response = "";
                foreach (DiscordEmoji emoji in ctx.Guild.Emojis.Values)
                {
                    response += $"{(emoji.IsAnimated ? "A" : "N")} {emoji.Name} ({emoji.Id})\n";
                }
                _ = await ctx.RespondAsync(response);
            }

            [Command("features")]
            [RequireGuild]
            public async Task Features(CommandContext ctx)
            {
                _ = await ctx.RespondAsync(embed: new DiscordEmbedBuilder()
                   .WithAuthor($"{ctx.Guild.Name} / Features", null, ctx.Guild.IconUrl)
                   .WithDescription(ctx.Guild.Features.Count == 0 ? "This server has no features" : string.Join("\n", ctx.Guild.Features))
                   );
            }

            [GroupCommand]
            [Command("info")]
            [RequireGuild]
            public async Task Info(CommandContext context)
            {
                MaterialEmbedBuilder builder = new MaterialEmbedBuilder(context.Client).WithTitle($"{context.Guild.Name} / {context.GetString("GeneralTerms_Information")}", context.Guild.IconUrl);

                builder.AddSection(null, context.GetString("GeneralTerms_General"),
                    new MaterialEmbedListTile(
                        context.Guild.IsLarge ? "account-group" : "account-multiple",
                        context.GetString("Server_Info_Members_Title"),
                        string.Format(context.GetString("Server_Info_Members_Value"), context.Guild.MemberCount.ToString())),

                    new MaterialEmbedListTile(
                        "calendar-plus",
                        context.GetString("Server_Info_CreatedOn_Title"),
                        string.Format(context.GetString("Server_Info_CreatedOn_Value"), context.Guild.CreationTimestamp.ToString()))
                    );

                builder.AddSection(null, context.GetString("GeneralTerms_BotRelated"),
                    new MaterialEmbedListTile(
                        "earth",
                        context.GetString("Server_Info_Region"),
                        context.Guild.VoiceRegion.Name),

                    new MaterialEmbedListTile(
                        context.Guild.VoiceRegion.IsVIP,
                        "VIP")
                    );

                //string logFile = $"data\\guild\\{context.Guild.Id}.log.json";
                //int loggedMessages = context.Guild.Logging().Value && File.Exists(logFile) ? JsonConvert.DeserializeObject<List<DiscordMessage>>(File.ReadAllText(logFile)).Count() : 0;

                builder.AddSection(null, context.GetString("GeneralTerms_BotRelated"),
                    new MaterialEmbedListTile(
                        context.Guild.AAP().Value,
                        context.GetString("Server_Info_AAP")
                    ),
                    new MaterialEmbedListTile(
                        context.Guild.ShitpostingEnabled().Value,
                        context.GetString("Server_Info_Shitposting")
                    )
                );

                _ = await context.RespondAsync(embed: builder.Build());
            }
        }
    }
}