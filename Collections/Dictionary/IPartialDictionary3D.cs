using System.Collections.Generic;
#if !NETSTANDARD2_0
using System.Linq;
#endif

#if NET5_0_OR_GREATER
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#endif

namespace TrueMogician.Extensions.Collections.Dictionary;

public interface IPartialDictionary3D<TKey1, TKey2, TValue> : IDictionary3D<TKey1, TKey2, TValue> {
	ICollection<TKey1> FirstKeys
#if NETSTANDARD2_0
		{ get; }
#else
		=> Keys.Select(k => k.Item1).Distinct().ToArray();
#endif

	IDictionary<TKey2, TValue> this[TKey1 key1]
#if NETSTANDARD2_0
		{ get; }
#else
		=> TryGetValues(key1, out var dict) ? dict : throw new KeyNotFoundException();
#endif

	void Add(TKey1 key1, IEnumerable<KeyValuePair<TKey2, TValue>> values)
#if NETSTANDARD2_0
		;
#else
	{
		foreach (var (key2, value) in values)
			Add(key1, key2, value);
	}
#endif

	bool ContainsKey(TKey1 key1)
#if NETSTANDARD2_0
		;
#else
		=> Keys.Any(k => Comparer1.Equals(k.Item1, key1));
#endif

	int Remove(TKey1 key1)
#if NETSTANDARD2_0
		;
#else
		=> Keys.Where(k => Comparer1.Equals(k.Item1, key1))
			.Select(k => Remove(k.Item1, k.Item2))
			.Count(b => b);
#endif

	bool TryGetValues(TKey1 key1, out IDictionary<TKey2, TValue> values)
#if NETSTANDARD2_0
		;
#else
	{
		values = this.Where(t => Comparer1.Equals(t.Item1, key1))
			.ToDictionary(t => t.Item2, t => t.Item3);
		return values.Count > 0;
	}
#endif
}