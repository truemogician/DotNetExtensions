using System;
using System.Collections.Generic;
using NUnit.Framework;
using TrueMogician.Extensions.Collections.Dictionary;

namespace Collections.Test {
	[TestFixture]
	public class TuplePartialDictionary3DTests {
		[SetUp]
		public void Setup() {
			_dict = new TuplePartialDictionary3D<string, int, string>();
		}

		private TuplePartialDictionary3D<string, int, string> _dict = null!;

		[Test]
		public void FirstKeys_ShouldReturnUniqueKey1Components() {
			_dict.Add("A", 1, "Val1");
			_dict.Add("A", 2, "Val2");
			_dict.Add("B", 1, "Val3");

			var firstKeys = _dict.FirstKeys;

			Assert.That(firstKeys.Count, Is.EqualTo(2));
			Assert.That(firstKeys, Contains.Item("A"));
			Assert.That(firstKeys, Contains.Item("B"));
		}

		[Test]
		public void FirstKeys_ShouldUpdateAfterRemoval() {
			_dict.Add("A", 1, "Val1");
			_dict.Remove("A", 1);

			Assert.That(_dict.FirstKeys, Is.Empty);
		}

		[Test]
		public void ContainsKey_Partial_ShouldReturnCorrectStatus() {
			_dict.Add("Group1", 10, "Data");

			Assert.That(_dict.ContainsKey("Group1"), Is.True);
			Assert.That(_dict.ContainsKey("Group2"), Is.False);
		}

		[Test]
		public void ContainsKey_Partial_ShouldRespectComparer() {
			var insensitiveDict = new TuplePartialDictionary3D<string, int, string>(
				new TupleDictionary3D<string, int, string>(
					StringComparer.OrdinalIgnoreCase,
					EqualityComparer<int>.Default
				)
			);

			insensitiveDict.Add("UPPER", 1, "Val");

			Assert.That(insensitiveDict.ContainsKey("upper"), Is.True);
		}

		[Test]
		public void Indexer_Partial_Get_ShouldReturnSubDictionary() {
			_dict.Add("A", 1, "V1");
			_dict.Add("A", 2, "V2");
			_dict.Add("B", 3, "V3");

			var subDict = _dict["A"];

			Assert.That(subDict.Count, Is.EqualTo(2));
			Assert.That(subDict[1], Is.EqualTo("V1"));
			Assert.That(subDict[2], Is.EqualTo("V2"));
			Assert.That(subDict.ContainsKey(3), Is.False);
		}

		[Test]
		public void Indexer_Partial_Get_MissingKey_ShouldThrow() {
			Assert.Throws<KeyNotFoundException>(() => { _ = _dict["NonExistent"]; });
		}

		[Test]
		public void TryGetValues_ShouldReturnTrueAndValues_WhenExists() {
			_dict.Add("A", 1, "V1");

			bool result = _dict.TryGetValues("A", out var values);

			Assert.That(result, Is.True);
			Assert.That(values, Is.Not.Null);
			Assert.That(values.Count, Is.EqualTo(1));
			Assert.That(values[1], Is.EqualTo("V1"));
		}

		[Test]
		public void TryGetValues_ShouldReturnFalse_WhenMissing() {
			bool result = _dict.TryGetValues("Missing", out var values);

			Assert.That(result, Is.False);
			Assert.That(values, Is.Empty);
		}

		[Test]
		public void Add_Partial_Bulk_ShouldAddAllItems() {
			var bulkItems = new Dictionary<int, string> {
				{ 10, "Ten" },
				{ 20, "Twenty" }
			};

			_dict.Add("GroupX", bulkItems);

			Assert.That(_dict.Count, Is.EqualTo(2));
			Assert.That(_dict["GroupX", 10], Is.EqualTo("Ten"));
			Assert.That(_dict["GroupX", 20], Is.EqualTo("Twenty"));
		}

		[Test]
		public void Add_Partial_Bulk_ShouldThrowIfAnyDuplicateExists() {
			_dict.Add("A", 1, "Existing");

			var newItems = new Dictionary<int, string> {
				{ 2, "New" },
				{ 1, "Conflict" } // Collision
			};

			var count = _dict.Count;
			Assert.Throws<ArgumentException>(() => _dict.Add("A", newItems));
            // Should be atomic
            Assert.That(_dict.Count, Is.EqualTo(count));
		}

		[Test]
		public void Remove_Partial_ShouldRemoveAllMatches() {
			_dict.Add("A", 1, "V1");
			_dict.Add("A", 2, "V2");
			_dict.Add("B", 3, "V3");

			int removedCount = _dict.Remove("A");

			Assert.That(removedCount, Is.EqualTo(2));
			Assert.That(_dict.ContainsKey("A", 1), Is.False);
			Assert.That(_dict.ContainsKey("A", 2), Is.False);
			Assert.That(_dict.ContainsKey("B", 3), Is.True);
		}

		[Test]
		public void Remove_Partial_ShouldSafelyHandleEnumerationBug() {
			for (var i = 0; i < 10; i++)
				_dict.Add("DeleteMe", i, $"Val{i}");

			int count = _dict.Remove("DeleteMe");
			Assert.That(count, Is.EqualTo(10));
			Assert.That(_dict.Count, Is.EqualTo(0));
		}
	}
}