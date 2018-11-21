using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Spp;
using Spp.IO;
using Spp.Values.Enumeration;
using Spp.Values;

namespace Spp.Values {
	internal class Sequence : Value {
		private List<Value> _children;

		internal const string StartPattern = "[";

		internal Sequence (Position position) : base(position) {
			_children = new List<Value>();
		}

		internal new static Sequence Parse (Reader reader) {
			Sequence sequence;
			Position pos;

			pos = reader.Position;
			reader.Assert('[');
			sequence = new Sequence(pos);
			reader.Skip(" \t\n");

			if (reader.Match("]")) {
				reader.Read();
				return sequence;
			}

			while (true) {
				sequence.Add(Value.Parse(reader));

				reader.Skip(" \t\n");
				switch (reader.Peek()) {
					case ']': {
						reader.Read();
						return sequence;
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

		internal override Value Get (Value index) {
			int key;

			if (!(index is Num)) {
				throw new CompileException("Expected integer index.", index.Position);
			}

			key = index.ToInt();

			if (key < 0) {
				throw new CompileException("Index must be nonnegative.", index.Position);
			}
			if (key >= _children.Count) {
				throw new CompileException("Index out of range.", index.Position);
			}

			return _children[key];
		}

		internal override IEnumerator<Value> ToEnumerator () {
			return _children.GetEnumerator();
		}

		internal override void Stringify (TextWriter writer) {
			bool firstIteration;

			writer.Write('[');

			firstIteration = true;
			foreach (Value entry in _children) {
				if (!firstIteration) {
					writer.Write(", ");
				} else {
					firstIteration = false;
				}
				entry.Stringify(writer);
			}
			writer.Write(']');
		}

		internal void Add (Value value) {
			_children.Add(value);
		}
	}
}