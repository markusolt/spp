using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
  internal class SequenceRecipe : Expression {
    private List<Expression> _children;

    internal static readonly Parser<Expression> Parser = new ParseToken<Expression>("array", "[", _parse);

    internal SequenceRecipe (Position position, List<Expression> children) : base(position) {
      _children = children;
    }

    internal override Value Evaluate (Compiler compiler) {
      List<Value> evaluatedChildren;

      evaluatedChildren = new List<Value>(_children.Count);
      foreach (Expression entry in _children) {
        evaluatedChildren.Add(entry.Evaluate(compiler));
      }

      return new Sequence(_position, evaluatedChildren);
    }

    private static Expression _parse (Reader reader) {
      List<Expression> children;
      Position position;

      position = reader.Position;
      reader.Read();
      children = new List<Expression>();
      reader.Skip(" \t\n");

      if (reader.Match("]")) {
        reader.Read();
        return new SequenceRecipe(position, children);
      }

      while (true) {
        children.Add(Expression.ExpressionParser.Parse(reader));

        reader.Skip(" \t\n");
        switch (reader.Peek()) {
          case ']': {
            reader.Read();
            return new SequenceRecipe(position, children);
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
