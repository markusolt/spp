using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Variable {
		private Position _position;
		private string _name;
		private Variable _next;

		internal const string StartPattern = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
		internal const string ContinuePattern = StartPattern + "-0123456789";

		internal Variable (Position position, string name, Variable next) {
			_position = position;
			_name = name;
			_next = next;
		}

		internal static Variable Parse (Reader reader) {
			Position position;
			string name;

			if (!reader.Match(StartPattern)) {
				throw new CompileException("Expected variable.", reader.Position);
			}

			position = reader.Position;
			name = reader.Consume(ContinuePattern);

			if (reader.Match(".")) {
				reader.Read();
				return new Variable(position, name, Variable.Parse(reader));
			}
			return new Variable(position, name, null);
		}

		internal Position Position {
			get {
				return _position;
			}
		}

		internal Value Find (Value collection) {
			if (_next != null) {
				return _next.Find(collection.Get(_name, _position));
			}
			return collection.Get(_name, _position);
		}

		internal void Set (Value collection, Value value) {
			if (_next != null) {
				_next.Set(collection.Get(_name, _position), value);
			}
			collection.Set(_name, _position, value);
		}
	}
}