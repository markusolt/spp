using System;
using Spp.IO;

namespace Spp.IO {
	internal class ParseToken<T> : Parser<T> {
		private string _pattern;
		private Func<Reader, T> _function;

		internal ParseToken (string name, string pattern, Func<Reader, T> function) : base(name) {
			_pattern = pattern;
			_function = function;
		}

		internal override bool Match (char c) {
			return _pattern.IndexOf(c) > -1;
		}

		internal override T Parse (Reader reader) {
			if (!Match(reader.Peek())) {
				throw new CompileException("Illegal character " + reader.PrettyPeek() + " in " + _name + ".", reader.Position);
			}

			return _function(reader);
		}
	}
}
