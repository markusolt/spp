using System;
using System.IO;
using System.Text;
using Spp.IO;

namespace Spp.IO {
  internal class Reader : IDisposable {
    private TextReader _reader;
    private Position _position;
    private StringBuilder _buffer;
    private StringBuilder _backtrackingBuffer;
    private bool _endOfReader;

    internal Reader (TextReader reader, string fileName) : this() {
      _reader = reader;
      _position = new Position(fileName);

      _updateEndOfReader();
    }

    private Reader () {
      _buffer = new StringBuilder();
      _backtrackingBuffer = new StringBuilder();
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

      i = _peekRaw();
      while (i == 13) {
        _readRaw();
        i = _peekRaw();
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

      i = _readRaw();
      while (i == 13) {
        i = _readRaw();
      }
      _updateEndOfReader();

      if (i == -1) {
        return '\n';
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

      return Pretty(Peek());
    }

    internal string Pretty (char c) {
      switch (c) {
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
          return c.ToString();
        }
      }
    }

    internal bool Match (string pattern) {
      return pattern.IndexOf(Peek()) > -1;
    }

    internal bool MatchWord (string word) {
      Position position;

      _buffer.Clear();
      position = _position;

      foreach (char c in word) {
        if (Peek() != c) {
          Undo(_buffer.ToString(), position);
          return false;
        }
        Read();
      }

      return true;
    }

    internal void Undo (string section, Position position) {
      _backtrackingBuffer.Insert(0, section);
      _position = position;
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

    internal void Assert (char c) {
      if (Peek() != c) {
        throw new CompileException("Expected " + Pretty(c) + ".", Position);
      }
      Read();
    }

    private int _peekRaw () {
      if (_backtrackingBuffer.Length > 0) {
        return (int) _backtrackingBuffer[0];
      }
      return _reader.Peek();
    }

    private int _readRaw () {
      int i;

      if (_backtrackingBuffer.Length > 0) {
        i = _peekRaw();
        _backtrackingBuffer.Remove(0, 1);
        return i;
      }
      return _reader.Read();
    }

    private void _updateEndOfReader () {
      _endOfReader = _reader.Peek() == -1 && _backtrackingBuffer.Length == 0;
    }
  }
}
