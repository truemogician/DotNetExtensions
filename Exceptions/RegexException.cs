using System;
using System.Text.RegularExpressions;

namespace TrueMogician.Exceptions {
	public class RegexException : Exception {
		public RegexException(string message = null, Exception innerException = null) : base(message, innerException) { }

		public RegexException(Regex pattern, string message = null, Exception innerException = null) : this(message, innerException) => Pattern = pattern;

		public Regex Pattern { get; }
	}

	public class RegexNotMatchException : RegexException {
		public RegexNotMatchException(string message = null, Exception innerException = null) : base(message, innerException) { }

		public RegexNotMatchException(Regex pattern, string message = null, Exception innerException = null) : base(pattern, message, innerException) { }

		public RegexNotMatchException(string input, Regex pattern, string message = null, Exception innerException = null) : base(pattern, message, innerException) => Input = input;

		public string Input { get; }
	}
}