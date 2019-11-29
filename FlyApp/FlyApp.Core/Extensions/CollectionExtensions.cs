using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlyApp.Core.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Inserts the <paramref name="items"/> into the <paramref name="list"/> in a sorted manner.
        /// Sorting is specified using <paramref name="comparer"/>. If the letter is <code>null</code>, 
        /// items will be appended at the end of <paramref name="list"/>
        /// </summary>
        /// <typeparam name="T">Type of the list</typeparam>
        /// <param name="list">The list in which we need to insert.</param>
        /// <param name="items">The items to insert.</param>
        /// <param name="comparer">The comparer.</param>
        public static void InsertSorted<T>(this IList<T> list, IEnumerable<T> items, Comparison<T> comparer = null)
        {
            if(items == null)
            {
                return;
            }

            list = list ?? new List<T>();
            if(comparer == null)
            {
                foreach(T obj in items)
                {
                    list.Add(obj);
                }
            }
            else
            {
                List<T> sorted = items.ToList();
                sorted.Sort(comparer);
                int first = 0, second = 0;
                while(second < sorted.Count)
                {
                    if(first >= list.Count)
                    {
                        list.Add(sorted[second++]);
                    }
                    else if(comparer.Invoke(list[first], sorted[second]) > 0)
                    {
                        list.Insert(first++, sorted[second++]);
                    }
                    else if(comparer.Invoke(list[first], sorted[second]) == 0)
                    {
                        first++;
                        second++;
                    }
                    else
                    {
                        first++;
                    }
                }
            }
        }

        public static void AddSorted<T>(this IList<T> list, T item) where T : IComparable
        {
            if(list.Count == 0)
            {
                list.Add(item);
                return;
            }

            if(list[list.Count - 1].CompareTo(item) <= 0)
            {
                list.Add(item);
                return;
            }

            if(list[0].CompareTo(item) >= 0)
            {
                list.Insert(0, item);
                return;
            }

            int index = list.ToList().BinarySearch(item);
            if(index < 0)
            {
                index = ~index;
            }

            list.Insert(index, item);
        }

        /// <summary>
        /// Counts the number of items for non-generic <see cref="IEnumerable"/> collection.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>The count.</returns>
        public static int Count(this IEnumerable enumerable)
        {
            if(enumerable == null)
            {
                return 0;
            }

            if(enumerable is ICollection itemsList)
            {
                return itemsList.Count;
            }

            IEnumerator enumerator = enumerable.GetEnumerator();
            var count = 0;
            while(enumerator.MoveNext())
            {
                count++;
            }

            return count;
        }

        /// <summary>
        /// Gets the element at specified <paramref name="position"/> for a non-generic <see cref="IEnumerable"/> collection.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="position">The position.</param>
        /// <returns>Element at specified position</returns>
        public static object GetElementAt(this IEnumerable items, int position)
        {
            if(items == null)
            {
                return null;
            }

            if(items is IList itemsList)
            {
                return itemsList[position];
            }

            IEnumerator enumerator = items.GetEnumerator();
            for(var index = 0; index <= position; index++)
            {
                enumerator.MoveNext();
            }

            return enumerator.Current;
        }

        /// <summary>
        /// Gets the element at specified <paramref name="position"/> for a generic <see cref="IEnumerable{T}"/> collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="position">The position.</param>
        /// <returns>Element at specified position</returns>
        public static T GetItemAtIndex<T>(this IEnumerable<T> items, int position)
        {
            object item = items.GetElementAt(position);
            return (T) item;
        }

        /// <summary>
        /// Returns the index of the <paramref name="obj"/> inside non-generic <see cref="IEnumerable"/> collection.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static int IndexOf(this IEnumerable items, object obj)
        {
            if(items is IList itemsList)
            {
                return itemsList.IndexOf(obj);
            }

            var index = 0;
            foreach(object item in items)
            {
                if(item == obj)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Appends the range of items specified by <paramref name="toInsert"/> at the end <paramref name="items"/> list.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="toInsert">The range of items to insert.</param>
        public static void AddRange(this IList items, IEnumerable toInsert)
        {
            foreach(object item in toInsert)
            {
                items.Add(item);
            }
        }

        /// <summary>
        /// Appends the range of items specified by <paramref name="toInsert"/> at the end <paramref name="items"/> list.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="toInsert">The range of items to insert.</param>
        public static void AddRange<T>(this IList<T> items, IEnumerable<T> toInsert)
        {
            foreach(T item in toInsert)
            {
                items.Add(item);
            }
        }

        /// <summary>
        /// Inserts the range of items specified by <paramref name="toInsert"/> into the <paramref name="items"/> list at <paramref name="index"/> position.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="index">The position to insert.</param>
        /// <param name="toInsert">The range of items to insert.</param>
        public static void InsertRange(this IList items, int index, IEnumerable toInsert)
        {
            foreach(object item in toInsert)
            {
                items.Insert(index++, item);
            }
        }

        /// <summary>
        /// Casts a non-generic <paramref name="items"/> list to object array
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>Object array.</returns>
        public static object[] ToArray(this IEnumerable items)
        {
            if(items == null)
            {
                return null;
            }

            IEnumerator enumerator = items.GetEnumerator();
            List<object> result = new List<object>();
            while(enumerator.MoveNext())
            {
                result.Add(enumerator.Current);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Compares to <see cref="IEnumerable"/> collections bit by bit for equality using <see cref="ObjectExtensions.ObjectsEqual"/> extension method.
        /// Collections are considered equal if they are both <code>null</code>, or they have same length and are equal bit by bit.
        /// </summary>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <returns><code>true</code> if collections are considered equal</returns>
        public static bool EnumerableEqual(this IEnumerable first, IEnumerable second)
        {
            if(first == null && second == null)
            {
                return true;
            }

            if(first == null || second == null)
            {
                return false;
            }

            IEnumerator enumerator1 = first.GetEnumerator();
            IEnumerator enumerator2 = second.GetEnumerator();
            do
            {
                bool hasNext1 = enumerator1.MoveNext();
                bool hasNext2 = enumerator2.MoveNext();
                if(hasNext1 != hasNext2)
                {
                    return false;
                }

                if(hasNext1 == false)
                {
                    return true;
                }

                object value1 = enumerator1.Current;
                object value2 = enumerator2.Current;
                if(!value1.ObjectsEqual(value2))
                {
                    return false;
                }
            } while(true);
        }

        /// <summary>
        /// Gets the sub list inside <paramref name="collection"/> starting at index <paramref name="startIndex"/> with length <paramref name="count"/> or until
        /// the method reaches the end of collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>Sublist from collection.</returns>
        public static List<T> GetSubList<T>(this IEnumerable<T> collection, int startIndex, int count)
        {
            List<T> list = new List<T>();
            IList<T> enumerable = collection as IList<T> ?? collection.ToList();
            count = Math.Min(enumerable.Count, startIndex + count);
            for(int index = startIndex; index < count; index++)
            {
                list.Add(enumerable.GetItemAtIndex(index));
            }

            return list;
        }

        /// <summary>
        /// An extension method to allow mapping the <paramref name="source"/> collection to another collection
        /// using the <paramref name="mapper"/> transformation function
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="mapper">The mapper function.</param>
        /// <returns></returns>
        public static List<TDestination> MapTo<TSource, TDestination>(this IEnumerable<TSource> source, Func<TSource, TDestination> mapper)
        {
            return source.Select(mapper.Invoke).ToList();
        }

        /// <summary>
        /// Extension method to be able to enumerate items of <paramref name="enumeration"/> collection in linq expressions.
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="enumeration">The collection.</param>
        /// <param name="action">The action to execute for each item.</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach(T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Extension method to be able to enumerate items of <paramref name="enumeration"/> collection in linq expressions with index.
        /// </summary>
        /// <typeparam name="T">The collection</typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <param name="action">The action to execute for each item.</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T, int> action)
        {
            var idx = 0;
            foreach(T item in enumeration)
            {
                action(item, idx++);
            }
        }

        public static void ReloadItem<T>(this IList<T> items, T item)
        {
            int index = items.IndexOf(item);
            items.Remove(item);
            items.Insert(index, item);
        }
    }
}