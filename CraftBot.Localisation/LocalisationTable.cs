using CraftBot.Localisation.Exceptions;
using NLog;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CraftBot.Localisation
{
    public static class LocalisationTable
    {
        static LocalisationTable() => LoadFromWeb();

        private const char Separator = '\t';
        public static int Columns;
        public static string[,] Entries;
        public static int Rows;

        public static int GetKeyIndex(string key)
        {
            for (int i = 0; i < Rows; i++)
            {
                if (Entries[0, i] == key)
                {
                    return i;
                }
            }
            return -1;
        }

        public static string[] GetKeys()
        {
            if (IsEmpty())
            {
                LoadFromWeb().GetAwaiter().GetResult();
            }

            string[] keys = new string[Rows - 1];
            for (int i = 1; i < Rows; i++)
            {
                keys[i - 1] = Entries[0, i];
            }
            return keys;
        }

        public static int GetLanguageIndex(string shortCode)
        {
            for (int i = 2; i < Columns; i++)
            {
                if (Entries[i, 0] == shortCode)
                {
                    return i;
                }
            }
            return -1;
        }

        public static string GetString(string language, string key)
        {
            if (IsEmpty())
            {
                LoadFromWeb().GetAwaiter().GetResult();
            }

            LogManager.GetCurrentClassLogger().Debug($"Searching for '{key}' in localisation table with {Rows} rows and {Columns} columns");

            int keyIndex = GetKeyIndex(key);
            if (keyIndex == -1)
            {
                throw new KeyNotFoundException(key);
            }

            int languageIndex = GetLanguageIndex(language);
            if (languageIndex == -1)
            {
                throw new ColumnNotFoundException(language);
            }

            string value = Entries[languageIndex, keyIndex];
            if (string.IsNullOrWhiteSpace(value))
            {
                if (language == "en")
                {
                    throw new KeyNotFoundException("Following key was not translated: " + key);
                }
                else
                {
                    return Language.GetDefaultLanguage().GetString(key);
                }
            }
            else
            {
                return value;
            }

            throw new KeyNotFoundException("Following key was not found: " + key);
        }

        public static string GetString(this Language language, string key) => GetString(language.TwoLetterCode, key);

        public static bool IsEmpty() => Entries == null ? true : Entries.Length == 0 ? true : Rows == 0 || Columns == 0;

        //public static void LoadFromResource() => LoadFromString(Encoding.UTF8.GetString(Properties.Resources.CraftBot_Localisation___Strings));

        public static void LoadFromString(string input)
        {
            string[] lines = input.Split(new[] { "\r\n" }, StringSplitOptions.None);
            Rows = lines.Length;
            Columns = lines[0].Split(Separator).Length;
            Entries = new string[Columns, Rows];

            for (int column = 0; column < Columns; column++)
            {
                for (int line = 0; line < Rows; line++)
                {
                    string[] values = lines[line].Split(Separator);
                    Entries[column, line] = values[column];
                }
            }
        }

        public static async Task LoadFromWeb(string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQKxImHesPD7nz1gCvVmTStr6ul9cUH7p9qnskcoSWkNQWSGh3AwCp3INGBOwcQhecNKkNTdmmJrObU/pub?gid=1615480763&single=true&output=tsv")
        {
            using (var webClient = new WebClient())
            {
                byte[] bytes = await webClient.DownloadDataTaskAsync(url);
                string text = Encoding.UTF8.GetString(bytes);
                LoadFromString(text);
            }
        }
    }
}