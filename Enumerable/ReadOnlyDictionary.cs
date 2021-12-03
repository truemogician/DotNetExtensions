using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.Enumerable {
	public class ValueReadOnlyDictionary<TKey, TValue, TReadOnlyValue> : IReadOnlyDictionary<TKey, TReadOnlyValue> where TValue : TReadOnlyValue {
		private readonly IDictionary<TKey, TValue> _dictionary;

		public ValueReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) => _dictionary = dictionary;

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<TKey, TReadOnlyValue>> GetEnumerator()
			=> _dictionary
				.Select(x => new KeyValuePair<TKey, TReadOnlyValue>(x.Key, x.Value))
				.GetEnumerator();

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <inheritdoc />
		public int Count => _dictionary.Count;

		/// <inheritdoc />
		public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

		/// <inheritdoc />
		public bool TryGetValue(TKey key, out TReadOnlyValue value) {
			bool result = _dictionary.TryGetValue(key, out var v);
			value = v;
			return result;
		}

		/// <inheritdoc />
		public TReadOnlyValue this[TKey key] => _dictionary[key];

		/// <inheritdoc />
		public IEnumerable<TKey> Keys => _dictionary.Keys;

		/// <inheritdoc />
		public IEnumerable<TReadOnlyValue> Values => _dictionary.Values.AsType<TReadOnlyValue>();
	}

	public class KeyReadOnlyDictionary<TKey, TValue, TReadOnlyKey> : IReadOnlyDictionary<TReadOnlyKey, TValue> where TKey : TReadOnlyKey {
		private readonly IDictionary<TKey, TValue> _dictionary;

		public KeyReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) => _dictionary = dictionary;

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<TReadOnlyKey, TValue>> GetEnumerator()
			=> _dictionary
				.Select(x => new KeyValuePair<TReadOnlyKey, TValue>(x.Key, x.Value))
				.GetEnumerator();

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <inheritdoc />
		public int Count => _dictionary.Count;

		/// <param name="key">
		///     The key to locate in the <see cref="KeyReadOnlyDictionary{TKey,TValue,TReadOnlyKey}" />. Note that
		///     though the type is <see cref="TReadOnlyKey" />, it has to be <see cref="TKey" />, or an
		///     <see cref="InvariantTypeException" /> will be thrown
		/// </param>
		/// <inheritdoc />
		public bool ContainsKey(TReadOnlyKey key) => key is TKey realKey ? _dictionary.ContainsKey(realKey) : throw new InvariantTypeException(typeof(TKey), key?.GetType());

		/// <param name="key">
		///     <inheritdoc cref="ContainsKey" />
		/// </param>
		/// <inheritdoc />
		public bool TryGetValue(TReadOnlyKey key, out TValue value) => key is TKey realKey ? _dictionary.TryGetValue(realKey, out value) : throw new InvariantTypeException(typeof(TKey), key?.GetType());

		/// <param name="key">
		///     <inheritdoc cref="ContainsKey" />
		/// </param>
		/// <inheritdoc />
		public TValue this[TReadOnlyKey key] => key is TKey realKey ? _dictionary[realKey] : throw new InvariantTypeException(typeof(TKey), key?.GetType());

		/// <inheritdoc />
		public IEnumerable<TReadOnlyKey> Keys => _dictionary.Keys.AsType<TReadOnlyKey>();

		/// <inheritdoc />
		public IEnumerable<TValue> Values => _dictionary.Values;
	}

	public class KeyValueReadOnlyDictionary<TKey, TValue, TReadOnlyKey, TReadOnlyValue> : IReadOnlyDictionary<TReadOnlyKey, TReadOnlyValue> where TKey : TReadOnlyKey where TValue : TReadOnlyValue {
		private readonly IDictionary<TKey, TValue> _dictionary;

		public KeyValueReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) => _dictionary = dictionary;

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<TReadOnlyKey, TReadOnlyValue>> GetEnumerator()
			=> _dictionary
				.Select(x => new KeyValuePair<TReadOnlyKey, TReadOnlyValue>(x.Key, x.Value))
				.GetEnumerator();

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <inheritdoc />
		public int Count => _dictionary.Count;

		/// <param name="key">
		///     <inheritdoc cref="KeyReadOnlyDictionary{TKey,TValue,TReadOnlyKey}.ContainsKey" />
		/// </param>
		/// <inheritdoc />
		public bool ContainsKey(TReadOnlyKey key) => key is TKey realKey ? _dictionary.ContainsKey(realKey) : throw new InvariantTypeException(typeof(TKey), key?.GetType());

		/// <param name="key">
		///     <inheritdoc cref="ContainsKey" />
		/// </param>
		/// <inheritdoc />
		public bool TryGetValue(TReadOnlyKey key, out TReadOnlyValue value) {
			if (key is not TKey realKey)
				throw new InvariantTypeException(typeof(TKey), key?.GetType());
			bool result = _dictionary.TryGetValue(realKey, out var v);
			value = v;
			return result;
		}

		/// <param name="key">
		///     <inheritdoc cref="ContainsKey" />
		/// </param>
		/// <inheritdoc />
		public TReadOnlyValue this[TReadOnlyKey key] => key is TKey realKey ? _dictionary[realKey] : throw new InvariantTypeException(typeof(TKey), key?.GetType());

		/// <inheritdoc />
		public IEnumerable<TReadOnlyKey> Keys => _dictionary.Keys.AsType<TReadOnlyKey>();

		/// <inheritdoc />
		public IEnumerable<TReadOnlyValue> Values => _dictionary.Values.AsType<TReadOnlyValue>();
	}

	public static class DictionaryExtensions {
		public static ValueReadOnlyDictionary<TKey, TValue, TReadOnlyValue> ToValueReadOnly<TKey, TValue, TReadOnlyValue>(this IDictionary<TKey, TValue> dictionary) where TValue : TReadOnlyValue => new(dictionary);

		public static KeyReadOnlyDictionary<TKey, TValue, TReadOnlyKey> ToKeyReadOnly<TKey, TValue, TReadOnlyKey>(this IDictionary<TKey, TValue> dictionary) where TKey : TReadOnlyKey => new(dictionary);

		public static KeyValueReadOnlyDictionary<TKey, TValue, TReadOnlyKey, TReadOnlyValue> ToKeyReadOnly<TKey, TValue, TReadOnlyKey, TReadOnlyValue>(this IDictionary<TKey, TValue> dictionary) where TKey : TReadOnlyKey where TValue : TReadOnlyValue => new(dictionary);
	}
}