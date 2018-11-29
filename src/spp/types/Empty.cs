using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal class Empty : Value {
    internal Empty () {}

    internal override TextWriter Stringify (TextWriter writer, bool root) {
      writer.Write("null");
      return writer;
    }
  }
}
