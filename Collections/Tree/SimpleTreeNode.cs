using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueMogician.Extensions.Events;

namespace TrueMogician.Extensions.Collections.Tree {
	/// <summary>
	///     Base class of basic tree node that provides only the basic structure and functionality of a tree.<br />
	///     Note that the synchronization between <c>Parent</c> and <c>Children</c> needs to be handled manually.
	///     For automatic synchronization, refer to <see cref="TreeNode{T}" />
	/// </summary>
	/// <typeparam name="T">Type of the derived class</typeparam>
	public abstract class SimpleTreeNode<T> : IEnumerable<T> where T : SimpleTreeNode<T> {
		// ReSharper disable once InconsistentNaming
		protected internal readonly IList<T> _children;

		private T? _parent;

		protected SimpleTreeNode() : this(null) => _children = new List<T>();

		protected SimpleTreeNode(T? parent) {
			_children = new List<T>();
			_parent = parent;
		}

		protected internal SimpleTreeNode(IList<T> children, T? parent = null) : this(parent) => _children = children;

		public event ValueChangedEventHandler<T?> ParentChanged = delegate { };

		/// <summary>
		///     Default enumerator. <see cref="TraversalOrder.PreOrder" /> will be used.
		/// </summary>
		public IEnumerator<T> GetEnumerator() => GetEnumerator(TraversalOrder.PreOrder);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		///     Parent node
		/// </summary>
		public virtual T? Parent {
			get => _parent;
			set => SetParent(value);
		}

		/// <summary>
		///     Child nodes collection
		/// </summary>
		public virtual IList<T> Children => _children;

		public bool IsLeaf => _children.Count == 0;

		public bool IsRoot => Parent is null;

		public T Root {
			get {
				var root = This;
				while (!root.IsRoot)
					root = root.Parent!;
				return root;
			}
		}

		public IEnumerable<T> Ancestors {
			get {
				for (var n = Parent; n is not null; n = n.Parent)
					yield return n;
			}
		}

		public IEnumerable<T> Descendents => Children.SelectMany(n => n);

		public IEnumerable<T> Leaves => this.Where(n => n.IsLeaf);

		protected T This => (T)this;

		public static void Unlink(T node) {
			var parent = node._parent;
			if (parent is null)
				foreach (var child in node._children)
					child.SetParent(null);
			else {
				int index = parent._children.IndexOf(node);
				node.SetParent(null);
				parent._children.RemoveAt(index);
				foreach (var child in node._children) {
					child.SetParent(parent);
					parent._children.Insert(index++, child);
				}
			}
		}

		/// <param name="order">
		///     Traversal order, possible values are <see cref="TraversalOrder.PreOrder" />,
		///     <see cref="TraversalOrder.PostOrder" /> and <see cref="TraversalOrder.BreadthFirst" />.
		///     <see cref="TraversalOrder.InOrder" /> is not supported because this tree
		///     node isn't binary-guaranteed.
		/// </param>
		public IEnumerator<T> GetEnumerator(TraversalOrder order) {
			switch (order) {
				case TraversalOrder.InOrder: throw new ArgumentOutOfRangeException(nameof(order), "Non-binary tree cannot be traversed in InOrder");
				case TraversalOrder.BreadthFirst: {
					var queue = new Queue<T>();
					queue.Enqueue(This);
					while (queue.Count > 0) {
						var cur = queue.Dequeue();
						yield return cur;
						foreach (var child in _children)
							queue.Enqueue(child);
					}
					break;
				}
				default: {
					if (order == TraversalOrder.PreOrder)
						yield return This;
					foreach (var child in _children)
						foreach (var node in child)
							yield return node;
					if (order == TraversalOrder.PostOrder)
						yield return This;
					break;
				}
			}
		}

		/// <summary>
		///     Check whether this node is the child of <paramref name="node" />
		/// </summary>
		public bool IsChildOf(T node) {
			var cur = Parent;
			while (cur is not null && !cur.Equals(node) && !cur.Equals(this))
				cur = cur.Parent;
			return node.Equals(cur);
		}

		/// <summary>
		///     Check whether this node is the ancestor of <paramref name="node" />
		/// </summary>
		public bool IsAncestorOf(T node) => node.IsChildOf(This);

		protected internal void SetParent(T? value) {
			var old = _parent;
			_parent = value;
			ParentChanged(this, new ValueChangedEventArgs<T?>(old, value));
		}
	}

	/// <summary>
	///     Basic tree node. If inheritance required, use <see cref="SimpleTreeNode{T}" /> instead.
	/// </summary>
	public sealed class SimpleTreeNode : SimpleTreeNode<SimpleTreeNode> {
		public SimpleTreeNode() { }

		public SimpleTreeNode(SimpleTreeNode? parent) : base(parent) { }
	}

	/// <summary>
	///     Basic tree node with node value. If inheritance required, use <see cref="SimpleTreeNode{T}" /> instead.
	/// </summary>
	public sealed class ValuedSimpleTreeNode<T> : SimpleTreeNode<ValuedSimpleTreeNode<T>> {
		public ValuedSimpleTreeNode(T value) : this(value, null) { }

		public ValuedSimpleTreeNode(T value, ValuedSimpleTreeNode<T>? parent) : base(parent) => Value = value;

		public T Value { get; set; }
	}

	public enum TraversalOrder : byte {
		PreOrder,

		InOrder,

		PostOrder,

		BreadthFirst
	}
}