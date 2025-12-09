using System;
using System.Collections.Generic;
using System.Linq;

#if NET5_0_OR_GREATER
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#endif

namespace TrueMogician.Extensions.Collections.Dictionary;

/// <summary>
///     Implementation of <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" /> based on
///     <see cref="TupleDictionary3D{TKey1, TKey2, TValue}" />.
/// </summary>
/// <inheritdoc cref="IPartialDictionary3D{TKey1, TKey2, TValue}" />
public class TuplePartialDictionary3D<TKey1, TKey2, TValue>
	: TupleDictionary3D<TKey1, TKey2, TValue>, IPartialDictionary3D<TKey1, TKey2, TValue> {
	public TuplePartialDictionary3D() { }

	public TuplePartialDictionary3D(int capacity) : base(capacity) { }

	public TuplePartialDictionary3D(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
		: base(comparer1, comparer2) { }

	public TuplePartialDictionary3D(IDictionary3D<TKey1, TKey2, TValue> other) : base(other) { }

	/// <inheritdoc/>
	public ICollection<TKey1> FirstKeys => Keys.Select(k => k.Item1).Distinct().ToArray();

	/// <inheritdoc/>
	public IDictionary<TKey2, TValue> this[TKey1 key1]
		=> TryGetValues(key1, out var dict) ? dict : throw new KeyNotFoundException();

	/// <inheritdoc/>
    public void Add(TKey1 key1, IEnumerable<KeyValuePair<TKey2, TValue>> values) {
		foreach (var p in values)
			Add(key1, p.Key, p.Value);
	}

	/// <inheritdoc/>
    public bool ContainsKey(TKey1 key1) => Keys.Any(k => Comparer1.Equals(k.Item1, key1));

	/// <inheritdoc/>
    public int Remove(TKey1 key1) => Keys.Where(k => Comparer1.Equals(k.Item1, key1))
		.Select(k => Remove(k.Item1, k.Item2))
		.Count(b => b);

	/// <inheritdoc/>
	public bool TryGetValues(TKey1 key1, out IDictionary<TKey2, TValue> values) {
		values = this.Where(t => Comparer1.Equals(t.Item1, key1))
			.ToDictionary(t => t.Item2, t => t.Item3);
		return values.Count > 0;
	}
}