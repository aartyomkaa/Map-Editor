using System;

namespace CodeBase.Logic
{
    [Serializable]
    public class SerializationWrapper<T>
    {
        public T[] Items;

        public SerializationWrapper(T[] items)
        {
            Items = items;
        }
    }
}