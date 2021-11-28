using System;
using System.Text.RegularExpressions;

namespace TrueMogician.Exceptions {
	public class RegexException : ExceptionWithDefaultMessage {
		public RegexException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		public RegexException(Regex pattern, string? message = null, Exception? innerException = null) : this(message, innerException) => Set(nameof(Pattern), pattern);

		public Regex? Pattern => Get<Regex>(nameof(Pattern));
	}

	public class RegexNotMatchException : RegexException {
		public RegexNotMatchException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		public RegexNotMatchException(Regex pattern, string? message = null, Exception? innerException = null) : base(pattern, message, innerException) { }

		public RegexNotMatchException(string input, Regex pattern, string? message = null, Exception? innerException = null) : base(pattern, message, innerException) => Set(nameof(Input), input);

		public string? Input => Get<string>(nameof(Input));

		protected override string DefaultMessage => Input is null ? $"{Input} doesn't match pattern {Pattern}" : $"Pattern{(Pattern is null ? "" : $" {Pattern}")} not matched";
	}
}