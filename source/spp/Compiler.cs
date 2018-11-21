using System;
using System.IO;
using System.Text;
using Spp.IO;
using Spp.Values;
using Spp;

namespace Spp {
	internal class Compiler {
		private string _filePath;
		private Reader _reader;
		private TextWriter _writer;
		private StringBuilder _buffer;
		private Map _variables;

		internal Compiler (string filePath) : this() {
			_filePath = Path.GetFullPath(filePath); // TODO: handle errors
		}

		private Compiler () {
			_buffer = new StringBuilder();
		}

		internal TextWriter Writer {
			get {
				return _writer;
			}
			set {
				if (_writer != null) {
					_writer.Dispose();
				}
				_writer = value;
			}
		}

		internal Map Variables {
			get {
				return _variables;
			}
		}

		internal void Compile () {
			try {
				_reader = new Reader(new StreamReader(_filePath), _filePath, this); // TODO: handle erros
				_writer = null;
				_variables = new Map(_reader.Position);

				while (!_reader.EndOfReader) {
					_compileStep();
				}
			} finally {
				_reader.Dispose();
				if (_writer != null) {
					_writer.Dispose();
					_writer = null;
				}
			}
		}

		private void _compileStep () {
			char c;

			_buffer.Clear();
			_reader.Consume(" \t", _buffer);

			switch (_reader.Peek()) {
				case '#': {
					_reader.Read();
					_reader.Skip(" \t");
					Command.Parse(_reader, Instruction.All).Invoke(this);
					_reader.Read();
					return;
				}
				case '\n': {
					_reader.Read();
					return;
				}
			}

			if (_writer == null) {
				throw new CompileException("No open output file.", _reader.Position);
			}

			_writer.Write(_buffer.ToString());

			while (true) {
				c = _reader.Read();
				switch (c) {
					case '\n': {
						_writer.Write(_writer.NewLine);
						return;
					}
					case '$': {
						Variable.Parse(_reader).Find(_reader.Compiler.Variables).Stringify(_writer, true);
						_reader.Assert('$');
						break;
					}
					default: {
						_writer.Write(c);
						break;
					}
				}
			}
		}
	}
}
