using CraftBot.Base.Plugins;
using CraftBot.Helper;

using DSharpPlus.CommandsNext;

using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CraftBot.Base
{
    public static class PluginLoader
    {
        private static AppDomain PluginAppDomain = null;
        public static List<Plugin> Plugins;
        public static bool UseSecondaryAppDomain = false;

        public static void LoadPlugin(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }
            else if (!path.ToLower().EndsWith(".dll"))
            {
                throw new ArgumentException("File extension seems invalid (supported file types are .dll)", nameof(path));
            }
            else if (!File.Exists(path))
            {
                throw new FileNotFoundException("Path for plugin couldn't be found", path);
            }

            Assembly assembly = UseSecondaryAppDomain ? PluginAppDomain.Load(File.ReadAllBytes(path)) : Assembly.LoadFrom(path);
            LoadPlugin(assembly);
        }

        public static void LoadPlugin(Assembly assembly)
        {
            Type[] assemblyTypes = assembly.GetTypes();
            Type pluginClass = assemblyTypes.FirstOrDefault(t => t.BaseType == typeof(Plugin));

            if (pluginClass != null)
            {
                var plugin = (Plugin)Activator.CreateInstance(pluginClass);
                plugin.OnPluginLoaded();
                Plugins.Add(plugin);
            }
            else
            {
                throw new InvalidPluginException();
            }
        }

        public static void LoadPlugins()
        {
            if (UseSecondaryAppDomain)
            {
                UnloadPlugins();
            }

            string[] dllFileNames = Directory.GetFiles(PathHelper.PluginsDirectory, "*.dll", SearchOption.AllDirectories);

            Plugins = new List<Plugin>();

            LogManager.GetCurrentClassLogger().Info($"Loading plugins... ({dllFileNames.Count()} file(s))");

            foreach (string path in dllFileNames)
            {
                try
                {
                    LoadPlugin(path);
                }
                catch (Exception exception)
                {
                    LogManager.GetCurrentClassLogger().Error(exception, "Failed to load plugin from " + path);
                }
            }

            LogManager.GetCurrentClassLogger().Info($"{Plugins.Count} plugins loaded");
        }

        public static void RegisterPlugin(Plugin plugin, CommandsNextExtension extension)
        {
            LogManager.GetCurrentClassLogger().Info($"Registering '{plugin.Name}' from {plugin.Author}");

            Type[] assemblyTypes = plugin.GetType().Assembly.GetTypes();
            Type commandClass = assemblyTypes.FirstOrDefault(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(CommandClass)));

            if (commandClass == null)
            {
                LogManager.GetCurrentClassLogger().Warn($"{plugin.Name} doesn't have a main command class");
                return;
            }

            extension.RegisterCommands(commandClass);

            plugin.OnPluginRegistered();

            LogManager.GetCurrentClassLogger().Info($"Registered plugin '{plugin.Name}'");
        }

        public static void RegisterPlugins()
        {
            LogManager.GetCurrentClassLogger().Info($"Registering plugins... ({Plugins.Count()} plugin(s))");

            foreach (Plugin plugin in Plugins)
            {
                RegisterPlugin(plugin, Program.Commands);
            }

            LogManager.GetCurrentClassLogger().Info("All plugins registered");
        }

        public static void UnloadPlugins()
        {
            LogManager.GetCurrentClassLogger().Info("Unloading plugins...");
            if (PluginAppDomain != null)
            {
                AppDomain.Unload(PluginAppDomain);
            }
            else
            {
                PluginAppDomain = AppDomain.CreateDomain("plugins");
            }
            LogManager.GetCurrentClassLogger().Info("Plugins unloaded");
        }
    }
}