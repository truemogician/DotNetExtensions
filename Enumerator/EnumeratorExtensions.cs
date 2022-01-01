using System;
using System.Collections.Generic;

namespace Enumerator {
	public static class EnumeratorExtensions {
		/// <summary>
		///     Return <paramref name="enumerator" />'s current value and move next.
		/// </summary>
		public static T GetAndMoveNext<T>(this IEnumerator<T> enumerator) {
			var result = enumerator.Current;
			enumerator.MoveNext();
			return result;
		}

		/// <summary>
		///     Move next and return <paramref name="enumerator" />'s current value.
		/// </summary>
		public static T MoveNextAndGet<T>(this IEnumerator<T> enumerator) {
			enumerator.MoveNext();
			return enumerator.Current;
		}

		/// <summary>
		///     Move <paramref name="enumerator" /> for <paramref name="step" /> times
		/// </summary>
		/// <param name="step">Number of steps to move, default is <c>1</c></param>
		/// <returns>The <paramref name="enumerator" /> itself</returns>
		public static IEnumerator<T> Move<T>(this IEnumerator<T> enumerator, int step = 1) {
			if (step < 0)
				throw new ArgumentOutOfRangeException(nameof(step), $"Parameter {nameof(step)} cannot be negative");
			while (step-- > 0)
				enumerator.MoveNext();
			return enumerator;
		}

		/// <summary>
		///     Create an <see cref="ExtendedEnumerator{T}" /> from <paramref name="enumerator" />. Note that
		///     <paramref name="enumerator" /> should be an not-started <see cref="IEnumerator{T}" /> instead of a midway one.
		/// </summary>
		public static IExtendedEnumerator<T> ToExtended<T>(this IEnumerator<T> enumerator) => new ExtendedEnumerator<T>(enumerator);

		/// <returns>
		///     <paramref name="enumerator" /> itself if <paramref name="enumerator" /> is already an
		///     <see cref="IExtendedEnumerator{T}" />, otherwise a newly created <see cref="ExtendedEnumerator{T}" />
		/// </returns>
		public static IExtendedEnumerator<T> AsExtended<T>(this IEnumerator<T> enumerator) => enumerator is IExtendedEnumerator<T> e ? e : new ExtendedEnumerator<T>(enumerator);
	}
}
