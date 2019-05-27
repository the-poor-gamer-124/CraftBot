using CraftBot.Localisation.Exceptions;
using System;
using System.Globalization;
using System.Linq;

namespace CraftBot.Localisation
{
    public class Language
    {
        #region Public Base.Program

        public static Language[] SupportedLanguages { get; } = new[]
        {
            new Language("en"),
            new Language("de"),
            new Language("fr"),
            new Language("ja"),
            new Language("pt"),
            new Language("hu"),
            new Language("ro"),
            new Language("ru"),
            new Language("es"),
            new Language("pl"),
            new Language("al", "alm","Alman","Alman"),
            new Language("le", "let","Leetspeak","Alman"),
            new Language("zl", "zal","Zalgo","̛̖͈̟̭̯̠Z͖͓͔͎̹͉ͅà̱̪̠̖̟̮l̴g̬̠͖̪o̵̲̯̙"),
            new Language("lc", "cat","Lolcat","LOLcat"),
        };

        #endregion Public Base.Program

        #region Public Constructors

        public Language(string languageCode)
        {
            if (languageCode == null)
            {
                throw new ArgumentNullException(nameof(languageCode));
            }

            var cultureInfo = new CultureInfo(languageCode);

            this.TwoLetterCode = cultureInfo.TwoLetterISOLanguageName;
            this.ThreeLetterCode = cultureInfo.ThreeLetterISOLanguageName;
            this.EnglishName = cultureInfo.EnglishName;
            this.NativeName = cultureInfo.NativeName;
        }

        public Language(CultureInfo cultureInfo)
        {
            this.TwoLetterCode = cultureInfo.TwoLetterISOLanguageName;
            this.ThreeLetterCode = cultureInfo.ThreeLetterISOLanguageName;
            this.EnglishName = cultureInfo.EnglishName;
            this.NativeName = cultureInfo.NativeName;
        }

        public Language(string twoLetterCode, string threeLetterCode, string englishName, string nativeName)
        {
            this.TwoLetterCode = twoLetterCode ?? throw new ArgumentNullException(nameof(twoLetterCode));
            this.ThreeLetterCode = threeLetterCode ?? throw new ArgumentNullException(nameof(threeLetterCode));
            this.EnglishName = englishName ?? throw new ArgumentNullException(nameof(englishName));
            this.NativeName = nativeName ?? throw new ArgumentNullException(nameof(nativeName));
        }

        #endregion Public Constructors

        #region Public Properties

        public string EnglishName { get; set; }
        public string NativeName { get; set; }
        public string ThreeLetterCode { get; set; }
        public string TwoLetterCode { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static Language GetDefaultLanguage() => GetLanguage("en");

        public static Language GetLanguage(string name) => SupportedLanguages.FirstOrDefault(l => new[] { l.TwoLetterCode, l.ThreeLetterCode, l.NativeName, l.EnglishName }.Any(i => i.Equals(name, StringComparison.OrdinalIgnoreCase)));

        public static bool HasFallback(string key) => Language.GetDefaultLanguage().HasKey(key);

        public string GetString(string key) => LocalisationTable.GetString(this.TwoLetterCode, key);

        public bool HasKey(string key)
        {
            int keyIndex = LocalisationTable.GetKeyIndex(key);
            if (keyIndex == -1)
            {
                throw new KeyNotFoundException(key);
            }

            int languageIndex = LocalisationTable.GetLanguageIndex(this.TwoLetterCode);
            if (languageIndex == -1)
            {
                throw new ColumnNotFoundException(this.TwoLetterCode);
            }

            return !string.IsNullOrWhiteSpace(LocalisationTable.Entries[languageIndex, keyIndex]);
        }

        #endregion Public Methods
    }
}