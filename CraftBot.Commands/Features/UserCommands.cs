using CraftBot.Helper;
using CraftBot.Profiles;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.Linq;
using System.Threading.Tasks;

namespace CraftBot.Commands.Features
{
    public partial class MainCommands : BaseCommandModule
    {
        #region Public Classes

        [Group("user")]
        public class UserCommands : BaseCommandModule
        {
            #region Public Methods

            [Command("quoting")]
            public async Task EnableQuoting(CommandContext context, bool enable)
            {
                await context.User.SetQuotingAsync(enable);

                bool enabled = await context.User.IsQuotingEnabledAsync();

                await context.RespondAsync($"Quoting has been {(enabled ? "en" : "dis")}abled");
            }

            [Command("delaydelete")]
            [Aliases("dd", "delaydeleting","delete")]
            public async Task EnableDelayDelete(CommandContext context, bool enable)
            {
                await context.User.SetDelayDeletingAsync(enable);

                bool enabled = await context.User.GetDelayDeletingAsync();

                await context.RespondAsync($"Delay delete has been {(enabled ? "en" : "dis")}abled");
            }

            [GroupCommand]
            [Command("info")]
            public async Task Info(CommandContext context, DiscordUser user = null)
            {
                if (user == null)
                {
                    user = context.User;
                }

                var builder = new MaterialEmbedBuilder(context.Client);
                builder.WithTitle($"{user.Username} / {await context.GetStringAsync("GeneralTerms_Information")}", user.AvatarUrl);

                builder.AddSection(null, await context.GetStringAsync("User_Info_Identification"),
                    new MaterialEmbedListTile(
                        "pencil",
                        await context.GetStringAsync("User_Info_Username"),
                        user.Username
                    ),
                    new MaterialEmbedListTile(
                        "pound",
                        await context.GetStringAsync("User_Info_Discriminator"),
                        user.Discriminator.ToString()
                    ),
                    new MaterialEmbedListTile(
                        "id",
                        await context.GetStringAsync("User_Info_Id"),
                        user.Id.ToString()
                    )
                );

                builder.AddSection(null, await context.GetStringAsync("GeneralTerms_General"),
                    new MaterialEmbedListTile(
                        "calendar-plus",
                        await context.GetStringAsync("User_Info_UserSince_Title"),
                        string.Format(await context.GetStringAsync("User_Info_UserSince_Value"), user.CreationTimestamp.ToString())
                    ),
                    new MaterialEmbedListTile(
                        user.IsBot,
                        await context.GetStringAsync("User_Info_IsBot")
                    ),
                    user.Verified.HasValue ? new MaterialEmbedListTile(
                        user.Verified.Value,
                        await context.GetStringAsync("User_Info_IsVerified")
                    ) : null
                );

                bool isMember = context.Guild.Members.Any(m => m.Value.Id == user.Id);
                if (isMember)
                {
                    DiscordMember member = await context.Guild.GetMemberAsync(user.Id);

                    builder.AddSection(null, await context.GetStringAsync("User_Info_MemberInformation"), !string.IsNullOrWhiteSpace(member.Nickname) ?
                        new MaterialEmbedListTile(
                            "pencil",
                            await context.GetStringAsync("User_Info_Nickname"),
                            member.Nickname
                        ) : null,
                        new MaterialEmbedListTile(
                            "calendar-plus",
                            await context.GetStringAsync("User_Info_MemberSince_Title"),
                            string.Format(await context.GetStringAsync("User_Info_MemberSince_Value"), member.JoinedAt.ToString())
                        ),
                        new MaterialEmbedListTile(
                            member.IsOwner,
                            await context.GetStringAsync("User_Info_IsOwner")
                        )
                    );
                }

                builder.AddSection(null, await context.GetStringAsync("GeneralTerms_BotRelated"),
                    new MaterialEmbedListTile(
                        await user.IsQuotingEnabledAsync(),
                        await context.GetStringAsync("User_Info_Quoting")
                    ),
                    new MaterialEmbedListTile(
                        await user.GetDelayDeletingAsync(),
                        await context.GetStringAsync("User_Info_DelayDelete")
                    )
                );

                if (user.IsCurrent)
                {
                    builder.WithFooter(await context.GetStringAsync("User_Info_ThatsMe"));
                }

                await context.RespondAsync(embed: builder.Build());
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}