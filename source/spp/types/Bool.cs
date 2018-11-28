using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Bool : Value {
		private bool _payload;

		internal Bool (Position position, bool payload) : base(position) {
			_payload = payload;
		}

		internal override bool IsBool { get { return true; } }

		internal override bool AsBool () { return _payload; }

		internal override TextWriter Stringify (TextWriter buffer, bool root) {
			buffer.Write(_payload ? "true" : "false");
			return buffer;
		}
	}
}
