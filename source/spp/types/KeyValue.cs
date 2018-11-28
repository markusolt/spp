using System;
using System.Collections.Generic;
using Spp.IO;
using Spp;

namespace Spp {
	internal class KeyValue : Map {
		internal KeyValue (Position position, string key, Value value) : base(position, new Dictionary<string, Value> { { "key", new Text(default(Position), key) }, { "value", value } }) {}

		internal override bool IsKeyValue { get { return true; } }

		internal string Key { get { return _children["key"].AsString(); } }

		internal Value Value { get { return _children["value"]; } }

		internal override KeyValue AsKeyValue () {
			return this;
		}
	}
}