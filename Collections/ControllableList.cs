using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TrueMogician.Extensions.Enumerable;

namespace TrueMogician.Extensions.Collections {
	/// <summary>
	///     A list of strongly typed object with changed and cancelable changing events
	/// </summary>
	public class ControllableList<T> : IExtendedList<T> {
		#region Fields
		private readonly List<T> _list;
		#endregion

		#region Constructors
		public ControllableList() => _list = new List<T>();

		public ControllableList(int capacity) => _list = new List<T>(capacity);

		public ControllableList(IEnumerable<T> collection) {
			var arr = collection.AsArray();
			_list = new List<T>(arr.Length);
			AddRange(arr);
		}
		#endregion

		#region Interface Implementations
		public int Count => _list.Count;

		public bool IsReadOnly => false;

		public T this[int index] {
			get => _list[index];
			set {
				ThrowHelper.WhenNegativeOrGreaterEqual(index, nameof(index), _list.Count);
				var old = _list[index];
				if (ChangingEventEnabled && !OnListChanging(new ControllableListReplacingEventArgs<T>(old, value, index)))
					return;
				_list[index] = value;
				if (ChangedEventEnabled)
					OnListChanged(new ControllableListReplacedEventArgs<T>(old, value, index));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

		public void Add(T item) {
			if (ChangingEventEnabled && !OnListChanging(new ControllableListAddingEventArgs<T>(item, _list.Count)))
				return;
			_list.Add(item);
			if (ChangedEventEnabled)
				OnListChanged(new ControllableListAddedEventArgs<T>(item, _list.Count - 1));
		}

		public void Clear() {
			if (!ChangingEventEnabled && !ChangedEventEnabled)
				_list.Clear();
			else {
				var list = new List<T>(_list);
				if (ChangingEventEnabled && !OnListChanging(new ControllableListRemovingRangeEventArgs<T>(list, 0)))
					return;
				_list.Clear();
				if (ChangedEventEnabled)
					OnListChanged(new ControllableListRangeRemovedEventArgs<T>(list, 0));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(T item) => _list.Contains(item);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

		/// <returns>
		///     <see langword="true" /> if <paramref name="item" /> was successfully removed from the
		///     <see cref="ControllableList{T}" />; otherwise, <see langword="false" />. This method also
		///     returns <see langword="false" /> if the operation is canceled or <paramref name="item" /> is not found in the
		///     original <see cref="ControllableList{T}" />.
		/// </returns>
		public bool Remove(T item) {
			int index = _list.IndexOf(item);
			if (index == -1 || ChangingEventEnabled && !OnListChanging(new ControllableListRemovingEventArgs<T>(item, index)))
				return false;
			_list.RemoveAt(index);
			if (ChangedEventEnabled)
				OnListChanged(new ControllableListRemovedEventArgs<T>(item, index));
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item) => _list.IndexOf(item);

		public void Insert(int index, T item) {
			ThrowHelper.WhenNegativeOrGreaterEqual(index, nameof(index), _list.Count);
			if (ChangingEventEnabled && !OnListChanging(new ControllableListAddingEventArgs<T>(item, index)))
				return;
			_list.Insert(index, item);
			if (ChangedEventEnabled)
				OnListChanged(new ControllableListAddedEventArgs<T>(item, index));
		}

		public void RemoveAt(int index) {
			ThrowHelper.WhenNegativeOrGreaterEqual(index, nameof(index), _list.Count);
			var item = _list[index];
			if (ChangingEventEnabled && !OnListChanging(new ControllableListRemovingEventArgs<T>(item, index)))
				return;
			_list.RemoveAt(index);
			if (ChangedEventEnabled)
				OnListChanged(new ControllableListRemovedEventArgs<T>(item, index));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion

		#region Events
		/// <summary>
		///     Occurs when a modification to the <see cref="ControllableList{T}" /> is about to happen. Set
		///     <see cref="ControllableListChangingEventArgs.Cancel" /> to true to cancel the modification
		/// </summary>
		public event ControllableListChangingEventHandler ListChanging = delegate { };

		/// <summary>
		///     Occurs when a modification to the <see cref="ControllableList{T}" /> has already happened.
		/// </summary>
		public event ControllableListChangedEventHandler ListChanged = delegate { };
		#endregion

		#region Indexers
		#if NETSTANDARD2_1
		public T this[Index index] {
			get => index.IsFromEnd ? this[Count - index.Value] : this[index.Value];
			set {
				if (index.IsFromEnd)
					this[Count - index.Value] = value;
				else
					this[index.Value] = value;
			}
		}

		public IList<T> this[Range range] {
			get {
				int start = range.Start is var l && l.IsFromEnd ? _list.Count - l.Value : l.Value;
				int end = range.End is var r && r.IsFromEnd ? _list.Count - r.Value : r.Value;
				return GetRange(start, end - start);
			}
			set {
				int start = range.Start is var l && l.IsFromEnd ? _list.Count - l.Value : l.Value;
				int end = range.End is var r && r.IsFromEnd ? _list.Count - r.Value : r.Value;
				SetRange(start, end - start, value);
			}
		}
		#endif
		#endregion

		#region Properties
		/// <inheritdoc cref="List{T}.Capacity" />
		public int Capacity {
			get => _list.Capacity;
			set => _list.Capacity = value;
		}

		/// <summary>
		///     Set to false to disable <see cref="ListChanging" /> event and save relevant space and time cost
		/// </summary>
		public bool ChangingEventEnabled { get; set; } = true;

		/// <summary>
		///     Set to false to disable <see cref="ListChanged" /> event and save relevant space and time cost
		/// </summary>
		public bool ChangedEventEnabled { get; set; } = true;
		#endregion

		#region Methods
		#region Mapped Methods
		/// <inheritdoc cref="List{T}.IndexOf(T, int)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int index) => _list.IndexOf(item, index);

		/// <inheritdoc cref="List{T}.IndexOf(T, int, int)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int index, int count) => _list.IndexOf(item, index, count);

		/// <inheritdoc cref="List{T}.LastIndexOf(T)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int LastIndexOf(T item) => _list.LastIndexOf(item);

		/// <inheritdoc cref="List{T}.LastIndexOf(T, int)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int LastIndexOf(T item, int index) => _list.LastIndexOf(item, index);

		/// <inheritdoc cref="List{T}.LastIndexOf(T, int, int)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int LastIndexOf(T item, int index, int count) => _list.LastIndexOf(item, index, count);

		/// <inheritdoc cref="List{T}.Find(Predicate{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Find(Predicate<T> match) => _list.Find(match);

		/// <inheritdoc cref="List{T}.FindLast(Predicate{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T FindLast(Predicate<T> match) => _list.FindLast(match);

		/// <inheritdoc cref="List{T}.FindAll(Predicate{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public List<T> FindAll(Predicate<T> match) => _list.FindAll(match);

		/// <inheritdoc cref="List{T}.FindIndex(Predicate{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindIndex(Predicate<T> match) => _list.FindIndex(match);

		/// <inheritdoc cref="List{T}.FindIndex(int, Predicate{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindIndex(int index, Predicate<T> match) => _list.FindIndex(index, match);

		/// <inheritdoc cref="List{T}.FindIndex(int,int,Predicate{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindIndex(int index, int count, Predicate<T> match) => _list.FindIndex(index, count, match);

		/// <inheritdoc cref="List{T}.FindLastIndex(Predicate{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindLastIndex(Predicate<T> match) => _list.FindLastIndex(match);

		/// <inheritdoc cref="List{T}.FindLastIndex(int, Predicate{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindLastIndex(int index, Predicate<T> match) => _list.FindLastIndex(index, match);

		/// <inheritdoc cref="List{T}.FindLastIndex(int,int,Predicate{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int FindLastIndex(int index, int count, Predicate<T> match) => _list.FindLastIndex(index, count, match);

		/// <inheritdoc cref="List{T}.GetRange(int,int)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IList<T> GetRange(int index, int count) => _list.GetRange(index, count);
		#endregion

		/// <summary>
		///     Set a slice of the source <see cref="ControllableList{T}" /> to <paramref name="values" />. The number of elements
		///     in the slice doesn't have to be equal to that of <paramref name="values" />.
		/// </summary>
		public void SetRange(int index, int count, IEnumerable<T> values) {
			var arr = values.AsArray();
			if (count == 0) {
				InsertRange(index, arr);
				return;
			}
			if (arr.Length == 0) {
				RemoveRange(index, count);
				return;
			}
			ThrowHelper.WhenNegativeOrGreater(index, nameof(index), _list.Count);
			ThrowHelper.WhenNegativeOrGreater(count, nameof(count), _list.Count - index, "");
			int replaceCount = Math.Min(count, arr.Length);
			if (replaceCount == 1)
				this[index] = arr[0];
			else {
				if (!ChangingEventEnabled && !ChangedEventEnabled)
					for (var i = 0; i < replaceCount; ++i)
						_list[index + i] = arr[i];
				else {
					var old = _list.GetRange(index, count);
					var @new = replaceCount == arr.Length ? arr : arr.Take(replaceCount).ToArray();
					if (ChangingEventEnabled && !OnListChanging(new ControllableListReplacingRangeEventArgs<T>(old, @new, index)))
						return;
					for (var i = 0; i < replaceCount; ++i)
						_list[index + i] = arr[i];
					if (ChangedEventEnabled)
						OnListChanged(new ControllableListRangeReplacedEventArgs<T>(old, @new, index));
				}
			}
			if (count > replaceCount)
				RemoveRange(index + replaceCount, count - replaceCount);
			else if (arr.Length > replaceCount)
				InsertRange(index + replaceCount, arr.Skip(replaceCount));
		}

		public void AddRange(params T[] items) {
			switch (items.Length) {
				case 0: return;
				case 1:
					Add(items[0]);
					break;
				default: {
					if (ChangingEventEnabled && !OnListChanging(new ControllableListAddingRangeEventArgs<T>(items, _list.Count)))
						return;
					_list.AddRange(items);
					if (ChangedEventEnabled)
						OnListChanged(new ControllableListRangeAddedEventArgs<T>(items, _list.Count - items.Length));
					break;
				}
			}
		}

		/// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange(IEnumerable<T> items) => AddRange(items.AsArray());

		public void InsertRange(int index, params T[] items) {
			ThrowHelper.WhenNegativeOrGreaterEqual(index, nameof(index), _list.Count);
			switch (items.Length) {
				case 0: return;
				case 1:
					Insert(index, items[0]);
					break;
				default: {
					if (ChangingEventEnabled && !OnListChanging(new ControllableListAddingRangeEventArgs<T>(items, index)))
						return;
					_list.InsertRange(index, items);
					if (ChangedEventEnabled)
						OnListChanged(new ControllableListRangeAddedEventArgs<T>(items, index));
					break;
				}
			}
		}

		/// <inheritdoc cref="List{T}.InsertRange(int, IEnumerable{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InsertRange(int index, IEnumerable<T> items) => InsertRange(index, items.AsArray());

		/// <inheritdoc cref="List{T}.RemoveRange(int,int)" />
		public void RemoveRange(int index, int count) {
			ThrowHelper.WhenNegativeOrGreater(index, nameof(index), _list.Count);
			ThrowHelper.WhenNegativeOrGreater(count, nameof(count), _list.Count - index, "");
			switch (count) {
				case 0: return;
				case 1:
					RemoveAt(index);
					break;
				default: {
					if (!ChangingEventEnabled && !ChangedEventEnabled)
						_list.RemoveRange(index, count);
					else {
						var slice = _list.GetRange(index, count);
						if (ChangingEventEnabled && !OnListChanging(new ControllableListRemovingRangeEventArgs<T>(slice, index)))
							return;
						_list.RemoveRange(index, count);
						if (ChangedEventEnabled)
							OnListChanged(new ControllableListRangeRemovedEventArgs<T>(slice, index));
					}
					break;
				}
			}
		}

		/// <inheritdoc cref="List{T}.Reverse(int,int)" />
		public void Reverse(int index, int count) {
			ThrowHelper.WhenNegativeOrGreater(index, nameof(index), _list.Count);
			ThrowHelper.WhenNegativeOrGreater(count, nameof(count), _list.Count - index, "");
			if (count < 2 || ChangingEventEnabled && !OnListChanging(new ControllableListReorderingEventArgs<T>(_list.GetRange(index, count), index)))
				return;
			_list.Reverse(index, count);
			if (ChangedEventEnabled)
				OnListChanged(new ControllableListReorderedEventArgs<T>(_list.GetRange(index, count), index));
		}

		/// <inheritdoc cref="List{T}.Reverse()" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reverse() => Reverse(0, _list.Count);

		/// <inheritdoc cref="List{T}.Sort(int, int, IComparer{T})" />
		public void Sort(int index, int count, IComparer<T> comparer) {
			ThrowHelper.WhenNegativeOrGreater(index, nameof(index), _list.Count);
			ThrowHelper.WhenNegativeOrGreater(count, nameof(count), _list.Count - index, "");
			if (ChangingEventEnabled && !OnListChanging(new ControllableListReorderingEventArgs<T>(_list.GetRange(index, count), index)))
				return;
			_list.Sort(index, count, comparer);
			if (ChangedEventEnabled)
				OnListChanged(new ControllableListReorderedEventArgs<T>(_list.GetRange(index, count), index));
		}

		/// <inheritdoc cref="List{T}.Sort(IComparer{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort(IComparer<T> comparer) => Sort(0, _list.Count, comparer);

		/// <inheritdoc cref="List{T}.Sort()" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort() => Sort(0, _list.Count, Comparer<T>.Default);

		/// <inheritdoc cref="List{T}.Sort(int, int, IComparer{T})" />
		/// <param name="comparison">
		///     <inheritdoc cref="List{T}.Sort(Comparison{T})" />
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort(int index, int count, Comparison<T> comparison) => Sort(index, count, Comparer<T>.Create(comparison));

		/// <inheritdoc cref="List{T}.Sort(Comparison{T})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sort(Comparison<T> comparison) => Sort(0, _list.Count, Comparer<T>.Create(comparison));

		/// <returns>True to proceed, false to cancel the operation</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected bool OnListChanging(ControllableListChangingEventArgs args) {
			ListChanging(this, args);
			return !args.Cancel;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void OnListChanged(ControllableListChangedEventArgs args) => ListChanged(this, args);
		#endregion

		private static class ThrowHelper {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			/// <param name="min">Minimum value(inclusive)</param>
			/// <param name="max">Maximum value(exclusive)</param>
			private static void WhenNotIn(int value, string name, int min, int max, string? message = null) {
				if (value < min || value >= max) {
					message ??= $"Parameter {name} must be greater than or equal to {min} and less than {max}";
					throw new ArgumentOutOfRangeException(name, value, message);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void WhenNegativeOrGreater(int value, string name, int max, string? message = null) => WhenNotIn(value, name, 0, max - 1, message ?? $"Parameter {name} must be positive and no greater than {max}");

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void WhenNegativeOrGreaterEqual(int value, string name, int max, string? message = null) => WhenNotIn(value, name, 0, max, message ?? $"Parameter {name} must be positive and less than {max}");
		}
	}

	public enum ControllableListChangeAction : byte {
		Add,

		AddRange,

		Remove,

		RemoveRange,

		Replace,

		ReplaceRange,

		Reorder
	}

	#region ControllableListChangedEventArgs
	public abstract class ControllableListChangedEventArgs : EventArgs {
		public abstract ControllableListChangeAction Action { get; }
	}

	public abstract class ControllableListAddedRemovedEventArgs<T> : ControllableListChangedEventArgs {
		protected ControllableListAddedRemovedEventArgs(T value, int index) {
			Value = value;
			Index = index;
		}

		public T Value { get; }

		public int Index { get; }
	}

	public abstract class ControllableListRangeChangedEventArgs<T> : ControllableListChangedEventArgs {
		protected ControllableListRangeChangedEventArgs(IReadOnlyList<T> values, int index) {
			Values = values;
			StartIndex = index;
		}

		public IReadOnlyList<T> Values { get; }

		public int Count => Values.Count;

		public int StartIndex { get; }

		public int EndIndex => StartIndex + Values.Count;
	}

	public class ControllableListAddedEventArgs<T> : ControllableListAddedRemovedEventArgs<T> {
		public ControllableListAddedEventArgs(T value, int index) : base(value, index) { }

		public override ControllableListChangeAction Action => ControllableListChangeAction.Add;
	}

	public class ControllableListRangeAddedEventArgs<T> : ControllableListRangeChangedEventArgs<T> {
		public ControllableListRangeAddedEventArgs(IReadOnlyList<T> values, int index) : base(values, index) { }

		public override ControllableListChangeAction Action => ControllableListChangeAction.AddRange;
	}

	public class ControllableListRemovedEventArgs<T> : ControllableListAddedRemovedEventArgs<T> {
		public ControllableListRemovedEventArgs(T value, int index) : base(value, index) { }

		public override ControllableListChangeAction Action => ControllableListChangeAction.Remove;
	}

	public class ControllableListRangeRemovedEventArgs<T> : ControllableListRangeChangedEventArgs<T> {
		public ControllableListRangeRemovedEventArgs(IReadOnlyList<T> values, int index) : base(values, index) { }

		public override ControllableListChangeAction Action => ControllableListChangeAction.RemoveRange;
	}

	public class ControllableListReplacedEventArgs<T> : ControllableListChangedEventArgs {
		public ControllableListReplacedEventArgs(T oldValue, T newValue, int index) {
			OldValue = oldValue;
			NewValue = newValue;
			Index = index;
		}

		public override ControllableListChangeAction Action => ControllableListChangeAction.Replace;

		public T OldValue { get; }

		public T NewValue { get; }

		public int Index { get; }
	}

	public class ControllableListRangeReplacedEventArgs<T> : ControllableListChangedEventArgs {
		public ControllableListRangeReplacedEventArgs(IReadOnlyList<T> oldValues, IReadOnlyList<T> newValues, int index) {
			if (oldValues.Count != newValues.Count)
				throw new ArgumentException("oldValue and newValue should contains the same number of items");
			OldValues = oldValues;
			NewValues = newValues;
			StartIndex = index;
		}

		public override ControllableListChangeAction Action => ControllableListChangeAction.ReplaceRange;

		public IReadOnlyList<T> OldValues { get; }

		public IReadOnlyList<T> NewValues { get; }

		public int Count => OldValues.Count;

		public int StartIndex { get; }

		public int EndIndex => StartIndex + OldValues.Count;
	}

	public class ControllableListReorderedEventArgs<T> : ControllableListRangeChangedEventArgs<T> {
		public override ControllableListChangeAction Action => ControllableListChangeAction.Reorder;

		public ControllableListReorderedEventArgs(IReadOnlyList<T> values, int index) : base(values, index) { }
	}
	#endregion

	#region ControllableListChangingEventArgs
	public abstract class ControllableListChangingEventArgs : EventArgs {
		public abstract ControllableListChangeAction Action { get; }

		public bool Cancel { get; set; } = false;
	}

	public abstract class ControllableListAddingRemovingEventArgs<T> : ControllableListChangingEventArgs {
		protected ControllableListAddingRemovingEventArgs(T value, int index) {
			Value = value;
			Index = index;
		}

		public T Value { get; }

		public int Index { get; }
	}

	public abstract class ControllableListChangingRangeEventArgs<T> : ControllableListChangingEventArgs {
		protected ControllableListChangingRangeEventArgs(IReadOnlyList<T> values, int index) {
			Values = values;
			StartIndex = index;
		}

		public IReadOnlyList<T> Values { get; }

		public int Count => Values.Count;

		public int StartIndex { get; }

		public int EndIndex => StartIndex + Values.Count;
	}

	public class ControllableListAddingEventArgs<T> : ControllableListAddingRemovingEventArgs<T> {
		public ControllableListAddingEventArgs(T value, int index) : base(value, index) { }

		public override ControllableListChangeAction Action => ControllableListChangeAction.Add;
	}

	public class ControllableListAddingRangeEventArgs<T> : ControllableListChangingRangeEventArgs<T> {
		public ControllableListAddingRangeEventArgs(IReadOnlyList<T> values, int index) : base(values, index) { }

		public override ControllableListChangeAction Action => ControllableListChangeAction.AddRange;
	}

	public class ControllableListRemovingEventArgs<T> : ControllableListAddingRemovingEventArgs<T> {
		public ControllableListRemovingEventArgs(T value, int index) : base(value, index) { }

		public override ControllableListChangeAction Action => ControllableListChangeAction.Remove;
	}

	public class ControllableListRemovingRangeEventArgs<T> : ControllableListChangingRangeEventArgs<T> {
		public ControllableListRemovingRangeEventArgs(IReadOnlyList<T> values, int index) : base(values, index) { }

		public override ControllableListChangeAction Action => ControllableListChangeAction.RemoveRange;
	}

	public class ControllableListReplacingEventArgs<T> : ControllableListChangingEventArgs {
		public ControllableListReplacingEventArgs(T oldValue, T newValue, int index) {
			OldValue = oldValue;
			NewValue = newValue;
			Index = index;
		}

		public override ControllableListChangeAction Action => ControllableListChangeAction.Replace;

		public T OldValue { get; }

		public T NewValue { get; }

		public int Index { get; }
	}

	public class ControllableListReplacingRangeEventArgs<T> : ControllableListChangingEventArgs {
		public ControllableListReplacingRangeEventArgs(IReadOnlyList<T> oldValues, IReadOnlyList<T> newValues, int index) {
			if (oldValues.Count != newValues.Count)
				throw new ArgumentException("oldValue and newValue should contains the same number of items");
			OldValues = oldValues;
			NewValues = newValues;
			StartIndex = index;
		}

		public override ControllableListChangeAction Action => ControllableListChangeAction.ReplaceRange;

		public IReadOnlyList<T> OldValues { get; }

		public IReadOnlyList<T> NewValues { get; }

		public int Count => OldValues.Count;

		public int StartIndex { get; }

		public int EndIndex => StartIndex + OldValues.Count;
	}

	public class ControllableListReorderingEventArgs<T> : ControllableListChangingRangeEventArgs<T> {
		public override ControllableListChangeAction Action => ControllableListChangeAction.Reorder;

		public ControllableListReorderingEventArgs(IReadOnlyList<T> values, int index) : base(values, index) { }
	}
	#endregion

	public delegate void ControllableListChangedEventHandler(object? sender, ControllableListChangedEventArgs args);

	public delegate void ControllableListChangingEventHandler(object? sender, ControllableListChangingEventArgs args);
}