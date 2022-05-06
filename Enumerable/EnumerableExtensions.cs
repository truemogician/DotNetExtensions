using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.Enumerable {
	public static class EnumerableExtensions {
		#region AsList, AsArray, AsIList
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

		/// <summary>
		///     Returns the <paramref name="source" /> itself if it's already <see cref="IList{T}" />,
		///     else creates an <see cref="T:T[]" />.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IList<T> AsIList<T>(this IEnumerable<T> source) => source is IList<T> list ? list : source.ToArray();
		#endregion

		#region ToIndexed, ToIndexDictionary
		/// <summary>
		///     Creates an <see cref="IndexedEnumerable{T}" /> from <paramref name="source" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IndexedEnumerable<T> ToIndexed<T>(this IEnumerable<T> source) => new(source);

		/// <summary>
		///     Creates a <see cref="Dictionary{TKey,TValue}" /> that uses <paramref name="source" />'s items as key
		///     and their indices as value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<T, int> ToIndexDictionary<T>(this IEnumerable<T> source) => source.ToIndexed().ToDictionary(x => x.Value, x => x.Index);
		#endregion

		#region SameOrDefault
		/// <summary>
		///     Checks whether all elements from <see cref="IEnumerable{T}" /> are equal, and returns the first element if true, or
		///     <see langword="default" /> if <see cref="IEnumerable{T}" /> is empty; otherwise, throws an
		///     <see cref="InvalidOperationException" />
		/// </summary>
		/// <returns>
		///     The first element if all elements are equal, or <see langword="default" /> if <see cref="IEnumerable{T}" /> is
		///     empty
		/// </returns>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? SameOrDefault<T>(this IEnumerable<T> source) => source.SameOrDefault(x => x);

		/// <summary>
		///     Checks whether all elements from <see cref="IEnumerable{T}" /> are equal
		///     by <paramref name="comparer" />, and returns the first element if true, or <see langword="default" /> if
		///     <see cref="IEnumerable{T}" /> is empty; otherwise, throws an
		///     <see cref="InvalidOperationException" />
		/// </summary>
		/// <returns>
		///     The first element if all elements are equal by <paramref name="comparer" />, or
		///     <see langword="default" /> if <see cref="IEnumerable{T}" /> is empty
		/// </returns>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? SameOrDefault<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) => source.SameOrDefault(x => x, comparer);

		/// <summary>
		///     Checks whether all values projected from <see cref="IEnumerable{T}" /> by <paramref name="predicate" /> are equal,
		///     and returns the first value if true, or <see langword="default" /> if
		///     <see cref="IEnumerable{T}" /> is empty; otherwise, throws an <see cref="InvalidOperationException" />
		/// </summary>
		/// <param name="predicate">A transform function to apply to each element.</param>
		/// <returns>
		///     Projected value of the first element if all values are equal, or
		///     <see langword="default" /> if <see cref="IEnumerable{T}" /> is empty
		/// </returns>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult? SameOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate) =>
			SameOrDefault(source, predicate, (a, b) => a?.Equals(b) == true);

		/// <summary>
		///     Checks whether all values projected from <see cref="IEnumerable{T}" /> by <paramref name="predicate" /> are equal
		///     by <paramref name="comparer" />, and returns the first value if true, or <see langword="default" /> if
		///     <see cref="IEnumerable{T}" /> is empty; otherwise, throws an
		///     <see cref="InvalidOperationException" />
		/// </summary>
		/// <param name="predicate">A transform function to apply to each element.</param>
		/// <returns>
		///     Projected value of the first element if all values are equal by <paramref name="comparer" />, or
		///     <see langword="default" /> if <see cref="IEnumerable{T}" /> is empty
		/// </returns>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult? SameOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate, IEqualityComparer<TResult> comparer) =>
			SameOrDefault(source, predicate, comparer.Equals);

		private static TResult? SameOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate, Func<TResult, TResult, bool> comparer) {
			TResult? reference = default;
			var first = true;
			foreach (var item in source)
				if (first) {
					reference = predicate(item);
					first = false;
				}
				else if (!comparer(reference!, predicate(item)))
					throw new InvalidOperationException("Values aren't the same");
			return reference;
		}
		#endregion

		#region Same
		/// <summary>
		///     Checks whether all elements in <see cref="IEnumerable{T}" /> are equal, or throws
		///     <see cref="InvalidOperationException" /> if <see cref="IEnumerable{T}" /> is empty.
		/// </summary>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Same<T>(this IEnumerable<T> source) => source.Same(x => x);

		/// <summary>
		///     Checks whether all elements in <see cref="IEnumerable{T}" /> are equal by <paramref name="comparer" />, or throws
		///     <see cref="InvalidOperationException" /> if <see cref="IEnumerable{T}" /> is empty.
		/// </summary>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Same<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) => source.Same(x => x, comparer);

		/// <summary>
		///     Checks whether all values projected by <paramref name="predicate" /> from <see cref="IEnumerable{T}" /> are equal,
		///     or throws <see cref="InvalidOperationException" /> if
		///     <see cref="IEnumerable{T}" /> is empty.
		/// </summary>
		/// <param name="predicate">A transform function to apply to each element.</param>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Same<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate) =>
			Same(source, predicate, (a, b) => a?.Equals(b) == true);

		/// <summary>
		///     Checks whether all values projected by <paramref name="predicate" /> from <see cref="IEnumerable{T}" /> are equal
		///     by <paramref name="comparer" />, or throws <see cref="InvalidOperationException" /> if
		///     <see cref="IEnumerable{T}" /> is empty.
		/// </summary>
		/// <param name="predicate">A transform function to apply to each element.</param>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Same<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate, IEqualityComparer<TResult> comparer) =>
			Same(source, predicate, comparer.Equals);

		private static bool Same<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate, Func<TResult, TResult, bool> comparer) {
			using var enumerator = source.GetEnumerator();
			bool success = enumerator.MoveNext();
			if (!success)
				throw new InvalidOperationException("Sequence contains no element");
			var reference = predicate(enumerator.Current);
			while (enumerator.MoveNext())
				if (!comparer(reference, predicate(enumerator.Current)))
					return false;
			return true;
		}
		#endregion

		#region Unique
		/// <summary>
		///     Checks whether <paramref name="source" /> holds distinct values
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<T>(this IEnumerable<T> source) => source.Unique(null);

		/// <summary>
		///     Checks whether <paramref name="source" /> holds distinct values, by <see cref="IEqualityComparer{T}" />
		///     <paramref name="comparer" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer) => source.Unique(x => x, comparer);

		/// <summary>
		///     Checks whether <paramref name="source" /> holds distinct values produced by <paramref name="predicate" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unique<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate) => source.Unique(predicate, null);

		/// <summary>
		///     Checks whether <paramref name="source" /> holds distinct values produced by <paramref name="predicate" />, by
		///     <see cref="IEqualityComparer{T}" /> <paramref name="comparer" />
		/// </summary>
		public static bool Unique<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate, IEqualityComparer<TResult>? comparer) {
			var count = 0;
			var set = comparer is null ? new HashSet<TResult>() : new HashSet<TResult>(comparer);
			foreach (var item in source) {
				++count;
				set.Add(predicate(item));
			}
			return count == set.Count;
		}
		#endregion

		#region Each, ForEach
		/// <summary>
		///     Applies <paramref name="action" /> to each element of <see cref="IEnumerable{T}" />, and returns itself for
		///     chaining actions. <br /> Note that if chaining is not intended, use
		///     <see cref="ForEach{T}(IEnumerable{T},Action{T})" /> instead,
		///     since unused return value will be optimized out and thus <paramref name="action" /> won't get performed.
		/// </summary>
		/// <param name="action">An action to be performed on each element of <see cref="IEnumerable{T}" /></param>
		/// <returns>The <see cref="IEnumerable{T}" /> itself</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (var item in source) {
				action(item);
				yield return item;
			}
		}

		/// <inheritdoc cref="Each{T}(IEnumerable{T},Action{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T, int> action) {
			var index = 0;
			foreach (var item in source) {
				action(item, index++);
				yield return item;
			}
		}

		/// <summary>
		///     Applies <paramref name="action" /> to each element of <see cref="IEnumerable{T}" />.
		/// </summary>
		/// <param name="action">An action to be performed on each element of <see cref="IEnumerable{T}" /></param>
		/// <returns>The <see cref="IEnumerable{T}" /> itself</returns>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (var item in source)
				action(item);
		}

		/// <inheritdoc cref="ForEach{T}(IEnumerable{T},Action{T})" />
		public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
			var index = 0;
			foreach (var item in source)
				action(item, index++);
		}
		#endregion

		#region SelectSingleOrMany
		/// <summary>
		///     Projects each element of a sequence to a new sequence or single element, and flattens the results into one sequence
		/// </summary>
		/// <param name="selector">
		///     A transform function to apply to each element. Should return <typeparamref name="TResult" /> or
		///     a sequence of <typeparamref name="TResult" />
		/// </param>
		/// <exception cref="TypeException" />
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
		#endregion

		#region IndexJoin
		/// <summary>
		///     Creates a tuple <see cref="IEnumerable{T}" />, whose element consists of elements from <paramref name="source1" />
		///     and <paramref name="source2" /> at the same index.
		/// </summary>
		/// <param name="source1">
		///     The first sequence to join, should contain the same number of elements as
		///     <paramref name="source2" />.
		/// </param>
		/// <param name="source2">
		///     The second sequence to join, should contain the same number of elements as
		///     <paramref name="source1" />.
		/// </param>
		/// <returns>
		///     An <see cref="IEnumerable{T}" />, holding elements of <paramref name="source1" /> and
		///     <paramref name="source2" /> at the same index in a tuple.
		/// </returns>
		/// <exception cref="InvalidOperationException">Two sequence contains different number of elements</exception>
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

		/// <summary>
		///     Creates a tuple <see cref="IEnumerable{T}" />, whose element consists of elements from <paramref name="source1" />,
		///     <paramref name="source2" /> and <paramref name="source3" /> at the same index.
		/// </summary>
		/// <param name="source1">
		///     The first sequence to join, should contain the same number of elements as
		///     <paramref name="source2" /> and <paramref name="source3" />.
		/// </param>
		/// <param name="source2">
		///     The second sequence to join, should contain the same number of elements as
		///     <paramref name="source1" /> and <paramref name="source3" />.
		/// </param>
		/// <param name="source3">
		///     The third sequence to join, should contain the same number of elements as
		///     <paramref name="source1" /> and <paramref name="source2" />.
		/// </param>
		/// <returns>
		///     An <see cref="IEnumerable{T}" />, holding elements of <paramref name="source1" />,
		///     <paramref name="source2" /> and <paramref name="source3" /> at the same index in a tuple.
		/// </returns>
		/// <exception cref="InvalidOperationException">Three sequence contains different number of elements</exception>
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

		/// <summary>
		///     Creates a tuple <see cref="IEnumerable{T}" />, whose element consists of elements from <paramref name="source1" />,
		///     <paramref name="source2" />, <paramref name="source3" /> and <paramref name="source4" /> at the same index.
		/// </summary>
		/// <param name="source1">
		///     The first sequence to join, should contain the same number of elements as
		///     <paramref name="source2" />, <paramref name="source3" /> and <paramref name="source4" />.
		/// </param>
		/// <param name="source2">
		///     The second sequence to join, should contain the same number of elements as
		///     <paramref name="source1" />, <paramref name="source3" /> and <paramref name="source4" />.
		/// </param>
		/// <param name="source3">
		///     The third sequence to join, should contain the same number of elements as
		///     <paramref name="source1" />, <paramref name="source2" /> and <paramref name="source4" />.
		/// </param>
		/// <param name="source4">
		///     The fourth sequence to join, should contain the same number of elements as
		///     <paramref name="source1" />, <paramref name="source2" /> and <paramref name="source3" />.
		/// </param>
		/// <returns>
		///     An <see cref="IEnumerable{T}" />, holding elements of <paramref name="source1" />,
		///     <paramref name="source2" />, <paramref name="source3" /> and <paramref name="source4" /> at the same index in a
		///     tuple.
		/// </returns>
		/// <exception cref="InvalidOperationException">Four sequence contains different number of elements</exception>
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
		#endregion

		#region Append
		/// <summary>
		///     Appends several <paramref name="items" /> to <paramref name="source" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] items) => source.Concat(items);
		#endregion

		#region Split
		/// <summary>
		///     Splits <paramref name="source" /> into 2 parts according to
		///     the result of <paramref name="predicate" /> on each item.
		/// </summary>
		/// <returns>
		///     A tuple consists of two <see cref="List{T}" />, the first contains the items who returns <see langword="true" />
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
		#endregion

		#region IndexOf, LastIndexOf
		/// <summary>
		///     Searches for the specific <paramref name="item" /> and returns the zero-based index of the first occurrence within
		///     the entire <see cref="IEnumerable{T}" />
		/// </summary>
		/// <param name="item">
		///     The object to locate in the <see cref="IEnumerable{T}" />. The value can be <see langword="null" />
		///     for reference types.
		/// </param>
		/// <returns>
		///     The zero-based index of the first occurrence of <paramref name="item" /> within the entire
		///     <see cref="IEnumerable{T}" />, if found; otherwise, -1.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this IEnumerable<T> source, T item) => IndexOf(source, v => item?.Equals(v) == true);

		/// <summary>
		///     Searches for the item that satisfies <paramref name="predicate" /> and returns the zero-based index of the first
		///     occurrence within the entire <see cref="IEnumerable{T}" />
		/// </summary>
		/// <returns>
		///     The zero-based index of the first occurrence of an item satisfying <paramref name="predicate" /> within the
		///     entire <see cref="IEnumerable{T}" />, if found; otherwise, -1.
		/// </returns>
		public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
			int index = -1;
			foreach (var item in source) {
				++index;
				if (predicate(item))
					return index;
			}
			return -1;
		}

		/// <summary>
		///     Searches for the specific <paramref name="item" /> and returns the zero-based index of the last occurrence within
		///     the entire <see cref="IEnumerable{T}" />
		/// </summary>
		/// <param name="item">
		///     The object to locate in the <see cref="IEnumerable{T}" />. The value can be <see langword="null" />
		///     for reference types.
		/// </param>
		/// <returns>
		///     The zero-based index of the last occurrence of <paramref name="item" /> within the entire
		///     <see cref="IEnumerable{T}" />, if found; otherwise, -1.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOf<T>(this IEnumerable<T> source, T item) => LastIndexOf(source, v => item?.Equals(v) == true);

		/// <summary>
		///     Searches for the item that satisfies <paramref name="predicate" /> and returns the zero-based index of the last
		///     occurrence within the entire <see cref="IEnumerable{T}" />
		/// </summary>
		/// <returns>
		///     The zero-based index of the last occurrence of an item satisfying <paramref name="predicate" /> within the
		///     entire <see cref="IEnumerable{T}" />, if found; otherwise, -1.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate) => source.Reverse().IndexOf(predicate);
		#endregion

		#region CopyTo
		/// <inheritdoc cref="List{T}.CopyTo(T[])" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this IEnumerable<T> source, T[] array) => CopyTo(source, array, 0);

		/// <inheritdoc cref="List{T}.CopyTo(T[], int)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex) {
			foreach (var item in source)
				array[arrayIndex++] = item;
		}
		#endregion

		#region ToDictionary
		/// <summary>
		///     Creates a <see cref="Dictionary{TKey,TValue}" /> from an <see cref="IEnumerable{T}" /> according to specific
		///     element
		///     selector function.
		/// </summary>
		/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
		/// <exception cref="ArgumentException" />
		/// <exception cref="ArgumentNullException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Dictionary<TSource, TResult> ToDictionaryWith<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> elementSelector) =>
			source.ToDictionary(item => item, elementSelector);
		#endregion
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
