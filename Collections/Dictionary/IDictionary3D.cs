using System;
using System.Collections;
using System.Collections.Generic;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace TrueMogician.Extensions.Collections.Dictionary;

/// <summary>Represents a generic collection of values indexed by a composite key.</summary>
/// <typeparam name="TKey1">The type of the first component of the keys in the dictionary.</typeparam>
/// <typeparam name="TKey2">The type of the second component of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
public interface IDictionary3D<TKey1, TKey2, TValue>
	: ICollection<(TKey1, TKey2, TValue)>, IEnumerable<(TKey1, TKey2, TValue)>, IEnumerable {
	/// <summary>
	///     Gets an <see cref="ICollection{T}" /> containing the composite keys of the
	///     <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.
	/// </summary>
	/// <returns>
	///     An <see cref="ICollection{T}" /> containing the keys of the object that implements
	///     <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.
	/// </returns>
	ICollection<(TKey1, TKey2)> Keys { get; }

	/// <summary>
	///     Gets an <see cref="ICollection{T}" /> containing the values in the
	///     <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.
	/// </summary>
	/// <returns>
	///     An <see cref="ICollection{T}" /> containing the values in the object that implements
	///     <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.
	/// </returns>
	ICollection<TValue> Values { get; }

	/// <summary>Gets or sets the element with the specified composite key.</summary>
	/// <param name="key1">The first component of the key of the element to get or set.</param>
	/// <param name="key2">The second component of the key of the element to get or set.</param>
	/// <exception cref="ArgumentNullException">
	///     <paramref name="key1" /> or <paramref name="key2" /> is <see langword="null" />.
	/// </exception>
	/// <exception cref="KeyNotFoundException">The property is retrieved and the composite key is not found.</exception>
	/// <exception cref="NotSupportedException">
	///     The property is set and the <see cref="IDictionary3D{TKey1, TKey2, TValue}" />
	///     is read-only.
	/// </exception>
	/// <returns>The element with the specified key.</returns>
	TValue this[TKey1 key1, TKey2 key2] { get; set; }

	/// <summary>
	///     Adds an element with the provided composite key and value to the
	///     <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.
	/// </summary>
	/// <param name="key1">The object to use as the first component of the key of the element to add.</param>
	/// <param name="key2">The object to use as the second component of the key of the element to add.</param>
	/// <param name="value">The object to use as the value of the element to add.</param>
	/// <exception cref="ArgumentNullException">
	///     <paramref name="key1" /> or <paramref name="key2" /> is <see langword="null" />.
	/// </exception>
	/// <exception cref="ArgumentException">
	///     An element with the same composite key already exists in the
	///     <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.
	/// </exception>
	/// <exception cref="NotSupportedException">The <see cref="IDictionary3D{TKey1, TKey2, TValue}" /> is read-only.</exception>
	void Add(TKey1 key1, TKey2 key2, TValue value);

	/// <summary>
	///     Determines whether the <see cref="IDictionary3D{TKey1, TKey2, TValue}" /> contains an element with the
	///     specified composite key.
	/// </summary>
	/// <param name="key1">The first component of the key to locate in the <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.</param>
	/// <param name="key2">The second component of the key to locate in the <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.</param>
	/// <exception cref="ArgumentNullException">
	///     <paramref name="key1" /> or <paramref name="key2" /> is <see langword="null" />.
	/// </exception>
	/// <returns>
	///     <see langword="true" /> if the <see cref="IDictionary3D{TKey1, TKey2, TValue}" /> contains an element with the key;
	///     otherwise, <see langword="false" />.
	/// </returns>
	bool ContainsKey(TKey1 key1, TKey2 key2);

	/// <summary>
	///     Removes the element with the specified composite key from the
	///     <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.
	/// </summary>
	/// <param name="key1">The first component of the key of the element to remove.</param>
	/// <param name="key2">The second component of the key of the element to remove.</param>
	/// <exception cref="ArgumentNullException">
	///     <paramref name="key1" /> or <paramref name="key2" /> is <see langword="null" />.
	/// </exception>
	/// <exception cref="NotSupportedException">The <see cref="IDictionary3D{TKey1, TKey2, TValue}" /> is read-only.</exception>
	/// <returns>
	///     <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />. This method
	///     also returns <see langword="false" /> if the composite key was not found in the original
	///     <see cref="IDictionary3D{TKey1, TKey2, TValue}" />.
	/// </returns>
	bool Remove(TKey1 key1, TKey2 key2);

	/// <summary>Gets the value associated with the specified composite key.</summary>
	/// <param name="key1">The first component of the key whose value to get.</param>
	/// <param name="key2">The second component of the key whose value to get.</param>
	/// <param name="value">
	///     When this method returns, the value associated with the specified composite key, if the key is
	///     found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is
	///     passed uninitialized.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///     <paramref name="key1" /> or <paramref name="key2" /> is <see langword="null" />.
	/// </exception>
	/// <returns>
	///     <see langword="true" /> if the object that implements <see cref="IDictionary3D{TKey1, TKey2, TValue}" /> contains
	///     an element with the specified key; otherwise, <see langword="false" />.
	/// </returns>
	bool TryGetValue(
		TKey1 key1,
		TKey2 key2,
#if NET5_0_OR_GREATER
		[MaybeNullWhen(false)]
#endif
		out TValue value
	);
}