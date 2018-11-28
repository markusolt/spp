using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class SequenceRecipe : ValueRecipe {
		private List<ValueRecipe> _children;

		internal static readonly Parser<ValueRecipe> Parser = new ParseToken<ValueRecipe>("array", "[", _parse);

		internal SequenceRecipe (Position position, List<ValueRecipe> children) : base(position) {
			_children = children;
		}

		internal override Value Evaluate (Compiler compiler) {
			List<Value> evaluatedChildren;

			evaluatedChildren = new List<Value>(_children.Count);
			foreach (ValueRecipe entry in _children) {
				evaluatedChildren.Add(entry.Evaluate(compiler));
			}

			return new Sequence(_position, evaluatedChildren);
		}

		private static ValueRecipe _parse (Reader reader) {
			List<ValueRecipe> children;
			Position position;

			position = reader.Position;
			reader.Read();
			children = new List<ValueRecipe>();
			reader.Skip(" \t\n");

			if (reader.Match("]")) {
				reader.Read();
				return new SequenceRecipe(position, children);
			}

			while (true) {
				children.Add(ValueRecipe.ValueRecipeParser.Parse(reader));

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
