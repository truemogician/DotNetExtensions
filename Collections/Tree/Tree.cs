using System;

namespace TrueMogician.Extensions.Collections.Tree {
	public class Tree<T> {
		public Tree(T root) => Root = root;

		public T Root { get; }

		public override string? ToString() => Root?.ToString();

		public override int GetHashCode() => Root?.GetHashCode() ?? throw new NullReferenceException("Root is null");

#if NET5_0_OR_GREATER
		public override bool Equals(object? obj) => obj is Tree<T> tree ? Equals(Root, tree.Root) : Equals(Root, obj);
#else
		public override bool Equals(object obj) => obj is Tree<T> tree ? Equals(Root, tree.Root) : Equals(Root, obj);
#endif

		public static implicit operator Tree<T>(T root) => new(root);

		public static implicit operator T(Tree<T> tree) => tree.Root;
	}
}