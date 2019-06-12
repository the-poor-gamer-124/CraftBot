using CraftBot.Base.Plugins;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using System;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands : BaseCommandModule
    {
        public partial class ServerCommands : BaseCommandModule
        {
            [Command("shitposting")]
            [RequirePermissions(Permissions.Administrator)]
            [RequireGuild]
            public async Task Shitposting(CommandContext ctx, bool enable)
            {
                await ctx.Guild.SetShitpostingAsync(enable);
                await ctx.Message.RespondAsync($"Shitposting has been {(enable ? "enabled" : "disabled")}.");
            }
        }
    }

    public static class Shitposting
    {
        //Fake property
        public static async Task<bool> IsShitpostingEnabledAsync(this DiscordGuild guild) => await guild.GetAsync<bool>("shitposting");

        public static async Task SetShitpostingAsync(this DiscordGuild guild, bool enable) => await guild.SetAsync("shitposting", enable);

        public static async Task DiscordClient_MessageCreated(MessageCreateEventArgs e)
        {
            if (await e.Guild.IsShitpostingEnabledAsync())
            {
                if (e.Message.Content.Contains("loli", StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] shitposts = new string[]
                    {
                        "https://cdn.discordapp.com/attachments/453373657817939969/535908602968539137/image0.png",
                        "https://cdn.discordapp.com/attachments/453373657817939969/507930284013125642/41863471_310676196149309_6969784159564726272_n.png",
                        "https://cdn.discordapp.com/attachments/453373657817939969/494548316131164170/42660470_2186564228298815_4197030247671005184_n.png",
                        "https://cdn.discordapp.com/attachments/453373657817939969/492310663843282955/42186213_699752817090112_4746836402870681600_n.png"
                    };

                    await e.Message.RespondAsync(shitposts[Plugin.Random.Next(shitposts.Length) - 1]);
                }
            }
        }
    }
}
