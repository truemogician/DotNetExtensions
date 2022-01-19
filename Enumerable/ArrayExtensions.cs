using System.Collections.Generic;

namespace TrueMogician.Extensions.Enumerable {
	public static class ArrayExtensions {
		public static IEnumerator<T> GetGenericEnumerator<T>(this T[] array) => (IEnumerator<T>)array.GetEnumerator();
	}
}