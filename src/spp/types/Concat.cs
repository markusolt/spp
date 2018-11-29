using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal class Concat : Expression {
    private Expression[] _contents;

    internal Concat (Position position, Expression[] contents) : base(position) {
      _contents = contents;
    }

    internal override Value Evaluate (Compiler compiler) {
      StringWriter writer;

      writer = new StringWriter();
      foreach (Expression entry in _contents) {
        entry.Evaluate(compiler).Stringify(writer, true);
      }
      return new Text(_position, writer.ToString());
    }
  }
}
