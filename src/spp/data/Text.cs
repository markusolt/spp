using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Spp;
using Spp.IO;
using Spp.Lexing;
using Spp.Data;

namespace Spp.Data {
  internal class Text : Value {
    private string _payload;

    internal static readonly Parser<Expression> Parser = new ParseToken<Expression>("\"", _parse);

    internal Text (string payload) {
      _payload = payload;
    }

    internal Text (Position position, string payload) : base(position) {
      _payload = payload;
    }

    internal override bool IsString { get { return true; } }

    internal override string AsString () { return _payload; }

    internal override TextWriter Stringify (TextWriter buffer, bool root) {
      if (root) {
        buffer.Write(_payload);
        return buffer;
      }

      buffer.Write('"');
      foreach (char c in _payload) {
        switch (c) {
          case '"': {
            buffer.Write("\\\"");
            break;
          }
          case '$': {
            buffer.Write(@"\$");
            break;
          }
          case '\\': {
            buffer.Write(@"\\");
            break;
          }
          case '\r': {
            buffer.Write(@"\r");
            break;
          }
          case '\n': {
            buffer.Write(@"\n");
            break;
          }
          case '\t': {
            buffer.Write(@"\t");
            break;
          }
          case '\f': {
            buffer.Write(@"\f");
            break;
          }
          default: {
            buffer.Write(c);
            break;
          }
        }
      }
      buffer.Write('"');
      return buffer;
    }

    private static Expression _parse (Reader reader) {
      StringBuilder buffer;
      char c;
      List<Expression> list;
      Position rootPos;
      Position pos;

      rootPos = reader.Position;
      reader.Read();

      buffer = new StringBuilder();
      c = ' ';
      list = new List<Expression>();

      while (c != '"') {
        pos = reader.Position;
        c = reader.Read();
        switch (c) {
          case '\\': {
            switch (reader.Read()) {
              case '"': {
                buffer.Append('"');
                break;
              }
              case '$': {
                buffer.Append('$');
                break;
              }
              case '\\': {
                buffer.Append('\\');
                break;
              }
              case '/': {
                buffer.Append('/');
                break;
              }
              case 'r': {
                buffer.Append('\r');
                break;
              }
              case 'n': {
                buffer.Append('\n');
                break;
              }
              case 't': {
                buffer.Append('\t');
                break;
              }
              case 'f': {
                buffer.Append('\f');
                break;
              }
              default: {
                throw new CompileException("Unkown escape sequence.", pos);
              }
            }
            break;
          }
          case '"': {
            break;
          }
          case '\n': {
            throw new CompileException("Unclosed quoted string.", pos);
          }
          case '$': {
            if (buffer.Length > 0) {
              list.Add(new Text(rootPos, buffer.ToString()));
              buffer.Clear();
            }
            reader.Skip(" \t");
            list.Add(Expression.ExpressionParser.Parse(reader));
            break;
          }
          default: {
            buffer.Append(c);
            break;
          }
        }
      }

      if (buffer.Length > 0) {
        list.Add(new Text(rootPos, buffer.ToString()));
        buffer.Clear();
      }

      if (list.Count == 1) {
        return list[0];
      }
      return new Concat(rootPos, list.ToArray());
    }
  }
}
