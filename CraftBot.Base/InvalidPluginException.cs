using System;

namespace CraftBot.Base
{
    [Serializable]
    internal class InvalidPluginException : Exception
    {
        public InvalidPluginException() : base("Provided plugin seems invalid.")
        {
        }
    }
}