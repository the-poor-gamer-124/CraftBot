using CraftBot.Base;
using CraftBot.Base.Plugins;

using CraftBot.Helper;
using CraftBot.Profiles;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands : BaseCommandModule
    {
        public partial class BotCommands : BaseCommandModule
        {
            [Group("flags")]
            public class DebugFlagCommands : BaseCommandModule
            {
                [RequireUserGroup]
                [Command("get")]
                public async Task Get(CommandContext context, string flag)
                {
                    bool value = Program.Flags[flag];
                    await context.RespondAsync(context.GetStringAsync("Bot_DebugFlags_GetMessage", flag, value.ToString()));
                }

                [RequireUserGroup]
                [Command("set")]
                public async Task Set(CommandContext context, string flag, bool value)
                {
                    Program.Flags[flag] = value;
                    await context.RespondAsync(context.GetStringAsync("Bot_DebugFlags_SetMessage", flag, value.ToString()));
                }
            }
        }
    }
}