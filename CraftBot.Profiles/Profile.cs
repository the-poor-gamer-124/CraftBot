using CraftBot.Base.Plugins;
using CraftBot.Localisation;
using DSharpPlus.Entities;
using System;

namespace CraftBot.Profiles
{
    public class Profile
    {
        #region Public Constructors

        public Profile(DiscordUser user) => this.User = user;

        #endregion Public Constructors

        #region Public Properties

        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - this.Birthday.Year;
                if (this.GetLastBirthday() > DateTime.Now)
                {
                    age--;
                }
                return age;
            }
        }

        public string Biography
        {
            get => (string)this.User.GetValue("profile.bio", string.Empty);
            set => this.User.SetValue("profile.bio", value);
        }

        public string BiographyImage
        {
            get => (string)this.User.GetValue("profile.bioimg", string.Empty);
            set => this.User.SetValue("profile.bioimg", value);
        }

        public DateTime Birthday
        {
            get => DateTime.Parse((string)this.User.GetValue("profile.birthday", DateTime.MinValue.ToBinary().ToString()));
            set => this.User.SetValue("profile.birthday", value.ToBinary().ToString());
        }

        public ProfileGender Gender
        {
            get => (ProfileGender)Enum.Parse(typeof(ProfileGender), (string)this.User.GetValue("profile.gender", "Unset"));
            set => this.User.SetValue("profile.gender", value.ToString());
        }

        public Language Language
        {
            get => this.User.GetValue("profile.lang", null) is string value ? Language.GetLanguage(value) : Language.GetDefaultLanguage();
            set => this.User.SetValue("profile.lang", value.ThreeLetterCode.ToString());
        }

        public DiscordUser User { get; } = null;

        #endregion Public Properties

        #region Public Methods

        public DateTime GetLastBirthday() => this.Birthday.AddYears(DateTime.Now.Year - this.Birthday.Year);

        public DateTime GetNextBirthday()
        {
            DateTime date = this.GetLastBirthday();
            if (DateTime.Now > date)
            {
                date = date.AddYears(1);
            }
            return date;
        }

        #endregion Public Methods
    }
}