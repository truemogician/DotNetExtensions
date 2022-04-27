using System.Collections.Generic;
using System.Linq;

namespace TrueMogician.Extensions.Array {
	public static class ArrayExtensions {
		public static IEnumerator<T> GetGenericEnumerator<T>(this T[] array) => array.AsEnumerable().GetEnumerator();
	}
}