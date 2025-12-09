using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if !NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
#endif

namespace TrueMogician.Extensions.Collections.Dictionary;

/// <summary>
///     Implementation of <see cref="IDictionary3D{TKey1, TKey2, TValue}" /> based on
///     <see cref="Dictionary{TKey,TValue}" /> with tuple keys.
/// </summary>
/// <inheritdoc cref="IDictionary3D{TKey1, TKey2, TValue}" />
public class TupleDictionary3D<TKey1, TKey2, TValue> : IDictionary3D<TKey1, TKey2, TValue> {
	private readonly Dictionary<(TKey1, TKey2), TValue> _dict;

	public TupleDictionary3D() => _dict = new Dictionary<(TKey1, TKey2), TValue>();

	public TupleDictionary3D(int capacity) => _dict = new Dictionary<(TKey1, TKey2), TValue>(capacity);

	public TupleDictionary3D(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2) {
		Comparer1 = comparer1;
		Comparer2 = comparer2;
		_dict = new Dictionary<(TKey1, TKey2), TValue>(new TupleEqualityComparer<TKey1, TKey2>(comparer1, comparer2));
	}

	public TupleDictionary3D(IDictionary3D<TKey1, TKey2, TValue> other) : this(other.Comparer1, other.Comparer2) {
		foreach (var (key1, key2, value) in other)
			_dict[(key1, key2)] = value;
	}

	public TupleDictionary3D(
		IDictionary3D<TKey1, TKey2, TValue> other,
		IEqualityComparer<TKey1> comparer1,
		IEqualityComparer<TKey2> comparer2
	) : this(comparer1, comparer2) {
		foreach (var (key1, key2, value) in other)
			_dict[(key1, key2)] = value;
	}

	public int Count => _dict.Count;

	public bool IsReadOnly => false;

	public ICollection<(TKey1, TKey2)> Keys => _dict.Keys;

	public ICollection<TValue> Values => _dict.Values;

	public TValue this[TKey1 key1, TKey2 key2] {
		get => _dict.TryGetValue((key1, key2), out var value) ? value : throw new KeyNotFoundException();
		set => _dict[(key1, key2)] = value;
	}

	public IEnumerator<(TKey1, TKey2, TValue)> GetEnumerator()
		=> _dict.Select(p => (p.Key.Item1, p.Key.Item2, p.Value)).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Add((TKey1, TKey2, TValue) item)
		=> _dict.Add((item.Item1, item.Item2), item.Item3);

	public void Clear() => _dict.Clear();

	public bool Contains((TKey1, TKey2, TValue) item)
		=> _dict.TryGetValue((item.Item1, item.Item2), out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Item3);

	public void CopyTo((TKey1, TKey2, TValue)[] array, int arrayIndex) {
		if (array is null)
			throw new ArgumentNullException(nameof(array));
		if (arrayIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		if (array.Length - arrayIndex < Count)
			throw new ArgumentException("The destination array has fewer elements than the collection.");
		foreach (var tuple in this)
			array[arrayIndex++] = tuple;
	}

	public bool Remove((TKey1, TKey2, TValue) item) {
		if (_dict.TryGetValue((item.Item1, item.Item2), out var value)) {
			if (EqualityComparer<TValue>.Default.Equals(value, item.Item3)) {
				_dict.Remove((item.Item1, item.Item2));
				return true;
			}
		}
		return false;
	}

	public void Add(TKey1 key1, TKey2 key2, TValue value) => _dict.Add((key1, key2), value);

	public bool ContainsKey(TKey1 key1, TKey2 key2) => _dict.ContainsKey((key1, key2));

	public bool Remove(TKey1 key1, TKey2 key2) => _dict.Remove((key1, key2));

	public bool TryGetValue(
		TKey1 key1,
		TKey2 key2,
#if !NETSTANDARD2_0
		[MaybeNullWhen(false)]
#endif
		out TValue value
	) => _dict.TryGetValue((key1, key2), out value);

	public IEqualityComparer<TKey1> Comparer1 { get; } = EqualityComparer<TKey1>.Default;

	public IEqualityComparer<TKey2> Comparer2 { get; } = EqualityComparer<TKey2>.Default;

	/// <summary>
	///     Creates a new <see cref="TupleDictionary3D{TKey2,TKey1,TValue}" /> with transposed keys.
	/// </summary>
	public TupleDictionary3D<TKey2, TKey1, TValue> Transpose() {
		var result = new TupleDictionary3D<TKey2, TKey1, TValue>(Comparer2, Comparer1);
		foreach (var (key1, key2, value) in this)
			result[key2, key1] = value;
		return result;
	}
}

public class TupleEqualityComparer<T1, T2>(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2)
	: IEqualityComparer<(T1, T2)> {
	public bool Equals((T1, T2) x, (T1, T2) y) => comparer1.Equals(x.Item1, y.Item1) && comparer2.Equals(x.Item2, y.Item2);

	public int GetHashCode((T1, T2) obj) {
		unchecked {
			var h1 = obj.Item1 is null ? 0 : comparer1.GetHashCode(obj.Item1);
			var h2 = obj.Item2 is null ? 0 : comparer2.GetHashCode(obj.Item2);
			return (h1 * 397) ^ h2;
		}
	}
}