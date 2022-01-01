using System.Linq;
using NUnit.Framework;
using TrueMogician.Extensions.Collections.Tree;

namespace Collections.Tree.Test {
	public class StatisticalTreeNodeTests {
		public static ValuedStatisticalTreeNode<string?> CreateNewNode(string? value = null) => new(value);

		public ValuedStatisticalTreeNode<string?> Root = CreateNewNode("root");

		public ValuedStatisticalTreeNode<string?>[] Nodes = new ValuedStatisticalTreeNode<string?>[8];

		[SetUp]
		public void BuildTree() {
			for (var i = 0; i < Nodes.Length; ++i)
				Nodes[i] = CreateNewNode($"{i}");
			Nodes[0].Parent = Root;
			Nodes[1].Parent = Root;
			Nodes[2].Parent = Root;
			Nodes[3].Parent = Nodes[1];
			Nodes[4].Parent = Nodes[1];
			Nodes[5].Parent = Nodes[3];
			Nodes[6].Parent = Nodes[3];
			Nodes[7].Parent = Nodes[2];
		}

		[Test]
		public void StatisticsTest() {
			Assert.AreEqual(Root.Depth, 0);
			Assert.AreEqual(Root.Height, 4);
			Assert.AreEqual(Root.Size, 9);
			Assert.AreEqual(Nodes[3].Depth, 2);
			Assert.AreEqual(Nodes[3].Height, 2);
			Assert.AreEqual(Nodes[3].Size, 3);
		}

		[Test]
		public void UtilitiesTest() {
			Assert.IsTrue(Root.IsRoot);
			Assert.IsTrue(Nodes[0].IsLeaf);
			Assert.IsTrue(Root.IsAncestorOf(Nodes[7]));
			Assert.IsFalse(Nodes[5].IsChildOf(Nodes[0]));
			Assert.AreEqual(new[] {Nodes[0], Nodes[5], Nodes[6], Nodes[4], Nodes[7]}, Root.Leaves.ToArray());
			Assert.AreEqual(new[] {Nodes[3], Nodes[1], Root}, Nodes[6].Ancestors.ToArray());
			Assert.AreEqual(new[] { Nodes[3], Nodes[5], Nodes[6], Nodes[4] }, Nodes[1].Descendents.ToArray());
			Assert.AreSame(Nodes[1], ValuedStatisticalTreeNode<string?>.GetLatestCommonAncestor(Nodes[5], Nodes[4]));
			ValuedStatisticalTreeNode<string?>.Unlink(Nodes[1]);
			Assert.AreEqual(4, Root.Children.Count);
			Assert.IsNull(Nodes[1].Parent);
			Assert.AreEqual(0, Nodes[1].Children.Count);
		}
	}
}