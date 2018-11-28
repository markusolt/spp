using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
  internal abstract class ValueRecipe {
    protected Position _position;

    internal static readonly Parser<ValueRecipe> KeywordParser = new ParseToken<ValueRecipe>("variable", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_", _keywordParser);
    internal static readonly Parser<ValueRecipe> GroupedParser = new ParseToken<ValueRecipe>("(", "(", _groupedParser);
    internal static readonly Parser<ValueRecipe> ValueRecipeParser = new ParseGroup<ValueRecipe>("value", new Parser<ValueRecipe>[] {GroupedParser, KeywordParser, Num.Parser, Text.Parser, MapRecipe.Parser, SequenceRecipe.Parser});

    protected ValueRecipe () {
      _position = default(Position);
    }

    protected ValueRecipe (Position position) {
      _position = position;
    }

    internal virtual bool IsVariable { get { return false; } }

    internal Position Position { get { return _position; } set { _position = value; } }

    internal virtual Variable AsVariable () {
      throw new CompileException("Expected a variable identifier.", _position);
    }

    internal abstract Value Evaluate (Compiler compiler);

    private static ValueRecipe _keywordParser (Reader reader) {
      Position position;
      string key;
      List<ValueRecipe> args;
      Signature sig;
      Variable current;
      ValueRecipe auto;

      position = reader.Position;
      key = reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-0123456789");

      reader.Skip(" \t");

      if (reader.Match("(")) {
        args = new List<ValueRecipe>();

        do {
          reader.Read();
          reader.Skip(" \t\n");
          args.Add(ValueRecipeParser.Parse(reader));
          reader.Skip(" \t");
        } while (reader.Match(","));
        reader.Assert(')');

        sig = new Signature(key.ToLower(), args.Count);

        if (!Instruction.Instructions.ContainsKey(sig)) {
          throw new CompileException("Unkown instruction \"" + key + "\" that takes " + args.Count + " arguments.", position);
        }

        return new Command(position, Instruction.Instructions[sig], args.ToArray());
      }

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

    private static ValueRecipe _groupedParser (Reader reader) {
      ValueRecipe res;

      reader.Read();
      reader.Skip(" \t\n");
      res = ValueRecipeParser.Parse(reader);
      reader.Skip(" \t\n");
      reader.Assert(')');
      return res;
    }
  }
}
