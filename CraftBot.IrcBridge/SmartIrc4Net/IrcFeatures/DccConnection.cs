/*
 *
 * SmartIrc4net - the IRC library for .NET/C# <http://smartirc4net.sf.net>
 *
 * Copyright (c) 2008-2009 Thomas Bruderer <apophis@apophis.ch> <http://www.apophis.ch>
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
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace Meebey.SmartIrc4net
{
    /// <summary>
    /// Baseclass for all DccConnections
    /// </summary>
    public class DccConnection
    {
        #region Private Variables

        protected TcpClient Connection;
        protected TcpListener DccServer;
        protected IPAddress ExternalIPAdress;
        protected IrcFeatures Irc;
        protected bool isConnected = false;
        protected bool isValid = true;
        protected IPEndPoint LocalEndPoint;
        protected bool reject = false;
        protected IPEndPoint RemoteEndPoint;
        protected long session;
        protected DateTime Timeout;
        protected string User;

        private class Session
        {
            private static long next;
            internal static long Next => ++next;
        }

        #endregion Private Variables

        #region Public Base.Program

        /// <summary>
        /// Returns false when the Connections is not Valid (before or after Connection)
        /// </summary>
        public bool Connected => isConnected;

        /// <summary>
        /// Returns the Nick of the User we have a DCC with
        /// </summary>
        public string Nick => User;

        /// <summary>
        /// Returns false when the Connections is not Valid anymore (only at the end)
        /// </summary>
        public bool Valid => isValid && (isConnected || (DateTime.Now < Timeout));

        #endregion Public Base.Program

        #region Public DCC Events

        public event DccChatLineHandler OnDccChatReceiveLineEvent;

        public event DccConnectionHandler OnDccChatRequestEvent;

        public event DccChatLineHandler OnDccChatSentLineEvent;

        public event DccConnectionHandler OnDccChatStartEvent;

        public event DccConnectionHandler OnDccChatStopEvent;

        public event DccSendPacketHandler OnDccSendReceiveBlockEvent;

        public event DccSendRequestHandler OnDccSendRequestEvent;

        public event DccSendPacketHandler OnDccSendSentBlockEvent;

        public event DccConnectionHandler OnDccSendStartEvent;

        public event DccConnectionHandler OnDccSendStopEvent;

        protected virtual void DccChatReceiveLineEvent(DccChatEventArgs e)
        {
            OnDccChatReceiveLineEvent?.Invoke(this, e); Irc.DccChatReceiveLineEvent(e);
        }

        protected virtual void DccChatRequestEvent(DccEventArgs e)
        {
            OnDccChatRequestEvent?.Invoke(this, e); Irc.DccChatRequestEvent(e);
        }

        protected virtual void DccChatSentLineEvent(DccChatEventArgs e)
        {
            OnDccChatSentLineEvent?.Invoke(this, e); Irc.DccChatSentLineEvent(e);
        }

        protected virtual void DccChatStartEvent(DccEventArgs e)
        {
            OnDccChatStartEvent?.Invoke(this, e); Irc.DccChatStartEvent(e);
        }

        protected virtual void DccChatStopEvent(DccEventArgs e)
        {
            OnDccChatStopEvent?.Invoke(this, e); Irc.DccChatStopEvent(e);
        }

        protected virtual void DccSendReceiveBlockEvent(DccSendEventArgs e)
        {
            OnDccSendReceiveBlockEvent?.Invoke(this, e); Irc.DccSendReceiveBlockEvent(e);
        }

        protected virtual void DccSendRequestEvent(DccSendRequestEventArgs e)
        {
            OnDccSendRequestEvent?.Invoke(this, e); Irc.DccSendRequestEvent(e);
        }

        protected virtual void DccSendSentBlockEvent(DccSendEventArgs e)
        {
            OnDccSendSentBlockEvent?.Invoke(this, e); Irc.DccSendSentBlockEvent(e);
        }

        protected virtual void DccSendStartEvent(DccEventArgs e)
        {
            OnDccSendStartEvent?.Invoke(this, e); Irc.DccSendStartEvent(e);
        }

        protected virtual void DccSendStopEvent(DccEventArgs e)
        {
            OnDccSendStopEvent?.Invoke(this, e); Irc.DccSendStopEvent(e);
        }

        #endregion Public DCC Events

        internal DccConnection()
        {
            //Each DccConnection gets a Unique Identifier (just used internally until we have a TcpClient connected)
            session = Session.Next;
            // If a Connection is not established within 120 Seconds we invalidate the DccConnection (see property Valid)
            Timeout = DateTime.Now.AddSeconds(120);
        }

        internal virtual void InitWork(object stateInfo) => throw new NotSupportedException();

        internal bool IsSession(long session) => session == this.session;

        #region Public Methods

        public void Disconnect()
        {
            isConnected = false;
            isValid = false;
        }

        public void RejectRequest()
        {
            Irc.SendMessage(SendType.CtcpReply, User, "ERRMSG DCC Rejected");
            reject = true;
            isValid = false;
        }

        public override string ToString() => "DCC Session " + session + " of " + this.GetType().ToString() + " is " + (isConnected ? "connected to " + RemoteEndPoint.Address.ToString() : "not connected") + "[" + this.User + "]";

        #endregion Public Methods

        #region protected Helper Functions

        protected string DccIntToHost(long ip)
        {
            var ep = new IPEndPoint(ip, 80);
            char[] sep = { '.' };
            string[] ipparts = ep.Address.ToString().Split(sep);
            return ipparts[3] + "." + ipparts[2] + "." + ipparts[1] + "." + ipparts[0];
        }

        protected string FilterMarker(string msg)
        {
            string result = "";
            foreach (char c in msg)
            {
                if (c != IrcConstants.CtcpChar)
                {
                    result += c;
                }
            }
            return result;
        }

        protected byte[] GetAck(long SentBytes)
        {
            byte[] acks = new byte[4];
            acks[0] = (byte)((SentBytes >> 24) % 256);
            acks[1] = (byte)((SentBytes >> 16) % 256);
            acks[2] = (byte)((SentBytes >> 8) % 256);
            acks[3] = (byte)(SentBytes % 256);
            return acks;
        }

        [Obsolete]
        protected long HostToDccInt(IPAddress ip)
        {
            long temp = (ip.Address & 0xff) << 24;
            temp |= (ip.Address & 0xff00) << 8;
            temp |= (ip.Address >> 8) & 0xff00;
            temp |= (ip.Address >> 24) & 0xff;
            return temp;
        }

        #endregion protected Helper Functions
    }
}