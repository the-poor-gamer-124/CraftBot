using System;

namespace CraftBot.Base.Plugins
{
    /// <summary>
    /// Identifies the class used for commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandClass : Attribute
    {
        public CommandClass()
        {
        }
    }
}