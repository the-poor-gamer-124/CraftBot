using DSharpPlus.Net.WebSocket;

using System;
using System.Runtime.InteropServices;

namespace CraftBot.Base
{
    public static class CompatibleWebSocket
    {
        public static WebSocketClientFactoryDelegate GetWebSocketClient()
        {
            //bool isMono = Type.GetType("Mono.Runtime") != null;
            //if (isMono)
            //{
            //    return WebSocketSharpClient.CreateNew;
            //}

            bool isWindows7 = Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
            if (isWindows7)
            {
                //HACK: Uncommment if you want to have support for .NET Framework
                //bool IsNetFramework = RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
                //if (IsNetFramework)
                //{
                //    return WebSocket4NetClient.CreateNew;
                //}

                bool IsNetCore = RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);
                if (IsNetCore)
                {
                    return WebSocket4NetCoreClient.CreateNew;
                }
            }
            return WebSocketClient.CreateNew;

            throw new Exception("A compatible WebSocket client hasn't been found.");
        }
    }
}