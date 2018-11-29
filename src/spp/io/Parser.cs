using System;
using Spp.IO;

namespace Spp.IO {
	internal abstract class Parser<T> {
		protected string _name;

		protected Parser (string name) {
			_name = name;
		}
		internal abstract bool Match (char c);

		internal abstract T Parse (Reader reader);
	}
}
