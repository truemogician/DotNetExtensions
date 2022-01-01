using System.Collections;
using System.Collections.Generic;

namespace Enumerator {
	public class ExtendedEnumerator<T> : IExtendedEnumerator<T> {
		private readonly IEnumerator<T> _enumerator;

		/// <param name="enumerator">A not-yet-started enumerator</param>
		public ExtendedEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;

		public bool MoveNext() {
			Success = _enumerator.MoveNext();
			if (Success)
				++Index;
			return Success;
		}

		public void Reset() {
			_enumerator.Reset();
			Index = -1;
			Success = false;
		}

		public T Current => _enumerator.Current;

		object? IEnumerator.Current => Current;

		public void Dispose() {
			_enumerator.Dispose();
			Index = -1;
			Success = false;
		}

		public int Index { get; private set; } = -1;

		public bool Success { get; private set; } = false;
	}

	public interface IExtendedEnumerator<out T> : IEnumerator<T> {
		int Index { get; }

		bool Success { get; }
	}
}