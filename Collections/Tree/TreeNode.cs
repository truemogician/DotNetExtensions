using System;
using System.Linq;
using TrueMogician.Extensions.Enumerable;

namespace TrueMogician.Extensions.Collections.Tree {
	/// <summary>
	///     Common tree node base class that ensures the synchronization between <c>Children</c> and <c>Parent</c>
	/// </summary>
	/// <typeparam name="T">
	///     <inheritdoc />
	/// </typeparam>
	public abstract class TreeNode<T> : SimpleTreeNode<T> where T : TreeNode<T> {
		private bool _updateParentOnChildrenChange = true;

		protected TreeNode() : this(null) { }

		protected TreeNode(T? parent) : base(new ControllableList<T>(), parent) {
			PrivateChildren.ListChanging += (_, baseArgs) => {
				switch (baseArgs) {
					case ControllableListAddingEventArgs<T> addingArgs when ReferenceEquals(addingArgs.Value.Parent, this):
						baseArgs.Cancel = true;
						break;
					case ControllableListAddingRangeEventArgs<T> args:
						var list = args.Values.Where(node => !ReferenceEquals(node.Parent, this)).ToList();
						if (list.Count != args.Count) {
							args.Cancel = true;
							PrivateChildren.InsertRange(args.Index, list);
						}
						break;
					case ControllableListReplacingEventArgs<T> replacingArgs when ReferenceEquals(replacingArgs.NewValue.Parent, this):                      throw new InvalidOperationException($"The replacing node {replacingArgs.NewValue} already presents in Children");
					case ControllableListReplacingRangeEventArgs<T> replacingRangeArgs when replacingRangeArgs.NewValues.Any(n => ReferenceEquals(n, this)): throw new InvalidOperationException("Some nodes in the replacing range already present in Children");
				}
			};
			PrivateChildren.ListChanged += (_, baseArgs) => {
				if (!_updateParentOnChildrenChange)
					return;
				switch (baseArgs) {
					case ControllableListAddedEventArgs<T> args:
						args.Value.Parent?.Children.Remove(args.Value);
						args.Value.SetParent(This);
						break;
					case ControllableListRemovedEventArgs<T> args:
						args.Value.SetParent(null);
						break;
					case ControllableListReplacedEventArgs<T> args:
						if (!ReferenceEquals(args.OldValue, args.NewValue)) {
							args.OldValue.SetParent(null);
							args.NewValue.Parent?.Children.Remove(args.NewValue);
							args.NewValue.SetParent(This);
						}
						break;
					case ControllableListRangeAddedEventArgs<T> args:
						foreach (var node in args.Values) {
							node.Parent?.Children.Remove(node);
							node.SetParent(This);
						}
						break;
					case ControllableListRangeRemovedEventArgs<T> args:
						foreach (var value in args.Values)
							value.SetParent(null);
						break;
					case ControllableListRangeReplacedEventArgs<T> args:
						foreach (var (old, @new) in args.OldValues.IndexJoin(args.NewValues))
							if (!ReferenceEquals(old, @new)) {
								old.SetParent(null);
								@new.Parent?.Children.Remove(@new);
								@new.SetParent(This);
							}
						break;
				}
			};
		}

		public event ControllableListChangedEventHandler ChildrenChanged {
			add => PrivateChildren.ListChanged += value;
			remove => PrivateChildren.ListChanged -= value;
		}

		/// <summary>
		///     Parent node. Note that when modified, the <see cref="Children" /> of the new and old parent node will be
		///     automatically synchronized.
		/// </summary>
		public override T? Parent {
			get => base.Parent;
			set {
				if (Equals(base.Parent, value))
					return;
				if (base.Parent is not null) {
					base.Parent._updateParentOnChildrenChange = false;
					base.Parent._children.Remove(This);
					base.Parent._updateParentOnChildrenChange = true;
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
		///     Caution: DO NOT use indexers to reorder, which may break the synchronization with <see cref="Parent" />, thus all
		///     operations that attempt to add or replace with nodes that already exist in <see cref="Children" /> will result in
		///     <see cref="InvalidOperationException" />.<br />
		///     If reordering is really needed, use use <see cref="IExtendedList{T}.Swap" />,
		///     <see cref="IExtendedList{T}.Reverse()" /> or <see cref="IExtendedList{T}.Sort()" /> instead.
		/// </summary>
		public new IExtendedList<T> Children => PrivateChildren;

		public new static void Unlink(T node) {
			var parent = node.Parent;
			if (parent is null)
				foreach (var child in node.Children)
					child.Parent = null;
			else {
				int index = parent.Children.IndexOf(node);
				node.Parent = null;
				parent.Children.InsertRange(index, node.Children);
			}
		}

		private ControllableList<T> PrivateChildren => (ControllableList<T>)_children;
	}

	/// <summary>
	///     Tree node that ensures the synchronization between <c>Children</c> and <c>Parent</c>.
	///     If inheritance required, use <see cref="TreeNode{T}" /> instead.
	/// </summary>
	public sealed class TreeNode : TreeNode<TreeNode> {
		public TreeNode() { }

		public TreeNode(TreeNode? parent) : base(parent) { }
	}

	/// <summary>
	///     Tree node with value that ensures the synchronization between <c>Children</c> and <c>Parent</c>.
	///     If inheritance required, use <see cref="TreeNode{T}" /> instead.
	/// </summary>
	public sealed class ValuedTreeNode<T> : TreeNode<ValuedTreeNode<T>> {
		public ValuedTreeNode(T value) : this(value, null) { }

		public ValuedTreeNode(T value, ValuedTreeNode<T>? parent) : base(parent) => Value = value;

		public T Value { get; set; }
	}
}