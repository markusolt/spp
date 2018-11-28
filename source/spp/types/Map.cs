using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Map : Value {
		protected Dictionary<string, Value> _children;

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

		internal override bool IsEnumerable { get { return true; } }

		internal override bool IsKeyValue { get { return _children.ContainsKey("key") && _children["key"].IsString && _children.ContainsKey("value"); } }

		internal override IEnumerable<Value> AsEnumerable () { return new EnumerationMap<KeyValuePair<string, Value>, Value>(_children.GetEnumerator(), _enumerationConverter); }

		internal override KeyValue AsKeyValue () {
			if (IsKeyValue) {
				return new KeyValue(_position, _children["key"].AsString(), _children["value"]);
			}

			throw new CompileException("Expected a key value pair.", _position);
		}

		internal override void Push (Value entry) {
			KeyValue extension = entry.AsKeyValue();
			_children[extension.Key] = extension.Value;
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

		private static Map _enumerationConverter (KeyValuePair<string, Value> entry) {
			return new Map(default(Position), new Dictionary<string, Value> {
				{ "key", new Text(default(Position), entry.Key) },
				{ "value", entry.Value }
			});
		}
	}
}
