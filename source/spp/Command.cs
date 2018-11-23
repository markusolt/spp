using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Command {
		private Position _position;
		private Action<Compiler, Variable, Value, Command> _function;
		private Variable _var;
		private Value _val;
		private Command _chain;

		internal Command (Position position, Action<Compiler, Variable, Value, Command> function, Variable var, Value val, Command chain) {
			_position = position;
			_function = function;
			_var = var;
			_val = val;
			_chain = chain;
		}

		internal static Command Parse (Reader reader, Dictionary<string, Instruction> instructions) {
			string key;
			Position pos;
			Instruction instr;
			Variable var;
			Value val;
			Command chain;

			if (!reader.Match("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")) {
				throw new CompileException("Expected instruction.", reader.Position);
			}

			pos = reader.Position;
			key = reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			if (!instructions.ContainsKey(key.ToLower())) {
				throw new CompileException("Unkown instruction \"" + key + "\".", pos);
			}
			instr = instructions[key.ToLower()];
			_space(reader);

			var = null;
			if (instr.HasVar) {
				var = Variable.ParseVariable(reader);
				_space(reader);
			}

			val = null;
			if (instr.HasVal) {
				val = Value.ParseValue(reader);
				_space(reader);
			}

			chain = null;
			if (instr.Chain != null) {
				chain = Command.Parse(reader, instr.Chain);
				_space(reader);
			}

			if (!reader.Match("\n") && !reader.EndOfReader) {
				throw new CompileException("Expected end of instruction.", reader.Position);
			}

			return new Command(pos, instr.Function, var, val, chain);
		}

		private static void _space (Reader reader) {
			if (!reader.Match(" \t\n") && !reader.EndOfReader) {
				throw new CompileException("Illegal character " + reader.PrettyPeek() + ".", reader.Position);
			}

			reader.Skip(" \t");
		}

		internal void Invoke (Compiler compiler) {
			Value val;

			val = _val;
			if (val != null) {
				val = val.Evaluate(compiler, null);
			}

			_function(compiler, _var, val, _chain);
		}
	}
}
