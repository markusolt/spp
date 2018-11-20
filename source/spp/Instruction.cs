using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
	internal struct Instruction {
		internal Action<Variable, Value, Command> Function;
		internal bool HasVar;
		internal bool HasVal;
		internal Dictionary<string, Instruction> Chain;

		internal static readonly Dictionary<string, Instruction> All = new Dictionary<string, Instruction>();

		static Instruction () {
			All.Add("error", new Instruction(_error, false, true,  null));
			All.Add("try",   new Instruction(_try,   false, false, All));
		}

		internal Instruction (Action<Variable, Value, Command> function, bool hasVar, bool hasVal, Dictionary<string, Instruction> chain) {
			Function = function;
			HasVar = hasVar;
			HasVal = hasVal;
			Chain = chain;
		}

		private static void _error (Variable var, Value val, Command chain) {
			throw new CompileException(val.ToString(), val.Position);
		}

		private static void _try (Variable var, Value val, Command chain) {
			try {
				chain.Invoke();
			} catch (CompileException) {}
		}
	}
}