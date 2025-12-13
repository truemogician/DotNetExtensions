using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.Enumerable;

public static class EnumerableExtensions {
	extension<T>(IEnumerable<T> source) {
		#region SelectSingleOrMany
		/// <summary>
		///     Projects each element of a sequence to a new sequence or single element, and flattens the results into one sequence
		/// </summary>
		/// <param name="selector">
		///     A transform function to apply to each element. Should return <typeparamref name="TResult" /> or
		///     a sequence of <typeparamref name="TResult" />
		/// </param>
		/// <exception cref="TypeException" />
		public IEnumerable<TResult> SelectSingleOrMany<TResult>(Func<T, object> selector) {
			foreach (var item in source) {
				object result = selector(item);
				switch (result) {
					case null: continue;
					case IEnumerable<TResult> subEnumerable: {
						foreach (var subItem in subEnumerable)
							yield return subItem;
						break;
					}
					case TResult res: yield return res; break;
					default:          throw new TypeException(result.GetType(), $"Should be covariant with {typeof(TResult).FullName} or {typeof(IEnumerable<TResult>).FullName}");
				}
			}
		}
		#endregion

		#region Append
		/// <summary>
		///     Appends several <paramref name="items" /> to <paramref name="source" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<T> Append(params T[] items) => source.Concat(items);
		#endregion

		#region Split
		/// <summary>
		///     Splits <paramref name="source" /> into 2 parts according to the result of <paramref name="predicate" /> on each item.
		/// </summary>
		/// <returns>
		///     A tuple consists of two <see cref="List{T}" />, the first containing the items that return <see langword="true" />
		///     on <paramref name="predicate" />, the second containing the <see langword="false" /> ones.
		/// </returns>
		public (List<T> TrueList, List<T> FalseList) Split(Func<T, bool> predicate) {
			var trueList = new List<T>();
			var falseList = new List<T>();
			foreach (var item in source)
				(predicate(item) ? trueList : falseList).Add(item);
			return (trueList, falseList);
		}
		#endregion

		#region ToDictionary
		/// <summary>
		///     Creates a <see cref="Dictionary{TKey,TValue}" /> from an <see cref="IEnumerable{T}" /> according to specific
		///     element selector function.
		/// </summary>
		/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
		/// <exception cref="ArgumentException" />
		/// <exception cref="ArgumentNullException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Dictionary<T, TResult> ToDictionaryWith<TResult>(Func<T, TResult> elementSelector) =>
			source.ToDictionary(item => item, elementSelector);
		#endregion

		#region AsList, AsArray, AsIList
		/// <summary>
		///     Returns the <paramref name="source" /> itself if it's already <see cref="List{T}" />, else creates one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public List<T> AsList() => source as List<T> ?? source.ToList();

		/// <summary>
		///     Returns the <paramref name="source" /> itself if it's already an array of <typeparamref name="T" />, else creates one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] AsArray() => source as T[] ?? source.ToArray();

		/// <summary>
		///     Returns the <paramref name="source" /> itself if it's already <see cref="IList{T}" />, else creates an array of
		///     <typeparamref name="T" />.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IList<T> AsIList() => source as IList<T> ?? source.ToArray();
		#endregion

		#region ToIndexed, ToIndexDictionary
		/// <summary>
		///     Creates an <see cref="IndexedEnumerable{T}" /> from <paramref name="source" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IndexedEnumerable<T> ToIndexed() => new(source);

		/// <summary>
		///     Creates a <see cref="Dictionary{TKey,TValue}" /> that uses <paramref name="source" />'s items as key
		///     and their indices as value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Dictionary<T, int> ToIndexDictionary() => source.ToIndexed().ToDictionary(x => x.Value, x => x.Index);
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
		public T? SameOrDefault() => source.SameOrDefault(x => x);

