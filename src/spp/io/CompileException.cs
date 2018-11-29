using System;
using Spp.IO;

namespace Spp.IO {
  internal class CompileException : Exception {
    private string _message;
    private Position _position;

    internal CompileException (string message, Position pos) : base(message) {
      _message = pos.ToString() + ": " + "Error: " + message;
      _position = pos;
    }

    internal CompileException (string message, Position pos, Exception innerException) : base(message, innerException) {
      _message = pos.ToString() + ": " + "Error: " + message;
      _position = pos;
    }

    public Position Position {
      get {
        return _position;
      }
    }

    public override string ToString () {
      return _message;
    }
  }
}
