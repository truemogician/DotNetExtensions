using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.Enumerable {
	public static class EnumerableExtensions {
		/// <summary>
		///     Returns the <paramref name="source" /> itself if it's already <see cref="List{T}" />, else creates one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> AsList<T>(this IEnumerable<T> source) => source is List<T> list ? list : source.ToList();

		/// <summary>
		///     Returns the <paramref name="source" /> itself if it's already <see cref="T:T[]" />, else creates one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] AsArray<T>(this IEnumerable<T> source) => source is T[] array ? array : source.ToArray();

		[Obsolete("This extension method does the same job as Enumerable.Cast<T>, use this instead")]
		public static IEnumerable<T> AsType<T>(this IEnumerable source) => from object item in source select item is T result ? result : throw new InvalidCastException();

		/// <summary>
		///     Creates an <see cref="IndexedEnumerable{T}" /> from <paramref name="source" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IndexedEnumerable<T> ToIndexed<T>(this IEnumerable<T> source) => new(source);

		/// <summary>
		///     Create a <see cref="Dictionary{TKey,TValue}" /> that uses <paramref name="source" />'s items as key
		///     and their indices as value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<T, int> ToIndexDictionary<T>(this IEnumerable<T> source) => source.ToIndexed().ToDictionary(x => x.Value, x => x.Index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? SameOrDefault<T>(this IEnumerable<T> source) => source.SameOrDefault(x => x);

		public static TResult? SameOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate) {
			TResult? reference = default;
			var first = true;
			foreach (var item in source)
				if (first) {
					reference = predicate(item);
					first = false;
				}
				else if (reference?.Equals(item) != true)
					throw new InvalidOperationException("Values aren't the same");
			return reference;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Same<T>(this IEnumerable<T> source) => source.Same(x => x);

		public static TResult Same<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate) {
			using var enumerator = source.GetEnumerator();
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
		public static bool Unique<T>(this IEnumerable<T> source) => source.Unique(null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer) => source.Unique(x => x, comparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate) => source.Unique(predicate, null);

		public static bool Unique<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate, IEqualityComparer<TResult>? comparer) {
			var count = 0;
			var set = comparer is null ? new HashSet<TResult>() : new HashSet<TResult>(comparer);
			foreach (var item in source) {
				++count;
				set.Add(predicate(item));
			}
			return count == set.Count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Each<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (var item in source)
				action(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Each<T>(this IEnumerable<T> source, Action<T, int> action) {
			foreach ((var item, int index) in source.ToIndexed())
				action(item, index);
		}

		public static IEnumerable<TResult> SelectSingleOrMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, object> selector) {
			foreach (var item in source) {
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

		public static IEnumerable<(T1 First, T2 Second)> IndexJoin<T1, T2>(this IEnumerable<T1> source1, IEnumerable<T2> source2) {
			using var e1 = source1.GetEnumerator();
			using var e2 = source2.GetEnumerator();
			bool status1 = e1.MoveNext(), status2 = e2.MoveNext();
			while (status1 || status2) {
				yield return (e1.Current, e2.Current);
				status1 = e1.MoveNext();
				status2 = e2.MoveNext();
			}
		}

		public static IEnumerable<(T1 First, T2 Second, T3 Third)> IndexJoin<T1, T2, T3>(this IEnumerable<T1> source1, IEnumerable<T2> source2, IEnumerable<T3> source3) {
			using var e1 = source1.GetEnumerator();
			using var e2 = source2.GetEnumerator();
			using var e3 = source3.GetEnumerator();
			bool status1 = e1.MoveNext(), status2 = e2.MoveNext(), status3 = e3.MoveNext();
			while (status1 || status2 || status3) {
				yield return (e1.Current, e2.Current, e3.Current);
				status1 = e1.MoveNext();
				status2 = e2.MoveNext();
				status3 = e3.MoveNext();
			}
		}

		public static IEnumerable<(T1 First, T2 Second, T3 Third, T4 Fourth)> IndexJoin<T1, T2, T3, T4>(this IEnumerable<T1> source1, IEnumerable<T2> source2, IEnumerable<T3> source3, IEnumerable<T4> source4) {
			using var e1 = source1.GetEnumerator();
			using var e2 = source2.GetEnumerator();
			using var e3 = source3.GetEnumerator();
			using var e4 = source4.GetEnumerator();
			bool status1 = e1.MoveNext(), status2 = e2.MoveNext(), status3 = e3.MoveNext(), status4 = e4.MoveNext();
			while (status1 || status2 || status3 || status4) {
				yield return (e1.Current, e2.Current, e3.Current, e4.Current);
				status1 = e1.MoveNext();
				status2 = e2.MoveNext();
				status3 = e3.MoveNext();
				status4 = e4.MoveNext();
			}
		}

		/// <summary>
		///     Append several <paramref name="items" /> to <paramref name="source" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] items) => source.Concat(items);

		/// <summary>
		///     Split <paramref name="source" /> into 2 parts according to
		///     the result of <paramref name="predicate" /> on each item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="predicate"></param>
		/// <returns>
		///     A tuple consists of 2 <see cref="List{T}" />, the first contains the items who returns <see langword="true" />
		///     on <paramref name="predicate" />, the second contains the <see langword="false" /> ones.
		/// </returns>
		public static (List<T> TrueList, List<T> FalseList) Split<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
			var trueList = new List<T>();
			var falseList = new List<T>();
			foreach (var item in source)
				if (predicate(item))
					trueList.Add(item);
				else
					falseList.Add(item);
			return (trueList, falseList);
		}

		/// <inheritdoc cref="List{T}.IndexOf(T)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this IEnumerable<T> source, T item) => IndexOf(source, v => item?.Equals(v) == true);

		public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
			int index = -1;
			foreach (var item in source) {
				++index;
				if (predicate(item))
					return index;
			}
			return -1;
		}

		/// <inheritdoc cref="List{T}.LastIndexOf(T)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOf<T>(this IEnumerable<T> source, T item) => LastIndexOf(source, v => item?.Equals(v) == true);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate) => source.Reverse().IndexOf(predicate);

		/// <inheritdoc cref="List{T}.CopyTo(T[])" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this IEnumerable<T> source, T[] array) => CopyTo(source, array, 0);

		/// <inheritdoc cref="List{T}.CopyTo(T[], int)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex) {
			foreach (var item in source)
				array[arrayIndex++] = item;
		}
	}

	/// <summary>
	///     An enumerable class that yields index along with value.
	/// </summary>
	public class IndexedEnumerable<T> : IEnumerable<(T Value, int Index)> {
		public IndexedEnumerable(IEnumerable<T> source) => Enumerable = source;

		protected IEnumerable<T> Enumerable { get; }

		public IEnumerator<(T Value, int Index)> GetEnumerator() {
			var index = 0;
			foreach (var item in Enumerable)
				yield return (item, index++);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
