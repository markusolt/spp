using System;
using System.Collections;
using System.Collections.Generic;
using Spp.IO;
using Spp.Values.Enumeration;

namespace Spp.Values.Enumeration {
	internal class MapEnumerator : IEnumerator<Value>, IEnumerator, IDisposable {
		IEnumerator<KeyValuePair<string, Value>> _enumerator;
		Dictionary<string, Value> _dictionary;
		Value _current;

		internal MapEnumerator (Dictionary<string, Value> dictionary) {
			_dictionary = new Dictionary<string, Value>(dictionary);
			Reset();
		}

		public Value Current {
			get {
				return _current;
			}
		}

		Object IEnumerator.Current {
			get {
				return _current;
			}
		}

		public void Reset () {
			_enumerator = _dictionary.GetEnumerator();
			_current = new Map(default(Position));
		}

		public bool MoveNext () {
			if (_enumerator.MoveNext()) {
				_current.Set(new Text(default(Position), "key"), new Text(default(Position), _enumerator.Current.Key));
				_current.Set(new Text(default(Position), "value"), _enumerator.Current.Value);
				return true;
			}
			return false;
		}

		public void Dispose () {
			if (_enumerator != null) {
				_enumerator.Dispose();
				_enumerator = null;
			}
		}
	}
}