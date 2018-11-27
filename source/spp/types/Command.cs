using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Command : ValueRecipe {
		private Func<Compiler, Variable[], ValueRecipe[], Value> _function;
		private Variable[] _variables;
		private ValueRecipe[] _values;

		internal static readonly Parser<ValueRecipe> Parser = new ParseToken<ValueRecipe>("command", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", _parse);

		internal Command (Position position, Func<Compiler, Variable[], ValueRecipe[], Value> function, Variable[] variables, ValueRecipe[] values) : base(position) {
			_function = function;
			_variables = variables;
			_values = values;
		}

		internal override Value Evaluate (Compiler compiler) {
			Value result = _function(compiler, _variables, _values);
			result.Position = Position;
			return result;
		}

		private static Command _parse (Reader reader) {
			return _parse(reader, Instruction.Root);
		}

		private static Command _parse (Reader reader, Dictionary<string, Instruction> instructions) {
			string key;
			Position pos;
			Instruction instr;
			Variable[] variables;
			ValueRecipe[] values;

			pos = reader.Position;
			key = reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			if (!instructions.ContainsKey(key.ToLower())) {
				throw new CompileException("Unkown instruction \"" + key + "\".", pos);
			}
			instr = instructions[key.ToLower()];

			variables = new Variable[instr.VariableCount];
			for (int i = 0; i < variables.Length; i++) {
				_space(reader);
				variables[i] = ValueRecipe.ValueRecipeParser.Parse(reader).AsVariable();
			}

			values = new ValueRecipe[instr.ValueCount];
			for (int i = 0; i < values.Length; i++) {
				_space(reader);
				values[i] = ValueRecipe.ValueRecipeParser.Parse(reader);
			}

			return new Command(pos, instr.Function, variables, values);
		}

		private static void _space (Reader reader) {
			if (!reader.Match(" \t\n") && !reader.EndOfReader) {
				throw new CompileException("Illegal character " + reader.PrettyPeek() + ".", reader.Position);
			}

			reader.Skip(" \t");
		}
	}
}
