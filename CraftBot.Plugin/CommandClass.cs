using System;

namespace CraftBot.Plugin
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