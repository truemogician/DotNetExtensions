using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueMogician.Extensions.Collections.Tree {
	public static class TreeUtilities {
		public static Forest<TTreeNode> BuildForest<TSource, TTreeNode>(IEnumerable<TSource> source, Func<TSource, TTreeNode> treeNodeSelector, Func<TSource, TTreeNode?> parentSelector) where TTreeNode : ITree<TTreeNode> {
			var treeNodes = source.Select(
				node => {
					var treeNode = treeNodeSelector(node);
					treeNode.Parent = parentSelector(node);
					return treeNode;
				}
			);
			return new Forest<TTreeNode>(treeNodes.Where(t => t.Parent is null).Select(t => new Tree<TTreeNode>(t)));
		}

		public static Forest<ValuedTreeNode<T>> BuildForest<T>(IEnumerable<T> source, Func<T, T?> parentSelector) {
			var treeNodes = source.Select(v => new ValuedTreeNode<T>(v)).ToArray();
			var dictionary = treeNodes.ToDictionary(t => t.Value);
			foreach (var node in treeNodes) {
				var parentValue = parentSelector(node.Value);
				if (parentValue is not null)
					node.Parent = dictionary[node.Value];
			}
			return new Forest<ValuedTreeNode<T>>(treeNodes.Where(t => t.Parent is null).Select(t => new Tree<ValuedTreeNode<T>>(t)));
		}

		public static Forest<ValuedTreeNode<TValue>> BuildForestByKey<TValue, TKey>(IEnumerable<TValue> source, Func<TValue, TKey> keySelector, Func<TValue, TKey?> parentKeySelector) {
			var src = source.ToArray();
			var dictionary = src.ToDictionary(keySelector);
			return BuildForest(src, v => parentKeySelector(v) is var k && k is null ? default : dictionary[k]);
		}
	}
}