using System.Collections.Generic;
using UnityEngine.Events;

namespace ReWriteUGUI
{
    public class NewObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();

        private readonly UnityAction<T> m_ActionOnGet;

        private readonly UnityAction<T> m_ActionOnRelease;


        public NewObjectPool(UnityAction<T> onGet, UnityAction<T> onRelease)
        {
            this.m_ActionOnGet = onGet;
            this.m_ActionOnRelease = onRelease;
        }


        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
                element = new T();
            else
                element = m_Stack.Pop();

            if (m_ActionOnGet != null)
                m_ActionOnGet(element);

            return element;
        }


        public void Release(T element)
        {
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);

            m_Stack.Push(element);
        }
    }
}