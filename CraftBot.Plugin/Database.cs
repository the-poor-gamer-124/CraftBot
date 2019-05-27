using DSharpPlus.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace CraftBot.Plugin
{
    public enum DatabaseType
    {
        /// <summary>
        /// Database of a <see cref="DiscordGuild"/>
        /// </summary>
        Guild,

        /// <summary>
        /// Database of a <see cref="DiscordChannel"/>
        /// </summary>
        Channel,

        /// <summary>
        /// Database of a <see cref="DiscordUser"/>
        /// </summary>
        User,
    }

    //TODO: Make object oriented database, make it return JObjects instead of strings/poorly serialized objects.
    public static class Database
    {
        #region Public Methods

        public static ulong[] GetChannels()
        {
            string[] files = Directory.GetFiles(@"data\channel", "*.json");
            ulong[] ids = new ulong[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                ids[i] = ulong.Parse(Path.GetFileNameWithoutExtension(files[i]));
            }
            return ids;
        }

        public static Dictionary<string, object> GetDatabase(DatabaseType type, ulong id)
        {
            string filename = null;
            switch (type)
            {
                case DatabaseType.Guild: filename = $@"data\guild\{id}.json"; break;
                case DatabaseType.Channel: filename = $@"data\channel\{id}.json"; break;
                case DatabaseType.User: filename = $@"data\user\{id}.json"; break;
            }
            return File.Exists(filename)
                ? JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(filename))
                : new Dictionary<string, object>();
        }

        public static object GetValue(this Dictionary<string, object> db, string key, object defaultvalue) => db == null || !db.ContainsKey(key) ? defaultvalue : db[key];

        public static object GetValue(DatabaseType type, ulong id, string key, object defaultValue = null) => GetValue(GetDatabase(type, id), key, defaultValue);

        public static void SetDatabase(DatabaseType type, ulong id, Dictionary<string, object> Database)
        {
            string filename = null;
            switch (type)
            {
                case DatabaseType.Guild: filename = $@"data\guild\{id}.json"; break;
                case DatabaseType.Channel: filename = $@"data\channel\{id}.json"; break;
                case DatabaseType.User: filename = $@"data\user\{id}.json"; break;
            }
            File.WriteAllText(filename, JsonConvert.SerializeObject(Database));
        }

        public static void SetValue(DatabaseType type, ulong id, string key, object value)
        {
            Dictionary<string, object> db = GetDatabase(type, id);
            if (db.ContainsKey(key))
            {
                db[key] = value;
            }
            else
            {
                db.Add(key, value);
            }
            SetDatabase(type, id, db);
        }

        #endregion Public Methods

        #region Extensions

        public static object GetValue(this DiscordGuild guild, string key, object defaultValue = null) => GetValue(DatabaseType.Guild, guild.Id, key, defaultValue);

        public static object GetValue(this DiscordUser user, string key, object defaultValue = null) => GetValue(DatabaseType.User, user.Id, key, defaultValue);

        public static object GetValue(this DiscordChannel channel, string key, object defaultValue = null) => GetValue(DatabaseType.Channel, channel.Id, key, defaultValue);

        public static void SetValue(this DiscordGuild guild, string key, object value) => SetValue(DatabaseType.Guild, guild.Id, key, value);

        public static void SetValue(this DiscordUser user, string key, object value) => SetValue(DatabaseType.User, user.Id, key, value);

        public static void SetValue(this DiscordChannel channel, string key, object value) => SetValue(DatabaseType.Channel, channel.Id, key, value);

        #endregion Extensions
    }
}