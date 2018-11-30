using System;
using System.Collections.Generic;
using Spp.IO;
using Spp.Lexing;
using Spp.Data;
using Spp;

namespace Spp {
  internal abstract class Expression {
    protected Position _position;
    protected Position _firstPosition;
    private bool _missingPosition;

    internal static readonly Parser<Expression> KeywordParser = new ParseToken<Expression>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_", _keywordParser);
    internal static readonly Parser<Expression> GroupedParser = new ParseToken<Expression>("(", _groupedParser);
    internal static readonly Parser<Expression> ExpressionParser = new ParseGroup<Expression>(new Parser<Expression>[] {GroupedParser, KeywordParser, Variable.Parser, Num.Parser, Text.Parser, MapRecipe.Parser, SequenceRecipe.Parser});

    protected Expression () {
      _missingPosition = true;
    }

    protected Expression (Position position) {
      _position = position;
      _firstPosition = position;
      _missingPosition = false;
    }

    internal virtual bool IsVariable { get { return false; } }

    internal Position Position {
      get {
        return _position;
      }
      set {
        if (_missingPosition) {
          _missingPosition = false;
          _firstPosition = value;
        }
        _position = value;
      }
    }

    internal Position FirstPosition {
      get {
        return _firstPosition;
      }
      set {
        _position = value;
        _firstPosition = value;
        _missingPosition = false;
      }
    }

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

      return Variable.Parse(reader, position, key, false);
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
