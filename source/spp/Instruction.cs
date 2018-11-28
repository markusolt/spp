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
			{"equals",   new Instruction(_equals,   0, 2)},
			{"loadtext", new Instruction(_loadText, 0, 1)},
			{"loadjson", new Instruction(_loadJson, 0, 1)},
			{"find",     new Instruction(_find,     0, 1)},
			{"contains", new Instruction(_contains, 0, 2)},
			{"where",    new Instruction(_where,    1, 2)},
			{"push",     new Instruction(_push,     0, 2)}

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
			List<Value> results;

			IEnumerator<Value> enumerator = val.AsEnumerable().GetEnumerator();
			enumerator.Reset();

			results = new List<Value>();
			while (enumerator.MoveNext()) {
				_let(compiler, variables, new ValueRecipe[] {enumerator.Current});
				results.Add(values[1].Evaluate(compiler));
			}

			return new Sequence(default(Position), results);
		}

		private static string _resolveFile (string basePath, string path, Position position, bool canCreate) {
			try {
				path = Path.Combine(basePath, path);
			} catch (ArgumentException e) {
				throw new CompileException("Illegal characters in path.", position, e);
			}

			if (!canCreate && !File.Exists(path)) {
				throw new CompileException("File does not exist.", position);
			}

			if (canCreate && !Directory.Exists(Path.GetDirectoryName(path))) {
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}

			return path;
		}

		private static string _resolveDirectory (string basePath, string path, Position position, bool canCreate) {
			try {
				path = Path.Combine(basePath, path);
			} catch (ArgumentException e) {
				throw new CompileException("Illegal characters in path.", position, e);
			}

			if (!canCreate && !Directory.Exists(path)) {
				throw new CompileException("Directory does not exist.", position);
			}

			return path;
		}

		private static Value _cdinput (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string path;

			path = _resolveDirectory(compiler.CdInput, values[0].Evaluate(compiler).AsString(), values[0].Position, false);

			compiler.CdInput = path;
			return new Text(default(Position), path);
		}

		private static Value _input (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string filePath;

			filePath = _resolveFile(compiler.CdInput, values[0].Evaluate(compiler).AsString(), values[0].Position, false);

			compiler.CompileInsert(filePath);
			return new Text(default(Position), filePath);
		}

		private static Value _cdoutput (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string path;

			path = _resolveDirectory(compiler.CdOutput, values[0].Evaluate(compiler).AsString(), values[0].Position, true);

			compiler.CdOutput = path;
			return new Text(default(Position), path);
		}

		private static Value _output (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string filePath;

			filePath = _resolveFile(compiler.CdOutput, values[0].Evaluate(compiler).AsString(), values[0].Position, true);

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

			filePath = _resolveFile(compiler.CdInput, values[0].Evaluate(compiler).AsString(), values[0].Position, false);

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

			filePath = _resolveFile(compiler.CdInput, values[0].Evaluate(compiler).AsString(), values[0].Position, false);

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

		private static Value _find (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			List<Value> files;

			files = new List<Value>();
			foreach (string s in Directory.GetFiles(compiler.CdInput, values[0].Evaluate(compiler).AsString())) {
				files.Add(new Text(default(Position), s));
			}

			return new Sequence(default(Position), files);
		}

		private static Value _contains (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			string v2 = values[1].Evaluate(compiler).ToString();
			IEnumerator<Value> enumerator = values[0].Evaluate(compiler).AsEnumerable().GetEnumerator();
			Value e;

			while (enumerator.MoveNext()) {
				e = enumerator.Current;
				if (e.ToString() == v2) {
					return new Bool(default(Position), true);
				}
			}
			return new Bool(default(Position), false);
		}

		private static Value _where (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			Value v1 = values[0].Evaluate(compiler);

			_let(compiler, variables, new ValueRecipe[] {v1});

			values[1].Evaluate(compiler);
			return Value.Empty;
		}

		private static Value _push (Compiler compiler, Variable[] variables, ValueRecipe[] values) {
			Value v1 = values[0].Evaluate(compiler);
			Value v2 = values[1].Evaluate(compiler);

			v1.Push(v2);
			return v1;
		}
	}
}
