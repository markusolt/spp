using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal abstract class Value : ValueRecipe {
		protected Value (Position position) : base(position) {}

		internal virtual bool IsInt { get { return false; } }

		internal virtual bool IsString { get { return false; } }

		internal virtual bool IsEnumerator { get { return false; } }

		internal override Value Evaluate (Compiler compiler) { return this; }

		internal virtual Value Copy () { return this; }

		internal virtual int AsInt () {
			throw new CompileException("Expected an integer.", _position);
		}

		internal virtual string AsString () {
			throw new CompileException("Expected a string.", _position);
		}

		internal virtual IEnumerator<Value> AsEnumerator () {
			throw new CompileException("Expected a list.", _position);
		}

		internal abstract TextWriter Stringify (TextWriter buffer, bool root);

		public override string ToString () {
			return Stringify(new StringWriter(), true).ToString();
		}
	}
}
