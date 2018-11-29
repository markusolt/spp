using System;
using System.Collections;
using System.Collections.Generic;
using Spp.IO;

namespace Spp.IO {
	internal class EnumerationMap<T1, T2> : IEnumerable<T2>, IEnumerable, IEnumerator<T2>, IEnumerator, IDisposable {
		private IEnumerator<T1> _enumerator;
		private Func<T1, T2> _converter;

		internal EnumerationMap(IEnumerator<T1> enumerator, Func<T1, T2> converter) {
			_enumerator = enumerator;
			_converter = converter;
		}

		public T2 Current { get { return _converter(_enumerator.Current); } }

		Object IEnumerator.Current { get { return Current; } }

		IEnumerator IEnumerable.GetEnumerator () { return this; }

		public IEnumerator<T2> GetEnumerator () { return this; }

		public void Reset () {
			_enumerator.Reset();
		}

		public bool MoveNext () {
			return _enumerator.MoveNext();
		}

		public void Dispose () {
			_enumerator.Dispose();
		}
	}
}