using System;
using Spp.IO;

namespace Spp.IO {
	internal class CompileException : Exception {
		private string _message;

		internal CompileException (string message, Position pos) : base(message) {
			_message = pos.ToString() + ": " + "Error: " + message;
		}

		internal CompileException (string message, Position pos, Exception innerException) : base(message, innerException) {
			_message = pos.ToString() + ": " + "Error: " + message;
		}

		public override string ToString () {
			return _message;
		}
	}
}
