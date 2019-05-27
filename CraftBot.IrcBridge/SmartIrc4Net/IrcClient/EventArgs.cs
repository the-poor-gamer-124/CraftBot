/*
 * $Id$
 * $URL$
 * $Rev$
 * $Author$
 * $Date$
 *
 * SmartIrc4net - the IRC library for .NET/C# <http://smartirc4net.sf.net>
 *
 * Copyright (c) 2003-2005 Mirco Bauer <meebey@meebey.net> <http://www.meebey.net>
 *
 * Full LGPL License: <http://www.gnu.org/licenses/lgpl.txt>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Meebey.SmartIrc4net
{
    /// <summary>
    ///
    /// </summary>
    public class ActionEventArgs : CtcpEventArgs
    {
        internal ActionEventArgs(IrcMessageData data, string actionmsg) : base(data, "ACTION", actionmsg) => this.ActionMessage = actionmsg;

        public string ActionMessage { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class AwayEventArgs : IrcEventArgs
    {
        internal AwayEventArgs(IrcMessageData data, string who, string awaymessage) : base(data)
        {
            this.Who = who;
            this.AwayMessage = awaymessage;
        }

        public string AwayMessage { get; }
        public string Who { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class BanEventArgs : IrcEventArgs
    {
        internal BanEventArgs(IrcMessageData data, string channel, string who, string hostmask) : base(data)
        {
            this.Channel = channel;
            this.Who = who;
            this.Hostmask = hostmask;
        }

        public string Channel { get; }

        public string Hostmask { get; }
        public string Who { get; }
    }

    public class BounceEventArgs : IrcEventArgs
    {
        internal BounceEventArgs(IrcMessageData data, string server, int port) : base(data)
        {
            this.Server = server;
            this.Port = port;
        }

        /// <summary>
        /// Port of the server to which the user is being redirected.
        /// May be -1 if not successfully parsed from the message.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Hostname/address of the server to which the user is being redirected.
        /// May be null if not successfully parsed from the message.
        /// </summary>
        public string Server { get; private set; }
    }

    /// <summary>
    /// User gained channel admin status (non-RFC, channel mode +a, prefix &amp;).
    /// </summary>
    public class ChannelAdminEventArgs : ChannelRoleChangeEventArgs
    {
        internal ChannelAdminEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    public class ChannelModeChangeEventArgs : IrcEventArgs
    {
        internal ChannelModeChangeEventArgs(IrcMessageData data, string channel, List<ChannelModeChangeInfo> modeChanges) : base(data)
        {
            this.Channel = channel;
            this.ModeChanges = modeChanges;
        }

        public string Channel { get; private set; }
        public List<ChannelModeChangeInfo> ModeChanges { get; private set; }
    }

    /// <summary>
    /// Event arguments for any change in channel role.
    /// </summary>
    public class ChannelRoleChangeEventArgs : IrcEventArgs
    {
        internal ChannelRoleChangeEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data)
        {
            this.Channel = channel;
            this.Who = who;
            this.Whom = whom;
        }

        public string Channel { get; private set; }
        public string Who { get; private set; }
        public string Whom { get; private set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class CtcpEventArgs : IrcEventArgs
    {
        internal CtcpEventArgs(IrcMessageData data, string ctcpcmd, string ctcpparam) : base(data)
        {
            this.CtcpCommand = ctcpcmd;
            this.CtcpParameter = ctcpparam;
        }

        public string CtcpCommand { get; }

        public string CtcpParameter { get; }
    }

    /// <summary>
    /// User lost channel admin status (non-RFC, channel mode -a).
    /// </summary>
    public class DeChannelAdminEventArgs : ChannelRoleChangeEventArgs
    {
        internal DeChannelAdminEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    /// <summary>
    /// User lost halfop status (non-RFC, channel mode -h).
    /// </summary>
    public class DehalfopEventArgs : ChannelRoleChangeEventArgs
    {
        internal DehalfopEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    /// <summary>
    /// User lost op status (channel mode -o).
    /// </summary>
    public class DeopEventArgs : ChannelRoleChangeEventArgs
    {
        internal DeopEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    /// <summary>
    /// User lost owner status (non-RFC, channel mode -q).
    /// </summary>
    public class DeownerEventArgs : ChannelRoleChangeEventArgs
    {
        internal DeownerEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    /// <summary>
    /// User lost voice status (channel mode -v).
    /// </summary>
    public class DevoiceEventArgs : ChannelRoleChangeEventArgs
    {
        internal DevoiceEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class ErrorEventArgs : IrcEventArgs
    {
        internal ErrorEventArgs(IrcMessageData data, string errormsg) : base(data) => this.ErrorMessage = errormsg;

        public string ErrorMessage { get; }
    }

    /// <summary>
    /// User gained halfop status (non-RFC, channel mode +h, prefix %).
    /// </summary>
    public class HalfopEventArgs : ChannelRoleChangeEventArgs
    {
        internal HalfopEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class InviteEventArgs : IrcEventArgs
    {
        internal InviteEventArgs(IrcMessageData data, string channel, string who) : base(data)
        {
            this.Channel = channel;
            this.Who = who;
        }

        public string Channel { get; }

        public string Who { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class JoinEventArgs : IrcEventArgs
    {
        internal JoinEventArgs(IrcMessageData data, string channel, string who) : base(data)
        {
            this.Channel = channel;
            this.Who = who;
        }

        public string Channel { get; }

        public string Who { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class KickEventArgs : IrcEventArgs
    {
        internal KickEventArgs(IrcMessageData data, string channel, string who, string whom, string kickreason) : base(data)
        {
            this.Channel = channel;
            this.Who = who;
            this.Whom = whom;
            this.KickReason = kickreason;
        }

        public string Channel { get; }

        public string KickReason { get; }
        public string Who { get; }

        public string Whom { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class ListEventArgs : IrcEventArgs
    {
        internal ListEventArgs(IrcMessageData data, ChannelInfo listInfo) : base(data) => this.ListInfo = listInfo;

        public ChannelInfo ListInfo { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class MotdEventArgs : IrcEventArgs
    {
        internal MotdEventArgs(IrcMessageData data, string motdmsg) : base(data) => this.MotdMessage = motdmsg;

        public string MotdMessage { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class NamesEventArgs : IrcEventArgs
    {
        internal NamesEventArgs(IrcMessageData data, string channel, string[] userlist, string[] rawUserList) : base(data)
        {
            this.Channel = channel;
            this.UserList = userlist;
            this.RawUserList = rawUserList;
        }

        public string Channel { get; }
        public string[] RawUserList { get; private set; }
        public string[] UserList { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class NickChangeEventArgs : IrcEventArgs
    {
        internal NickChangeEventArgs(IrcMessageData data, string oldnick, string newnick) : base(data)
        {
            this.OldNickname = oldnick;
            this.NewNickname = newnick;
        }

        public string NewNickname { get; }
        public string OldNickname { get; }
    }

    /// <summary>
    /// User gained op status (channel mode +o, prefix @).
    /// </summary>
    public class OpEventArgs : ChannelRoleChangeEventArgs
    {
        internal OpEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    /// <summary>
    /// User gained owner status (non-RFC, channel mode +q, prefix ~).
    /// </summary>
    public class OwnerEventArgs : ChannelRoleChangeEventArgs
    {
        internal OwnerEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class PartEventArgs : IrcEventArgs
    {
        internal PartEventArgs(IrcMessageData data, string channel, string who, string partmessage) : base(data)
        {
            this.Channel = channel;
            this.Who = who;
            this.PartMessage = partmessage;
        }

        public string Channel { get; }

        public string PartMessage { get; }
        public string Who { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class PingEventArgs : IrcEventArgs
    {
        internal PingEventArgs(IrcMessageData data, string pingdata) : base(data) => this.PingData = pingdata;

        public string PingData { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class PongEventArgs : IrcEventArgs
    {
        internal PongEventArgs(IrcMessageData data, TimeSpan lag) : base(data) => Lag = lag;

        public TimeSpan Lag { get; private set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class QuitEventArgs : IrcEventArgs
    {
        internal QuitEventArgs(IrcMessageData data, string who, string quitmessage) : base(data)
        {
            this.Who = who;
            this.QuitMessage = quitmessage;
        }

        public string QuitMessage { get; }
        public string Who { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class TopicChangeEventArgs : IrcEventArgs
    {
        internal TopicChangeEventArgs(IrcMessageData data, string channel, string who, string newtopic) : base(data)
        {
            this.Channel = channel;
            this.Who = who;
            this.NewTopic = newtopic;
        }

        public string Channel { get; }

        public string NewTopic { get; }
        public string Who { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class TopicEventArgs : IrcEventArgs
    {
        internal TopicEventArgs(IrcMessageData data, string channel, string topic) : base(data)
        {
            this.Channel = channel;
            this.Topic = topic;
        }

        public string Channel { get; }

        public string Topic { get; }
    }

    /// <summary>
    ///
    /// </summary>
    public class UnbanEventArgs : IrcEventArgs
    {
        internal UnbanEventArgs(IrcMessageData data, string channel, string who, string hostmask) : base(data)
        {
            this.Channel = channel;
            this.Who = who;
            this.Hostmask = hostmask;
        }

        public string Channel { get; }

        public string Hostmask { get; }
        public string Who { get; }
    }

    /// <summary>
    /// User gained voice status (channel mode +v, prefix +).
    /// </summary>
    public class VoiceEventArgs : ChannelRoleChangeEventArgs
    {
        internal VoiceEventArgs(IrcMessageData data, string channel, string who, string whom) : base(data, channel, who, whom)
        {
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class WhoEventArgs : IrcEventArgs
    {
        internal WhoEventArgs(IrcMessageData data, WhoInfo whoInfo) : base(data) => this.WhoInfo = whoInfo;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public string Channel => this.WhoInfo.Channel;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public int HopCount => this.WhoInfo.HopCount;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public string Host => this.WhoInfo.Host;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public string Ident => this.WhoInfo.Ident;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public bool IsAway => this.WhoInfo.IsAway;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public bool IsIrcOp => this.WhoInfo.IsIrcOp;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public bool IsOp => this.WhoInfo.IsOp;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public bool IsVoice => this.WhoInfo.IsVoice;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public string Nick => this.WhoInfo.Nick;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public string Realname => this.WhoInfo.Realname;

        [Obsolete("Use WhoEventArgs.WhoInfo instead.")]
        public string Server => this.WhoInfo.Server;

        public WhoInfo WhoInfo { get; }
    }
}