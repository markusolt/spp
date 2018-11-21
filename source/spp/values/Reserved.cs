using System;
using System.Collections.Generic;
using System.IO;
using Spp;
using Spp.IO;
using Spp.Values;

namespace Spp.Values {
	internal class Reserved : Value {
		private Compiler _compiler;
		private Variable _variable;

		internal const string StartPattern = Variable.StartPattern;

		internal Reserved (Position position, Compiler compiler, Variable variable) : base(position) {
			_compiler = compiler;
			_variable = variable;
		}

		internal new static Reserved Parse (Reader reader) {
			return new Reserved(reader.Position, reader.Compiler, Variable.Parse(reader));
		}

		internal override IEnumerator<Value> ToEnumerator () {
			Value target;

			target = _variable.Find(_compiler.Variables);
			target.Position = Position;
			return target.ToEnumerator();
		}

		internal override void Stringify (TextWriter writer) {
			Value target;

			target = _variable.Find(_compiler.Variables);
			target.Position = Position;
			target.Stringify(writer);
		}

		public override string ToString () {
			Value target;

			target = _variable.Find(_compiler.Variables);
			target.Position = Position;
			return target.ToString();
		}
	}
}