using CraftBot.Base.Plugins;
using CraftBot.Helper;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;

using Newtonsoft.Json;

using NLog;
using NLog.Config;
using NLog.Targets;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using static CraftBot.Base.Helpers;

using LogLevel = NLog.LogLevel;

namespace CraftBot.Base
{
    public static partial class Program
    {
        public static DiscordClient Client;
        public static CommandsNextExtension Commands;
        public static Config Config;
        public static DebugFlags Flags = new DebugFlags();
        public static InteractivityExtension Interactivity;

        private static void CheckRequiredDirectories()
        {
            foreach (string directory in new[] {
                "data",
                Path.Combine("data", "user"),
                Path.Combine("data", "guild"),
                Path.Combine("data", "channel"),
                "error",
                "plugins"
            })
            {
                CheckDirectory(directory);
            }
        }

        private static void LoadConfig()
        {
            string configPath = Path.Combine("data", "config.json");

            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(new Config()));
                Environment.Exit(-1);
                return;
            }

            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
        }

        private static async Task RunAsync()
        {
            LogManager.GetCurrentClassLogger().Info("Connecting");
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static void SetupEventHandlers()
        {
            Commands.CommandErrored += (e) =>
            {
                if (e.Exception.GetType() == typeof(CommandNotFoundException))
                {
                    return e.Context.RespondAsync(embed: new DiscordEmbedBuilder()
                        .WithTitle($"{"help-circle".GetEmojiString(Client)} Unknown command")
                        .WithDescription("Try `help` to find correct syntax.")
                        .WithColor(new DiscordColor("#536DFE"))
                    );
                }
                else if (e.Exception is ChecksFailedException checkException)
                {
                    string failedList = "";

                    foreach (CheckBaseAttribute check in checkException.Command.ExecutionChecks)
                    {
                        failedList += checkException.FailedChecks.Contains(check) ? "-" : "+";
                        failedList += " " + check.ToDescription() + "\n";
                    }

                    string id = HandleException(e.Exception);
                    return e.Context.RespondAsync(embed: new DiscordEmbedBuilder()
                        .WithTitle($"{"alert-circle".GetEmojiString(Client)} Checks on command failed")
                        .WithDescription($"CraftBot refused to continue running the command because it has failed the following checks:\n\n```diff\n{failedList}```")
                        .AddField("Command", e.Command.Name, true)
                        .WithColor(new DiscordColor("#FF5252"))
                    );
                }
                else
                {
                    string id = HandleException(e.Exception);
                    var builder = new DiscordEmbedBuilder();
                    builder.WithTitle($"{"alert-circle".GetEmojiString(Client)} CraftBot encountered an error");
                    builder.AddField("Exception Type", e.Exception.GetType().ToString(), false);
                    builder.AddField("Exception Message", e.Exception.Message, false);
                    builder.AddField("Command", e.Command.Name, true);
                    if (!string.IsNullOrWhiteSpace(e.Exception.Source))
                    {
                        builder.AddField("Source", e.Exception.Source, true);
                    }
                    builder.AddField("Id", Path.GetFileNameWithoutExtension(id), true);
                    builder.WithColor(new DiscordColor("#FF5252"));
                    return e.Context.RespondAsync(embed: builder);
                }
            };

            Client.Ready += async (e) =>
            {
                LogManager.GetCurrentClassLogger().Info("Ready");
                await e.Client.UpdateStatusAsync(new DiscordActivity(Debugger.IsAttached ? "Debugging" : null), UserStatus.Online);
            };
            Client.GuildCreated += async (e) => await UploadGuildCount(Config.Tokens["discordbotsorg"], Client.Guilds.Count);
            Client.GuildDeleted += async (e) => await UploadGuildCount(Config.Tokens["discordbotsorg"], Client.Guilds.Count);
            Client.ClientErrored += async (e) => LogManager.GetCurrentClassLogger().Error(e.Exception, $"Client threw exception at event {e.EventName}");
            Client.DebugLogger.LogMessageReceived += (s, e) => LogDSharpPlusLogMessage(e);
        }

        public static void LogDSharpPlusLogMessage(DebugLogMessageEventArgs e)
        {
            LogLevel logLevel = LogLevel.Off;
            switch (e.Level)
            {
                case DSharpPlus.LogLevel.Debug: logLevel = LogLevel.Debug; break;
                case DSharpPlus.LogLevel.Info: logLevel = LogLevel.Info; break;
                case DSharpPlus.LogLevel.Warning: logLevel = LogLevel.Warn; break;
                case DSharpPlus.LogLevel.Error: logLevel = LogLevel.Error; break;
                case DSharpPlus.LogLevel.Critical: logLevel = LogLevel.Fatal; break;
            }

            LogManager.GetLogger(e.Application).Log(logLevel, e.Exception, e.Message);
        }

        private static void SetupLogging()
        {
            var logConfig = new LoggingConfiguration();
            var logLayout = new BetterLayout();

            logConfig.AddTarget(new FileTarget("file")
            {
                CreateDirs = true,
                FileName = "CraftBot.log",
                KeepFileOpen = true,
                AutoFlush = true,
                Layout = logLayout,
            });

            logConfig.AddTarget(new ColoredConsoleTarget("console")
            {
                DetectConsoleAvailable = true,
                ErrorStream = true,
                OptimizeBufferReuse = true,
                UseDefaultRowHighlightingRules = true,
                Layout = logLayout
            });

            foreach (Target target in logConfig.AllTargets)
            {
                logConfig.AddRuleForAllLevels(target);
            }

            LogManager.Configuration = logConfig;
        }

        public static void Main()
        {
            Console.Title = "CraftBot";

            SetupLogging();

            LogManager.GetCurrentClassLogger().Info("Starting");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            LogManager.GetCurrentClassLogger().Info($"Set security protocol to {ServicePointManager.SecurityProtocol.ToString()}");

            CheckRequiredDirectories();
            LoadConfig();


            Client = new DiscordClient(new DiscordConfiguration()
            {
                AutoReconnect = true,
                LogLevel = DSharpPlus.LogLevel.Debug,
                Token = Config.Tokens["discord"],
                TokenType = TokenType.Bot,
                UseInternalLogHandler = false,
                WebSocketClientFactory = CompatibleWebSocket.GetWebSocketClient(),
            });

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = Config.Triggers
            });

            Interactivity = Client.UseInteractivity(new InteractivityConfiguration());

            SetupEventHandlers();

            PluginLoader.LoadPlugins();
            PluginLoader.RegisterPlugins();

            RunAsync().GetAwaiter().GetResult();

            _ = Console.ReadLine();
        }
    }
}