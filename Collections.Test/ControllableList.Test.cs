using NUnit.Framework;
using TrueMogician.Extensions.Collections;

namespace Collections.Test {
	using ChangingHandler = ControllableListChangingEventHandler;
	using ChangedHandler = ControllableListChangedEventHandler;

	public class ControllableListTests {
		public ControllableList<int> List = new();

		[Test]
		public void AddTest() {
			var items = new[] {0, 1, 2, 3, 4};
			ChangingHandler changingHandler = (_, baseArgs) => {
				if (baseArgs is ControllableListAddingEventArgs<int> args)
					if (args.Value == 2)
						args.Cancel = true;
			};
			ChangedHandler changedHandler = (_, baseArgs) => {
				if (baseArgs is ControllableListAddedEventArgs<int> args)
					if (args.Value == 2)
						Assert.Fail();
			};
			List.ListChanging += changingHandler;
			List.ListChanged += changedHandler;
			foreach (int item in items)
				List.Add(item);
			Assert.AreEqual(4, List.Count);
			List.ListChanging -= changingHandler;
			List.ListChanged -= changedHandler;
			changingHandler = (_, baseArgs) => {
				if (baseArgs is ControllableListAddingRangeEventArgs<int> args) {
					if (args.Count == 1)
						Assert.Fail();
					else if (args.Count > 3)
						args.Cancel = true;
				}
			};
			changedHandler = (_, baseArgs) => {
				if (baseArgs is ControllableListRangeAddedEventArgs<int> args)
					if (args.Count > 3)
						Assert.Fail();
			};
			List.ListChanging += changingHandler;
			List.ListChanged += changedHandler;
			List.InsertRange(2, 2);
			List.AddRange(5, 6, 7, 8);
			List.AddRange(5, 6, 7);
			Assert.AreEqual(8, List.Count);
		}
	}
}