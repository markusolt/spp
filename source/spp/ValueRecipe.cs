using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal abstract class ValueRecipe {
		protected Position _position;

		internal static readonly Parser<ValueRecipe> ValueRecipeParser = new ParseGroup<ValueRecipe>("value", new Parser<ValueRecipe>[] {Num.Parser, Text.Parser, Command.Parser, Variable.Parser, MapRecipe.Parser});

		protected ValueRecipe (Position position) {
			_position = position;
		}

		internal virtual bool IsVariable { get { return false; } }

		internal Position Position { get { return _position; } set { _position = value; } }

		internal virtual Variable AsVariable () {
			throw new CompileException("Expected a variable identifier.", _position);
		}

		internal abstract Value Evaluate (Compiler compiler);
	}
}
