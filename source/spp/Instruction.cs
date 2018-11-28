using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Spp.IO;
using Spp;

namespace Spp {
	internal struct Instruction {
		internal Func<Compiler, Variable[], ValueRecipe[], Value> Function;
		internal int VariableCount;
		internal int ValueCount;

		internal static readonly Dictionary<string, Instruction> Root = new Dictionary<string, Instruction>() {
			{"warning",  new Instruction(_warn,     0, 1)},
			{"error",    new Instruction(_error,    0, 1)},
			{"try",      new Instruction(_try,      0, 1)},
			{"let",      new Instruction(_let,      1, 1)},
			{"input",    new Instruction(_input,    0, 1)},
			{"cdinput",  new Instruction(_cdinput,  0, 1)},
			{"output",   new Instruction(_output,   0, 1)},
			{"cdoutput", new Instruction(_cdoutput, 0, 1)},
			{"close",    new Instruction(_close,    0, 0)},
			{"for",      new Instruction(_for,      1, 2)},
			{"add",      new Instruction(_add,      0, 2)},
			{"if",       new Instruction(_if,       0, 2)},
			{"always",   new Instruction(_always,   0, 0)},
			{"never",    new Instruction(_never,    0, 0)},
			{"equals",   new Instruction(_equals,   0, 2)}
		};

		internal Instruction (Func<Compiler, Variable[], ValueRecipe[], Value> function, int variableCount, int valueCount) {
			Function = function;
			VariableCount = variableCount;
			ValueCount = valueCount;
		}

		private static Value _warn (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			Value val = values[0].Evaluate(compiler);

			Console.WriteLine(val.Position.ToString() + ": Warning: " + val.ToString());
			return Value.Empty;
		}

		private static Value _error (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			Value val = values[0].Evaluate(compiler);

			throw new CompileException(val.ToString(), val.Position);
		}

		private static Value _try (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			try {
				return values[0].Evaluate(compiler);
			} catch (CompileException e) {
				Console.WriteLine(e.Position.ToString() + ": Caught: Error: " + e.Message);
			}
			return Value.Empty;
		}

		private static Value _let (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			Value val = values[0].Evaluate(compiler);

			variables[0].Set(compiler,val);
			return Value.Empty;
		}

		private static Value _for (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			Value val = values[0].Evaluate(compiler);

			IEnumerator<Value> enumerator = val.AsEnumerator();
			enumerator.Reset();

			while (enumerator.MoveNext()) {
				_let(compiler, variables, new ValueRecipe[] {enumerator.Current});
				values[1].Evaluate(compiler);
			}

			return Value.Empty;
		}

		private static string _resolvePath (string basePath, string filePath, Position position, bool canCreate) {
			try {
				filePath = Path.Combine(basePath, filePath);
			} catch (ArgumentException e) {
				throw new CompileException("Illegal characters in path.", position, e);
			}

			if (!canCreate && !File.Exists(filePath)) {
				throw new CompileException("File does not exist.", position);
			}

			return filePath;
		}

		private static Value _cdinput (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string path;

			Value val = values[0].Evaluate(compiler);

			try {
				path = Path.GetFullPath(Path.Combine(compiler.CdInput, val.AsString()));
			} catch (ArgumentException e) {
				throw new CompileException("Illegal characters in path.", val.Position, e);
			}

			if (!Directory.Exists(path)) {
				throw new CompileException("Directory does not exist.", val.Position);
			}

			compiler.CdInput = path;
			return new Text(default(Position), path);
		}

		private static Value _input (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string filePath;

			filePath = _resolvePath(compiler.CdInput, values[0].Evaluate(compiler).AsString(), values[0].Position, false);

			compiler.CompileInsert(filePath);
			return new Text(default(Position), filePath);
		}

		private static Value _cdoutput (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string path;

			Value val = values[0].Evaluate(compiler);

			try {
				path = Path.GetFullPath(Path.Combine(compiler.CdOutput, val.AsString()));
			} catch (ArgumentException e) {
				throw new CompileException("Illegal characters in path.", val.Position, e);
			}

			if (!Directory.Exists(path)) {
				throw new CompileException("Directory does not exist.", val.Position);
			}

			compiler.CdOutput = path;
			return new Text(default(Position), path);
		}

		private static Value _output (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string filePath;

			filePath = _resolvePath(compiler.CdInput, values[0].Evaluate(compiler).AsString(), values[0].Position, true);

			// qlikview requires its xml files to be utf8-bom -> UTF8Encoding(true)
			compiler.Writer = new StreamWriter(filePath, false, new UTF8Encoding(true));
			return new Text(default(Position), filePath);
		}

		private static Value _close (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			compiler.Writer = null;
			return Value.Empty;
		}

		private static Value _add (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			int i1 = values[0].Evaluate(compiler).AsInt();
			int i2 = values[1].Evaluate(compiler).AsInt();

			return new Num(default(Position), i1 + i2);
		}

		private static Value _if (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			bool b1 = values[0].Evaluate(compiler).AsBool();

			if (b1) {
				return values[1].Evaluate(compiler);
			}
			return Value.Empty;
		}

		private static Value _always (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			return new Bool(default(Position), true);
		}

		private static Value _never (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			return new Bool(default(Position), false);
		}

		private static Value _equals (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			Value v1 = values[0].Evaluate(compiler);
			Value v2 = values[1].Evaluate(compiler);

			return new Bool(default(Position), v1.ToString() == v2.ToString());
		}

		private static Value _loadText (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string filePath;
			StreamReader reader;
			string contents;

			filePath = _resolvePath(compiler.CdInput, values[0].Evaluate(compiler).AsString(), values[0].Position, false);

			reader = new StreamReader(filePath);
			contents = reader.ReadToEnd().Trim();
			reader.Dispose();

			return new Text(default(Position), contents);
		}

		private static Value _loadJson (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string filePath;
			Reader reader;
			Position position;
			Value res;

			filePath = _resolvePath(compiler.CdInput, values[0].Evaluate(compiler).AsString(), values[0].Position, false);

			reader = new Reader(new StreamReader(filePath), filePath);
			position = reader.Position;
			switch (reader.Peek()) {
				case '{': {
					res = MapRecipe.Parser.Parse(reader).Evaluate(compiler);
					reader.Dispose();
					return res;
				}
				case '[': {
					res = SequenceRecipe.Parser.Parse(reader).Evaluate(compiler);
					reader.Dispose();
					return res;
				}
				default: {
					reader.Dispose();
					throw new CompileException("Expected Json.", position);
				}
			}
		}
	}
}
