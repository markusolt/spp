using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal class MapRecipe : Expression {
    private Dictionary<string, Expression> _children;

    internal static readonly Parser<Expression> Parser = new ParseToken<Expression>("object", "{", _parse);

    internal MapRecipe (Position position, Dictionary<string, Expression> children) : base(position) {
      _children = children;
    }

    internal override Value Evaluate (Compiler compiler) {
      Dictionary<string, Value> evaluatedChildren;

      evaluatedChildren = new Dictionary<string, Value>();
      foreach (KeyValuePair<string, Expression> entry in _children) {
        evaluatedChildren.Add(entry.Key, entry.Value.Evaluate(compiler));
      }

      return new Map(_position, evaluatedChildren);
    }

    private static Expression _parse (Reader reader) {
      Dictionary<string, Expression> children;
      string key;
      Position rootPosition;
      Position position;
      bool quotedKey;

      rootPosition = reader.Position;
      reader.Read();
      children = new Dictionary<string, Expression>();
      reader.Skip(" \t\n");

      if (reader.Match("}")) {
        reader.Read();
        return new MapRecipe(rootPosition, children);
      }

      while (true) {
        position = reader.Position;
        quotedKey = reader.Match("\"");

        if (quotedKey) {
          reader.Assert('"');
        }

        key = reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
        if (quotedKey) {
          reader.Assert('"');
        }

        if (key.Length == 0) {
          throw new CompileException("Invalid key \"" + key + "\".", position);
        }
        if (children.ContainsKey(key)) {
          throw new CompileException("Multiple entries with the same key \"" + key + "\" found.", position);
        }
        reader.Skip(" \t\n");
        reader.Assert(':');
        reader.Skip(" \t\n");

        children.Add(key, Expression.ExpressionParser.Parse(reader));

        reader.Skip(" \t\n");
        switch (reader.Peek()) {
          case '}': {
            reader.Read();
            return new MapRecipe(rootPosition, children);
          }
          case ',': {
            reader.Read();
            reader.Skip(" \t\n");
            break;
          }
          default: {
            throw new CompileException("Expected \",\".", reader.Position);
          }
        }
      }
    }
  }
}
