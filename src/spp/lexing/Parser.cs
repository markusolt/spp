using System;
using Spp.IO;
using Spp.Lexing;

namespace Spp.Lexing {
  internal abstract class Parser<T> {
    internal abstract bool Match (char c);

    internal abstract T Parse (Reader reader);
  }
}
