using System;
using System.Collections.Generic;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace TrueMogician.Extensions.Collections {
	public interface IExtendedList<T> : IList<T> {
		#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
		public T this[Index index] {
			get {
				IList<T> iList = this;
				return index.IsFromEnd ? iList[Count - index.Value] : iList[index.Value];
			}
			set {
				IList<T> iList = this;
				if (index.IsFromEnd)
					iList[Count - index.Value] = value;
				else
					iList[index.Value] = value;
			}
		}

		public IList<T> this[Range range] {
			get {
				int start = range.Start is var l && l.IsFromEnd ? Count - l.Value : l.Value;
				int end = range.End is var r && r.IsFromEnd ? Count - r.Value : r.Value;
				return GetRange(start, end - start);
			}
		}
		#endif
		public IList<T> GetRange(int index, int count);

		public void AddRange(IEnumerable<T> items);

		public void InsertRange(int index, IEnumerable<T> items);

		public void RemoveRange(int index, int count);

		public int IndexOf(T item, int index, int count);

		public int LastIndexOf(T item, int index, int count);

		public T? Find(Predicate<T> match);

		public T? FindLast(Predicate<T> match);

		public List<T> FindAll(Predicate<T> match);

		public int FindIndex(int index, int count, Predicate<T> match);

		public int FindLastIndex(int index, int count, Predicate<T> match);

		public void Sort(int index, int count, IComparer<T> comparer);

		public void Reverse(int index, int count);

		/// <summary>
		///     Swap the item at <paramref name="index1" /> with the item at <paramref name="index2" />
		/// </summary>
		public void Swap(int index1, int index2);

		#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange(params T[] items) => AddRange((IEnumerable<T>)items);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InsertRange(int index, params T[] items) => InsertRange(index, (IEnumerable<T>)items);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int index) => IndexOf(item, index, Count - index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public new int IndexOf(T item) => IndexOf(item, 0, Count);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int IList<T>.IndexOf(T item) => IndexOf(item);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int LastIndexOf(T item, int index) => LastIndexOf(item, index, Count - index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int LastIndexOf(T item) => LastIndexOf(item, 0, Count);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindIndex(int index, Predicate<T> match) => FindIndex(index, Count - index, match);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindIndex(Predicate<T> match) => FindIndex(0, Count, match);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindLastIndex(int index, Predicate<T> match) => FindLastIndex(index, Count - index, match);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindLastIndex(Predicate<T> match) => FindLastIndex(0, Count, match);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort(IComparer<T> comparer) => Sort(0, Count, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort() => Sort(0, Count, Comparer<T>.Default);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort(int index, int count, Comparison<T> comparison) => Sort(index, count, Comparer<T>.Create(comparison));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort(Comparison<T> comparison) => Sort(0, Count, Comparer<T>.Create(comparison));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reverse() => Reverse(0, Count);
		#endif

		#if NETSTANDARD2_0
		public void AddRange(params T[] items);

		public void InsertRange(int index, params T[] items);

		public int IndexOf(T item, int index);

		public int LastIndexOf(T item, int index);

		public int LastIndexOf(T item);

		public int FindIndex(int index, Predicate<T> match);

		public int FindIndex(Predicate<T> match);

		public int FindLastIndex(int index, Predicate<T> match);

		public int FindLastIndex(Predicate<T> match);

		public void Sort(IComparer<T> comparer);

		public void Sort();

		public void Sort(int index, int count, Comparison<T> comparison);

		public void Sort(Comparison<T> comparison);

		public void Reverse();
		#endif
	}
}