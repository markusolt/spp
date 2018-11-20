using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Command {
		private Action<Variable, Value, Command> _function;
		private Position _position;
		private Variable _var;
		private Value _val;
		private Command _chain;

		internal Command (Action<Variable, Value, Command> function, Position position, Variable var, Value val, Command chain) {
			_function = function;
			_position = position;
			_var = var;
			_val = val;
			_chain = chain;
		}

		internal Position Position {
			get {
				return _position;
			}
		}

		internal void Invoke () {
			_function(_var, _val, _chain);
		}
	}
}