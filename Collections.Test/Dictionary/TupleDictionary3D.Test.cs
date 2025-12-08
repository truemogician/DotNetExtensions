using System;
using System.Collections.Generic;
using NUnit.Framework;
using TrueMogician.Extensions.Collections.Dictionary;

namespace Collections.Test {
	[TestFixture]
	public class TupleDictionary3DTests {
		[SetUp]
		public void Setup() {
			_dict = new TupleDictionary3D<string, int, string>();
		}

		private TupleDictionary3D<string, int, string> _dict = null!;

		[Test]
		public void Add_And_Indexer_Get_ShouldWork() {
			_dict.Add("A", 1, "Value1");
			_dict["B", 2] = "Value2"; // Setter add

			Assert.That(_dict.Count, Is.EqualTo(2));
			Assert.That(_dict["A", 1], Is.EqualTo("Value1"));
			Assert.That(_dict["B", 2], Is.EqualTo("Value2"));
		}

		[Test]
		public void Indexer_Set_ShouldUpdateExistingValue() {
			_dict.Add("A", 1, "Original");
			_dict["A", 1] = "Updated";

			Assert.That(_dict["A", 1], Is.EqualTo("Updated"));
			Assert.That(_dict.Count, Is.EqualTo(1));
		}

		[Test]
		public void Indexer_Get_MissingKey_ShouldThrow() {
			Assert.Throws<KeyNotFoundException>(() => {
				string _ = _dict["Missing", 99];
			});
		}

		[Test]
		public void ContainsKey_ShouldReturnCorrectStatus() {
			_dict.Add("A", 1, "Val");

			Assert.That(_dict.ContainsKey("A", 1), Is.True);
			Assert.That(_dict.ContainsKey("A", 2), Is.False); // Wrong key2
			Assert.That(_dict.ContainsKey("B", 1), Is.False); // Wrong key1
		}

		[Test]
		public void Remove_ByKeys_ShouldWork() {
			_dict.Add("A", 1, "Val");

			bool removed = _dict.Remove("A", 1);
			bool removedMissing = _dict.Remove("Z", 99);

			Assert.That(removed, Is.True);
			Assert.That(removedMissing, Is.False);
			Assert.That(_dict.Count, Is.EqualTo(0));
		}

		[Test]
		public void TryGetValue_ShouldWork() {
			_dict.Add("A", 1, "Val");

			bool found = _dict.TryGetValue("A", 1, out string? val);
			bool notFound = _dict.TryGetValue("Z", 99, out string? nullVal);

			Assert.That(found, Is.True);
			Assert.That(val, Is.EqualTo("Val"));

			Assert.That(notFound, Is.False);
			Assert.That(nullVal, Is.Null);
		}

		[Test]
		public void Clear_ShouldEmptyCollection() {
			_dict.Add("A", 1, "V1");
			_dict.Add("B", 2, "V2");
			_dict.Clear();

			Assert.That(_dict.Count, Is.EqualTo(0));
			Assert.That(_dict.Keys, Is.Empty);
		}

		[Test]
		public void ICollection_Add_ShouldWork() {
			_dict.Add(("A", 1, "Val"));

			Assert.That(_dict.ContainsKey("A", 1), Is.True);
			Assert.That(_dict["A", 1], Is.EqualTo("Val"));
		}

		[Test]
		public void ICollection_Contains_ShouldCheckValueEquality() {
			_dict.Add("A", 1, "Val");

			// Correct keys, correct value
			Assert.That(_dict.Contains(("A", 1, "Val")), Is.True);
			// Correct keys, wrong value
			Assert.That(_dict.Contains(("A", 1, "WrongVal")), Is.False);
			// Wrong keys
			Assert.That(_dict.Contains(("Z", 99, "Val")), Is.False);
		}

