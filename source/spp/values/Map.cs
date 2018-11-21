using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Spp;
using Spp.IO;
using Spp.Values.Enumeration;
using Spp.Values;

namespace Spp.Values {
	internal class Map : Value {
		private Dictionary<string, Value> _children;

		internal const string StartPattern = "{";

		internal Map (Position position) : base(position) {
			_children = new Dictionary<string, Value>();
		}

		internal new static Map Parse (Reader reader) {
			Map map;
			string key;
			Position pos;

			pos = reader.Position;
			reader.Assert('{');
			map = new Map(pos);
			reader.Skip(" \t\n");

			if (reader.Match("}")) {
				reader.Read();
				return map;
			}

			while (true) {
				pos = reader.Position;
				if (reader.Match("\"")) {
					reader.Read();
					key = _readKey(reader);
					reader.Assert('"');
				} else {
					key = _readKey(reader);
				}
				if (map.ContainsKey(key)) {
					throw new CompileException("Duplicate keys with value \"" + key + "\".", pos);
				}
				reader.Skip(" \t\n");
				reader.Assert(':');
				reader.Skip(" \t\n");

				map.Add(key, Value.Parse(reader));

				reader.Skip(" \t\n");
				switch (reader.Peek()) {
					case '}': {
						reader.Read();
						return map;
					}
					case ',': {
						reader.Read();
						reader.Skip(" \t\n");
						break;
					}
					default: {
						throw new CompileException("Expected \",\".", reader.Position);
					}
				}
			}
		}

		private static string _readKey (Reader reader) {
			if (!reader.Match(Variable.StartPattern)) {
				throw new CompileException("Expected key.", reader.Position);
			}
			return reader.Consume(Variable.ContinuePattern);
		}

		internal override Value Get (string key, Position position) {
			if (!_children.ContainsKey(key)) {
				throw new CompileException("Unkown member name \"" + key + "\".", position);
			}

			return _children[key];
		}

		internal override void Set (string key, Position position, Value value) {
			_children[key] = value;
		}

		internal override IEnumerator<Value> ToEnumerator () {
			return new MapEnumerator(_children);
		}

		internal override void Stringify (TextWriter writer) {
			bool firstIteration;

			writer.Write('{');

			firstIteration = true;
			foreach (KeyValuePair<string, Value> entry in _children) {
				if (!firstIteration) {
					writer.Write(", ");
				} else {
					firstIteration = false;
				}
				writer.Write(entry.Key);
				writer.Write(": ");
				entry.Value.Stringify(writer);
			}
			writer.Write('}');
		}

		internal void Add (string key, Value value) {
			_children[key] = value;
		}

		internal bool ContainsKey (string key) {
			return _children.ContainsKey(key);
		}
	}
}