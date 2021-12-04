using System;

namespace TrueMogician.Extensions.Events {
	/// <summary>
	///     Argument of <see cref="ValueChangedEventHandler{T}" />, holding old and new values when some value is about to
	///     change.
	/// </summary>
	/// <typeparam name="T">Type of the changing value</typeparam>
	public class ValueChangingEventArgs<T> : EventArgs {
		public ValueChangingEventArgs() { }

		public ValueChangingEventArgs(T? oldValue, T? newValue) {
			OldValue = oldValue;
			NewValue = newValue;
		}

		public new static ValueChangedEventArgs<T> Empty { get; } = new();

		public T? OldValue { get; }

		public T? NewValue { get; }

		/// <summary>
		///     If set to true, the changing operation should be cancelled. But it's not guaranteed, since it depends on the
		///     specific implementation.
		/// </summary>
		public bool Cancel { get; set; } = false;
	}

	public delegate void ValueChangingEventHandler<T>(object? sender, ValueChangingEventArgs<T> args);
}