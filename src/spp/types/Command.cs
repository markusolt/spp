using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal class Command : Expression {
    private Instruction _instr;
    private Expression[] _values;

    internal Command (Position position, Instruction instr, Expression[] values) : base(position) {
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
