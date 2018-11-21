using System;
using System.IO;
using Spp.IO;
using Spp.Values;
using Spp;

namespace Spp {
	internal abstract class Value {
		private Position _position;

		internal Value (Position position) {
			_position = position;
		}

		internal static Value Parse (Reader reader) {
			if (reader.Match(Text.StartPattern)) {
				return Text.Parse(reader);
			}

			if (reader.Match(Map.StartPattern)) {
				return Map.Parse(reader);
			}

			if (reader.Match(Reserved.StartPattern)) {
				return Reserved.Parse(reader);
			}

			throw new CompileException("Expected value.", reader.Position);
		}

		internal Position Position {
			get {
				return _position;
			}
		}

		internal virtual Value Get (string key, Position position) {
			throw new CompileException("Object does not support members.", position);
		}

		internal virtual void Set (string key, Position position, Value value) {
			throw new CompileException("Object does not support members.", position);
		}

		internal abstract void Stringify (TextWriter writer);

		public override string ToString () {
			StringWriter buffer;

			buffer = new StringWriter();
			Stringify(buffer);
			return buffer.ToString();
		}
	}
}