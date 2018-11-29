using System;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Data;

namespace Spp.Data {
  internal class Bool : Value {
    private bool _payload;

    internal Bool (bool payload) {
      _payload = payload;
    }

    internal Bool (Position position, bool payload) : base(position) {
      _payload = payload;
    }

    internal override bool IsBool { get { return true; } }

    internal override bool AsBool () { return _payload; }

    internal override TextWriter Stringify (TextWriter writer, bool root) {
      writer.Write(_payload ? "true" : "false");
      return writer;
    }
  }
}
