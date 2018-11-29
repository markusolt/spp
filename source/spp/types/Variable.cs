using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal class Variable : Expression {
    private Value _key;
    private Variable _parent;

    internal Variable (Position position, string key, Variable parent) : base(position) {
      _key = new Text(position, key);
      _parent = parent;
    }

    internal Variable (Position position, Value key, Variable parent) : base(position) {
      key.Position = position;

      _key = key;
      _parent = parent;
    }

    internal override bool IsVariable { get { return true; } }

    internal override Variable AsVariable () {
      return this;
    }

    internal override Value Evaluate (Compiler compiler) {
      Value node;

      node = _parent == null ? compiler.Variables : _parent.Evaluate(compiler);
      node = node[_key];
      node.Position = _position;
      return node;
    }

    internal void Set (Compiler compiler, Value payload) {
      Value parent;

      parent = _parent == null ? compiler.Variables : _parent.Evaluate(compiler);
      parent[_key] = payload;
    }
  }
}
