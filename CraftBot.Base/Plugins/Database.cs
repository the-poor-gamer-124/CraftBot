using DSharpPlus.Entities;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CraftBot.Base.Plugins
{
    struct DatabaseIdentifier
    {
        public readonly string Group;
        public readonly ulong Id;

        public DatabaseIdentifier(string group, ulong id)
        {
            Group = group ?? throw new ArgumentNullException(nameof(group));
            Id = id;
        }
    }

    public class Database : IAsyncDisposable
    {
        private List<Tuple<DatabaseIdentifier, JObject>> OpenedDatabases { get; } = new List<Tuple<DatabaseIdentifier, JObject>>();

        public async ValueTask DisposeAsync()
        {
            for (int i = OpenedDatabases.Count - 1; i >= 0; i--)
            {
                Tuple<DatabaseIdentifier, JObject> tuple = OpenedDatabases[i];

                string path = GetPath(tuple.Item1);
                string json = JsonConvert.SerializeObject(tuple.Item2);

                await File.WriteAllTextAsync(path, json);

                OpenedDatabases.RemoveAt(i);
            }
        }

        public async Task SetAsync(string group, ulong id, string key, object value)
        {
            JObject database = await GetDatabaseAsync(group, id);
            database[key] = JToken.FromObject(value);
        }

        public async Task<T> GetAsync<T>(string group, ulong id, string key)
        {
            JObject database = await GetDatabaseAsync(group, id);

            if (database.TryGetValue(key, out JToken value))
            {
                return value.ToObject<T>();
            }
            else
            {
                throw new KeyNotFoundException($"Key {key} wasn't found.");
            }
        }

        public async Task<T> GetAsync<T>(string group, ulong id, string key, T @default)
        {
            try
            {
                return await GetAsync<T>(group, id, key);
            }
            catch (KeyNotFoundException)
            {
                return @default;
            }
        }

        public bool IsOpen(string group, ulong id) => this.OpenedDatabases.Any(t => t.Item1.Group == group && t.Item1.Id == id);

        private string GetPath(DatabaseIdentifier identifier) => GetPath(identifier.Group, identifier.Id);

        public string GetPath(string group, ulong id) => Path.Combine("data", group.ToLower(), id + ".json");

        public async Task OpenDatabaseAsync(string group, ulong id)
        {
            var identifier = new DatabaseIdentifier(group, id);
            string path = GetPath(identifier);
            string json = await File.ReadAllTextAsync(path);

            var jArray = (JObject)JsonConvert.DeserializeObject(json);

            OpenedDatabases.Add(Tuple.Create(identifier, jArray));
        }

        public async Task<JObject> GetDatabaseAsync(string group, ulong id)
        {
            if (!IsOpen(group, id))
            {
                await OpenDatabaseAsync(group, id);
            }

            return this.OpenedDatabases.First(t => t.Item1.Group == group && t.Item1.Id == id).Item2;
        }

        public ulong[] GetDatabases(string group)
        {
            string path = Path.Combine("data", group.ToLower());
            string[] paths = Directory.GetFiles(path, "*.json");
            ulong[] ids = new ulong[paths.Length];

            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = ulong.Parse(Path.GetFileNameWithoutExtension(paths[i]));
            }

            return ids;
        }
    }

    public static class DatabaseDSharpPlusExtensions
    {
        public static async Task<T> GetAsync<T>(this DiscordGuild guild, string key) => await Program.Database.GetAsync<T>("guild", guild.Id, key);

        public static async Task<T> GetAsync<T>(this DiscordGuild guild, string key, T @default) => await Program.Database.GetAsync<T>("guild", guild.Id, key, @default);

        public static async Task SetAsync(this DiscordGuild guild, string key, object value) => await Program.Database.SetAsync("guild", guild.Id, key, value);

        public static async Task<T> GetAsync<T>(this DiscordChannel channel, string key) => await Program.Database.GetAsync<T>("channel", channel.Id, key);

        public static async Task<T> GetAsync<T>(this DiscordChannel channel, string key, T @default) => await Program.Database.GetAsync<T>("channel", channel.Id, key, @default);

        public static async Task SetAsync(this DiscordChannel channel, string key, object value) => await Program.Database.SetAsync("channel", channel.Id, key, value);

        public static async Task<T> GetAsync<T>(this DiscordUser user, string key) => await Program.Database.GetAsync<T>("user", user.Id, key);

        public static async Task<T> GetAsync<T>(this DiscordUser user, string key, T @default) => await Program.Database.GetAsync<T>("user", user.Id, key, @default);

        public static async Task SetAsync(this DiscordUser user, string key, object value) => await Program.Database.SetAsync("user", user.Id, key, value);

    }

    public static class DatabaseEscapeAsyncExtensions
    {
        public static T Get<T>(this DiscordGuild guild, string key) => guild.GetAsync<T>(key).GetAwaiter().GetResult();

        public static T Get<T>(this DiscordGuild guild, string key, T @default) => guild.GetAsync<T>(key, @default).GetAwaiter().GetResult();

        public static async void Set(this DiscordGuild guild, string key, object value) => await Program.Database.SetAsync("guild", guild.Id, key, value);

        public static T Get<T>(this DiscordChannel channel, string key) => channel.GetAsync<T>(key).GetAwaiter().GetResult();

        public static T Get<T>(this DiscordChannel channel, string key, T @default) => channel.GetAsync<T>(key, @default).GetAwaiter().GetResult();

        public static async void Set(this DiscordChannel channel, string key, object value) => await Program.Database.SetAsync("channel", channel.Id, key, value);

        public static T Get<T>(this DiscordUser user, string key) => user.GetAsync<T>(key).GetAwaiter().GetResult();

        public static T Get<T>(this DiscordUser user, string key, T @default) => user.GetAsync<T>(key, @default).GetAwaiter().GetResult();

        public static async void Set(this DiscordUser user, string key, object value) => await Program.Database.SetAsync("user", user.Id, key, value);

    }
}