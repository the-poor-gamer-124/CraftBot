using System;
using System.Threading;
using System.Reflection;

namespace CraftBot.Helper
{
    public static class ThreadEx
    {
        public static void Abort(this Thread thread)
        {
            MethodInfo abort = null;
            foreach (MethodInfo m in thread.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (m.Name.Equals("AbortInternal") && m.GetParameters().Length == 0) abort = m;
            }
            if (abort == null)
            {
                //throw new Exception("Failed to get Thread.Abort method");
            }
            else
            {
                abort.Invoke(thread, new object[0]);
            }
        }
    }
}