		/// <summary>
		///     Checks whether all elements from <see cref="IEnumerable{T}" /> are equal
		///     by <paramref name="comparer" />, and returns the first element if true, or <see langword="default" /> if
		///     <see cref="IEnumerable{T}" /> is empty; otherwise, throws an <see cref="InvalidOperationException" />
		/// </summary>
		/// <returns>
		///     The first element if all elements are equal by <paramref name="comparer" />, or
		///     <see langword="default" /> if <see cref="IEnumerable{T}" /> is empty
		/// </returns>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T? SameOrDefault(IEqualityComparer<T> comparer) => source.SameOrDefault(x => x, comparer);

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
		public TResult? SameOrDefault<TResult>(Func<T, TResult> predicate) =>
			SameOrDefault(source, predicate, (a, b) => a?.Equals(b) == true);

		/// <summary>
		///     Checks whether all values projected from <see cref="IEnumerable{T}" /> by <paramref name="predicate" /> are equal
		///     by <paramref name="comparer" />, and returns the first value if true, or <see langword="default" /> if
		///     <see cref="IEnumerable{T}" /> is empty; otherwise, throws an <see cref="InvalidOperationException" />
		/// </summary>
		/// <param name="predicate">A transform function to apply to each element.</param>
		/// <returns>
		///     Projected value of the first element if all values are equal by <paramref name="comparer" />, or
		///     <see langword="default" /> if <see cref="IEnumerable{T}" /> is empty
		/// </returns>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TResult? SameOrDefault<TResult>(Func<T, TResult> predicate, IEqualityComparer<TResult> comparer) =>
			SameOrDefault(source, predicate, comparer.Equals);

