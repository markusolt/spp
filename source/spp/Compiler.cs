using System;
using System.IO;
using System.Text;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Compiler {
		private string _filePath;
		private Reader _reader;
		private Parser _parser;
		private TextWriter _writer;
		private StringBuilder _buffer;

		internal Compiler (string filePath) : this() {
			_filePath = Path.GetFullPath(filePath); // TODO: handle errors
		}

		private Compiler () {
			_buffer = new StringBuilder();
		}

		internal void Compile () {
			try {
				_reader = new Reader(new StreamReader(_filePath), _filePath); // TODO: handle erros
				_parser = new Parser(_reader);
				_writer = null;

				while (!_reader.EndOfReader) {
					_compileStep();
				}
			} finally {
				_reader.Dispose();
			}
		}

		private void _compileStep () {
			_buffer.Clear();
			_reader.Consume(" \t", _buffer);

			switch (_reader.Peek()) {
				case '#': {
					_reader.Read();
					_reader.Skip(" ");
					_parser.ParseInstruction().Invoke();
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
			_reader.ConsumeUntil("\\n", _writer);

			_writer.Write(_writer.NewLine);
			_reader.Read();
		}
	}
}
