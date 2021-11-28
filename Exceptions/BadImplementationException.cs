using System;

namespace TrueMogician.Exceptions {
	/// <summary>
	///     Throw when an abstract member's implementation doesn't meet the requirements
	/// </summary>
	public class BadImplementationException : ExceptionWithDefaultMessage {
		public BadImplementationException(string message = null, Exception innerException = null) : base(message, innerException) { }

		public BadImplementationException(string memberName, string message = null, Exception innerException = null) : base(message, innerException) => this[nameof(MemberName)] = memberName;

		public string MemberName => Get<string>(nameof(MemberName));

		protected override string DefaultMessage => $"{MemberName} is badly implemented";
	}
}