		[Test]
		public void ICollection_Remove_ShouldRespectValueEquality() {
			_dict.Add("A", 1, "Val");

			// Try remove with wrong value (should fail)
			bool removedWrongVal = _dict.Remove(("A", 1, "Other"));
			Assert.That(removedWrongVal, Is.False);
			Assert.That(_dict.Count, Is.EqualTo(1));

			// Remove with correct value
			bool removedCorrect = _dict.Remove(("A", 1, "Val"));
			Assert.That(removedCorrect, Is.True);
			Assert.That(_dict.Count, Is.EqualTo(0));
		}

		[Test]
		public void CopyTo_ShouldCopyTuples() {
			_dict.Add("A", 1, "V1");
			_dict.Add("B", 2, "V2");

			var array = new (string, int, string)[2];
			_dict.CopyTo(array, 0);

			// Note: Dictionary order is undefined, so we check existence
			Assert.That(array, Has.Member(("A", 1, "V1")));
			Assert.That(array, Has.Member(("B", 2, "V2")));
		}

		[Test]
		public void GetEnumerator_ShouldEnumerateTriples() {
			_dict.Add("A", 1, "V1");
			_dict.Add("B", 2, "V2");

			var list = new List<(string, int, string)>();
			foreach (var item in _dict)
				list.Add(item);

			Assert.That(list.Count, Is.EqualTo(2));
			Assert.That(list, Has.Member(("A", 1, "V1")));
		}

		[Test]
		public void Keys_And_Values_Properties_ShouldMatchContent() {
			_dict.Add("A", 1, "V1");

			Assert.That(_dict.Keys, Does.Contain(("A", 1)));
			Assert.That(_dict.Values, Does.Contain("V1"));
		}

		[Test]
		public void Constructor_WithComparers_ShouldBeCaseInsensitive() {
			// Case-insensitive string keys
			var insensitiveDict = new TupleDictionary3D<string, string, int>(
				StringComparer.OrdinalIgnoreCase,
				StringComparer.OrdinalIgnoreCase
			) { { "KeyA", "KeyB", 100 } };

			// Should retrieve using lowercase
			Assert.That(insensitiveDict["keya", "keyb"], Is.EqualTo(100));
			Assert.That(insensitiveDict.Comparer1, Is.EqualTo(StringComparer.OrdinalIgnoreCase));
		}

		[Test]
		public void Constructor_Copy_ShouldCopyItems() {
			_dict.Add("A", 1, "Val");
			var copy = new TupleDictionary3D<string, int, string>(_dict);

			Assert.That(copy.Count, Is.EqualTo(1));
			Assert.That(copy["A", 1], Is.EqualTo("Val"));
		}

		[Test]
		public void Transpose_ShouldSwapKeys() {
			// Original: <string, int, string>
			_dict.Add("A", 1, "Value1");
			_dict.Add("B", 2, "Value2");

			// Transposed: <int, string, string>
			var transposed = _dict.Transpose();

			Assert.That(transposed.Count, Is.EqualTo(2));
			// Key1 becomes Key2, Key2 becomes Key1
			Assert.That(transposed[1, "A"], Is.EqualTo("Value1"));
			Assert.That(transposed[2, "B"], Is.EqualTo("Value2"));
		}

		[Test]
		public void Transpose_ShouldSwapComparers() {
			var comp1 = StringComparer.OrdinalIgnoreCase;
			var comp2 = StringComparer.CurrentCulture;
			var complexDict = new TupleDictionary3D<string, string, int>(comp1, comp2);
			var transposed = complexDict.Transpose();

			Assert.That(transposed.Comparer1, Is.EqualTo(comp2));
			Assert.That(transposed.Comparer2, Is.EqualTo(comp1));
		}

		[Test]
		public void NullKeys_ShouldWork_IfTuplesAllowIt() {
			// Note: Standard Dictionary allows null keys if the type is nullable 
			// and the comparer handles it. Tuple equality usually handles null components.

			var nullDict = new TupleDictionary3D<string?, string?, int> {
				{ null, null, 1 },
				{ "A", null, 2 }
			};

			Assert.That(nullDict[null, null], Is.EqualTo(1));
			Assert.That(nullDict["A", null], Is.EqualTo(2));
		}
	}
}