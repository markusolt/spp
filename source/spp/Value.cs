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

			throw new CompileException("Expected value.", reader.Position);
		}

		internal Position Position {
			get {
				return _position;
			}
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