using System.Collections.Generic;

namespace TrueMogician.Extensions.Collections.Tree {
	public interface ITree<T> where T : ITree<T> {
		public T? Parent { get; set; }

		public IList<T> Children { get; }
	}
}