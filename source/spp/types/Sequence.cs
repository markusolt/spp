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

		internal override bool IsEnumerator { get { return true; } }

		internal override IEnumerator<Value> AsEnumerator () { return _children.GetEnumerator(); }

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
