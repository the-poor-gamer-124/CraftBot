using CraftBot.Base.Plugins;
using CraftBot.ServerSpecial.MessengerGeek;

using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using NLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace CraftBot.ServerSpecial
{
    public static class MessengerGeekFeed
    {
        private static List<int> AlreadySentTopics;

        public const ulong ChannelId = 574813572182573067;

        //                               ms   | sec | min
        public const int CheckInterval = 1000 * 060 * 015;

        public static Timer CheckTimer = new Timer(CheckInterval) { AutoReset = false };
        public static DiscordChannel FeedChannel;
        public static DiscordWebhook Webhook;

        static MessengerGeekFeed() => CheckTimer.Elapsed += CheckTimer_Elapsed;

        private static async void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogManager.GetCurrentClassLogger().Info("Pulling newest topics from MessengerGeek forum...");
            try
            {
                List<Topic> topics = await API.GetLatestTopics();
                List<Topic> newTopics = topics.FindAll(t => !AlreadySentTopics.Contains(t.Id) && !t.Pinned);

                LogManager.GetCurrentClassLogger().Info($"Pulled {newTopics.Count} new topic(s)");

                var embeds = new List<DiscordEmbed>();

                foreach (Topic topic in newTopics)
                {
                    try
                    {
                        embeds.Add(await topic.ToEmbedAsync());
                        AlreadySentTopics.Add(topic.Id);
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetCurrentClassLogger().Error(ex, "Failed to send message of topic");
                    }
                }

                if (embeds.Count != 0)
                {
                    await Webhook.ExecuteAsync(null, "MessengerGeek", "https://i.imgur.com/qCPt6J4.png", false, embeds, null);
                }
            }
            catch (WebException ex)
            {
                _ = await FeedChannel.SendMessageAsync(embed: new DiscordEmbedBuilder()
                    .WithTitle("Failed to pull topics")
                    .WithDescription($"API seems to be down, detailed information here: \n```{ex.GetType().ToString()}: {ex.Message}```"));
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Failed pulling topics from MessengerGeek");
            }

            await FeedChannel.SetAsync("sentTopics", string.Join(",", AlreadySentTopics));
            CheckTimer.Start();
        }

        public static async Task DiscordClient_Ready(ReadyEventArgs e)
        {
            FeedChannel = await Base.Program.Client.GetChannelAsync(ChannelId);

            Webhook = await e.Client.GetWebhookAsync(574815359597805578);

            string values = await FeedChannel.GetAsync<string>("sentTopics", null);
            AlreadySentTopics = values == null ? new List<int>() : Array.ConvertAll(values.Split(','), s => int.Parse(s)).ToList();

            CheckTimer_Elapsed(null, null);
        }

        public static async Task<DiscordEmbed> ToEmbedAsync(this Topic topic)
        {
            Post post = await topic.GetPost(0);

            string processedDescription = post.Uncooked.Length > 2048 ? post.Uncooked.Truncate(2045) + "..." : post.Uncooked;

            int authorId = topic.Posters.First(p => p.Description.Contains("Original Poster")).UserId;
            User author = API.LastResponse.Users.First(u => u.Id == authorId);

            return new DiscordEmbedBuilder()
            {
                Author = new EmbedAuthor()
                {
                    Name = author.GetName(),
                    IconUrl = author.GetAvatarUrl(64),
                    Url = $"https://wink.messengergeek.com/u/{author.Username}/",
                },
                Footer = new EmbedFooter()
                {
                    IconUrl = "https://i.imgur.com/qCPt6J4.png",
                    Text = $"MessengerGeek • {topic.Views} views • {topic.LikeCount} likes • {topic.ReplyCount} replies",
                },
                //----
                Title = topic.Title,
                Description = processedDescription,
                //----
                Timestamp = topic.CreatedAt,
                Color = new DiscordColor("#FEE44A"),
                ImageUrl = topic.ImageUrl,
                Url = "https://wink.messengergeek.com/t/" + topic.Id
            }.Build();
        }

        public static string Truncate(this string value, int maxLength) => string.IsNullOrEmpty(value) ? value : value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }
}