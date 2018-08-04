using System.Collections.Generic;

namespace ReWriteUGUI
{
    public static class NewListPool<T>
    {
        private static readonly NewObjectPool<List<T>> s_listPool =
            new NewObjectPool<List<T>>(null, list => list.Clear());


        public static List<T> Get()
        {
            return s_listPool.Get();
        }

        public static void Release(List<T> element)
        {
            s_listPool.Release(element);
        }
    }
}