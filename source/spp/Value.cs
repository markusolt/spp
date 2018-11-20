using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Value {
		private Position _position;

		internal Value (Position position) {
			_position = position;
		}

		internal Position Position {
			get {
				return _position;
			}
		}
	}
}