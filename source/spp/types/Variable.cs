using System;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Variable : ValueRecipe {
		private Value _key;
		private Variable _next;

		internal static readonly Parser<ValueRecipe> Parser = new ParseToken<ValueRecipe>("variable", ".", _parse);

		internal Variable (Position position, Value key, Variable next) : base(position) {
			_key = key;
			_next = next;
		}

		internal override bool IsVariable { get { return true; } }

		internal override Variable AsVariable () {
			return this;
		}

		internal override Value Evaluate (Compiler compiler) {
			Value res;
			res = _evaluate(compiler.Variables);
			res.Position = _position;
			return res;
		}

		internal void Set (Compiler compiler, Value payload) {
			_set(compiler.Variables, payload);
		}

		private Value _evaluate (Value parent) {
			if (_next != null) {
				return _next._evaluate(parent[_key]);
			}
			return parent[_key];
		}

		private void _set (Value parent, Value payload) {
			if (_next != null) {
				_next._set(parent[_key], payload);
			}
			parent[_key] = payload;
		}

		private static Variable _parse (Reader reader) {
			Position position;
			Value key;

			position = reader.Position;
			reader.Read();
			key = new Text(reader.Position, reader.Consume("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789"));
			if (key.AsString().Length == 0) {
				throw new CompileException("Expected variable identifier.", reader.Position);
			}

			if (Parser.Match(reader.Peek())) {
				return new Variable(position, key, Parser.Parse(reader).AsVariable());
			}
			return new Variable(position, key, null);
		}
	}
}
