using System;
using Spp.IO;
using Spp.Values;
using Spp;

namespace Spp {
	internal class Variable {
		private Value _name;
		private Variable _next;

		internal const string StartPattern = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
		internal const string ContinuePattern = StartPattern + "-0123456789";

		internal Variable (Value name, Variable next) {
			_name = name;
			_next = next;
		}

		internal Variable Next {
			get {
				return _next;
			}
			set {
				_next = value;
			}
		}

		internal static Variable Parse (Reader reader) {
			Variable root;
			Variable current;

			if (!reader.Match(StartPattern)) {
				throw new CompileException("Expected variable.", reader.Position);
			}

			root = new Variable(new Text(reader.Position, reader.Consume(ContinuePattern)), null);
			current = root;

			while (reader.Match("[")) {
				reader.Read();
				current.Next = new Variable(Value.Parse(reader), null);
				current = current.Next;
				reader.Assert(']');
			}

			if (reader.Match(".")) {
				reader.Read();
				current.Next = Variable.Parse(reader);
			}
			return root;
		}

		internal Value Find (Value collection) {
			if (_next != null) {
				return _next.Find(collection.Get(_name.Evaluate()));
			}
			return collection.Get(_name.Evaluate());
		}

		internal void Set (Value collection, Value value) {
			if (_next != null) {
				_next.Set(collection.Get(_name.Evaluate()), value);
			}
			collection.Set(_name.Evaluate(), value);
		}
	}
}