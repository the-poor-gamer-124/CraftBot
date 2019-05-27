using CraftBot.Helper;

using DSharpPlus;
using DSharpPlus.Entities;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CraftBot.Base
{
    public static class Helpers
    {
        public static Random random = new Random();

        public static void CheckDirectory(string Directory)
        {
            if (!System.IO.Directory.Exists(Directory))
            {
                _ = System.IO.Directory.CreateDirectory(Directory);
                LogManager.GetCurrentClassLogger().Warn($"Missing folder '{Directory}' has been created");
            }
        }

        public static string HandleException(Exception e)
        {
            LogManager.GetCurrentClassLogger().Error(e, "Got exception");

            string id = RandomString(6) + ".txt";
            if (e.InnerException != null)
            {
                _ = HandleException(e.InnerException);
            }
            try
            {
                File.WriteAllText(Path.Combine(PathHelper.ErrorReportsDirectory, id), e.ToString());
                return id;
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Failed to save report");
            }
            return null;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static async void Shutdown(DiscordClient client)
        {
            LogManager.GetCurrentClassLogger().Info("Shutting down");
            await client.UpdateStatusAsync(new DiscordActivity("Shutting Down..."), UserStatus.DoNotDisturb);
            await client.DisconnectAsync();
            Environment.Exit(-1);
        }

        public static async Task UploadGuildCount(string token, int guildCount)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    LogManager.GetCurrentClassLogger().Info("Sending guild count to DiscordBots.org...");
                    using (var webclient = new HttpClient())
                    using (var content = new StringContent($"{{ \"server_count\": {guildCount}}}", Encoding.UTF8, "application/json"))
                    {
                        webclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
                        HttpResponseMessage response = await webclient.PostAsync("https://discordbots.org/api/bots/194893681600233472/stats", content);
                    }
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Warn("DiscordBots.org API token is null or empty");
                }
            }
            catch (Exception ex)
            {
                _ = HandleException(ex);
            }
        }
    }
}