using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Variable : ValueRecipe {
		private string _name;

		internal static readonly Parser<Variable> VariableParser = new ParseToken<Variable>("variable", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_", _parse);
		internal static readonly Parser<ValueRecipe> Parser = new ParseToken<ValueRecipe>("variable", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_", _parse);

		internal Variable (Position position, string name) : base(position) {
			_name = name;
		}

		internal override Value Evaluate (Compiler compiler) {
			return default(Value); // TODO: missing
		}

		internal void Set (Compiler compiler, Value payload) {
			// TODO: missing
		}

		private static Variable _parse (Reader reader) {
			return new Variable(reader.Position, reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789"));
		}
	}
}
