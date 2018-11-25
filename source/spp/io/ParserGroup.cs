using System;
using Spp.IO;

namespace Spp.IO {
	internal class ParseGroup<T> : Parser<T> {
		private Parser<T>[] _alternatives;

		internal ParseGroup (string name, Parser<T>[] alternatives) : base(name) {
			_alternatives = alternatives;
		}

		internal override bool Match (char c) {
			foreach (Parser<T> p in _alternatives) {
				if (p.Match(c)) {
					return true;
				}
			}

			return false;
		}

		internal override T Parse (Reader reader) {
			char c;

			c = reader.Peek();
			foreach (Parser<T> p in _alternatives) {
				if (p.Match(c)) {
					return p.Parse(reader);
				}
			}
			throw new CompileException("Illegal character " + reader.PrettyPeek() + " in " + _name + ".", reader.Position);
		}
	}
}
