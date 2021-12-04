using System;

namespace TrueMogician.Extensions.Events {
	/// <summary>
	///     Argument of <see cref="ValueChangedEventHandler{T}" />, holding old and new values when some value has already
	///     changed.
	/// </summary>
	/// <typeparam name="T">Type of the changed value</typeparam>
	public class ValueChangedEventArgs<T> : EventArgs {
		public ValueChangedEventArgs() { }

		public ValueChangedEventArgs(T? oldValue, T? newValue) {
			OldValue = oldValue;
			NewValue = newValue;
		}

		public new static ValueChangedEventArgs<T> Empty { get; } = new();

		public T? OldValue { get; }

		public T? NewValue { get; }
	}

	public delegate void ValueChangedEventHandler<T>(object? sender, ValueChangedEventArgs<T> args);
}