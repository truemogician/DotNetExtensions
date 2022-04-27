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
#endif