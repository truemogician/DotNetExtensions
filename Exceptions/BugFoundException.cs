using System;
using System.Text;

namespace TrueMogician.Exceptions {
	/// <summary>
	///     The exception that is thrown when code runs into somewhere unexpected, which means a potential bug is found.
	/// </summary>
	public class BugFoundException : ExceptionWithDefaultMessage {
		public BugFoundException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		public string? BugInformation { get; init; }

		protected override string? DefaultMessage {
			get {
				var builder = new StringBuilder("Congratulations on finding a bug! ");
				if (BugInformation is not null) {
					builder.AppendLine("Here's the information:");
					builder.AppendLine(BugInformation);
				}
				if (!string.IsNullOrEmpty(HelpLink))
					builder.Append($"Maybe you can contact the careless author by {HelpLink}");
				return builder.ToString();
			}
		}
	}
}