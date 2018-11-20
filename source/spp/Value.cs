using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Value {
		private Position _position;
		private int _value;

		internal Value (Position position, int value) {
			_position = position;
			_value = value;
		}

		internal Position Position {
			get {
				return _position;
			}
		}
	}
}