using System;
using System.Collections.Generic;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Types;

namespace Spp.Types {
	internal class Sequence : Value {
		private List<Value> _children;

		internal Sequence (Position position, List<Value> children) : base(position) {
			_children = children;
		}

		internal override Value this[Value obj] {
			get {
				int index = obj.AsInt();

				if (index < 0) {
					throw new CompileException("Indices must be nonnegative.", obj.Position);
				}

				if (index >= _children.Count) {
					throw new CompileException("Index out of range.", obj.Position);
				}

				return _children[index];
			}
			set {
				int index = obj.AsInt();

				if (index < 0) {
					throw new CompileException("Indices must be nonnegative.", obj.Position);
				}

				if (index >= _children.Count) {
					throw new CompileException("Index out of range.", obj.Position);
				}

				_children[index] = value;
			}
		}

		internal new static Sequence Parse (Reader reader) {
			List<Value> children;
			Position pos;

			if (!reader.Match("[")) {
				throw new CompileException("Expected a list.", reader.Position);
			}

			pos = reader.Position;
			reader.Assert('[');
			children = new List<Value>();
			reader.Skip(" \t\n");

			if (reader.Match("]")) {
				reader.Read();
				return new Sequence(pos, children);
			}

			while (true) {
				children.Add(Value.Parse(reader));

				reader.Skip(" \t\n");
				switch (reader.Peek()) {
					case ']': {
						reader.Read();
						return new Sequence(pos, children);
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

			writer.Write('[');

			firstIteration = true;
			foreach (Value entry in _children) {
				if (!firstIteration) {
					writer.Write(", ");
				} else {
					firstIteration = false;
				}
				entry.Stringify(writer, false);
			}
			writer.Write(']');
		}

		internal override IEnumerator<Value> AsEnumerator () {
			return _children.GetEnumerator();
		}
	}
}
