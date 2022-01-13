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
		public static Dictionary<T, int> ToIndexDictionary<T>(this IEnumerable<T> enumerable) => enumerable.ToIndexed().ToDictionary(x => x.Value, x => x.Index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? SameOrDefault<T>(this IEnumerable<T> enumerable) => enumerable.SameOrDefault(x => x);

		public static TResult? SameOrDefault<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> predicate) {
			TResult? reference = default;
			var first = true;
			foreach (var item in enumerable)
				if (first) {
					reference = predicate(item);
					first = false;
				}
				else if (reference?.Equals(item) != true)
					throw new InvalidOperationException("Values aren't the same");
			return reference;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Same<T>(this IEnumerable<T> enumerable) => enumerable.Same(x => x);

		public static TResult Same<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> predicate) {
			using var enumerator = enumerable.GetEnumerator();
			bool success = enumerator.MoveNext();
			if (!success)
				throw new InvalidOperationException("Sequence contains no element");
			var reference = predicate(enumerator.Current);
			while (enumerator.MoveNext())
				if (reference?.Equals(predicate(enumerator.Current)) != true)
					throw new InvalidOperationException("Values aren't the same");
			return reference;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<T>(this IEnumerable<T> enumerable) => enumerable.Unique(null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<T>(this IEnumerable<T> enumerable, IEqualityComparer<T>? comparer) => enumerable.Unique(x => x, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> predicate) => enumerable.Unique(predicate, null);

		public static bool Unique<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> predicate, IEqualityComparer<TResult>? comparer) {
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
					default: throw new TypeException(result.GetType(), $"Should be covariant with {typeof(TResult).FullName} or {typeof(IEnumerable<TResult>).FullName}");
				}
			}
		}

		public static IEnumerable<(T1 First, T2 Second)> IndexJoin<T1, T2>(this IEnumerable<T1> enumerable1, IEnumerable<T2> enumerable2) {
			using var e1 = enumerable1.GetEnumerator();
			using var e2 = enumerable2.GetEnumerator();
			bool status1 = e1.MoveNext(), status2 = e2.MoveNext();
			while (status1 || status2) {
				yield return (e1.Current, e2.Current);
				status1 = e1.MoveNext();
				status2 = e2.MoveNext();
			}
		}

		public static IEnumerable<(T1 First, T2 Second, T3 Third)> IndexJoin<T1, T2, T3>(this IEnumerable<T1> enumerable1, IEnumerable<T2> enumerable2, IEnumerable<T3> enumerable3) {
			using var e1 = enumerable1.GetEnumerator();
			using var e2 = enumerable2.GetEnumerator();
			using var e3 = enumerable3.GetEnumerator();
			bool status1 = e1.MoveNext(), status2 = e2.MoveNext(), status3 = e3.MoveNext();
			while (status1 || status2 || status3) {
				yield return (e1.Current, e2.Current, e3.Current);
				status1 = e1.MoveNext();
				status2 = e2.MoveNext();
				status3 = e3.MoveNext();
			}
		}

		public static IEnumerable<(T1 First, T2 Second, T3 Third, T4 Fourth)> IndexJoin<T1, T2, T3, T4>(this IEnumerable<T1> enumerable1, IEnumerable<T2> enumerable2, IEnumerable<T3> enumerable3, IEnumerable<T4> enumerable4) {
			using var e1 = enumerable1.GetEnumerator();
			using var e2 = enumerable2.GetEnumerator();
			using var e3 = enumerable3.GetEnumerator();
			using var e4 = enumerable4.GetEnumerator();
			bool status1 = e1.MoveNext(), status2 = e2.MoveNext(), status3 = e3.MoveNext(), status4 = e4.MoveNext();
			while (status1 || status2 || status3 || status4) {
				yield return (e1.Current, e2.Current, e3.Current, e4.Current);
				status1 = e1.MoveNext();
				status2 = e2.MoveNext();
				status3 = e3.MoveNext();
				status4 = e4.MoveNext();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, params T[] items) => enumerable.Concat(items);

		public static (IList<T> TrueList, IList<T> FalseList) Split<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) {
			var trueList = new List<T>();
			var falseList = new List<T>();
			foreach (var item in enumerable)
				if (predicate(item))
					trueList.Add(item);
				else
					falseList.Add(item);
			return (trueList, falseList);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this IEnumerable<T> enumerable, T item) => IndexOf(enumerable, v => item?.Equals(v) == true);

		public static int IndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) {
			int index = -1;
			foreach (var item in enumerable) {
				++index;
				if (predicate(item))
					return index;
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOf<T>(this IEnumerable<T> enumerable, T item) => LastIndexOf(enumerable, v => item?.Equals(v) == true);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) => enumerable.Reverse().IndexOf(predicate);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this IEnumerable<T> enumerable, T[] array) => CopyTo(enumerable, array, 0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this IEnumerable<T> enumerable, T[] array, int arrayIndex) {
			foreach (var item in enumerable)
				array[arrayIndex++] = item;
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
