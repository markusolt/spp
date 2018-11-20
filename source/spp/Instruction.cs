using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Instruction {
		private Action<VarName, Value, Command> _function;
		private bool _hasVar;
		private bool _hasVal;
		private Dictionary<string, Instruction> _chain;

		internal static readonly Dictionary<string, Instruction> _root = new Dictionary<string, Instruction> {
			{ "error", new Instruction(_error, false, true,  null ) },
			{ "try",   new Instruction(_try,   false, false, _root) }
		};

		internal Instruction (Action<VarName, Value, Command> function, bool hasVar, bool hasVal, Dictionary<string, Instruction> chain) {
			_function = function;
			_hasVar = hasVar;
			_hasVal = hasVal;
			_chain = chain;
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