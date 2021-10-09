using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.Enumerable {
	public static class EnumerableExtensions {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> AsList<T>(this IEnumerable<T> enumerable) => enumerable is List<T> list ? list : enumerable.ToList();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] AsArray<T>(this IEnumerable<T> enumerable) => enumerable is T[] array ? array : enumerable.ToArray();

		public static IEnumerable<T> AsType<T>(this IEnumerable enumerable) => from object item in enumerable select item is T result ? result : throw new InvalidCastException();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IndexedEnumerable<T> ToIndexed<T>(this IEnumerable<T> enumerable) => new(enumerable);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T SameOrDefault<T>(this IEnumerable<T> enumerable) => enumerable.SameOrDefault(x => x);

		public static TResult SameOrDefault<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> predicate) {
			TResult reference = default;
			var first = true;
			foreach (var item in enumerable)
				if (first) {
					reference = predicate(item);
					first = false;
				}
				else if (!predicate(item).Equals(reference))
					throw new InvalidOperationException("Values aren't the same");
			return reference;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Same<T>(this IEnumerable<T> enumerable) => enumerable.Same(x => x);

		public static TResult Same<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> predicate) {
			TResult reference = default;
			var first = true;
			foreach (var item in enumerable)
				if (first) {
					reference = predicate(item);
					first = false;
				}
				else if (!predicate(item).Equals(reference))
					throw new InvalidOperationException("Values aren't the same");
			return !first ? reference : throw new InvalidOperationException("Sequence contains no element");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<T>(this IEnumerable<T> enumerable) => enumerable.Unique(null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer) => enumerable.Unique(x => x, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> predicate) => enumerable.Unique(predicate, null);

		public static bool Unique<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> predicate, IEqualityComparer<TResult> comparer) {
			var count = 0;
			var set = comparer is null ? new HashSet<TResult>() : new HashSet<TResult>(comparer);
			foreach (var item in enumerable) {
				++count;
				set.Add(predicate(item));
			}
			return count == set.Count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action) {
			foreach (var item in enumerable)
				action(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Each<T>(this IEnumerable<T> enumerable, Action<T, int> action) {
			foreach ((var item, int index) in enumerable.ToIndexed())
				action(item, index);
		}

		public static IEnumerable<TResult> SelectSingleOrMany<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, object> selector) {
			foreach (var item in enumerable) {
				object result = selector(item);
				switch (result) {
					case null: continue;
					case IEnumerable<TResult> subEnumerable: {
						foreach (var subItem in subEnumerable)
							yield return subItem;
						break;
					}
					case TResult res:
						yield return res;
						break;
					default: throw new TypeException(result?.GetType(), $"Should be covariant with {typeof(TResult).FullName} or {typeof(IEnumerable<TResult>).FullName}");
				}
			}
		}
	}

	public class IndexedEnumerable<T> : IEnumerable<(T Value, int Index)> {
		public IndexedEnumerable(IEnumerable<T> enumerable) => Enumerable = enumerable;

		protected IEnumerable<T> Enumerable { get; }

		public IEnumerator<(T Value, int Index)> GetEnumerator() {
			var index = 0;
			foreach (var item in Enumerable)
				yield return (item, index++);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
