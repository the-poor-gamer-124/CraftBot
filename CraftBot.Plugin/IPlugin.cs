using System;

namespace CraftBot.Plugin
{
    public abstract class Plugin
    {
        public event EventHandler PluginLoaded;

        public event EventHandler PluginRegistered;

        public abstract string Author { get; }
        public abstract string Name { get; }
    }
}