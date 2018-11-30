using System;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Data;

namespace Spp.Data {
  internal class Empty : Value {
    internal Empty () {}

    internal Empty (Position position) : base(position) {}

    internal override TextWriter Stringify (TextWriter writer, bool root) {
      writer.Write("null");
      return writer;
    }

    internal override bool IsEmpty { get { return true; } }
  }
}
