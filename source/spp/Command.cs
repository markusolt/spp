using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Command {
		private Action<VarName, Value, Command> _function;
		private Position _position;
		private VarName _var;
		private Value _val;
		private Command _chain;

		internal Command (Action<VarName, Value, Command> function, Position position, VarName var, Value val, Command chain) {
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