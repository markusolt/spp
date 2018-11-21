using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Command {
		private Action<Compiler, Variable, Value, Command> _function;
		private Position _position;
		private Variable _var;
		private Value _val;
		private Command _chain;

		internal const string StartPattern = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		internal Command (Action<Compiler, Variable, Value, Command> function, Position position, Variable var, Value val, Command chain) {
			_function = function;
			_position = position;
			_var = var;
			_val = val;
			_chain = chain;
		}

		internal Position Position {
			get {
				return _position;
			}
		}

		internal static Command Parse (Reader reader, Dictionary<string, Instruction> pool) {
			string key;
			Position pos;
			Instruction instr;
			Variable var;
			Value val;
			Command chain;

			if (!reader.Match(StartPattern)) {
				throw new CompileException("Expected instruction.", reader.Position);
			}

			pos = reader.Position;
			key = reader.Consume(StartPattern + "");
			if (!pool.ContainsKey(key)) {
				throw new CompileException("Unkown instruction \"" + key + "\".", pos);
			}
			instr = pool[key];
			_space(reader);

			var = null;
			if (instr.HasVar) {
				var = Variable.Parse(reader);
				_space(reader);
			}

			val = null;
			if (instr.HasVal) {
				val = Value.Parse(reader);
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

			return new Command(instr.Function, pos, var, val, chain);
		}

		private static void _space (Reader reader) {
			if (!reader.Match(" \t\n") && !reader.EndOfReader) {
				throw new CompileException("Illegal character " + reader.PrettyPeek() + ".", reader.Position);
			}

			reader.Skip(" \t");
		}

		internal void Invoke (Compiler compiler) {
			_function(compiler, _var, _val, _chain);
		}
	}
}