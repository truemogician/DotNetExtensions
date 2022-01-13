using System;

namespace TrueMogician.Extensions.Collections {
	internal static class Utilities {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		internal static int ToInt(this Index index, int count) => index.IsFromEnd ? count - index.Value : index.Value;
#endif
	}
}