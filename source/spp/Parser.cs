using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Parser {
		private Reader _reader;

		private const string _defInstrName = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string _defNum = "0123456789";
		private const string _defVarName = _defInstrName;

		internal Parser (Reader reader) {
			_reader = reader;
		}

		private Command _parseInstruction (Dictionary<string, Instruction> pool) {
			string key;
			Position pos;
			Instruction instr;
			VarName var;
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
				var = _parseVarName();
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

		private VarName _parseVarName () {
			if (!_reader.Match(_defVarName)) {
				throw new CompileException("Expected variable name.", _reader.Position);
			}

			return new VarName (_reader.Position, _reader.Consume(_defVarName));
		}

		private Value _parseValue () {
			if (!_reader.Match(_defNum)) {
				throw new CompileException("Expected value.", _reader.Position);
			}

			return new Value (_reader.Position, int.Parse(_reader.Consume(_defNum)));
		}
	}
}