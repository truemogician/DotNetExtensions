using TrueMogician.Exceptions;

namespace TrueMogician.Extensions.Collections.Tree.Exceptions {
	public class SelfLoopException : ExceptionWithDefaultMessage {
		protected override string DefaultMessage => "Self loop detected";
	}
}