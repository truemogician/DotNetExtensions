using System;

namespace TrueMogician.Extensions.Collections.Tree {
	/// <summary>
	///     Common tree node base class that provides additional statistics properties />
	/// </summary>
	/// <typeparam name="T">
	///     <inheritdoc />
	/// </typeparam>
	public abstract class StatisticalTreeNode<T> : TreeNode<T> where T : StatisticalTreeNode<T> {
		private int _height;

		private int _size = 1;

		private int _depth;

		protected StatisticalTreeNode() : this(null) { }

		protected StatisticalTreeNode(T? parent) : base(parent) {
			ParentChanged += (_, _) => DepthUpToDate = false;
			ChildrenChanged += (_, _) => HeightSizeUpToDate = false;
		}

		/// <summary>
		///     Zero-based height of this node as a tree
		/// </summary>
		public int Height {
			get {
				UpdateHeightSize();
				return _height;
			}
		}

		/// <summary>
		///     Size of this node as a tree
		/// </summary>
		public int Size {
			get {
				UpdateHeightSize();
				return _size;
			}
		}

		/// <summary>
		///     Zero-based depth of this node
		/// </summary>
		public int Depth {
			get {
				UpdateDepth();
				return _depth;
			}
		}

		private bool HeightSizeUpToDate {
			get => _height >= 0;
			set {
				if (_height < 0 || value)
					return;
				if (!IsRoot)
					Parent!.HeightSizeUpToDate = false;
				_height = -1;
			}
		}

		private bool DepthUpToDate {
			get => _depth >= 0;
			set {
				if (_depth < 0 || value)
					return;
				foreach (var child in Children)
					child.DepthUpToDate = false;
				_depth = -1;
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
		private void UpdateHeightSize() {
			if (HeightSizeUpToDate)
				return;
			var height = 0;
			var size = 1;
			foreach (var child in Children) {
				child.UpdateHeightSize();
				height = Math.Max(height, child._height + 1);
				size += child._size;
			}
			_height = height;
			_size = size;
		}

		/// <summary>
		///     Update depth if out of date. Called in the getter of <see cref="Depth" />
		/// </summary>
		private void UpdateDepth() {
			if (DepthUpToDate)
				return;
			_depth = IsRoot ? 0 : Parent!.Depth + 1;
		}
	}

	/// <summary>
	///     Tree node class with statistics properties. If inheritance required, use <see cref="StatisticalTreeNode{T}" />
	///     instead.
	/// </summary>
	public sealed class StatisticalTreeNode : StatisticalTreeNode<StatisticalTreeNode> {
		public StatisticalTreeNode() { }

		public StatisticalTreeNode(StatisticalTreeNode? parent) : base(parent) { }
	}

	/// <summary>
	///     Tree node class with node value and statistics properties. If inheritance required, use
	///     <see cref="StatisticalTreeNode{T}" />
	///     instead.
	/// </summary>
	public sealed class ValuedStatisticalTreeNode<T> : StatisticalTreeNode<ValuedStatisticalTreeNode<T>> {
		public ValuedStatisticalTreeNode(T value) : this(value, null) { }

		public ValuedStatisticalTreeNode(T value, ValuedStatisticalTreeNode<T>? parent) : base(parent) => Value = value;

		public T Value { get; set; }
	}
}