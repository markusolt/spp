using System;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Types;

namespace Spp.Types {
	internal class Num : Value {
		private int _content;

		private const string _pattern = "0123456789";

		internal Num (Position position, int content) : base(position) {
			_content = content;
		}

		internal static Value Parse (Reader reader) {
			int first;
			Position pos;

			if (!reader.Match(_pattern)) {
				throw new CompileException("Expected an integer.", reader.Position);
			}

			pos = reader.Position;
			first = int.Parse(reader.Consume(_pattern));
			if (reader.Match(".")) {
				reader.Assert('.');
				reader.Assert('.');
				if (!reader.Match(_pattern)) {
					throw new CompileException("Expected an integer.", reader.Position);
				}
				return new Range(pos, first, int.Parse(reader.Consume(_pattern)));
			}

			return new Num(pos, first);
		}

		internal override void Stringify (TextWriter writer, bool root) {
			writer.Write(_content.ToString());
		}

		internal override int AsInt () {
			return _content;
		}
	}
}