		private TResult? SameOrDefault<TResult>(Func<T, TResult> predicate, Func<TResult, TResult, bool> comparer) {
			TResult? reference = default;
			var first = true;
			foreach (var item in source) {
				if (first) {
					reference = predicate(item);
					first = false;
				}
				else if (!comparer(reference!, predicate(item)))
					throw new InvalidOperationException("Values aren't the same");
			}
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
		public bool Same() => source.Same(x => x);

		/// <summary>
		///     Checks whether all elements in <see cref="IEnumerable{T}" /> are equal by <paramref name="comparer" />, or throws
		///     <see cref="InvalidOperationException" /> if <see cref="IEnumerable{T}" /> is empty.
		/// </summary>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Same(IEqualityComparer<T> comparer) => source.Same(x => x, comparer);

		/// <summary>
		///     Checks whether all values projected by <paramref name="predicate" /> from <see cref="IEnumerable{T}" /> are equal,
		///     or throws <see cref="InvalidOperationException" /> if <see cref="IEnumerable{T}" /> is empty.
		/// </summary>
		/// <param name="predicate">A transform function to apply to each element.</param>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Same<TResult>(Func<T, TResult> predicate) =>
			Same(source, predicate, (a, b) => a?.Equals(b) == true);

		/// <summary>
		///     Checks whether all values projected by <paramref name="predicate" /> from <see cref="IEnumerable{T}" /> are equal
		///     by <paramref name="comparer" />, or throws <see cref="InvalidOperationException" /> if
		///     <see cref="IEnumerable{T}" /> is empty.
		/// </summary>
		/// <param name="predicate">A transform function to apply to each element.</param>
		/// <exception cref="InvalidOperationException" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Same<TResult>(Func<T, TResult> predicate, IEqualityComparer<TResult> comparer) =>
			Same(source, predicate, comparer.Equals);

		private bool Same<TResult>(Func<T, TResult> predicate, Func<TResult, TResult, bool> comparer) {
			using var enumerator = source.GetEnumerator();
			bool success = enumerator.MoveNext();
			if (!success)
				throw new InvalidOperationException("Sequence contains no element");
			var reference = predicate(enumerator.Current);
			while (enumerator.MoveNext()) {
				if (!comparer(reference, predicate(enumerator.Current)))
					return false;
			}
			return true;
		}
		#endregion

		#region Unique
		/// <summary>
		///     Checks whether <paramref name="source" /> holds distinct values
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Unique() => source.Unique(null);

		/// <summary>
		///     Checks whether <paramref name="source" /> holds distinct values, by <see cref="IEqualityComparer{T}" />
		///     <paramref name="comparer" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Unique(IEqualityComparer<T>? comparer) => source.Unique(x => x, comparer);

		/// <summary>
		///     Checks whether <paramref name="source" /> holds distinct values produced by <paramref name="predicate" />
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Unique<TResult>(Func<T, TResult> predicate) => source.Unique(predicate, null);

		/// <summary>
		///     Checks whether <paramref name="source" /> holds distinct values produced by <paramref name="predicate" />, by
		///     <see cref="IEqualityComparer{T}" /> <paramref name="comparer" />
		/// </summary>
		public bool Unique<TResult>(Func<T, TResult> predicate, IEqualityComparer<TResult>? comparer) {
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
		///     since unused return value will be optimized out and thus <paramref name="action" /> will not be performed.
		/// </summary>
		/// <param name="action">An action to be performed on each element of <see cref="IEnumerable{T}" /></param>
		/// <returns>The <see cref="IEnumerable{T}" /> itself</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<T> Each(Action<T> action) {
			foreach (var item in source) {
				action(item);
				yield return item;
			}
		}

		/// <inheritdoc cref="Each{T}(IEnumerable{T},Action{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<T> Each(Action<T, int> action) {
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
		public void ForEach(Action<T> action) {
			foreach (var item in source)
				action(item);
		}

		/// <inheritdoc cref="ForEach{T}(IEnumerable{T},Action{T})" />
		public void ForEach(Action<T, int> action) {
			var index = 0;
			foreach (var item in source)
				action(item, index++);
		}
		#endregion

		#region IndexJoin
		/// <summary>
		///     Creates a tuple <see cref="IEnumerable{T}" />, whose element consists of elements from <paramref name="source" />
		///     and <paramref name="source2" /> at the same index.
		/// </summary>
		/// <param name="source2">
		///     The second sequence to join, should contain the same number of elements as <paramref name="source" />.
		/// </param>
		/// <returns>
		///     An <see cref="IEnumerable{T}" />, holding elements of <paramref name="source" /> and
		///     <paramref name="source2" /> at the same index in a tuple.
		/// </returns>
		/// <exception cref="InvalidOperationException">Two sequence contains different number of elements</exception>
		public IEnumerable<(T First, T2 Second)> IndexJoin<T2>(IEnumerable<T2> source2) {
			using var e1 = source.GetEnumerator();
			using var e2 = source2.GetEnumerator();
			bool status1 = e1.MoveNext(), status2 = e2.MoveNext();
			while (status1 || status2) {
				yield return (e1.Current, e2.Current);
				status1 = e1.MoveNext();
				status2 = e2.MoveNext();
			}
		}

		/// <summary>
		///     Creates a tuple <see cref="IEnumerable{T}" />, whose element consists of elements from <paramref name="source" />,
		///     <paramref name="source2" /> and <paramref name="source3" /> at the same index.
		/// </summary>
		/// <param name="source3">
		///     The third sequence to join, should contain the same number of elements as
		///     <paramref name="source" /> and <paramref name="source2" />.
		/// </param>
		/// <returns>
		///     An <see cref="IEnumerable{T}" />, holding elements of <paramref name="source" />,
		///     <paramref name="source2" /> and <paramref name="source3" /> at the same index in a tuple.
		/// </returns>
		/// <inheritdoc cref="EnumerableExtensions.IndexJoin{T,T2}" />
		public IEnumerable<(T First, T2 Second, T3 Third)> IndexJoin<T2, T3>(IEnumerable<T2> source2, IEnumerable<T3> source3) {
			using var e1 = source.GetEnumerator();
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
		///     Creates a tuple <see cref="IEnumerable{T}" />, whose element consists of elements from <paramref name="source" />,
		///     <paramref name="source2" />, <paramref name="source3" /> and <paramref name="source4" /> at the same index.
		/// </summary>
		/// <param name="source4">
		///     The fourth sequence to join, should contain the same number of elements as
		///     <paramref name="source" />, <paramref name="source2" /> and <paramref name="source3" />.
		/// </param>
		/// <returns>
		///     An <see cref="IEnumerable{T}" />, holding elements of <paramref name="source" />, <paramref name="source" />,
		///		<paramref name="source3" /> and <paramref name="source4" /> at the same index in a tuple.
		/// </returns>
		/// <inheritdoc cref="EnumerableExtensions.IndexJoin{T,T2,T3}" />
		public IEnumerable<(T First, T2 Second, T3 Third, T4 Fourth)> IndexJoin<T2, T3, T4>(IEnumerable<T2> source2, IEnumerable<T3> source3, IEnumerable<T4> source4) {
			using var e1 = source.GetEnumerator();
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
		public int IndexOf(T item) => IndexOf(source, v => item?.Equals(v) == true);

		/// <summary>
		///     Searches for the item that satisfies <paramref name="predicate" /> and returns the zero-based index of the first
		///     occurrence within the entire <see cref="IEnumerable{T}" />
		/// </summary>
		/// <returns>
		///     The zero-based index of the first occurrence of an item satisfying <paramref name="predicate" /> within the
		///     entire <see cref="IEnumerable{T}" />, if found; otherwise, -1.
		/// </returns>
		public int IndexOf(Func<T, bool> predicate) {
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
		public int LastIndexOf(T item) => LastIndexOf(source, v => item?.Equals(v) == true);

		/// <summary>
		///     Searches for the item that satisfies <paramref name="predicate" /> and returns the zero-based index of the last
		///     occurrence within the entire <see cref="IEnumerable{T}" />
		/// </summary>
		/// <returns>
		///     The zero-based index of the last occurrence of an item satisfying <paramref name="predicate" /> within the
		///     entire <see cref="IEnumerable{T}" />, if found; otherwise, -1.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int LastIndexOf(Func<T, bool> predicate) => source.Reverse().IndexOf(predicate);
		#endregion

		#region CopyTo
		/// <inheritdoc cref="List{T}.CopyTo(T[])" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] array) => CopyTo(source, array, 0);

		/// <inheritdoc cref="List{T}.CopyTo(T[], int)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] array, int arrayIndex) {
			foreach (var item in source)
				array[arrayIndex++] = item;
		}
		#endregion

		#region Count Comparer
		/// <summary>
		///     Determines whether the number of elements in the source sequence is larger than the specified
		///     <paramref name="count" />.
		/// </summary>
		/// <param name="count">The number of elements to compare against.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count" /> is negative.</exception>
		public bool CountLargerThan(int count) {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative");
			using var e = source.GetEnumerator();
			for (var i = 0; i <= count; ++i) {
				if (!e.MoveNext())
					return false;
			}
			return true;
		}

		/// <summary>
		///     Determines whether the number of elements in the source sequence is larger than or equal to the specified
		///     <paramref name="count" />.
		/// </summary>
		/// <inheritdoc cref="CountLargerThan" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CountLargerThanOrEqualTo(int count) => source.CountLargerThan(count - 1);

		/// <summary>
		///     Determines whether the number of elements in the source sequence is smaller than the specified
		///     <paramref name="count" />.
		/// </summary>
		/// <inheritdoc cref="CountLargerThan" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CountSmallerThan(int count) => !source.CountLargerThan(count - 1);

		/// <summary>
		///     Determines whether the number of elements in the source sequence is smaller than or equal to the specified
		///     <paramref name="count" />.
		/// </summary>
		/// <inheritdoc cref="CountLargerThan" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CountSmallerThanOrEqualTo(int count) => !source.CountLargerThan(count);
		#endregion

		#region Statistics
		/// <summary>
		///     Counts the occurrences of each element in the source sequence.
		/// </summary>
		/// <param name="comparer">An optional equality comparer to compare elements.</param>
		/// <returns>
		///     A dictionary where the keys are the elements from the source sequence and the values are their respective counts.
		/// </returns>
		public Dictionary<T, int> ToCountDictionary(IEqualityComparer<T>? comparer = null) {
			var dict = new Dictionary<T, int>(comparer);
			foreach (var item in source) {
				if (!dict.TryGetValue(item, out int count))
					count = 0;
				dict[item] = count + 1;
			}
			return dict;
		}

		/// <summary>
		///     Finds the mode (most frequently occurring element) in the source sequence along with its count.
		/// </summary>
		/// <param name="comparer">An optional equality comparer to compare elements.</param>
		/// <returns>
		///     A tuple containing the mode and its count. If the source sequence is empty, the mode will be null and the
		///     count will be 0.
		/// </returns>
		public (T? Mode, int Count) ModeAndCount(IEqualityComparer<T>? comparer = null) {
			var counts = source.ToCountDictionary(comparer);
			if (counts.Count == 0)
				return (default, 0);
			var pair = counts.Aggregate((l, r) => l.Value >= r.Value ? l : r);
			return (pair.Key, pair.Value);
		}

		/// <summary>
		///     Finds the mode (most frequently occurring element) in the source sequence.
		/// </summary>
		/// <returns>The mode element or <see langword="default" /> if the sequence is empty.</returns>
		/// <inheritdoc cref="ModeAndCount" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T? ModeOrDefault(IEqualityComparer<T>? comparer = null) => source.ModeAndCount(comparer).Mode;

		/// <returns>The mode element. An <see cref="InvalidOperationException" /> is thrown when the sequence is empty.</returns>
		/// <exception cref="InvalidOperationException" />
		/// <inheritdoc cref="ModeOrDefault" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Mode(IEqualityComparer<T>? comparer = null)
			=> source.ModeAndCount(comparer) is { Count: > 0 } r ? r.Mode! : throw new InvalidOperationException("Sequence contains no elements");

		/// <summary>
		///     Finds the modes (most frequently occurring elements) in the source sequence along with their count.
		/// </summary>
		/// <param name="comparer">An optional equality comparer to compare elements.</param>
		/// <returns>
		///     A tuple containing the modes and their count. If the source sequence is empty, the modes will be an empty
		///     array and the count will be 0.
		/// </returns>
		public (T[] Modes, int Count) ModesAndCount(IEqualityComparer<T>? comparer = null) {
			var counts = source.ToCountDictionary(comparer);
			if (counts.Count == 0)
				return ([], 0);
			int maxCount = counts.Values.Max();
			var modes = counts.Where(p => p.Value == maxCount).Select(kvp => kvp.Key).ToArray();
			return (modes, maxCount);
		}

		/// <summary>
		///     Finds the modes (most frequently occurring elements) in the source sequence.
		/// </summary>
		/// <param name="comparer">An optional equality comparer to compare elements.</param>
		/// <returns>An array of mode elements. If the sequence is empty, an empty array is returned.</returns>
		/// <inheritdoc cref="ModesAndCount" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] Modes(IEqualityComparer<T>? comparer = null) => source.ModesAndCount(comparer).Modes;
		#endregion
	}
}

/// <summary>
///     An enumerable class that yields index along with value.
/// </summary>
public class IndexedEnumerable<T>(IEnumerable<T> source) : IEnumerable<(T Value, int Index)> {
	public IEnumerator<(T Value, int Index)> GetEnumerator() {
		var index = 0;
		foreach (var item in Enumerable)
			yield return (item, index++);
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	protected IEnumerable<T> Enumerable { get; } = source;
}