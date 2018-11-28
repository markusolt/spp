using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal abstract class Value : ValueRecipe {
    internal static readonly Value Empty = new Empty();

    protected Value () {}

    protected Value (Position position) : base(position) {}

    internal virtual Value this[Value key] {
      get {
        throw new CompileException("Object is not a collection.", _position);
      }
      set {
        throw new CompileException("Object is not a collection.", _position);
      }
    }

    internal virtual bool IsInt { get { return false; } }

    internal virtual bool IsBool { get { return false; } }

    internal virtual bool IsString { get { return false; } }

    internal virtual bool IsEnumerable { get { return false; } }

    internal virtual bool IsKeyValue { get { return false; } }

    internal override Value Evaluate (Compiler compiler) { return this; }

    internal virtual Value Copy () { return this; }

    internal virtual int AsInt () {
      throw new CompileException("Expected an integer.", _position);
    }

    internal virtual bool AsBool () {
      throw new CompileException("Expected a boolean.", _position);
    }

    internal virtual string AsString () {
      throw new CompileException("Expected a string.", _position);
    }

    internal virtual IEnumerable<Value> AsEnumerable () {
      throw new CompileException("Expected a list.", _position);
    }

    internal virtual Map AsKeyValue () {
      throw new CompileException("Expected a key value pair.", _position);
    }

    internal virtual void Push (Value entry) {
      throw new CompileException("Object is not a collection.", _position);
    }

    internal abstract TextWriter Stringify (TextWriter buffer, bool root);

    public override string ToString () {
      return Stringify(new StringWriter(), true).ToString();
    }
  }
}
