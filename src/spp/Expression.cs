using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
  internal abstract class Expression {
    protected Position _position;

    internal static readonly Parser<Expression> KeywordParser = new ParseToken<Expression>("variable", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_", _keywordParser);
    internal static readonly Parser<Expression> GroupedParser = new ParseToken<Expression>("(", "(", _groupedParser);
    internal static readonly Parser<Expression> ExpressionParser = new ParseGroup<Expression>("expression", new Parser<Expression>[] {GroupedParser, KeywordParser, Num.Parser, Text.Parser, MapRecipe.Parser, SequenceRecipe.Parser});

    protected Expression () {
      _position = default(Position);
    }

    protected Expression (Position position) {
      _position = position;
    }

    internal virtual bool IsVariable { get { return false; } }

    internal Position Position { get { return _position; } set { _position = value; } }

    internal virtual Variable AsVariable () {
      throw new CompileException("Expected a variable identifier.", _position);
    }

    internal abstract Value Evaluate (Compiler compiler);

    internal static void SkipWhitespace (Reader reader) {
      do {
        reader.Skip(" \t\n");
        Compiler.SkipComment(reader);
      } while (reader.Match("\n") && !reader.EndOfReader);
    }

    private static Expression _keywordParser (Reader reader) {
      Position position;
      string key;
      string undoBuffer;
      Position undoPosition;
      List<Expression> args;
      Signature sig;
      Variable current;
      Expression auto;

      position = reader.Position;
      key = reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-0123456789");

      undoPosition = reader.Position;
      undoBuffer = reader.Consume(" \t");

      if (reader.Match("(")) {
        args = new List<Expression>();
        reader.Read();
        SkipWhitespace(reader);

        while(!reader.Match(")")) {
          args.Add(ExpressionParser.Parse(reader));
          SkipWhitespace(reader);
          if (reader.Match(")")) {
            break;
          }
          reader.Assert(',');
          SkipWhitespace(reader);
        }
        reader.Read();

        sig = new Signature(key.ToLower(), args.Count);

        if (!Instruction.Instructions.ContainsKey(sig)) {
          throw new CompileException("Unkown instruction \"" + key + "\" that takes " + args.Count + " arguments.", position);
        }

        return new Command(position, Instruction.Instructions[sig], args.ToArray());
      }

      reader.Undo(undoBuffer, undoPosition);

      if (Auto.Autos.ContainsKey(key)) {
        auto = Auto.Autos[key];
        auto.Position = position;
        return auto;
      }

      current = new Variable(position, key, null);

      if (reader.Match(".")) {
        do {
          reader.Read();
          current = new Variable(position, reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-0123456789"), current);
        } while (reader.Match("."));
      }

      return current;
    }

    private static Expression _groupedParser (Reader reader) {
      Expression res;

      reader.Read();
      reader.Skip(" \t\n");
      res = ExpressionParser.Parse(reader);
      reader.Skip(" \t\n");
      reader.Assert(')');
      return res;
    }
  }
}
