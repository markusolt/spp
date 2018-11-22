using System;
using Spp.IO;

namespace Spp.IO {
	internal class Parser<T> {
		private string _pattern;
		private Func<Reader, T> _function;

		internal Parser (string pattern, Func<Reader, T> function) {
			_pattern = pattern;
			_function = function;
		}

		internal bool Match (char c) {
			return _pattern.IndexOf(c) > -1;
		}

		internal T Parse (Reader reader) {
			return _function(reader);
		}
	}
}
