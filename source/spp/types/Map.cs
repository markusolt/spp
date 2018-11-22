using System;
using System.Collections.Generic;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Types;

namespace Spp.Types {
	internal class Map : Value {
		private Dictionary<string, Value> _children;

		internal Map (Position position, Dictionary<string, Value> children) : base(position) {
			_children = children;
		}

		internal override Value this[Value obj] {
			get {
				string key = obj.AsString();

				if (!_children.ContainsKey(key)) {
					throw new CompileException("Unkown member \"" + key + ".", obj.Position);
				}

				return _children[key];
			}
			set {
				string key = obj.AsString();

				_children[key] = value;
			}
		}

		internal new static Map Parse (Reader reader) {
			Dictionary<string, Value> children;
			string key;
			Position pos;

			if (!reader.Match("{")) {
				throw new CompileException("Expected a map.", reader.Position);
			}

			pos = reader.Position;
			reader.Assert('{');
			children = new Dictionary<string, Value>();
			reader.Skip(" \t\n");

			if (reader.Match("}")) {
				reader.Read();
				return new Map(pos, children);
			}

			while (true) {
				pos = reader.Position;
				key = Text.Parse(reader).ToString();
				if (children.ContainsKey(key)) {
					throw new CompileException("Duplicate keys with value \"" + key + "\".", pos);
				}
				reader.Skip(" \t\n");
				reader.Assert(':');
				reader.Skip(" \t\n");

				children.Add(key, Value.Parse(reader));

				reader.Skip(" \t\n");
				switch (reader.Peek()) {
					case '}': {
						reader.Read();
						return new Map(pos, children);
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

		internal override void Stringify (TextWriter writer, bool root) {
			bool firstIteration;

			writer.Write('{');

			firstIteration = true;
			foreach (KeyValuePair<string, Value> entry in _children) {
				if (!firstIteration) {
					writer.Write(", ");
				} else {
					firstIteration = false;
				}
				new Text(default(Position), entry.Key).Stringify(writer, false);
				writer.Write(": ");
				entry.Value.Stringify(writer, false);
			}
			writer.Write('}');
		}
	}
}
