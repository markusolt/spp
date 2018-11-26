using System;
using System.Collections.Generic;
using System.IO;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Concat : ValueRecipe {
		private List<ValueRecipe> _list;

		internal Concat (Position position, List<ValueRecipe> list) : base(position) {
			_list = list;
		}

		internal override Value Evaluate (Compiler compiler) {
			StringWriter buffer;

			buffer = new StringWriter();
			foreach (Value entry in _list) {
				entry.Evaluate(compiler).Stringify(buffer, true);
			}
			return new Text(_position, buffer.ToString());
		}
	}
}
