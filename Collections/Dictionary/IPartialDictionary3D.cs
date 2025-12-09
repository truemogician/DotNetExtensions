using System;
using System.Collections.Generic;
#if !NETSTANDARD2_0
using System.Linq;
#endif

#if NET5_0_OR_GREATER
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#endif

namespace TrueMogician.Extensions.Collections.Dictionary;

/// <summary>
///     Represents a generic collection of values indexed by a composite key, allowing partial access via the first key component.
/// </summary>
/// <typeparam name="TKey1">The type of the first component of the keys in the dictionary.</typeparam>
/// <typeparam name="TKey2">The type of the second component of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
public interface IPartialDictionary3D<TKey1, TKey2, TValue> : IDictionary3D<TKey1, TKey2, TValue> {
	/// <summary>
	///     Gets an <see cref="IReadOnlyCollection{T}" /> containing the unique first key components in the
	///     <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" />.
	/// </summary>
	/// <returns>
	///     An <see cref="IReadOnlyCollection{T}" /> containing the unique first key components of the object that implements
	///     <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" />.
	/// </returns>
	IReadOnlyCollection<TKey1> FirstKeys
#if NETSTANDARD2_0
		{ get; }
#else
		=> Keys.Select(k => k.Item1).Distinct().ToArray();
#endif

	/// <summary>
	///     Gets a collection of second key components and values associated with the specified first key component.
	/// </summary>
	/// <param name="key1">The first component of the key of the elements to retrieve.</param>
	/// <exception cref="KeyNotFoundException">The property is retrieved and <paramref name="key1" /> is not found.</exception>
	/// <returns>
	///     An <see cref="IReadOnlyDictionary{TKey, TValue}" /> containing the second key components and values associated with
	///     <paramref name="key1" />.
	/// </returns>
	IReadOnlyDictionary<TKey2, TValue> this[TKey1 key1]
#if NETSTANDARD2_0
		{ get; }
#else
		=> TryGetValues(key1, out var dict) ? dict : throw new KeyNotFoundException();
#endif

	/// <summary>
	///     Adds the specified collection of key/value pairs associated with the specified first key component.
	/// </summary>
	/// <param name="key1">The object to use as the first component of the key of the elements to add.</param>
	/// <param name="values">The collection of second key components and values to add.</param>
	/// <exception cref="ArgumentException">
	///     An element with the same composite key already exists in the
	///     <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" />.
	/// </exception>
	/// <exception cref="NotSupportedException">The <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" /> is read-only.</exception>
	void Add(TKey1 key1, IEnumerable<KeyValuePair<TKey2, TValue>> values)
#if NETSTANDARD2_0
		;
#else
	{
		var pairs = values.ToArray();
		if (pairs.Any(p => ContainsKey(key1, p.Key)))
			throw new ArgumentException("An element with the same key already exists in the dictionary.", nameof(values));
		foreach (var p in pairs)
			Add(key1, p.Key, p.Value);
	}
#endif

	/// <summary>
	///     Determines whether the <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" /> contains any elements with the
	///     specified first key component.
	/// </summary>
	/// <param name="key1">
	///     The first component of the key to locate in the <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" />.
	/// </param>
	/// <returns>
	///     <see langword="true" /> if the <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" /> contains any elements with
	///     the specified first key component; otherwise, <see langword="false" />.
	/// </returns>
	bool ContainsKey(TKey1 key1)
#if NETSTANDARD2_0
		;
#else
		=> Keys.Any(k => Comparer1.Equals(k.Item1, key1));
#endif

	/// <summary>
	///     Removes all elements with the specified first key component from the <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" />.
	/// </summary>
	/// <param name="key1">The first component of the key of the elements to remove.</param>
	/// <exception cref="NotSupportedException">The <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" /> is read-only.</exception>
	/// <returns>The number of elements removed from the <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" />.</returns>
	int Remove(TKey1 key1)
#if NETSTANDARD2_0
		;
#else
	{
		var keys = Keys.Where(k => Comparer1.Equals(k.Item1, key1)).ToArray();
		var count = 0;
		foreach (var (_, key2) in keys) {
			if (Remove(key1, key2))
				++count;
		}
		return count;
	}
#endif

	/// <summary>
	///     Gets the collection of values associated with the specified first key component.
	/// </summary>
	/// <param name="key1">The first component of the key whose values to get.</param>
	/// <param name="values">
	///     When this method returns, the dictionary of second key components and values associated with the
	///     specified first key component, if the key is found; otherwise, an empty dictionary.
	/// </param>
	/// <returns>
	///     <see langword="true" /> if the <see cref="IPartialDictionary3D{TKey1, TKey2, TValue}" /> contains elements with the
	///     specified first key component; otherwise, <see langword="false" />.
	/// </returns>
	bool TryGetValues(TKey1 key1, out IReadOnlyDictionary<TKey2, TValue> values)
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