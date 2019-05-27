using System;

namespace CraftBot.Localisation.Exceptions
{
    public class ColumnNotFoundException : Exception
    {
        public ColumnNotFoundException(string key) : base("Following column was not found:" + key)
        {
        }
    }
}