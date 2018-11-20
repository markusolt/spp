using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Parser {
		private Reader _reader;

		private const string _defInstrName = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string _defVariable = _defInstrName;

		internal Parser (Reader reader) {
			_reader = reader;
		}

		internal Command ParseInstruction () {
			return _parseInstruction(Instruction.All);
		}

		private Command _parseInstruction (Dictionary<string, Instruction> pool) {
			string key;
			Position pos;
			Instruction instr;
			Variable var;
			Value val;
			Command chain;

			pos = _reader.Position;
			key = _parseInstructionName();
			if (!pool.ContainsKey(key)) {
				throw new CompileException("Unkown instruction \"" + key + "\".", pos);
			}
			instr = pool[key];
			_parseHspace();

			var = null;
			if (instr.HasVar) {
				var = _parseVariable();
				_parseHspace();
			}

			val = null;
			if (instr.HasVal) {
				val = _parseValue();
				_parseHspace();
			}

			chain = null;
			if (instr.Chain != null) {
				chain = _parseInstruction(instr.Chain);
				_parseHspace();
			}

			if (!_reader.Match("\n") && !_reader.EndOfReader) {
				throw new CompileException("Expected end of line.", _reader.Position);
			}

			return new Command(instr.Function, pos, var, val, chain);
		}

		private string _parseInstructionName () {
			if (!_reader.Match(_defInstrName)) {
				throw new CompileException("Expected instruction.", _reader.Position);
			}

			return _reader.Consume(_defInstrName);
		}

		private void _parseHspace () {
			if (!_reader.Match(" \t\n") && !_reader.EndOfReader) {
				throw new CompileException("Illegal character " + _reader.PrettyPeek() + ".", _reader.Position);
			}

			_reader.Skip(" \t");
		}

		private Variable _parseVariable () {
			return Variable.Parse(_reader);
		}

		private Value _parseValue () {
			return Value.Parse(_reader);
		}
	}
}