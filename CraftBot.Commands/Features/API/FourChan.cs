using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

using FChan.Library;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using static CraftBot.Commands.Helpers;

namespace CraftBot.Commands.Features
{
    public static class FourChan
    {
        #region Public Methods

        public static DiscordEmbed ToDiscordEmbed(Post post) => ToDiscordEmbedBuilder(post).Build();

        public static DiscordEmbedBuilder ToDiscordEmbedBuilder(Post post)
        {
            var embed = new DiscordEmbedBuilder();
            embed = embed.WithColor(new DiscordColor("#66cc33"));
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor()
            {
                Name = post.Name + (string.IsNullOrWhiteSpace(post.Capcode) ? string.Empty : $" ({post.Capcode})"),
                IconUrl = string.IsNullOrWhiteSpace(post.Capcode) ? null : $"https://s.4cdn.org/image/{post.Capcode}icon.gif",
            };
            embed.Title = post.Subject;
            if (!string.IsNullOrWhiteSpace(post.Comment))
            {
                string desc = post.Comment;
                using (var myWriter = new StringWriter())
                {
                    HttpUtility.HtmlDecode(desc, myWriter);
                    desc = myWriter.ToString();
                }
                desc = desc.Replace("<br>", "\n");
                desc = Regex.Replace(desc, @"<[^>]+>", "").Trim();
                desc = Regex.Replace(desc, @"\s{2,}", " ");
                embed.Description = desc;
            }
            if (post.HasImage)
            {
                string url = Constants.GetImageUrl(post.Board, post.FileName, post.FileExtension);
                if (!string.IsNullOrWhiteSpace(post.Comment))
                {
                    embed.ThumbnailUrl = url;
                }
                else
                {
                    embed.ImageUrl = url;
                }
            }

            embed.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = $"4chan • /{post.Board}/ • {post.PostNumber}"
                     + (post.Replies.HasValue && post.Replies.Value > 0 ? $" • {post.Replies.Value} replies" : string.Empty)
                     + (post.Images.HasValue && post.Images.Value > 0 ? $" • {post.Images.Value} images" : string.Empty),
                IconUrl = "https://4chan.org/favicon.ico"
            };

            //var dateTimeOffset = new DateTimeOffset(new DateTime(2015, 05, 24, 10, 2, 0, DateTimeKind.Local));
            embed = embed.WithTimestamp(UnixTimeStampToDateTime(post.UnixTimestamp));

            return embed;
        }

        #endregion Public Methods
    }

    public partial class MainCommands : BaseCommandModule
    {
        #region Public Classes

        [Group("4chan")]
        public class FourChanCommands : BaseCommandModule
        {
            #region Public Methods

            [RequireNsfw]
            [Aliases("t")]
            [Command("thread")]
            public async Task GetThread(CommandContext ctx, string board, int number)
            {
                await ctx.TriggerTypingAsync();
                Thread thread = Chan.GetThread(board, number);
                var pages = new List<Page>();
                foreach (Post post in thread.Posts)
                {
                    pages.Add(new Page(embed: FourChan.ToDiscordEmbedBuilder(post)));
                }
                await Base.Program.Interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
            }

            [RequireNsfw]
            [Aliases("rt")]
            [Command("randomthread")]
            public async Task RandomThread(CommandContext ctx, string boardName = null)
            {
                await ctx.TriggerTypingAsync();
                BoardRootObject boardRootObject = await Chan.GetBoardAsync();
                Board board;

                if (!string.IsNullOrWhiteSpace(boardName))
                {
                    int index = boardRootObject.Boards.FindIndex(b => b.BoardName == boardName.ToLower());
                    if (index == -1)
                    {
                        _ = await ctx.RespondAsync($"Couldn't find board /{boardName.ToLower()}/!");
                        return;
                    }
                    else
                    {
                        board = boardRootObject.Boards[index];
                    }
                }
                else
                {
                    board = Helper.RandomItem.GetRandomItem(boardRootObject.Boards);
                }

                var posts = new List<Post>();
                for (int i = 1; i < board.Pages; i++)
                {
                    ThreadRootObject page = await Chan.GetThreadPageAsync(board.BoardName, i);
                    foreach (Thread thread in page.Threads)
                    {
                        posts.Add(thread.Posts[0]);
                    }
                }

                Post post = posts[Plugin.Random.Next(0, posts.Count - 1)];
                await this.GetThread(ctx, post.Board, post.PostNumber);
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}