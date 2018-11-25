using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Empty : Value {
		internal Empty (Position position) : base(position) {}

		internal override TextWriter Stringify (TextWriter buffer, bool root) {
			buffer.Write("null");
			return buffer;
		}
	}
}
