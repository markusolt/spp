using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal struct Instruction {
		internal Action<Compiler, Variable, Value, Command> Function;
		internal bool HasVar;
		internal bool HasVal;
		internal Dictionary<string, Instruction> Chain;

		internal static readonly Dictionary<string, Instruction> All = new Dictionary<string, Instruction>();

		static Instruction () {
			All.Add("warning", new Instruction(_warn,  false, true,  null));
			All.Add("try",     new Instruction(_try,   false, false, All));
			All.Add("let",     new Instruction(_let,   true,  true,  null));
			All.Add("for",     new Instruction(_for,   true,  true,  All));
			All.Add("write",   new Instruction(_write, false, true,  null));
			All.Add("close",   new Instruction(_close, false, false, null));
		}

		internal Instruction (Action<Compiler, Variable, Value, Command> function, bool hasVar, bool hasVal, Dictionary<string, Instruction> chain) {
			Function = function;
			HasVar = hasVar;
			HasVal = hasVal;
			Chain = chain;
		}

		private static void _warn (Compiler compiler, Variable var, Value val, Command chain) {
			Console.WriteLine(val.Position.ToString() + ": Warning: " + val.ToString());
		}

		private static void _try (Compiler compiler, Variable var, Value val, Command chain) {
			try {
				chain.Invoke(compiler);
			} catch (CompileException) {}
		}

		private static void _let (Compiler compiler, Variable var, Value val, Command chain) {
			var.Set(compiler.Variables, val);
		}

		private static void _for (Compiler compiler, Variable var, Value val, Command chain) {
			/*foreach (Value entry in val) {
				_let(compiler, var, entry, null);
				chain.Invoke(compiler);
			}*/
		}

		private static void _write (Compiler compiler, Variable var, Value val, Command chain) {
			compiler.Writer = new StreamWriter("demo/" + val.ToString(), false);
		}

		private static void _close (Compiler compiler, Variable var, Value val, Command chain) {
			compiler.Writer = null;
		}
	}
}
