using CraftBot.Base.Plugins;
using CraftBot.Helper;
using CraftBot.Localisation;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System;
using System.Threading.Tasks;

namespace CraftBot.Profiles
{
    [CommandClass]
    public class MainCommands : BaseCommandModule
    {
        [Group("profile")]
        [Aliases("p")]
        public class ProfileCommands : BaseCommandModule
        {
            #region Public Methods

            [Command("bio")]
            [Aliases("biography")]
            public async Task ChangeBiography(CommandContext context, params string[] text)
            {
                var builder = new MaterialEmbedBuilder(context.Client);
                var profile = new Profile(context.User)
                {
                    Biography = string.Join(" ", text)
                };

                builder.WithTitle($"{context.User.Username} / {context.GetString("Profiles_Profile")} / {context.GetString("Profiles_Biography")}", context.User.AvatarUrl)
                    .WithText(string.Format(context.GetString("Profiles_Biography_Response"), profile.Biography));

                _ = await context.RespondAsync(embed: builder.Build());
            }

            [Command("bioimg")]
            public async Task ChangeBiographyImage(CommandContext context, string imageUrl)
            {
                var builder = new MaterialEmbedBuilder(context.Client);
                var profile = new Profile(context.User)
                {
                    BiographyImage = imageUrl
                };

                builder.WithTitle($"{context.User.Username} / {context.GetString("Profiles_Profile")} / {context.GetString("Profiles_BiographyImage")}", context.User.AvatarUrl)
                    .WithText(context.GetString("Profiles_Biography_Response"))
                    .WithImage(profile.BiographyImage);

                _ = await context.RespondAsync(embed: builder.Build());
            }

            [Command("birthday")]
            public async Task ChangeBirthday(CommandContext context, int day, int month, int year = 1)
            {
                var builder = new MaterialEmbedBuilder(context.Client);
                var profile = new Profile(context.User)
                {
                    Birthday = new DateTime(year, month, day)
                };

                builder.WithTitle(context.User.Username + " / Profile / Birthday", context.User.AvatarUrl)
                    .WithText($"Your birthday has been set to {profile.Birthday.ToShortDateString()}");

                _ = await context.RespondAsync(embed: builder.Build());
            }

            [Command("gender")]
            public async Task ChangeGender(CommandContext context, string gender)
            {
                var builder = new MaterialEmbedBuilder(context.Client);
                var profile = new Profile(context.User)
                {
                    Gender = (ProfileGender)Enum.Parse(typeof(ProfileGender), gender, true)
                };

                builder.WithTitle($"{context.User.Username} / {context.GetString("Profiles_Profile")} / {context.GetString("Profiles_Gender")}", context.User.AvatarUrl)
                     .WithText(string.Format(context.GetString("Profiles_Gender_Response"), profile.Gender.ToString()));

                _ = await context.RespondAsync(embed: builder.Build());
            }

            [Command("language")]
            public async Task ChangeLanguage(CommandContext context, string languageCode)
            {
                var builder = new MaterialEmbedBuilder(context.Client);
                var profile = new Profile(context.User);
                var language = Language.GetLanguage(languageCode);

                if (language == null)
                {
                    builder.WithText(string.Format(context.GetString("Profiles_Language_NotFound"), languageCode));
                }
                else
                {
                    profile.Language = language;
                    builder.WithText(string.Format(context.GetString("Profiles_Language_Response"), profile.Language.NativeName));
                }

                builder.WithTitle($"{context.User.Username} / {context.GetString("Profiles_Profile")} / {context.GetString("Profiles_Language")}", context.User.AvatarUrl);

                _ = await context.RespondAsync(embed: builder.Build());
            }

            [Command("delete")]
            public async Task Delete(CommandContext context) => _ = await context.RespondAsync(context.GetString("Profiles_Delete_Confirm"));

            [Command("delete")]
            public async Task Delete(CommandContext context, bool confirmation)
            {
                if (confirmation)
                {
                    await context.TriggerTypingAsync();

                    _ = new Profile(context.User)
                    {
                        Biography = string.Empty,
                        BiographyImage = string.Empty,
                        Birthday = DateTime.MinValue,
                        Gender = ProfileGender.Unset,
                        Language = Language.GetDefaultLanguage()
                    };

                    await context.RespondAsync(context.GetString("Profiles_Delete_Success"));
                }
            }

            [GroupCommand]
            [Command("view")]
            public async Task View(CommandContext context, DiscordUser user = null)
            {
                if (user == null)
                {
                    user = context.User;
                }

                var builder = new MaterialEmbedBuilder(context.Client);
                var profile = new Profile(user);

                MaterialEmbedListTile genderListTile = null;
                MaterialEmbedListTile birthdayListTile = null;
                MaterialEmbedListTile ageListTile = null;
                MaterialEmbedListTile languageListTile = new MaterialEmbedListTile(
                    DiscordEmoji.FromUnicode(context.Client, "❓"),
                    context.GetString("Profiles_Language"),
                    context.GetLanguage().GetLocalisedLanguageName(profile.Language)
                );

                if (profile.Gender != ProfileGender.Unset)
                {
                    string icon = string.Empty;
                    string gender = string.Empty;

                    switch (profile.Gender)
                    {
                        case ProfileGender.Unset:  gender = context.GetString("GeneralTerms_Unset");                             break;
                        case ProfileGender.Male:   gender = context.GetString("Profiles_Gender_Male");   icon = "gender-male";   break;
                        case ProfileGender.Female: gender = context.GetString("Profiles_Gender_Female"); icon = "gender-female"; break;
                    }

                    genderListTile = new MaterialEmbedListTile(icon, context.GetString("Profiles_Gender"), gender);
                }
                if (!profile.Birthday.Equals(DateTime.MinValue))
                {
                    bool hasYearSpecified = profile.Birthday.Year != 1;

                    birthdayListTile = new MaterialEmbedListTile(
                        "blank",
                        context.GetString("Profiles_Birthday_Title"),
                        string.Format(
                            context.GetString(hasYearSpecified ? "Profiles_Birthday_Value_WithYear" : "Profiles_Birthday_Value_NoYear"),
                            profile.Birthday.Day,
                            profile.Birthday.Month,
                            profile.Birthday.Year,
                            (profile.GetNextBirthday() - DateTime.Now).Days
                        )
                    );

                    if (hasYearSpecified)
                    {
                        ageListTile = new MaterialEmbedListTile(
                            "blank",
                            context.GetString("Profiles_Age_Title"),
                            string.Format(context.GetString("Profiles_Age_Value"), profile.Age)
                        );
                    }
                }
                if (!string.IsNullOrWhiteSpace(profile.Biography))
                {
                    builder.AddSection(null, context.GetString("Profiles_Biography"), profile.Biography);
                }
                if (!string.IsNullOrWhiteSpace(profile.BiographyImage))
                {
                    builder.WithImage(profile.BiographyImage);
                }

                builder.WithTitle($"{user.Username} / {context.GetString("Profiles_Profile")}", user.AvatarUrl);
                builder.WithFooter(context.GetString("Profiles_Help"));
                builder.AddSection(null, context.GetString("GeneralTerms_General"),
                    languageListTile,
                    birthdayListTile,
                    ageListTile,
                    genderListTile
                );

                _ = await context.RespondAsync(embed: builder.Build());
            }
        }

        #endregion Public Methods
    }
}