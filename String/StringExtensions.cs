using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.String;

/// <summary>
///     Extension class for <see cref="string" />
/// </summary>
public static class StringExtensions {
	/// <param name="str">The current string.</param>
	extension(string str) {
		/// <summary>
		///     Convert the current string to <typeparamref name="T" />.
		///     This function has special treatment for <see cref="Enum" /> and types implementing <see cref="IConvertible" />,
		///     otherwise it'll look for constructor accepting only one <see cref="string" /> as parameter.
		/// </summary>
		/// <typeparam name="T">The type the current string will be converted to.</typeparam>
		/// <returns>A variable of <typeparamref name="T" /> converted from the current string.</returns>
		/// <exception cref="TypeException"></exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ConvertTo<T>() => (T)ConvertTo(str, typeof(T));

		/// <param name="targetType">The type the current string will be converted to.</param>
		/// <returns>A variable of <paramref name="targetType" /> converted from the current string.</returns>
		/// <inheritdoc cref="StringExtensions.ConvertTo{T}" />
		public object ConvertTo(Type targetType) {
			if (typeof(Enum).IsAssignableFrom(targetType))
				return Enum.Parse(targetType, str);
			if (typeof(IConvertible).IsAssignableFrom(targetType))
				return Convert.ChangeType(str, targetType);
			var constructor = targetType.GetConstructors().FirstOrDefault(c => c.GetParameters() is { Length: 1 } p && p[0].ParameterType == typeof(string));
			return constructor?.Invoke([str]) ?? throw new TypeException(targetType, $"{targetType.FullName} cannot be converted from a string");
		}

		/// <summary>
		///     Return a copy of the <see cref="string" /> with its first letter converted to uppercase.
		/// </summary>
		/// <param name="preserveRest">
		///     If <see langword="true" />, the case of the remaining string (string excluding the first letter) will remain
		///     untouched, otherwise it'll be converted to lowercase.
		///     Default is <see langword="false" />.
		/// </param>
		/// <returns>A capitalized equivalent of the current string.</returns>
		public string Capitalize(bool preserveRest = false) {
			return str.Length switch {
				0 => str,
				1 => str.ToUpper(),
				_ => char.ToUpper(str[0]) + (preserveRest ? str.Substring(1) : str.Substring(1).ToLower())
			};
		}

		/// <summary>
		///     Return a copy of the <see cref="string" /> with its first letter converted to uppercase using the casing rule of
		///     the invariant culture.
		/// </summary>
		/// <inheritdoc cref="Capitalize" />
		public string CapitalizeInvariant(bool preserveRest = false) {
			return str.Length switch {
				0 => str,
				1 => str.ToUpperInvariant(),
				_ => char.ToUpperInvariant(str[0]) + (preserveRest ? str.Substring(1) : str.Substring(1).ToLowerInvariant())
			};
		}
	}
}