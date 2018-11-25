using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal abstract class ValueRecipe {
		protected Position _position;

		internal static readonly Parser<ValueRecipe> ValueRecipeParser = new ParseGroup<ValueRecipe>(new Parser<ValueRecipe>[] {Num.Parser, Command.Parser});

		protected ValueRecipe (Position position) {
			_position = position;
		}

		internal Position Position { get { return _position; } }

		internal abstract Value Evaluate (Compiler compiler);
	}
}
