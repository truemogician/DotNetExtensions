using System;
using System.Globalization;
using System.Windows.Data;
using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.WPF;

#if NET5_0_OR_GREATER
public interface IValueConverter<TValue, TResult> : IValueConverter {
	object? IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
		if (!targetType.IsAssignableTo(typeof(TResult)))
			throw new InvariantTypeException(typeof(TResult), targetType);
		if (value is TValue v)
			return Convert(v, parameter, culture);
		throw new InvariantTypeException(typeof(TValue), value.GetType());
	}

	object? IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
		if (!targetType.IsAssignableTo(typeof(TValue)))
			throw new InvariantTypeException(typeof(TValue), targetType);
		if (value is TResult v)
			return ConvertBack(v, parameter, culture);
		throw new InvariantTypeException(typeof(TResult), value.GetType());
	}

	protected TResult Convert(TValue value, object parameter, CultureInfo culture);

	protected TValue ConvertBack(TResult value, object parameter, CultureInfo culture);
}

public interface IValueConverter<TValue, in TParameter, TResult> : IValueConverter<TValue, TResult> {
	TResult IValueConverter<TValue, TResult>.Convert(TValue value, object parameter, CultureInfo culture)
		=> parameter is TParameter p ? Convert(value, p, culture) : throw new InvariantTypeException(typeof(TParameter), parameter.GetType());

	TValue IValueConverter<TValue, TResult>.ConvertBack(TResult value, object parameter, CultureInfo culture)
		=> parameter is TParameter p ? ConvertBack(value, p, culture) : throw new InvariantTypeException(typeof(TParameter), parameter.GetType());

	protected TResult Convert(TValue value, TParameter parameter, CultureInfo culture);

	protected TValue ConvertBack(TResult value, TParameter parameter, CultureInfo culture);
}

public interface IOneWayValueConverter<in TValue, out TResult> : IValueConverter {
	object? IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
		if (!targetType.IsAssignableTo(typeof(TResult)))
			throw new InvariantTypeException(typeof(TResult), targetType);
		if (value is TValue v)
			return Convert(v, parameter, culture);
		throw new InvariantTypeException(typeof(TValue), value.GetType());
	}

	object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();

	protected TResult Convert(TValue value, object parameter, CultureInfo culture);
}
#elif NET462_OR_GREATER
public abstract class ValueConverter<TValue, TResult> : IValueConverter {
	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) {
		if (!typeof(TResult).IsAssignableFrom(targetType))
			throw new InvariantTypeException(typeof(TResult), targetType);
		if (value is TValue v)
			return Convert(v, parameter, culture);
		throw new InvariantTypeException(typeof(TValue), value.GetType());
	}

	public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
		if (!typeof(TValue).IsAssignableFrom(targetType))
			throw new InvariantTypeException(typeof(TValue), targetType);
		if (value is TResult v)
			return ConvertBack(v, parameter, culture);
		throw new InvariantTypeException(typeof(TResult), value.GetType());
	}

	protected abstract TResult Convert(TValue value, object parameter, CultureInfo culture);

	protected abstract TValue ConvertBack(TResult value, object parameter, CultureInfo culture);
}

public abstract class ValueConverter<TValue, TParameter, TResult> : ValueConverter<TValue, TResult> {
	protected sealed override TResult Convert(TValue value, object parameter, CultureInfo culture) =>
		parameter is TParameter p ? Convert(value, p, culture) : throw new InvariantTypeException(typeof(TParameter), parameter.GetType());

	protected sealed override TValue ConvertBack(TResult value, object parameter, CultureInfo culture) =>
		parameter is TParameter p ? ConvertBack(value, p, culture) : throw new InvariantTypeException(typeof(TParameter), parameter.GetType());

	protected abstract TResult Convert(TValue value, TParameter parameter, CultureInfo culture);

	protected abstract TValue ConvertBack(TResult value, TParameter parameter, CultureInfo culture);
}

public abstract class OneWayValueConverter<TValue, TResult> : ValueConverter<TValue, TResult> {
	protected override TValue ConvertBack(TResult value, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
#endif