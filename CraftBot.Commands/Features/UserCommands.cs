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
            public async Task EnableQuoting(CommandContext ctx, bool enable)
            {
                _ = ctx.User.QuotingEnabled(enable);
                bool enabled = ctx.User.QuotingEnabled().Value;
                _ = await ctx.RespondAsync($"Quoting has been {(enabled ? "en" : "dis")}abled");
            }

            [Command("delaydelete")]
            [Aliases("dd", "delaydeleting","delete")]
            public async Task EnableDelayDelete(CommandContext ctx, bool enable)
            {
                _ = ctx.User.DelayDeleteEnabled(enable);
                bool enabled = ctx.User.DelayDeleteEnabled().Value;
                _ = await ctx.RespondAsync($"Delay delete has been {(enabled ? "en" : "dis")}abled");
            }

            [GroupCommand]
            [Command("info")]
            public async Task Info(CommandContext context, DiscordUser user = null)
            {
                if (user == null)
                {
                    user = context.User;
                }

                bool isMember = context.Guild.Members.Any(m => m.Value.Id == user.Id);

                var builder = new MaterialEmbedBuilder(context.Client);
                builder.WithTitle($"{user.Username} / {context.GetString("GeneralTerms_Information")}", user.AvatarUrl);

                builder.AddSection(null, context.GetString("User_Info_Identification"),
                    new MaterialEmbedListTile(
                        "pencil",
                        context.GetString("User_Info_Username"),
                        user.Username
                    ),
                    new MaterialEmbedListTile(
                        "pound",
                        context.GetString("User_Info_Discriminator"),
                        user.Discriminator.ToString()
                    ),
                    new MaterialEmbedListTile(
                        "id",
                        context.GetString("User_Info_Id"),
                        user.Id.ToString()
                    )
                );

                builder.AddSection(null, context.GetString("GeneralTerms_General"),
                    new MaterialEmbedListTile(
                        "calendar-plus",
                        context.GetString("User_Info_UserSince_Title"),
                        string.Format(context.GetString("User_Info_UserSince_Value"), user.CreationTimestamp.ToString())
                    ),
                    new MaterialEmbedListTile(
                        user.IsBot,
                        context.GetString("User_Info_IsBot")
                    ),
                    user.Verified.HasValue ? new MaterialEmbedListTile(
                        user.Verified.Value,
                        context.GetString("User_Info_IsVerified")
                    ) : null
                );

                if (isMember)
                {
                    DiscordMember member = await context.Guild.GetMemberAsync(user.Id);

                    builder.AddSection(null, context.GetString("User_Info_MemberInformation"), !string.IsNullOrWhiteSpace(member.Nickname) ?
                        new MaterialEmbedListTile(
                            "pencil",
                            context.GetString("User_Info_Nickname"),
                            member.Nickname
                        ) : null,
                        new MaterialEmbedListTile(
                            "calendar-plus",
                            context.GetString("User_Info_MemberSince_Title"),
                            string.Format(context.GetString("User_Info_MemberSince_Value"), member.JoinedAt.ToString())
                        ),
                        new MaterialEmbedListTile(
                            member.IsOwner,
                            context.GetString("User_Info_IsOwner")
                        )
                    );
                }

                builder.AddSection(null, context.GetString("GeneralTerms_BotRelated"),
                    new MaterialEmbedListTile(
                        user.QuotingEnabled(),
                        context.GetString("User_Info_Quoting")
                    ),
                    new MaterialEmbedListTile(
                        user.DelayDeleteEnabled(),
                        context.GetString("User_Info_DelayDelete")
                    )
                );

                if (user.IsCurrent)
                {
                    builder.WithFooter(context.GetString("User_Info_ThatsMe"));
                }

                _ = await context.RespondAsync(embed: builder.Build());
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}