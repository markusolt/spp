using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal class VarName {
		private Position _position;
		private string _name;

		internal VarName (Position position, string name) {
			_position = position;
			_name = name;
		}

		internal Position Position {
			get {
				return _position;
			}
		}
	}
}