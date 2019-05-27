using CraftBot.Helper.Exceptions;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftBot.Helper
{
    public static class CustomEmoji
    {
        public static Dictionary<string, ulong> EmojiDictionary { get; } = new Dictionary<string, ulong>();

        public static ulong GetEmojiId(this string name)
        {
            name = name.Replace("-", "_");
            if (EmojiDictionary.ContainsKey(name))
            {
                return EmojiDictionary[name];
            }
            else
            {
                throw new EmojiNotFoundException(name);
            }
        }
        public static DiscordEmoji GetEmoji(this ulong id, DiscordClient client) => DiscordEmoji.FromGuildEmote(client, id);
        public static DiscordEmoji GetEmoji(this string name, DiscordClient client) => name.GetEmojiId().GetEmoji(client);
        public static DiscordEmoji GetEmoji(this bool value, DiscordClient client) => value ? DiscordEmoji.FromGuildEmote(client, 534026420603715604) : DiscordEmoji.FromGuildEmote(client, 534026420612104212);
        public static string GetEmojiString(this string name, DiscordClient client) => name.GetEmoji(client).ToString();
        public static string GetEmojiString(this bool value, DiscordClient client) => value.GetEmoji(client).ToString();
    }
}
