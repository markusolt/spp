using System;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Values;

namespace Spp.Values {
	internal class Num : Value {
		private int _content;

		internal const string StartPattern = "0123456789";

		internal Num (Position position, int content) : base(position) {
			_content = content;
		}

		internal new static Num Parse (Reader reader) {
			return new Num(reader.Position, int.Parse(reader.Consume(StartPattern + "")));
		}

		internal override void Stringify (TextWriter writer) {
			writer.Write(_content.ToString());
		}

		public override int ToInt () {
			return _content;
		}
	}
}