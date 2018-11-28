using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal abstract class ValueRecipe {
		protected Position _position;

		internal static readonly Parser<ValueRecipe> GroupParser = new ParseToken<ValueRecipe>("(", "(", _groupParser);
		internal static readonly Parser<ValueRecipe> ValueRecipeParser = new ParseGroup<ValueRecipe>("value", new Parser<ValueRecipe>[] {GroupParser, Num.Parser, Text.Parser, Command.Parser, Variable.Parser, MapRecipe.Parser, SequenceRecipe.Parser});

		protected ValueRecipe (Position position) {
			_position = position;
		}

		internal virtual bool IsVariable { get { return false; } }

		internal Position Position { get { return _position; } set { _position = value; } }

		internal virtual Variable AsVariable () {
			throw new CompileException("Expected a variable identifier.", _position);
		}

		internal abstract Value Evaluate (Compiler compiler);

		private static ValueRecipe _groupParser (Reader reader) {
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
