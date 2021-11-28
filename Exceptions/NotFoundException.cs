using System;

namespace TrueMogician.Exceptions {
	public class NotFoundException : ExceptionWithDefaultMessage {
		public NotFoundException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		public NotFoundException(object notFoundValue, string? message = null, Exception? innerException = null) : base(message, innerException) => this[nameof(NotFoundValue)] = notFoundValue;

		public object? NotFoundValue => Get<object>(nameof(NotFoundValue));

		protected override string? DefaultMessage => $"Value{(NotFoundValue is null ? "" : $" {NotFoundValue}")} not found";
	}
}