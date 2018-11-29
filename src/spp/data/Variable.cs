using System;
using System.Collections.Generic;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Lexing;
using Spp.Data;

namespace Spp.Data {
  internal class Variable : Expression {
    private string _key;
    private Variable _next;
    private bool _isNullable;

    internal static readonly Parser<Expression> Parser = new ParseToken<Expression>("?abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_", _parse);
    internal static readonly Parser<Expression> MemberParser = new ParseToken<Expression>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_", _parseMember);

    internal Variable (Position position, string key) : base(position) {
      _key = key;
      _isNullable = false;
    }

    internal Variable (Position position, string key, bool isNullable) : base(position) {
      _key = key;
      _isNullable = isNullable;
    }

    internal Variable Next { get { return _next; } set { _next = value; } }

    internal override bool IsVariable { get { return true; } }

    internal override Variable AsVariable () {
      return this;
    }

    internal override Value Evaluate (Compiler compiler) {
      Value node;

      node = null;
      for (int i = compiler.Variables.Count - 1; i > -1; i--) {
        if (compiler.Variables[i].Has(_key)) {
          node = compiler.Variables[i][_key];
          break;
        }
      }

      if (node == null && _isNullable) {
        node = Value.Empty;
        node.Position = _position;
        return node;
      }

      if (node == null) {
        throw new CompileException("Unkown variable \"" + _key + "\".", _position);
      }

      if (_next != null) {
        node = _next._evaluate(compiler, node, _isNullable);
      }

      node.Position = _position;
      return node;
    }

    private Value _evaluate (Compiler compiler, Value node, bool isNullable) {
      if (!node.Has(_key)) {
        if (!isNullable) {
          throw new CompileException("Unkown member \"" + _key + "\".", _position);
        }
        return Value.Empty;
      } else {
        node = node[_key];
      }

      if (_next != null) {
        return _next._evaluate(compiler, node, isNullable);
      }

      return node;
    }

    internal void Set (Compiler compiler, Value payload) {
      Value node;
      Variable var;

      if (_next == null) {
        compiler.Variables[compiler.Variables.Count - 1][_key] = payload;
        return;
      }

      node = null;
      for (int i = compiler.Variables.Count - 1; i > -1; i--) {
        if (compiler.Variables[i].Has(_key)) {
          node = compiler.Variables[i][_key];
          break;
        }
      }
      var = _next;

      if (node == null) {
        throw new CompileException("Unkown variable \"" + _key + "\".", _position);
      }

      while (var._next != null) {
        if (!node.Has(var._key)) {
          throw new CompileException("Unkown member \"" + var._key + "\".", var._position);
        }
        node = node[var._key];
        var = var._next;
      }

      node[var._key] = payload;
    }

    private static Expression _parse (Reader reader) {
      bool isNullable = false;
      Position position;
      string key;

      if (reader.Match("?")) {
        reader.Read();
        isNullable = true;
      }

      position = reader.Position;
      key = reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-0123456789");

      return Parse(reader, position, key, isNullable);
    }

    private static Expression _parseMember (Reader reader) {
      Position position;
      string key;

      position = reader.Position;
      key = reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-0123456789");

      return Parse(reader, position, key, false);
    }

    internal static Expression Parse (Reader reader, Position position, string key, bool isNullable) {
      Variable var;

      var = new Variable(position, key, isNullable);

      if (reader.Match(".")) {
        do {
          reader.Read();
          var.Next = Parser.Parse(reader).AsVariable();
        } while (reader.Match("."));
      }

      return var;
    }
  }
}
