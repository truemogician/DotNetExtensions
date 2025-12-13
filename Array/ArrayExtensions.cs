using System.Collections.Generic;
using System.Linq;

namespace TrueMogician.Extensions.Array;

public static class ArrayExtensions {
	extension<T>(T[] self) {
		public IEnumerator<T> GetGenericEnumerator() => self.AsEnumerable().GetEnumerator();
	}
}