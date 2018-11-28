using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal class Auto : ValueRecipe {
    private Value _payload;

    internal static readonly Dictionary<string, Auto> Autos = new Dictionary<string, Auto> {
      { "true",  new Auto(new Bool(true))  },
      { "false", new Auto(new Bool(false)) }
    };

    internal Auto (Value payload) {
      _payload = payload;
    }

    internal override Value Evaluate (Compiler compiler) {
      return _payload;
    }
  }
}
