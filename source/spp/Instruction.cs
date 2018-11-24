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

		internal static readonly Dictionary<string, Instruction> Root = new Dictionary<string, Instruction>();

		static Instruction () {
			Root.Add("warning",  new Instruction(_warn,     false, true,  null));
			Root.Add("error",    new Instruction(_error,    false, true,  null));
			Root.Add("try",      new Instruction(_try,      false, false, Root));
			Root.Add("let",      new Instruction(_let,      true,  true,  null));
			Root.Add("input",    new Instruction(_input,    false, true,  null));
			Root.Add("cdinput",  new Instruction(_cdinput,  false, true,  null));
			Root.Add("output",   new Instruction(_output,   false, true,  null));
			Root.Add("cdoutput", new Instruction(_cdoutput, false, true,  null));
			Root.Add("close",    new Instruction(_close,    false, false, null));
			Root.Add("for",      new Instruction(_for,      true,  true,  Root));
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

		private static void _error (Compiler compiler, Variable var, Value val, Command chain) {
			throw new CompileException(val.ToString(), val.Position);
		}

		private static void _try (Compiler compiler, Variable var, Value val, Command chain) {
			try {
				chain.Invoke(compiler);
			} catch (CompileException e) {
				Console.WriteLine(e.Position.ToString() + ": Caught: Error: " + e.Message);
			}
		}

		private static void _let (Compiler compiler, Variable var, Value val, Command chain) {
			var.Set(compiler, null, val);
		}

		private static void _for (Compiler compiler, Variable var, Value val, Command chain) {
			IEnumerator<Value> enumerator = val.AsEnumerator();
			enumerator.Reset();

			while (enumerator.MoveNext()) {
				_let(compiler, var, enumerator.Current, null);
				chain.Invoke(compiler);
			}
		}

		private static void _cdinput (Compiler compiler, Variable var, Value val, Command chain) {
			string path;

			try {
				path = Path.GetFullPath(Path.Combine(compiler.CdInput, val.AsString()));
			} catch (ArgumentException e) {
				throw new CompileException("Illegal characters in path.", val.Position, e);
			}

			if (!Directory.Exists(path)) {
				throw new CompileException("Directory does not exist.", val.Position);
			}

			compiler.CdInput = path;
		}

		private static void _input (Compiler compiler, Variable var, Value val, Command chain) {
			string filePath;

			try {
				filePath = Path.Combine(compiler.CdInput, val.AsString());
			} catch (ArgumentException e) {
				throw new CompileException("Illegal characters in path.", val.Position, e);
			}

			if (!File.Exists(filePath)) {
				throw new CompileException("File does not exist.", val.Position);
			}

			compiler.CompileInsert(filePath);
		}

		private static void _cdoutput (Compiler compiler, Variable var, Value val, Command chain) {
			string path;

			try {
				path = Path.GetFullPath(Path.Combine(compiler.CdOutput, val.AsString()));
			} catch (ArgumentException e) {
				throw new CompileException("Illegal characters in path.", val.Position, e);
			}

			if (!Directory.Exists(path)) {
				throw new CompileException("Directory does not exist.", val.Position);
			}

			compiler.CdOutput = path;
		}

		private static void _output (Compiler compiler, Variable var, Value val, Command chain) {
			string filePath;

			try {
				filePath = Path.Combine(compiler.CdOutput, val.AsString());
			} catch (ArgumentException e) {
				throw new CompileException("Illegal characters in path.", val.Position, e);
			}

			compiler.Writer = new StreamWriter(filePath, false);
		}

		private static void _close (Compiler compiler, Variable var, Value val, Command chain) {
			compiler.Writer = null;
		}
	}
}
