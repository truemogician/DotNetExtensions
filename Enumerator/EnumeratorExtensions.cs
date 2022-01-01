using System;
using System.Collections.Generic;

namespace Enumerator {
	public static class EnumeratorExtensions {
		public static T GetAndMoveNext<T>(this IEnumerator<T> enumerator) {
			var result = enumerator.Current;
			enumerator.MoveNext();
			return result;
		}

		public static T MoveNextAndGet<T>(this IEnumerator<T> enumerator) {
			enumerator.MoveNext();
			return enumerator.Current;
		}

		public static IEnumerator<T> Move<T>(this IEnumerator<T> enumerator, int step = 1) {
			if (step < 0)
				throw new ArgumentOutOfRangeException(nameof(step), $"Parameter {nameof(step)} cannot be negative");
			while (step-- > 0)
				enumerator.MoveNext();
			return enumerator;
		}

		public static IExtendedEnumerator<T> ToExtended<T>(this IEnumerator<T> enumerator) => new ExtendedEnumerator<T>(enumerator);

		public static IExtendedEnumerator<T> AsExtended<T>(this IEnumerator<T> enumerator) => enumerator is IExtendedEnumerator<T> e ? e : new ExtendedEnumerator<T>(enumerator);
	}
}
