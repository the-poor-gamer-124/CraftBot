using System.Collections.Generic;

namespace CraftBot.Base
{
    public class DebugFlags
    {
        private readonly Dictionary<string, bool> Flags = new Dictionary<string, bool>()
        {
            { "translate", true }
        };

        public bool this[string flag]
        {
            get => Flags.ContainsKey(flag.ToLowerInvariant()) ? Flags[flag.ToLowerInvariant()] : false;
            set => Flags[flag.ToLowerInvariant()] = value;
        }
    }
}