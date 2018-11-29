using System;
using Spp.IO;

namespace Spp.IO {
  internal struct Position {
    private string _fileName;
    private int _row;
    private int _column;

    private const int _startingRow = 1;
    private const int _startingColumn = 1;

    internal Position (string fileName) {
      _fileName = fileName;
      _row = _startingRow;
      _column = _startingColumn;
    }

    internal Position IncrementRow () {
      return new Position {
        _fileName = _fileName,
        _row = _row + 1,
        _column = _startingColumn,
      };
    }

    internal Position IncrementColumn () {
      return new Position {
        _fileName = _fileName,
        _row = _row,
        _column = _column + 1,
      };
    }

    public override string ToString () {
      return "\"" + _fileName + "\":" + _row + ":" + _column;
    }
  }
}
