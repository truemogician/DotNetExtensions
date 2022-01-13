using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TrueMogician.Extensions.Enumerable;

namespace TrueMogician.Extensions.Collections {
	public class ListSegment<T> : IList<T>, IReadOnlyList<T> {
		public ListSegment(IList<T> list) : this(list, 0) { }

		public ListSegment(IList<T> list, int offset) : this(list, offset, list.Count - offset) { }

		public ListSegment(IList<T> list, int offset, int count) {
			if (offset < 0 || offset >= list.Count - 1)
				throw new ArgumentOutOfRangeException(nameof(offset));
			if (count < 0 || offset + count > list.Count)
				throw new ArgumentOutOfRangeException(nameof(count));
			Buffer = list;
			Offset = offset;
			Count = count;
		}

		int ICollection<T>.Count => Count;

		public bool IsReadOnly => false;

		int IReadOnlyCollection<T>.Count => Count;

		public T this[int index] {
			get {
				ThrowIfOutOfRange(index, nameof(index));
				return Buffer[Offset + index];
			}
			set {
				ThrowIfOutOfRange(index, nameof(index));
				Buffer[Offset + index] = value;
			}
		}

		public IEnumerator<T> GetEnumerator() {
			for (int i = Offset; i < Offset + Count; ++i)
				yield return Buffer[i];
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(T item) {
			Buffer.Insert(Offset + Count, item);
			++Count;
		}

		public void Clear() {
			for (int i = Offset; i < Buffer.Count - Count; ++i)
				Buffer[i] = Buffer[i + Count];
			for (var i = 0; i < Count; ++i)
				Buffer.RemoveAt(Buffer.Count - 1);
			Count = 0;
		}

		public bool Contains(T item) => Enumerable.Contains(item);

		public void CopyTo(T[] array, int arrayIndex) => Enumerable.CopyTo(array, arrayIndex);

		public bool Remove(T item) {
			int index = IndexOf(item);
			if (index == -1)
				return false;
			RemoveAt(index);
			return true;
		}

		public int IndexOf(T item) => Enumerable.IndexOf(item);

		public void Insert(int index, T item) {
			ThrowIfOutOfRange(index, nameof(index));
			Buffer.Insert(Offset + index, item);
			++Count;
		}

		public void RemoveAt(int index) {
			ThrowIfOutOfRange(index, nameof(index));
			Buffer.RemoveAt(Offset + index);
			--Count;
		}

		private IEnumerable<T> Enumerable => Buffer.Skip(Offset).Take(Count);

		public int Count { get; private set; }

		public IList<T> Buffer { get; }

		public int Offset { get; }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		public ListSegment<T> this[Range range] => Slice(range.Start.ToInt(Count), range.End.ToInt(Count));
#endif

		public ListSegment<T> Slice(int offset) => Slice(offset, Count - offset);

		public ListSegment<T> Slice(int offset, int count) {
			ThrowIfOutOfRange(offset, nameof(offset));
			if (offset + count > Count)
				throw new ArgumentOutOfRangeException(nameof(count));
			return new ListSegment<T>(Buffer, Offset + offset, count);
		}

		public override bool Equals(object? obj) {
			if (obj is null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			return obj switch {
				ListSegment<T> seg => Equals(seg),
				IList<T> list      => Equals(list),
				_                  => false
			};
		}

		protected bool Equals(ListSegment<T> other) => Count == other.Count && Buffer.Equals(other.Buffer) && Offset == other.Offset;

		protected bool Equals(IList<T> list) => Buffer.Equals(list) && Offset == 0 && Count == list.Count;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		public override int GetHashCode() => HashCode.Combine(Buffer, Offset, Count);
#else
		public override int GetHashCode() {
			unchecked {
				int hashCode = Count;
				hashCode = (hashCode * 397) ^ Buffer.GetHashCode();
				hashCode = (hashCode * 397) ^ Offset;
				return hashCode;
			}
		}
#endif

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfOutOfRange(int index, string paramName) {
			if (index >= Count || index < 0)
				throw new ArgumentOutOfRangeException(paramName);
		}

		public static bool operator ==(ListSegment<T> seg1, ListSegment<T> seg2) => seg1.Equals(seg2);

		public static bool operator !=(ListSegment<T> seg1, ListSegment<T> seg2) => !(seg1 == seg2);

		public static bool operator ==(ListSegment<T> seg1, List<T> seg2) => seg1.Equals(seg2);

		public static bool operator !=(ListSegment<T> seg1, List<T> seg2) => !(seg1 == seg2);

		public static bool operator ==(ListSegment<T> seg1, ControllableList<T> seg2) => seg1.Equals(seg2);

		public static bool operator !=(ListSegment<T> seg1, ControllableList<T> seg2) => !(seg1 == seg2);

		public static implicit operator ListSegment<T>(List<T> list) => new(list);

		public static implicit operator ListSegment<T>(ControllableList<T> list) => new(list);
	}
}