using System;

namespace TrueMogician.Extensions.Collections.Tree {
	/// <summary>
	///     Common tree node base class that provides additional statistics properties />
	/// </summary>
	/// <typeparam name="T">
	///     <inheritdoc />
	/// </typeparam>
	public abstract class TreeNode<T> : SimpleTreeNode<T> where T : TreeNode<T> {
		private int _height;

		private int _size = 1;

		private int _depth;

		private bool _heightUpToDate = true;

		private bool _depthUpToDate = true;

		protected TreeNode() { }

		protected TreeNode(T? parent) : base(parent) { }

		/// <summary>
		///     Height of the subtree whose root is this node
		/// </summary>
		public int Height {
			get {
				UpdateHeightAndSize();
				return _height;
			}
		}

		/// <summary>
		///     Size of the subtree whose root is this node
		/// </summary>
		public int Size {
			get {
				UpdateHeightAndSize();
				return _size;
			}
		}

		/// <summary>
		///     Depth of the subtree whose root is this node
		/// </summary>
		public int Depth {
			get {
				UpdateDepth();
				return _depth;
			}
		}

		private bool HeightUpToDate {
			get => _heightUpToDate;
			set {
				if (_heightUpToDate && !value && !IsRoot)
					Parent!.HeightUpToDate = value;
				_heightUpToDate = value;
			}
		}

		private bool DepthUpToDate {
			get => _depthUpToDate;
			set {
				if (_depthUpToDate && !value)
					foreach (var child in Children)
						child.DepthUpToDate = value;
				_depthUpToDate = value;
			}
		}

		/// <summary>
		///     Calculate the latest common ancestor of <paramref name="node1" /> and <paramref name="node2" />
		/// </summary>
		/// <returns>Null if <paramref name="node1" /> and <paramref name="node2" /> aren't in the same tree</returns>
		public static T? GetLatestCommonAncestor(T node1, T node2) {
			if (node1.Depth > node2.Depth) {
				var temp = node1;
				node1 = node2;
				node2 = temp;
			}
			for (var i = 0; i < node2.Depth - node1.Depth; ++i)
				node2 = node2.Parent!;
			while (!node1.Equals(node2) && !node1.IsRoot && !node2.IsRoot) {
				node1 = node1.Parent!;
				node2 = node2.Parent!;
			}
			return node1.Equals(node2) ? node1 : null;
		}

		/// <summary>
		///     Update height and size if out of date. Called in the getters of <see cref="Height" /> and <see cref="Size" />.
		/// </summary>
		private void UpdateHeightAndSize() {
			if (HeightUpToDate)
				return;
			_height = 0;
			_size = 1;
			foreach (var child in Children) {
				child.UpdateHeightAndSize();
				_size += child._size;
				_height = Math.Max(_height, child._height + 1);
			}
			HeightUpToDate = true;
		}

		/// <summary>
		///     Update depth if out of date. Called in the getter of <see cref="Depth" />
		/// </summary>
		private void UpdateDepth() {
			if (DepthUpToDate)
				return;
			if (IsRoot) {
				_depth = 0;
				return;
			}
			Parent!.UpdateDepth();
			_depth = Parent.Depth + 1;
			DepthUpToDate = true;
		}
	}

	/// <summary>
	///     Tree node class with statistics properties. If inheritance required, use <see cref="TreeNode{T}" /> instead.
	/// </summary>
	public sealed class TreeNode : TreeNode<TreeNode> {
		public TreeNode() { }

		public TreeNode(TreeNode? parent) : base(parent) { }
	}

	/// <summary>
	///     Tree node class with node value and statistics properties. If inheritance required, use <see cref="TreeNode{T}" />
	///     instead.
	/// </summary>
	public sealed class ValuedTreeNode<T> : TreeNode<ValuedTreeNode<T>> {
		public ValuedTreeNode(T value) : this(value, null) { }

		public ValuedTreeNode(T value, ValuedTreeNode<T>? parent) : base(parent) => Value = value;

		public T Value { get; set; }
	}
}