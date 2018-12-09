using System;
using System.Collections.Generic;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Data;

namespace Spp.Data {
  internal class Auto : Expression {
    private Func<Compiler, Position, Value> _function;

    internal static readonly Dictionary<string, Auto> Autos = new Dictionary<string, Auto> {
      { "false", new Auto(_false) },
      { "null",  new Auto(_null)  },
      { "this",  new Auto(_this)  },
      { "true",  new Auto(_true)  }
    };

    internal Auto (Func<Compiler, Position, Value> function) {
      _function = function;
    }

    internal override Value Evaluate (Compiler compiler) {
      return _function(compiler, _position);
    }

    private static Value _this (Compiler compiler, Position position) {
      Value v;

      v = compiler.Variables[compiler.Variables.Count - 1];
      v.Position = position;
      return v;
    }

    private static Value _true (Compiler compiler, Position position) {
      return new Bool(position, true);
    }

    private static Value _false (Compiler compiler, Position position) {
      return new Bool(position, false);
    }

    private static Value _null (Compiler compiler, Position position) {
      return new Empty(position);
    }
  }
}
