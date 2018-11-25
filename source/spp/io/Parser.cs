using System;
using Spp.IO;

namespace Spp.IO {
	internal abstract class Parser<T> {
		internal abstract bool Match (char c);

		internal abstract T Parse (Reader reader);
	}
}
