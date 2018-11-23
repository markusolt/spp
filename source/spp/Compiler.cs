using System;
using System.IO;
using System.Text;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Compiler : IDisposable {
		private StringBuilder _buffer;
		private TextWriter _writer;
		private Value _memory;

		internal string CdInput;
		internal string CdOutput;

		internal Compiler () {
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

		internal Value Variables {
			get {
				return _memory;
			}
		}

		internal void Reset () {
			if (_writer != null) {
				_writer.Dispose();
				_writer = null;
			}
			_memory = Value.NewMap();
			CdInput = Path.GetFullPath(".");
			CdOutput = Path.GetFullPath(".");
		}

		internal void Compile (string filePath) {
			Reset();

			try {
				CompileInsert(filePath);
			} finally {
				if (_writer != null) {
					_writer.Dispose();
					_writer = null;
				}
			}
		}

		internal void CompileInsert (string filePath) {
			Reader reader;

			filePath = Path.GetFullPath(filePath); // TODO: handle erros
			reader = new Reader(new StreamReader(filePath), filePath); // TODO: handle erros

			try {
				while (!reader.EndOfReader) {
					_compileLine(reader);
				}
			} finally {
				reader.Dispose();
			}
		}

		private void _compileLine (Reader reader) {
			char c;

			_buffer.Clear();
			reader.Consume(" \t", _buffer);

			switch (reader.Peek()) {
				case '#': {
					reader.Read();
					reader.Skip(" \t");
					Command.Parse(reader, Instruction.Root).Invoke(this);
					reader.Read();
					return;
				}
				case '\n': {
					reader.Read();
					return;
				}
			}

			if (_writer == null) {
				throw new CompileException("No open output file.", reader.Position);
			}

			_writer.Write(_buffer.ToString());

			while (true) {
				c = reader.Read();
				switch (c) {
					case '\n': {
						_writer.Write(_writer.NewLine);
						return;
					}
					case '$': {
						Variable.ParseVariable(reader).Evaluate(this, null).Stringify(_writer, true);
						reader.Assert('$');
						break;
					}
					default: {
						_writer.Write(c);
						break;
					}
				}
			}
		}

		public void Dispose () {
			if (_writer != null) {
				_writer.Dispose();
				_writer = null;
			}
			_memory = null;
		}
	}
}
