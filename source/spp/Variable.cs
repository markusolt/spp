using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp.Types;
using Spp;

namespace Spp {
	internal class Variable : Value {
		private Value _name;
		private Variable _next;

		private const string _pattern = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-0123456789";

		internal Variable (Position position, Value name, Variable next) : base(position) {
			_name = name;
			_next = next;
		}

		internal static Variable Parse (Reader reader, bool isRooted) {
			Variable root;
			Variable current;
			Position pos;
			string key;

			if (!reader.Match(_pattern)) {
				throw new CompileException("Expected a variable.", reader.Position);
			}

			pos = reader.Position;
			key = reader.Consume(_pattern);
			if (isRooted && Instruction.Root.ContainsKey(key)) {
				throw new CompileException("Expected a variable.", pos);
			}
			root = new Variable(pos, new Text(pos, key), null);
			current = root;

			while (reader.Match("[")) {
				reader.Read();
				current._next = new Variable(reader.Position, Value.Parse(reader), null);
				current = current._next;
				reader.Assert(']');
			}

			if (reader.Match(".")) {
				reader.Read();
				current._next = Variable.Parse(reader, false);
			}
			return root;
		}

		internal override void Stringify (TextWriter writer, bool root) {
			throw new NotSupportedException("Variables should be evaluated before stringify.");
		}

		internal override Value Evaluate (Compiler compiler, Value node) {
			Value val;

			if (node == null) {
				node = compiler.Variables;
			}

			if (_next != null) {
				val = _next.Evaluate(compiler, node[_name.Evaluate(compiler, null)]);
				val.Position = Position;
				return val;
			}

			val = node[_name.Evaluate(compiler, null)];
			val.Position = Position;
			return val.Copy();
		}

		internal void Set (Compiler compiler, Value node, Value value) {
			if (node == null) {
				node = compiler.Variables;
			}

			if (_next != null) {
				_next.Set(compiler, node[_name.Evaluate(compiler, null)], value);
				return;
			}
			node[_name.Evaluate(compiler, null)] = value;
		}
	}
}
