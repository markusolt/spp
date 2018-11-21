using System;
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

		internal override void Stringify (TextWriter writer) {
			_variable.Find(_compiler.Variables).Stringify(writer);
		}

		public override string ToString () {
			return _variable.Find(_compiler.Variables).ToString();
		}
	}
}