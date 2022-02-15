using System.Collections;
using System.Collections.Generic;
using TrueMogician.Extensions.Enumerable;

namespace TrueMogician.Extensions.Collections.Tree {
	public class Forest<T> : IList<Tree<T>>, IReadOnlyList<Tree<T>> where T : ITree<T> {
		private readonly List<Tree<T>> _trees;

		public Forest() => _trees = new List<Tree<T>>();

		public Forest(IEnumerable<Tree<T>> trees) => _trees = trees.AsList();

		public int Count => _trees.Count;

		public bool IsReadOnly => false;

		public Tree<T> this[int index] {
			get => _trees[index];
			set => _trees[index] = value;
		}

		public IEnumerator<Tree<T>> GetEnumerator() => _trees.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(Tree<T> tree) => _trees.Add(tree);

		public void Clear() => _trees.Clear();

		public bool Contains(Tree<T> tree) => _trees.Contains(tree);

		public void CopyTo(Tree<T>[] array, int arrayIndex) => _trees.CopyTo(array, arrayIndex);

		public bool Remove(Tree<T> tree) => _trees.Remove(tree);

		public int IndexOf(Tree<T> tree) => _trees.IndexOf(tree);

		public void Insert(int index, Tree<T> tree) => _trees.Insert(index, tree);

		public void RemoveAt(int index) => _trees.RemoveAt(index);

		public static implicit operator Forest<T>(Tree<T> tree) => new(new[] { tree });
	}
}