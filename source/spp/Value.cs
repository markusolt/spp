using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp.Types;
using Spp;

namespace Spp {
	internal abstract class Value {
		private Position _position;

		private static Parser<Value>[] _parsers;

		static Value () {
			_parsers = new Parser<Value>[] {
				new Parser<Value>("{", Map.Parse),
				new Parser<Value>("0123456789", Num.Parse),
				new Parser<Value>("[", Sequence.Parse),
				new Parser<Value>("\"", Text.Parse),
				new Parser<Value>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_", Variable.Parse),
				new Parser<Value>(":", Auto.Parse),
			};
		}

		internal Value (Position position) {
			_position = position;
		}

		internal virtual Value this[Value key] {
			get {
				throw new CompileException("Object is not a collection.", _position);
			}
			set {
				throw new CompileException("Object is not a collection.", _position);
			}
		}

		internal static Value Parse (Reader reader) {
			char c;

			c = reader.Peek();
			foreach (Parser<Value> p in _parsers) {
				if (p.Match(c)) {
					return p.Parse(reader);
				}
			}

			throw new CompileException("Illegal character " + reader.PrettyPeek() + ".", reader.Position);
		}

		internal Position Position {
			get {
				return _position;
			}
			set {
				_position = value;
			}
		}

		internal static Value NewMap () {
			return new Map(default(Position), new Dictionary<string, Value>());
		}

		internal abstract void Stringify (TextWriter writer, bool root);

		internal virtual string AsString () {
			throw new CompileException("Expected a string value.", _position);
		}

		internal virtual int AsInt () {
			throw new CompileException("Expected an integer value.", _position);
		}

		internal virtual IEnumerator<Value> AsEnumerator () {
			throw new CompileException("Expected a collection.", _position);
		}

		internal virtual Value Evaluate (Compiler compiler, Value node) {
			return this;
		}

		internal virtual Value Copy () {
			return this;
		}

		public override string ToString () {
			StringWriter buffer;

			buffer = new StringWriter();
			Stringify(buffer, true);
			return buffer.ToString();
		}
	}
}
