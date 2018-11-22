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

		internal new static Num Parse (Reader reader) {
			return new Num(reader.Position, int.Parse(reader.Consume(_pattern)));
		}

		internal override void Stringify (TextWriter writer, bool root) {
			writer.Write(_content.ToString());
		}

		internal override int AsInt () {
			return _content;
		}
	}
}
