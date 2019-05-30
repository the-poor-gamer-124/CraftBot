using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CraftBot.Helper
{
    public static class PathHelper
    {
        public static string CraftBotDirectory = Environment.CurrentDirectory;

        public static string PathSeparator { get; } = Path.DirectorySeparatorChar.ToString(); 
        //Method 1: RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/"

        public static string ErrorReportsDirectory => CombinePath(CraftBotDirectory, "error");

        public static string DataDirectory => CombinePath(CraftBotDirectory, "data");

        public static string UserDataDirectory => CombinePath(DataDirectory, "user");

        public static string GuildDataDirectory => CombinePath(DataDirectory, "guild");

        public static string ChannelDataDirectory => CombinePath(DataDirectory, "channel");

        public static string PluginsDirectory => CombinePath(CraftBotDirectory, "plugins");

        public static string CombinePath(params string[] entries) => string.Join(PathSeparator, entries);
    }
}
