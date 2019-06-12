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
        public static bool UseSecondaryAppDomain { get; set; } = false;
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static void LoadPlugin(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            if (!path.ToLower().EndsWith(".dll"))
            {
                throw new ArgumentException("File extension seems invalid (supported file type is .dll)", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Path for plugin couldn't be found", path);
            }

            Assembly assembly;

            if (UseSecondaryAppDomain)
            {
                assembly = PluginAppDomain.Load(File.ReadAllBytes(path));
            }
            else
            {
                assembly = Assembly.LoadFrom(path);
            }

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

            Logger.Info($"Loading plugins... ({dllFileNames.Count()} file(s))");

            foreach (string path in dllFileNames)
            {
                try
                {
                    LoadPlugin(path);
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, "Failed to load plugin from " + path);
                }
            }

            Logger.Info($"{Plugins.Count} plugins loaded");
        }

        public static void RegisterPlugin(Plugin plugin, CommandsNextExtension extension)
        {
            Logger.Info($"Registering '{plugin.Name}' from {plugin.Author}");

            Type[] assemblyTypes = plugin.GetType().Assembly.GetTypes();
            Type commandClass = assemblyTypes.FirstOrDefault(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(CommandClass)));

            if (commandClass == null)
            {
                Logger.Warn($"{plugin.Name} doesn't have a main command class");
                return;
            }

            extension.RegisterCommands(commandClass);

            plugin.OnPluginRegistered();

            Logger.Info($"Registered plugin '{plugin.Name}'");
        }

        public static void RegisterPlugins()
        {
            Logger.Info($"Registering plugins... ({Plugins.Count()} plugin(s))");

            foreach (Plugin plugin in Plugins)
            {
                RegisterPlugin(plugin, Program.Commands);
            }

            Logger.Info("All plugins registered");
        }

        public static void UnloadPlugins()
        {
            Logger.Info("Unloading plugins...");

            if (PluginAppDomain != null)
            {
                AppDomain.Unload(PluginAppDomain);
            }
            else
            {
                PluginAppDomain = AppDomain.CreateDomain("plugins");
            }

            Logger.Info("Plugins unloaded");
        }
    }
}