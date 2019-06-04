using CraftBot.Helper;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CraftBot.Base.Plugins
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

    public static class Database
    {
        #region Public Methods

        public static ulong[] GetChannels()
        {
            string[] files = Directory.GetFiles(PathHelper.ChannelDataDirectory, "*.json");
            ulong[] ids = new ulong[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                ids[i] = ulong.Parse(Path.GetFileNameWithoutExtension(files[i]));
            }
            return ids;
        }

        public static Dictionary<string, object> GetDatabase(DatabaseType type, ulong id)
        {
            var filename = GetFilePath(type, id);
            return File.Exists(filename)
                ? JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(filename))
                : new Dictionary<string, object>();
        }

        private static string GetFilePath(DatabaseType type, ulong id)
        {
            switch (type)
            {
                default: throw new ArgumentException(nameof(type));
                case DatabaseType.Guild: return PathHelper.CombinePath(PathHelper.GuildDataDirectory, id + ".json");
                case DatabaseType.Channel: return PathHelper.CombinePath(PathHelper.ChannelDataDirectory, id + ".json");
                case DatabaseType.User: return PathHelper.CombinePath(PathHelper.UserDataDirectory, id + ".json");
            }
        }

        public static object GetValue(this Dictionary<string, object> database, string key, object defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("message", nameof(key));
            }

            if (database == null || !database.ContainsKey(key))
            {
                return defaultValue;
            }
            else
            {
                return database[key];
            }
        }

        public static object GetValue(DatabaseType type, ulong id, string key, object defaultValue = null) => GetValue(GetDatabase(type, id), key, defaultValue);

        public static void SetDatabase(DatabaseType type, ulong id, Dictionary<string, object> Database)
        {
            var filename = GetFilePath(type, id);
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

        public static void RemoveKey(DatabaseType type, ulong id, string key)
        {
            Dictionary<string, object> db = GetDatabase(type, id);
            if (db.ContainsKey(key))
            {
                db.Remove(key);
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

        public static void RemoveKey(this DiscordGuild guild, string key) => RemoveKey(DatabaseType.Guild, guild.Id, key);

        public static void RemoveKey(this DiscordUser user, string key) => RemoveKey(DatabaseType.User, user.Id, key);

        public static void RemoveKey(this DiscordChannel channel, string key) => RemoveKey(DatabaseType.Channel, channel.Id, key);

        #endregion Extensions
    }
}