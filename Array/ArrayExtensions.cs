using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TrueMogician.Extensions.Array;

public static class ArrayExtensions {
	extension<T>(T[] self) {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetGenericEnumerator() => self.AsEnumerable().GetEnumerator();
	}
}