using System.Linq;
using NUnit.Framework;
using TrueMogician.Extensions.Collections.Tree;

namespace Collections.Test {
	public class TreeUtilitiesTest {
		[Test]
		public void BuildForestTest() {
			var forest = TreeUtilities.BuildForest(
				new[] { "a", "b", "c" },
				v => v switch {
					"a" => null,
					"b" => "a",
					"c" => "a"
				}
			);
			var trees = forest.ToArray();
			Assert.AreEqual(1, trees.Length);
			Assert.AreEqual("a", trees[0].Root.Value);
		}
	}
}