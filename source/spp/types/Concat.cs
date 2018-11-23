using System;
using System.Collections.Generic;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Types;

namespace Spp.Types {
	internal class Concat : Value {
		private List<Value> _entries;

		internal Concat (Position position, List<Value> entries) : base(position) {
			_entries = entries;
		}

		internal override void Stringify (TextWriter writer, bool root) {
			throw new NotSupportedException("Concats should be evaluated before stringify.");
		}

		internal override Value Evaluate (Value root, Value node) {
			StringWriter buffer;

			buffer = new StringWriter();
			foreach (Value e in _entries) {
				e.Evaluate(root, root).Stringify(buffer, true);
			}
			return new Text(Position, buffer.ToString());
		}
	}
}
