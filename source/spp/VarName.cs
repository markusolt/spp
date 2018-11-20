using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal class VarName {
		private Position _position;

		internal VarName (Position position) {
			_position = position;
		}

		internal Position Position {
			get {
				return _position;
			}
		}
	}
}