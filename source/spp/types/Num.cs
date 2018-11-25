using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Num : Value {
		private int _payload;

		internal static readonly Parser<ValueRecipe> Parser = new ParseToken<ValueRecipe>("0123456789", _parse);

		protected Num (Position position, int payload) : base(position) {
			_payload = payload;
		}

		internal override bool IsInt { get { return true; } }

		internal override int AsInt () { return _payload; }

		internal override TextWriter Stringify (TextWriter buffer, bool root) {
			buffer.Write(_payload.ToString());
			return buffer;
		}

		private static ValueRecipe _parse (Reader reader) {
			return new Num(reader.Position, int.Parse(reader.Consume("0123456789")));
		}
	}
}
