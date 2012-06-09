using System;

namespace Torshify.Core
{
    internal static class EventExtensions
    {
        public static void RaiseEvent(this EventHandler @event, object sender, EventArgs e)
        {
            var handler = @event;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public static void RaiseEvent<T>(this EventHandler<T> @event, object sender, T e)
            where T : EventArgs
        {
            var handler = @event;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}