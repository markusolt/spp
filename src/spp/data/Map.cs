using System;
using System.Collections.Generic;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Data;

namespace Spp.Data {
  internal class Map : Value {
    protected Dictionary<string, Value> _children;

    internal Map () {
      _children = new Dictionary<string, Value>();
    }

    internal Map (Dictionary<string, Value> children) {
      _children = children;
    }

    internal Map (Position position, Dictionary<string, Value> children) : base(position) {
      _children = children;
    }

    internal override Value this[Value key] {
      get {
        if (!Has(key)) {
          throw new CompileException("Unkown member \"" + key.ToString() + "\".", key.Position);
        }

        return _children[key.AsString()];
      }
      set {
        _children[key.AsString()] = value;
      }
    }

    internal override bool IsEnumerable { get { return true; } }

    internal override bool IsKeyValue { get { return _children.ContainsKey("key") && _children["key"].IsString && _children.ContainsKey("value"); } }

    internal override bool Has (Value key) {
      return key.IsString && _children.ContainsKey(key.AsString());
    }

    internal override IEnumerable<Value> AsEnumerable () { return new EnumerationConverter<KeyValuePair<string, Value>, Value>(_children.GetEnumerator(), _enumerationConverter); }

    internal override Map AsKeyValue () {
      if (IsKeyValue) {
        return this;
      }

      throw new CompileException("Expected a key value pair.", _position);
    }

    internal override void Push (Value extension) {
      extension = extension.AsKeyValue();
      _children[extension[new Text("key")].AsString()] = extension[new Text("value")];
    }

    internal override TextWriter Stringify (TextWriter writer, bool root) {
      bool firstIteration;

      writer.Write('{');
      firstIteration = true;
      foreach (KeyValuePair<string, Value> entry in _children) {
        if (!firstIteration) {
          writer.Write(", ");
        } else {
          firstIteration = false;
        }
        new Text(default(Position), entry.Key).Stringify(writer, false);
        writer.Write(": ");
        entry.Value.Stringify(writer, false);
      }
      writer.Write('}');
      return writer;
    }

    private static Map _enumerationConverter (KeyValuePair<string, Value> entry) {
      return new Map(new Dictionary<string, Value> {
        { "key", new Text(entry.Key) },
        { "value", entry.Value }
      });
    }
  }
}
