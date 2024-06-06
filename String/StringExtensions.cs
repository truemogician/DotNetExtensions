using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.String {
	/// <summary>
	/// Extension class for <see cref="string"/>
	/// </summary>
	public static class StringExtensions {
        /// <summary>
        /// Convert <paramref name="str"/> to <typeparamref name="T"/>.
        /// This function has special treatment for <see cref="Enum"/> and types implementing <see cref="IConvertible"/>,
        /// otherwise it'll look for constructor accepting only one <see cref="string" /> as parameter.
        /// </summary>
        /// <typeparam name="T">The type <paramref name="str"/> will be converted to.</typeparam>
        /// <param name="str">The current string.</param>
		/// <returns>A variable of <typeparamref name="T"/> converted from <paramref name="str"/>.</returns>
        /// <exception cref="TypeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ConvertTo<T>(this string str) => (T)ConvertTo(str, typeof(T));

		/// <summary>
		/// Convert <paramref name="str"/> to <paramref name="targetType"/>.
		/// This function has special treatment for <see cref="Enum"/> and types implementing <see cref="IConvertible"/>,
		/// otherwise it'll look for constructor accepting only one <see cref="string" /> as parameter.
		/// </summary>
		/// <param name="str">The current string.</param>
		/// <param name="targetType">The type <paramref name="str"/> will be converted to.</param>
		/// <returns>A variable of <paramref name="targetType"/> converted from <paramref name="str"/>.</returns>
		/// <exception cref="TypeException"></exception>
		public static object ConvertTo(this string str, Type targetType) {
			if (typeof(Enum).IsAssignableFrom(targetType))
				return Enum.Parse(targetType, str);
			if (typeof(IConvertible).IsAssignableFrom(targetType))
				return Convert.ChangeType(str, targetType);
			var constructor = targetType.GetConstructors().FirstOrDefault(c => c.GetParameters() is var p && p.Length == 1 && p[0].ParameterType == typeof(string));
			return constructor?.Invoke([str]) ?? throw new TypeException(targetType, $"{targetType.FullName} cannot be converted from a string");
		}

		/// <summary>
		/// Return a copy of the <see cref="string" /> with its first letter converted to uppercase.
		/// </summary>
		/// <param name="str">The current string.</param>
		/// <param name="preserveRest">
		/// If <see langword="true" />, the case of the remaining string (string excluding the first letter) will remain untouched, otherwise it'll be converted to lowercase.  
		/// Default is <see langword="false" />.
		/// </param>
		/// <returns>A capitalized equivalent of <paramref name="str"/>.</returns>
		public static string Capitalize(this string str, bool preserveRest = false) {
			return str.Length switch {
				0 => str,
				1 => str.ToUpper(),
				_ => char.ToUpper(str[0]) + (preserveRest ? str.Substring(1) : str.Substring(1).ToLower())
			};
		}

        /// <summary>
        /// Return a copy of the <see cref="string" /> with its first letter converted to uppercase using the casing rule of the invariant culture.
        /// </summary>
        /// <param name="preserveRest">
        /// If <see langword="true" />, the case of the remaining string (string excluding the first letter) will remain untouched, otherwise it'll be converted to lowercase.  
        /// Default is <see langword="false" />.
        /// </param>
        /// <returns>A capitalized equivalent of <paramref name="str"/>.</returns>
        public static string CapitalizeInvariant(this string str, bool preserveRest = false) {
			return str.Length switch {
				0 => str,
				1 => str.ToUpperInvariant(),
				_ => char.ToUpperInvariant(str[0]) + (preserveRest ? str.Substring(1) : str.Substring(1).ToLowerInvariant())
			};
		}
    }
}
