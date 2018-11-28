using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Sequence : Value {
		private List<Value> _children;

		internal Sequence (Position position, List<Value> children) : base(position) {
			_children = children;
		}

		internal override bool IsEnumerable { get { return true; } }

		internal override IEnumerable<Value> AsEnumerable () { return _children; }

		internal override TextWriter Stringify (TextWriter buffer, bool root) {
			bool firstIteration;

			buffer.Write('[');

			firstIteration = true;
			foreach (Value entry in _children) {
				if (!firstIteration) {
					buffer.Write(", ");
				} else {
					firstIteration = false;
				}
				entry.Stringify(buffer, false);
			}
			buffer.Write(']');
			return buffer;
		}
	}
}
