using System;

namespace TrueMogician.Exceptions {
	public class EmptyCollectionException : ExceptionWithDefaultMessage {
		public EmptyCollectionException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		protected override string DefaultMessage => "Collection is empty";
	}
}