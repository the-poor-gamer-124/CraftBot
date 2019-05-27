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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Meebey.SmartIrc4net
{
    /// <summary>
    ///
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public class Channel
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"> </param>
        internal Channel(string name)
        {
            this.Name = name;
            this.ActiveSyncStart = DateTime.Now;
        }

#if LOG4NET
        ~Channel()
        {
            Logger.ChannelSyncing.Debug("Channel ("+Name+") destroyed");
        }
#endif

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        internal Hashtable UnsafeOps { get; } = Hashtable.Synchronized(new Hashtable(StringComparer.OrdinalIgnoreCase));

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        internal Hashtable UnsafeUsers { get; } = Hashtable.Synchronized(new Hashtable(StringComparer.OrdinalIgnoreCase));

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        internal Hashtable UnsafeVoices { get; } = Hashtable.Synchronized(new Hashtable(StringComparer.OrdinalIgnoreCase));

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public DateTime ActiveSyncStart { get; }

        private DateTime _activeSyncStop;

        public DateTime ActiveSyncStop
        {
            get => _activeSyncStop;
            set
            {
                _activeSyncStop = value;
                ActiveSyncTime = ActiveSyncStop.Subtract(this.ActiveSyncStart);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public TimeSpan ActiveSyncTime { get; private set; }

        public List<string> BanExceptions { get; } = new List<string>();

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public StringCollection Bans { get; } = new StringCollection();

        public List<string> InviteExceptions { get; } = new List<string>();

        public bool IsSycned { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public string Mode { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public string Name { get; }

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public Hashtable Ops => (Hashtable)this.UnsafeOps.Clone();

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public int UserLimit { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public Hashtable Users => (Hashtable)this.UnsafeUsers.Clone();

        /// <summary>
        ///
        /// </summary>
        /// <value> </value>
        public Hashtable Voices => (Hashtable)this.UnsafeVoices.Clone();
    }
}