using System;

namespace CraftBot.Base.Plugins
{
    public abstract class Plugin
    {
        public event EventHandler PluginLoaded;

        public event EventHandler PluginRegistered;

        public abstract string Author { get; }

        public abstract string Name { get; }

        public void OnPluginLoaded() => this.PluginLoaded?.Invoke(this, null);

        public void OnPluginRegistered() => this.PluginRegistered?.Invoke(this, null);
    }
}