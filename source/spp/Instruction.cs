using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
	internal struct Instruction {
		internal Action<VarName, Value, Command> Function;
		internal bool HasVar;
		internal bool HasVal;
		internal Dictionary<string, Instruction> Chain;

		internal static readonly Dictionary<string, Instruction> _root = new Dictionary<string, Instruction> {
			{ "error", new Instruction(_error, false, true,  null ) },
			{ "try",   new Instruction(_try,   false, false, _root) }
		};

		internal Instruction (Action<VarName, Value, Command> function, bool hasVar, bool hasVal, Dictionary<string, Instruction> chain) {
			Function = function;
			HasVar = hasVar;
			HasVal = hasVal;
			Chain = chain;
		}

		private static void _error (VarName var, Value val, Command chain) {
			throw new CompileException(val.ToString(), val.Position);
		}

		private static void _try (VarName var, Value val, Command chain) {
			try {
				chain.Invoke();
			} catch (CompileException) {}
		}
	}
}