using System;

namespace CraftBot.Localisation.Exceptions
{
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(string key) : base("Following key was not found:" + key)
        {
        }
    }
}