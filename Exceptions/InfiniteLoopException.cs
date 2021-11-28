using System;

namespace TrueMogician.Exceptions {
	public class InfiniteLoopException : ExceptionWithDefaultMessage {
		public InfiniteLoopException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		protected override string DefaultMessage => "Infinite loop detected";
	}
}