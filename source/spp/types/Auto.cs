using System;
using System.Collections.Generic;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Types;

namespace Spp.Types {
	internal class Auto : Value {
		private Func<Compiler, Position, Value> _evaluate;

		private static Dictionary<string, Func<Compiler, Position, Value>> _root = new Dictionary<string, Func<Compiler, Position, Value>> {
		};

		internal Auto (Position position, Func<Compiler, Position, Value> evaluate) : base(position) {
			_evaluate = evaluate;
		}

		internal new static Auto Parse (Reader reader) {
			string key;
			Position position;

			if (!reader.Match(":")) {
				throw new CompileException("Expected an auto.", reader.Position);
			}

			position = reader.Position;
			reader.Assert(':');
			key = reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			if (!_root.ContainsKey(key.ToLower())) {
				throw new CompileException("Unkown auto \"" + key + "\".", position);
			}

			return new Auto(position, _root[key.ToLower()]);
		}

		internal override void Stringify (TextWriter writer, bool root) {
			throw new NotSupportedException("Autos should be evaluated before stringify.");
		}

		internal override Value Evaluate (Compiler compiler, Value node) {
			return _evaluate(compiler, Position);
		}
	}
}
