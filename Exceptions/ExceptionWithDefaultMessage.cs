using System;
using System.Runtime.Serialization;

namespace TrueMogician.Exceptions {
	public abstract class ExceptionWithDefaultMessage : Exception {
		private readonly bool _useDefaultMessage;

		protected ExceptionWithDefaultMessage(string message = null, Exception innerException = null) : base(message, innerException) => _useDefaultMessage = message is null;

		protected ExceptionWithDefaultMessage(SerializationInfo info, StreamingContext context) : base(info, context) { }

		protected virtual string DefaultMessage { get; } = null;

		public sealed override string Message => _useDefaultMessage && DefaultMessage is not null ? DefaultMessage : base.Message;

		protected object this[object key] {
			get => Data.Contains(key) ? Data[key] : null;
			set => Data[key] = value;
		}

		protected T Get<T>(object key) => this[key] is T result ? result : default;

		protected void Set<T>(object key, T value) => this[key] = value;
	}
}
