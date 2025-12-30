using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TrueMogician.Extensions.List;

public static class ListExtensions {
	extension<T>(IList<T> list) {
		/// <inheritdoc cref="List{T}.RemoveRange" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveRange(int index) => RemoveRange(list, index, list.Count - index);

		/// <inheritdoc cref="List{T}.RemoveRange" />
		public void RemoveRange(int index, int count) {
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
		public void AddRange(IEnumerable<T> collection) {
			foreach (var item in collection)
				list.Add(item);
		}

		/// <inheritdoc cref="List{T}.InsertRange" />
		public void InsertRange(int index, IEnumerable<T> collection) {
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			var items = collection as IList<T> ?? collection.ToArray();
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
		public List<T> GetRange(int index) => GetRange(list, index, list.Count - index);

		/// <inheritdoc cref="List{T}.GetRange" />
		public List<T> GetRange(int index, int count) {
			if (index < 0 || index >= list.Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (count < 0 || index + count > list.Count)
				throw new ArgumentOutOfRangeException(nameof(count));
			return list.Skip(index).Take(count).ToList();
		}

		public IList<T> Sort() => Sort(list, Comparer<T>.Default);

		/// <summary>
		///     Sorts the elements in the entire <see cref="IList{T}" /> using the specific <paramref name="comparer" />
		/// </summary>
		/// <param name="comparer"></param>
		/// <returns>
		///     The sorted <paramref name="list" />
		/// </returns>
		public IList<T> Sort(IComparer<T> comparer) {
			switch (list) {
				case T[] array:   Array.Sort(array, comparer); break;
				case List<T> lst: lst.Sort(comparer); break;
				default:
					var ordered = list.OrderBy(e => e, comparer).ToArray();
					for (var i = 0; i < list.Count; ++i)
						list[i] = ordered[i];
					break;
			}
			return list;
		}

		/// <inheritdoc cref="List{T}.BinarySearch(T, IComparer{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(T item, IComparer<T> comparer) =>
			BinarySearch(list, item, comparer.Compare);

		/// <inheritdoc cref="List{T}.BinarySearch(T)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int BinarySearch(T item) => item is IComparable<T>
			? BinarySearch(list, item, (a, b) => (a as IComparable<T>)!.CompareTo(b))
			: BinarySearch(list, item, Comparer<T>.Default);

		private int BinarySearch(T item, Func<T, T, int> comparer) {
			int left = 0, right = list.Count - 1;
			while (left <= right) {
				int mid = (left + right) >> 1;
				switch (comparer(item, list[mid])) {
					case 0:   return mid;
					case < 0: right = mid - 1; break;
					case > 0: left = mid + 1; break;
				}
			}
			return ~left;
		}
	}

	extension<T>(IReadOnlyList<T> list) {
		/// <summary>
		///     Creates a <see cref="ListSegment{T}" /> that delimits the elements in the specified range.
		/// </summary>
		/// <param name="offset">The zero-based starting position of the range.</param>
		/// <param name="count">The number of elements in the range.</param>
		public ListSegment<T> Slice(int offset, int count = -1) => new(list, offset, count);

#if !NETSTANDARD2_0
		/// <param name="start">The index of the first element in the range.</param>
		/// <param name="end">The index of the end element (exclusive) in the range.</param>
		/// <inheritdoc cref="ListExtensions.Slice{T}(IReadOnlyList{T},int,int)" />
		public ListSegment<T> Slice(Index start, Index end) => new(list, start, end);

		/// <param name="range">The range of elements to slice.</param>
		/// <inheritdoc cref="ListExtensions.Slice{T}(IReadOnlyList{T},int,int)" />
		public ListSegment<T> Slice(Range range) => new(list, range);
#endif
	}
}