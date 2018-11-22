using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Types;

namespace Spp.Types {
	internal class Range : Value, IEnumerator<Value>, IEnumerator, IDisposable {
		private int _min;
		private int _max;
		private bool _reverse;
		private int _current;

		internal Range (Position position, int min, int max) : base(position) {
			_min = min;
			_max = max;
			_reverse = _max < _min;

			Reset();
		}

		Object IEnumerator.Current {
			get {
				return Current;
			}
		}

		public Value Current {
			get {
				return new Num(Position, _current);
			}
		}

		internal override void Stringify (TextWriter writer, bool root) {
			writer.Write('[');
			writer.Write(_min);
			writer.Write("..");
			writer.Write(_max);
			writer.Write(']');
		}

		internal override IEnumerator<Value> AsEnumerator () {
			return this;
		}

		public void Reset () {
			if (_reverse) {
				_current = _min + 1;
			} else {
				_current = _min -1;
			}
		}

		public bool MoveNext () {
			if (_reverse) {
				_current--;
				return _current >= _max;
			} else {
				_current++;
				return _current <= _max;
			}
		}

		public void Dispose () {}
	}
}
