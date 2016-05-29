namespace Toubab.Beinder.Tools
{
    using System;
    using System.Collections.Generic;

    public static class LinkedListUtilities
    {
        public static LinkedList<T> Shift<T>(this LinkedList<T> list, Func<T, bool> selector)
        {
            LinkedList<T> result = new LinkedList<T>();
            while (list.First != null && selector(list.First.Value))
            {
                var node = list.First;
                list.RemoveFirst();
                result.AddLast(node);
            }
                
            return result;
        }
    }
}

