using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TrueMogician.Extensions.List {
	public static class ListExtensions {
		/// <inheritdoc cref="List{T}.RemoveRange" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RemoveRange<T>(this IList<T> list, int index) => RemoveRange(list, index, list.Count - index);

		/// <inheritdoc cref="List{T}.RemoveRange" />
		public static void RemoveRange<T>(this IList<T> list, int index, int count) {
			if (index < 0 || index >= list.Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (count < 0 || index + count > list.Count)
				throw new ArgumentOutOfRangeException(nameof(count));
			if (count == 0)
				return;
			for (int i = index; i < list.Count - count; ++i)
				list[i] = list[i + count];
			for (var i = 0; i < count; ++i)
				list.RemoveAt(list.Count - 1);
		}

		/// <inheritdoc cref="List{T}.AddRange" />
		public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection) {
			foreach (var item in collection)
				list.Add(item);
		}

		/// <inheritdoc cref="List{T}.InsertRange" />
		public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> collection) {
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			var items = collection is IList<T> l ? l : collection.ToArray();
			var cache = list.GetRange(index, Math.Min(list.Count - index, items.Count));
			for (int i = index; i < index + cache.Count; ++i)
				list[i] = items[i - index];
			if (list.Count - index <= items.Count) {
				for (int i = cache.Count; i < items.Count; ++i)
					list.Add(items[i]);
				list.AddRange(cache);
			}
			else {
				int restCount = list.Count - index - items.Count;
				if (cache.Count <= restCount) {
					int length = list.Count;
					for (int i = length - cache.Count; i < length; ++i)
						list.Add(list[i]);
					for (int i = length - 1; i >= length - restCount + cache.Count; --i)
						list[i] = list[i - cache.Count];
					for (var i = 0; i < cache.Count; ++i)
						list[i + index + items.Count] = cache[i];
				}
				else {
					int length = list.Count;
					for (var i = 0; i < cache.Count - restCount; ++i)
						list.Add(cache[restCount + i]);
					for (int i = length - restCount - 1; i < length; ++i)
						list.Add(list[i]);
					for (var i = 0; i < restCount; ++i)
						list[i + index + items.Count] = cache[i];
				}
			}
		}

		/// <inheritdoc cref="List{T}.GetRange" />
		public static List<T> GetRange<T>(this IList<T> list, int index) => GetRange(list, index, list.Count - index);

		/// <inheritdoc cref="List{T}.GetRange" />
		public static List<T> GetRange<T>(this IList<T> list, int index, int count) {
			if (index < 0 || index >= list.Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (count < 0 || index + count > list.Count)
				throw new ArgumentOutOfRangeException(nameof(count));
			return list.Skip(index).Take(count).ToList();
		}

		public static IList<T> Sort<T>(this IList<T> list) => Sort(list, Comparer<T>.Default);

		/// <summary>
		///     Sorts the elements in the entire <see cref="IList{T}" /> using the specific <paramref name="comparer" />
		/// </summary>
		/// <param name="list"></param>
		/// <param name="comparer"></param>
		/// <returns>
		///     The sorted <paramref name="list" />
		/// </returns>
		public static IList<T> Sort<T>(this IList<T> list, IComparer<T> comparer) {
			switch (list) {
				case T[] array:
					Array.Sort(array, comparer);
					break;
				case List<T> lst:
					lst.Sort(comparer);
					break;
				default:
					var ordered = list.OrderBy(e => e, comparer).ToArray();
					for (var i = 0; i < list.Count; ++i)
						list[i] = ordered[i];
					break;
			}
			return list;
		}
	}
}
