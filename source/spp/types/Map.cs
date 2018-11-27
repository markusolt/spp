using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Map : Value {
		private Dictionary<string, Value> _children;

		internal Map () : base(default(Position)) {
			_children = new Dictionary<string, Value>();
		}

		internal Map (Position position, Dictionary<string, Value> children) : base(position) {
			_children = children;
		}

		internal override Value this[Value key] {
			get {
				string id;

				id = key.AsString();
				if (!_children.ContainsKey(id)) {
					throw new CompileException("Unkown memer name \"" + id + "\".", key.Position);
				}
				return _children[id];
			}
			set {
				_children[key.AsString()] = value;
			}
		}

		internal override TextWriter Stringify (TextWriter buffer, bool root) {
			bool firstIteration;

			buffer.Write('{');

			firstIteration = true;
			foreach (KeyValuePair<string, Value> entry in _children) {
				if (!firstIteration) {
					buffer.Write(", ");
				} else {
					firstIteration = false;
				}
				new Text(default(Position), entry.Key).Stringify(buffer, false);
				buffer.Write(": ");
				entry.Value.Stringify(buffer, false);
			}
			buffer.Write('}');
			return buffer;
		}
	}
}
