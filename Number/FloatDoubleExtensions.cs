using System;
using System.Runtime.CompilerServices;

namespace TrueMogician.Extensions.Number {
	public static class FloatDoubleExtensions {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualsWithTolerance(this float number, float other, float tolerance = float.Epsilon) => Math.Abs(number - other) < tolerance;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualsWithTolerance(this double number, double other, double tolerance = double.Epsilon) => Math.Abs(number - other) < tolerance;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualsWithTolerance(this float number, double other, float tolerance = float.Epsilon) => EqualsWithTolerance(number, (float)other, tolerance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualsWithTolerance(this double number, float other, float tolerance = float.Epsilon) => EqualsWithTolerance((float)number, other, tolerance);
	}
}
