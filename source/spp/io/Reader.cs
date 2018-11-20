using System;
using System.IO;
using System.Text;
using Spp.IO;

namespace Spp.IO {
	internal class Reader : IDisposable {
		private TextReader _reader;
		private Position _position;
		private StringBuilder _buffer;
		private bool _endOfReader;

		internal Reader (TextReader reader, string fileName) : this() {
			_reader = reader;
			_position = new Position(fileName);

			_updateEndOfReader();
		}

		private Reader () {
			_buffer = new StringBuilder();
			_endOfReader = true;
		}

		internal Position Position {
			get {
				return _position;
			}
		}

		internal bool EndOfReader {
			get {
				return _endOfReader;
			}
		}

		public void Dispose () {
			if (_reader != null) {
				_reader.Dispose();
				_reader = null;
			}
		}

		internal char Peek () {
			int i;

			if (_reader == null) {
				throw new NotSupportedException();
			}

			i = _reader.Peek();
			while (i == 13) {
				_reader.Read();
				i = _reader.Peek();
			}
			_updateEndOfReader();

			if (i == -1) {
				return '\n';
			}

			return (char) i;
		}

		internal char Read () {
			int i;

			if (_reader == null) {
				throw new NotSupportedException();
			}

			i = _reader.Read();
			while (i == 13) {
				i = _reader.Read();
			}
			_updateEndOfReader();

			if (i == -1) {
				return (char) 0;
			}

			_position = _position.IncrementColumn();
			if (i == 10) {
				_position = _position.IncrementRow();
			}

			return (char) i;
		}

		internal string PrettyPeek () {
			if (_endOfReader) {
				return "[end]";
			}

			switch (Peek()) {
				case '\0': {
					return @"[\0]";
				}
				case '\t': {
					return @"[\t]";
				}
				case '\n': {
					return @"[\n]";
				}
				case '\f': {
					return @"[\f]";
				}
				default: {
					return ("\"" + Peek() + "\"");
				}
			}
		}

		internal bool Match (string pattern) {
			return pattern.IndexOf(Peek()) > -1;
		}

		internal void Skip (string pattern) {
			while (!_endOfReader && Match(pattern)) {
				Read();
			}
		}

		internal void SkipUntil (string pattern) {
			while (!_endOfReader && !Match(pattern)) {
				Read();
			}
		}

		internal void Consume (string pattern, TextWriter writer) {
			while (!_endOfReader && Match(pattern)) {
				writer.Write(Read());
			}
		}

		internal StringBuilder Consume (string pattern, StringBuilder buffer) {
			while (!_endOfReader && Match(pattern)) {
				buffer.Append(Read());
			}

			return buffer;
		}

		internal string Consume (string pattern) {
			_buffer.Clear();
			return Consume(pattern, _buffer).ToString();
		}

		internal void ConsumeUntil (string pattern, TextWriter writer) {
			while (!_endOfReader && !Match(pattern)) {
				writer.Write(Read());
			}
		}

		internal StringBuilder ConsumeUntil (string pattern, StringBuilder buffer) {
			while (!_endOfReader && !Match(pattern)) {
				buffer.Append(Read());
			}

			return buffer;
		}

		internal string ConsumeUntil (string pattern) {
			_buffer.Clear();
			return ConsumeUntil(pattern, _buffer).ToString();
		}

		private void _updateEndOfReader () {
			if (_reader != null && _reader.Peek() == -1) {
				_reader.Dispose();
				_reader = null;
			}

			_endOfReader = _reader == null;
		}
	}
}
