using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.String {
	public static class StringExtensions {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ConvertTo<T>(this string str) => (T)ConvertTo(str, typeof(T));

		public static object ConvertTo(this string str, Type targetType) {
			if (typeof(Enum).IsAssignableFrom(targetType))
				return Enum.Parse(targetType, str);
			if (typeof(IConvertible).IsAssignableFrom(targetType))
				return Convert.ChangeType(str, targetType);
			var constructor = targetType.GetConstructors().FirstOrDefault(c => c.GetParameters() is var p && p.Length == 1 && p[0].ParameterType == typeof(string));
			return constructor?.Invoke(new object[] {str}) ?? throw new TypeException(targetType, $"{targetType.FullName} cannot be converted from a string");
		}
	}
}
