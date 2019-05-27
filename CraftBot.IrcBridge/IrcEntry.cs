using CraftBot.Helper;

using Meebey.SmartIrc4net;

using System;
using System.Collections.Generic;

namespace CraftBot.IrcBridge
{
    public class IrcEntry
    {
        #region Public Base.Program

        public List<ChannelLink> ChannelLinks = new List<ChannelLink>();

        public IrcClient IrcClient;

        public string ServerHost;
        public int ServerPort;

        public IrcEntry(string serverHost, int serverPort, List<ChannelLink> channelLinks)
        {
            ServerHost = serverHost ?? throw new ArgumentNullException(nameof(serverHost));
            ServerPort = serverPort;
            ChannelLinks = channelLinks ?? throw new ArgumentNullException(nameof(channelLinks));

            IrcClient = new IrcClient()
            {
                AutoReconnect = true,
                AutoRejoin = true,
                AutoRelogin = true,
                AutoRetry = true
            };
            IrcClient.OnConnected += IrcGateway.IrcClient_ConnectedAsync;
            IrcClient.OnQueryMessage += IrcGateway.IrcClient_OnMessageAsync;
            IrcClient.OnChannelMessage += IrcGateway.IrcClient_OnMessageAsync;
        }

        #endregion Public Base.Program
    }
}