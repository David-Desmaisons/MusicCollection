using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.ToolBox.Collection.Set
{
    public interface ILetterSimpleSet<T> : IEnumerable<T> where T:class
    {
        ILetterSimpleSet<T> Add(T item,out bool success);

        ILetterSimpleSet<T> Remove(T item, out bool success);

        bool Contains(T item);

        int Count { get; } 
    }

    internal static class LetterSimpleSetFactory<T> where T:class
    {
        public static int MaxList = 10;

        static internal ILetterSimpleSet<T> GetDefault()
        {
            return new SingleSet<T>();
        }

        static internal ILetterSimpleSet<T> GetDefault(T Item)
        {
            return new SingleSet<T>(Item);
        }

        static internal ILetterSimpleSet<T> GetDefault(IEnumerable<T> Items)
        {
            if (Items == null)
                throw new ArgumentException();

            IEnumerable<T> FiItems = Items.Distinct();

            int count = FiItems.Count();
            if (count > MaxList)
                return new SimpleHashSet<T>(FiItems);

            if (count > 1)
                return new ListSet<T>(MaxList, FiItems);

            if (count == 1)
                return GetDefault(FiItems.First());

            return GetDefault();
        }
    }
}
