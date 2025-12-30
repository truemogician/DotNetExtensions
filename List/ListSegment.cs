using System;
using System.Collections;
using System.Collections.Generic;

namespace TrueMogician.Extensions.List;

public class ListSegment<T>(IReadOnlyList<T> list, int offset = 0, int count = -1) : IReadOnlyList<T> {
#if !NETSTANDARD2_0
	public ListSegment(IReadOnlyList<T> list, Index start, Index end) :
		this(list, start.GetOffset(list.Count), end.GetOffset(list.Count) - start.GetOffset(list.Count)) { }

	public ListSegment(IReadOnlyList<T> list, Range range) : this(list, range.Start, range.End) { }
#endif

	public int Count { get; } = count switch {
		> 0 => count,
		-1  => list.Count - offset,
		_   => throw new ArgumentOutOfRangeException(nameof(count))
	};

	public T this[int index] => index >= 0 && index < Count
		? list[index + offset]
		: throw new ArgumentOutOfRangeException(nameof(index));

	public IEnumerator<T> GetEnumerator() {
		for (var i = 0; i < Count; ++i)
			yield return list[i + offset];
	}

	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();
}