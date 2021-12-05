using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TrueMogician.Extensions.Enumerable;
using TrueMogician.Extensions.Events;

namespace TrueMogician.Extensions.Collections.Tree {
	/// <summary>Basic tree node base class that provides only the basic structure and functionality of a tree</summary>
	/// <typeparam name="T">Type of the derived class</typeparam>
	public abstract class SimpleTreeNode<T> : IEnumerable<T> where T : SimpleTreeNode<T> {
		private T? _parent;

		private readonly ControllableList<T> _children = new() {ChangingEventEnabled = false};

		private bool _updateParentOnChildrenChange = true;

		protected SimpleTreeNode() : this(null) { }

		protected SimpleTreeNode(T? parent) {
			Parent = parent;
			_children.ListChanged += (_, baseArgs) => {
				if (!_updateParentOnChildrenChange)
					return;
				switch (baseArgs) {
					case ControllableListAddedEventArgs<T> args:
						// Remove duplicate child
						if (ReferenceEquals(args.Value._parent, this)) {
							_children.ChangedEventEnabled = false;
							_children.RemoveAt(args.Index);
							_children.ChangedEventEnabled = true;
						}
						else
							args.Value.SetParent(This);
						break;
					case ControllableListRemovedEventArgs<T> args:
						args.Value.SetParent(null);
						break;
					case ControllableListReplacedEventArgs<T> args:
						if (!ReferenceEquals(args.OldValue, args.NewValue)) {
							args.OldValue.SetParent(null);
							args.NewValue.SetParent(This);
						}
						break;
					case ControllableListRangeAddedEventArgs<T> args:
						_children.ChangedEventEnabled = false;
						for (var i = 0; i < args.Count; ++i)
							if (ReferenceEquals(args.Values[i]._parent, this))
								_children.RemoveAt(args.StartIndex + i);
							else
								args.Values[i].SetParent(This);
						_children.ChangedEventEnabled = true;
						break;
					case ControllableListRangeRemovedEventArgs<T> args:
						foreach (var value in args.Values)
							value.SetParent(null);
						break;
					case ControllableListRangeReplacedEventArgs<T> args:
						foreach (var (old, @new) in args.OldValues.IndexJoin(args.NewValues))
							if (!ReferenceEquals(old, @new)) {
								old.SetParent(null);
								@new.SetParent(This);
							}
						break;
				}
			};
		}

		public event ValueChangedEventHandler<T> ParentChanged = delegate { };

		public event ControllableListChangedEventHandler ChildrenChanged {
			add => _children.ListChanged += value;
			remove => _children.ListChanged -= value;
		}

		/// <summary>
		///     Default enumerator. <see cref="TraversalOrder.PreOrder" /> will be used.
		/// </summary>
		public IEnumerator<T> GetEnumerator() => GetEnumerator(TraversalOrder.PreOrder);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		///     Parent node. Note that when modified, the <see cref="Children" /> of the new and old parent node will be
		///     automatically synchronized.
		/// </summary>
		public T? Parent {
			get => _parent;
			set {
				if (Equals(_parent, value))
					return;
				if (_parent is not null) {
					_updateParentOnChildrenChange = false;
					_parent._children.Remove(This);
					_updateParentOnChildrenChange = true;
				}
				if (value is not null) {
					value._updateParentOnChildrenChange = false;
					value._children.Add(This);
					value._updateParentOnChildrenChange = true;
				}
				SetParent(value);
			}
		}

		/// <summary>
		///     Child nodes collection. Note that when the collection is modified, the <see cref="Parent" /> of the added or
		///     removed nodes will be automatically synchronized.<br />
		/// </summary>
		public IList<T> Children => _children;

		public bool IsLeaf => _children.Count == 0;

		[MemberNotNullWhen(false, nameof(Parent))]
		public bool IsRoot => Parent is null;

		public T Root {
			get {
				var root = This;
				while (!root.IsRoot)
					root = Root.Parent!;
				return root;
			}
		}

		public IEnumerable<T> Ancestors {
			get {
				for (var n = Parent; n is not null; n = n.Parent)
					yield return n;
			}
		}

		public IEnumerable<T> Leaves => this.Where(n => n.IsLeaf);

		protected T This => (T)this;

		/// <param name="order">
		///     Traversal order, possible values are <see cref="TraversalOrder.PreOrder" />,
		///     <see cref="TraversalOrder.PostOrder" /> and <see cref="TraversalOrder.BreadthFirst" />.
		///     <see cref="TraversalOrder.InOrder" /> is not supported because this tree
		///     node isn't binary-guaranteed.
		/// </param>
		public IEnumerator<T> GetEnumerator(TraversalOrder order) {
			if (order == TraversalOrder.InOrder)
				throw new ArgumentOutOfRangeException(nameof(order), "Non-binary tree cannot be traversed in InOrder");
			if (order == TraversalOrder.BreadthFirst) {
				var queue = new Queue<T>();
				queue.Enqueue(This);
				while (queue.Count > 0) {
					var cur = queue.Dequeue();
					yield return cur;
					foreach (var child in _children)
						queue.Enqueue(child);
				}
			}
			else {
				if (order == TraversalOrder.PreOrder)
					yield return This;
				foreach (var child in _children)
					foreach (var node in child)
						yield return node;
				if (order == TraversalOrder.PostOrder)
					yield return This;
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

		private void SetParent(T? value) {
			var old = _parent;
			_parent = value;
			ParentChanged(this, new ValueChangedEventArgs<T>(old, value));
		}
	}

	/// <summary>
	///     Basic tree node class. If inheritance required, use <see cref="SimpleTreeNode{T}" /> instead.
	/// </summary>
	public sealed class SimpleTreeNode : SimpleTreeNode<SimpleTreeNode> {
		public SimpleTreeNode() { }

		public SimpleTreeNode(SimpleTreeNode? parent) : base(parent) { }
	}

	/// <summary>
	///     Basic tree node class with node value. If inheritance required, use <see cref="SimpleTreeNode{T}" /> instead.
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