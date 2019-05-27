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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Meebey.SmartIrc4net
{
    /// <summary>
    /// Description of IrcFeatures2.
    /// </summary>
    ///
    public class IrcFeatures : IrcClient
    {
        #region Public Field Access

        /// <summary>
        /// To handle more or less CTCP Events, modify this collection to your needs.
        /// You can also change the Delegates to your own implementations.
        /// </summary>
        public Dictionary<string, CtcpDelegate> CtcpDelegates { get; } = new Dictionary<string, CtcpDelegate>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// The Source of the IRC Program is show in the CTCP Source Request
        /// </summary>
        public string CtcpSource { get; set; }

        /// <summary>
        /// This Url will be mentioned with the CTCP Url Request
        /// </summary>
        public string CtcpUrl { get; set; }

        /// <summary>
        /// This Info is shown with the CTCP UserInfo Request
        /// </summary>
        public string CtcpUserInfo { get; set; }

        /// <summary>
        /// Access to all DccConnections, Its not possible to change the collection itself,
        /// but you can use the public Members of the DccCollections or its inherited Classes.
        /// </summary>
        public ReadOnlyCollection<DccConnection> DccConnections => new ReadOnlyCollection<DccConnection>(_DccConnections);

        public IPAddress ExternalIpAdress { get; set; }

        #endregion Public Field Access

        #region private variables

        private readonly List<DccConnection> _DccConnections = new List<DccConnection>();
        internal DccSpeed Speed = DccSpeed.RfcSendAhead;

        #endregion private variables

        #region Public DCC Events (Global: All Dcc Events)

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

        internal void DccSendSentBlockEvent(DccSendEventArgs e) => OnDccSendSentBlockEvent?.Invoke(this, e);

        public void DccChatReceiveLineEvent(DccChatEventArgs e) => OnDccChatReceiveLineEvent?.Invoke(this, e);

        public void DccChatRequestEvent(DccEventArgs e) => OnDccChatRequestEvent?.Invoke(this, e);

        public void DccChatSentLineEvent(DccChatEventArgs e) => OnDccChatSentLineEvent?.Invoke(this, e);

        public void DccChatStartEvent(DccEventArgs e) => OnDccChatStartEvent?.Invoke(this, e);

        public void DccChatStopEvent(DccEventArgs e) => OnDccChatStopEvent?.Invoke(this, e);

        public void DccSendReceiveBlockEvent(DccSendEventArgs e) => OnDccSendReceiveBlockEvent?.Invoke(this, e);

        public void DccSendRequestEvent(DccSendRequestEventArgs e) => OnDccSendRequestEvent?.Invoke(this, e);

        public void DccSendStartEvent(DccEventArgs e) => OnDccSendStartEvent?.Invoke(this, e);

        public void DccSendStopEvent(DccEventArgs e) => OnDccSendStopEvent?.Invoke(this, e);

        #endregion Public DCC Events (Global: All Dcc Events)

        #region Public Interface Methods

        public IrcFeatures() : base()
        {
            // This method calls all the ctcp handlers defined below (or added anywhere else)
            this.OnCtcpRequest += new CtcpEventHandler(this.CtcpRequestsHandler);

            // Adding ctcp handler, all commands are lower case (.ToLower() in handler)
            this.CtcpDelegates.Add("version", this.CtcpVersionDelegate);
            this.CtcpDelegates.Add("clientinfo", this.CtcpClientInfoDelegate);
            this.CtcpDelegates.Add("time", this.CtcpTimeDelegate);
            this.CtcpDelegates.Add("userinfo", this.CtcpUserInfoDelegate);
            this.CtcpDelegates.Add("url", this.CtcpUrlDelegate);
            this.CtcpDelegates.Add("source", this.CtcpSourceDelegate);
            this.CtcpDelegates.Add("finger", this.CtcpFingerDelegate);
            // The DCC Handler
            this.CtcpDelegates.Add("dcc", this.CtcpDccDelegate);
            // Don't remove the Ping handler without your own implementation
            this.CtcpDelegates.Add("ping", this.CtcpPingDelegate);
        }

        /// <summary>
        /// Init a DCC Chat Session
        /// </summary>
        /// <param name="user">User to DCC</param>
        public void InitDccChat(string user) => this.InitDccChat(user, false);

        /// <summary>
        /// Init a DCC Chat Session
        /// </summary>
        /// <param name="user">User to DCC</param>
        /// <param name="passive">Passive DCC</param>
        public void InitDccChat(string user, bool passive) => this.InitDccChat(user, passive, Priority.Medium);

        /// <summary>
        /// Init a DCC Chat Session
        /// </summary>
        /// <param name="user">User to DCC</param>
        /// <param name="passive">Passive DCC</param>
        /// <param name="priority">Non Dcc Message Priority for Negotiation</param>
        public void InitDccChat(string user, bool passive, Priority priority)
        {
            var chat = new DccChat(this, user, this.ExternalIpAdress, passive, priority);
            _DccConnections.Add(chat);
            ThreadPool.QueueUserWorkItem(new WaitCallback(chat.InitWork));
            this.RemoveInvalidDccConnections();
        }

        /// <summary>
        /// Send a local File
        /// </summary>
        /// <param name="user">Destination of the File (no channel)</param>
        /// <param name="filepath">complete filepath, absolute or relative (carefull)</param>
        public void SendFile(string user, string filepath)
        {
            var fi = new FileInfo(filepath);
            if (fi.Exists)
            {
                this.SendFile(user, new FileStream(filepath, FileMode.Open), fi.Name, fi.Length, DccSpeed.RfcSendAhead, false, Priority.Medium);
            }
        }

        /// <summary>
        /// Send a local File passivly
        /// </summary>
        /// <param name="user">Destination of the File (no channel)</param>
        /// <param name="filepath">complete filepath, absolute or relative (carefull)</param>
        /// <param name="passive">Passive DCC</param>
        public void SendFile(string user, string filepath, bool passive)
        {
            var fi = new FileInfo(filepath);
            if (fi.Exists)
            {
                this.SendFile(user, new FileStream(filepath, FileMode.Open), fi.Name, fi.Length, DccSpeed.RfcSendAhead, passive, Priority.Medium);
            }
        }

        /// <summary>
        /// Send any Stream, active initiator, fast RfC method
        /// </summary>
        /// <param name="user">Destination of the File (no channel)</param>
        /// <param name="file">You can send any stream here</param>
        /// <param name="filename">give a filename for the remote User</param>
        /// <param name="filesize">give the length of the stream</param>
        public void SendFile(string user, Stream file, string filename, long filesize) => this.SendFile(user, file, filename, filesize, DccSpeed.RfcSendAhead, false);

        /// <summary>
        /// Send any Stream, full flexibility in Dcc Connection Negotiation
        /// </summary>
        /// <param name="user">Destination of the File (no channel)</param>
        /// <param name="file">You can send any stream here</param>
        /// <param name="filename">give a filename for the remote User</param>
        /// <param name="filesize">give the length of the stream</param>
        /// <param name="speed">What ACK Managment should be used</param>
        /// <param name="passive">Passive DCC</param>
        public void SendFile(string user, Stream file, string filename, long filesize, DccSpeed speed, bool passive) => this.SendFile(user, file, filename, filesize, speed, passive, Priority.Medium);

        /// <summary>
        /// Send any Stream, full flexibility in Dcc Connection Negotiation
        /// </summary>
        /// <param name="user">Destination of the File (no channel)</param>
        /// <param name="file">You can send any stream here</param>
        /// <param name="filename">give a filename for the remote User</param>
        /// <param name="filesize">give the length of the stream</param>
        /// <param name="speed">What ACK Managment should be used</param>
        /// <param name="passive">Passive DCC</param>
        /// <param name="priority">Non Dcc Message Priority for Negotiation</param>
        public void SendFile(string user, Stream file, string filename, long filesize, DccSpeed speed, bool passive, Priority priority)
        {
            var send = new DccSend(this, user, this.ExternalIpAdress, file, filename, filesize, speed, passive, priority);
            _DccConnections.Add(send);
            ThreadPool.QueueUserWorkItem(new WaitCallback(send.InitWork));
            this.RemoveInvalidDccConnections();
        }

        #endregion Public Interface Methods

        #region Private Methods

        private void CtcpRequestsHandler(object sender, CtcpEventArgs e)
        {
            if (this.CtcpDelegates.ContainsKey(e.CtcpCommand))
            {
                this.CtcpDelegates[e.CtcpCommand].Invoke(e);
            }
            else
            {
                /* No CTCP Handler for this Command */
            }
            this.RemoveInvalidDccConnections();
        }

        #endregion Private Methods

        #region implemented ctcp delegates, can be overwritten by changing the ctcpDelagtes Dictionary

        private void CtcpClientInfoDelegate(CtcpEventArgs e)
        {
            string clientInfo = "CLIENTINFO";
            foreach (KeyValuePair<string, CtcpDelegate> kvp in this.CtcpDelegates)
            {
                clientInfo = clientInfo + " " + kvp.Key.ToUpper();
            }
            this.SendMessage(SendType.CtcpReply, e.Data.Nick, clientInfo);
        }

        private void CtcpDccDelegate(CtcpEventArgs e)
        {
            if (e.Data.MessageArray.Length < 2)
            {
                this.SendMessage(SendType.CtcpReply, e.Data.Nick, "ERRMSG DCC missing parameters");
            }
            else
            {
                switch (e.Data.MessageArray[1])
                {
                    case "CHAT":
                        var chat = new DccChat(this, this.ExternalIpAdress, e);
                        _DccConnections.Add(chat);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(chat.InitWork));
                        break;

                    case "SEND":
                        if (e.Data.MessageArray.Length > 6 && (this.FilterMarker(e.Data.MessageArray[6]) != "T"))
                        {
                            long session = -1;
                            long.TryParse(this.FilterMarker(e.Data.MessageArray[6]), out session);
                            foreach (DccConnection dc in DccConnections)
                            {
                                if (dc.IsSession(session))
                                {
                                    ((DccSend)dc).SetRemote(e);
                                    ((DccSend)dc).AcceptRequest(null, 0);
                                    return;
                                }
                            }
                            this.SendMessage(SendType.CtcpReply, e.Data.Nick, "ERRMSG Invalid passive DCC");
                        }
                        else
                        {
                            var send = new DccSend(this, this.ExternalIpAdress, e);
                            _DccConnections.Add(send);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(send.InitWork));
                        }
                        break;

                    case "RESUME":
                        foreach (DccConnection dc in DccConnections)
                        {
                            if ((dc is DccSend) && ((DccSend)dc).TryResume(e))
                            {
                                return;
                            }
                        }
                        this.SendMessage(SendType.CtcpReply, e.Data.Nick, "ERRMSG Invalid DCC RESUME");
                        break;

                    case "ACCEPT":
                        foreach (DccConnection dc in DccConnections)
                        {
                            if ((dc is DccSend) && ((DccSend)dc).TryAccept(e))
                            {
                                return;
                            }
                        }
                        this.SendMessage(SendType.CtcpReply, e.Data.Nick, "ERRMSG Invalid DCC ACCEPT");
                        break;

                    case "XMIT":
                        this.SendMessage(SendType.CtcpReply, e.Data.Nick, "ERRMSG DCC XMIT not implemented");
                        break;

                    default:
                        this.SendMessage(SendType.CtcpReply, e.Data.Nick, "ERRMSG DCC " + e.CtcpParameter + " unavailable");
                        break;
                }
            }
        }

        private void CtcpFingerDelegate(CtcpEventArgs e) => this.SendMessage(SendType.CtcpReply, e.Data.Nick, "FINGER Don't touch little Helga there! ");

        private void CtcpPingDelegate(CtcpEventArgs e)
        {
            if (e.Data.Message.Length > 7)
            {
                this.SendMessage(SendType.CtcpReply, e.Data.Nick, "PING " + e.Data.Message.Substring(6, e.Data.Message.Length - 7));
            }
            else
            {
                this.SendMessage(SendType.CtcpReply, e.Data.Nick, "PING");    //according to RFC, it should be PONG!
            }
        }

        /// <summary>
        ///  This is the correct Rfc Ping Delegate, which is not used because all other clients do not use the PING According to RfC
        /// </summary>
        /// <param name="e"></param>
        //private void CtcpRfcPingDelegate(CtcpEventArgs e)
        //{
        //    if (e.Data.Message.Length > 7)
        //    {
        //        this.SendMessage(SendType.CtcpReply, e.Data.Nick, "PONG " + e.Data.Message.Substring(6, e.Data.Message.Length - 7));
        //    }
        //    else
        //    {
        //        this.SendMessage(SendType.CtcpReply, e.Data.Nick, "PONG");
        //    }
        //}

        private void CtcpSourceDelegate(CtcpEventArgs e) => this.SendMessage(SendType.CtcpReply, e.Data.Nick, "SOURCE " + (this.CtcpSource ?? "http://smartirc4net.meebey.net"));

        private void CtcpTimeDelegate(CtcpEventArgs e) => this.SendMessage(SendType.CtcpReply, e.Data.Nick, "TIME " + DateTime.Now.ToString("r"));

        private void CtcpUrlDelegate(CtcpEventArgs e) => this.SendMessage(SendType.CtcpReply, e.Data.Nick, "URL " + (this.CtcpUrl ?? "http://www.google.com"));

        private void CtcpUserInfoDelegate(CtcpEventArgs e) => this.SendMessage(SendType.CtcpReply, e.Data.Nick, "USERINFO " + (this.CtcpUserInfo ?? "No user info given."));

        private void CtcpVersionDelegate(CtcpEventArgs e) => this.SendMessage(SendType.CtcpReply, e.Data.Nick, "VERSION " + (this.CtcpVersion ?? this.VersionString));

        private string FilterMarker(string msg)
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

        //SendMessage(SendType.CtcpReply, e.Data.Nick, "FINGER " + this.Realname + " (" + this.Email + ") Idle " + this.Idle + " seconds (" + ((string.IsNullOrEmpty(this.Reason))?this.Reason:"-") + ") " );
        /// <summary>
        /// cleanup all old invalide DCCs (late cleaning)
        /// </summary>
        /// <param name="dc"></param>
        private void RemoveInvalidDccConnections()
        {
            //
            var invalidDc = new List<DccConnection>();
            foreach (DccConnection dc in DccConnections)
            {
                if ((!dc.Valid) && (!dc.Connected))
                {
                    invalidDc.Add(dc);
                }
            }

            foreach (DccConnection dc in invalidDc)
            {
                _DccConnections.Remove(dc);
            }
        }

        #endregion implemented ctcp delegates, can be overwritten by changing the ctcpDelagtes Dictionary
    }
}