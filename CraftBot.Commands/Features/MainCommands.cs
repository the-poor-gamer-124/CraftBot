using CraftBot.Base.Plugins;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using System.Threading.Tasks;
using CraftBot.Profiles;

namespace CraftBot.Commands.Features
{
    [CommandClass]
    public partial class MainCommands : BaseCommandModule
    {
        #region Public Methods

        [Command("areyouthere")]
        [Aliases("ayt")]
        [Description("It's a simple \"hello world\" command, used to check if the bot is operating/online.")]
        public async Task AreYouThere(CommandContext context) => await context.RespondAsync(string.Format(context.GetStringAsync("AreYouThere_Message"), context.User.Mention));

        [CraftBot.Helper.RequireUserGroup]
        [Command("repeat")]
        [Description("Repeats back the raw message in a code block.")]
        public async Task Repeat(CommandContext ctx, string text) => await ctx.RespondAsync($"```{text}```");

        #endregion Public Methods
    }
}