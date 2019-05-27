using System;

namespace CraftBot.Helper.Exceptions
{
    public class EmojiNotFoundException : Exception
    {
        public EmojiNotFoundException(string emojiName) : base("Following emoji was not found: " + emojiName) { }
    }
}
