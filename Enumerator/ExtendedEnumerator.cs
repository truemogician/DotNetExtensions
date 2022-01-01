using System.Collections;
using System.Collections.Generic;

namespace TrueMogician.Extensions.Enumerator {
	/// <summary>
	///     Default implementation of <see cref="IExtendedEnumerator{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
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

	/// <summary>
	///     An extended enumerator interface inheriting from <see cref="IEnumerator{T}" /> with additional information about
	///     the current state.
	/// </summary>
	public interface IExtendedEnumerator<out T> : IEnumerator<T> {
		/// <summary>
		///     Index of <see cref="IEnumerator{T}.Current" /> in the enumerator. Initially <c>-1</c>.
		/// </summary>
		int Index { get; }

		#pragma warning disable CS1574// XML comment has cref attribute that could not be resolved
		/// <summary>
		///     Result of the last call of <see cref="IEnumerator{T}.MoveNext" />, initially <see langword="false" />.
		/// </summary>
		#pragma warning restore CS1574
		bool Success { get; }
	}
}