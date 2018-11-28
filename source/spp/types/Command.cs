using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal class Command : ValueRecipe {
    private Instruction _instr;
    private ValueRecipe[] _values;

    internal Command (Position position, Instruction instr, ValueRecipe[] values) : base(position) {
      _instr = instr;
      _values = values;
    }

    internal override Value Evaluate (Compiler compiler) {
      Value result;

      result = _instr.Invoke(compiler, _values);
      result.Position = _position;
      return result;
    }
  }
}
