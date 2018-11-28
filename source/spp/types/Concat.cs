using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal class Concat : ValueRecipe {
    private ValueRecipe[] _contents;

    internal Concat (Position position, ValueRecipe[] contents) : base(position) {
      _contents = contents;
    }

    internal override Value Evaluate (Compiler compiler) {
      StringWriter writer;

      writer = new StringWriter();
      foreach (ValueRecipe entry in _contents) {
        entry.Evaluate(compiler).Stringify(writer, true);
      }
      return new Text(_position, writer.ToString());
    }
  }
}
