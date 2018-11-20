using System;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Variable {
		private Position _position;
		private string _name;

		internal const string StartPattern = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
		internal const string ContinuePattern = StartPattern + "-0123456789";

		internal Variable (Position position, string name) {
			_position = position;
			_name = name;
		}

		internal static Variable Parse (Reader reader) {
			if (!reader.Match(StartPattern)) {
				throw new CompileException("Expected variable.", reader.Position);
			}

			return new Variable(reader.Position, reader.Consume(ContinuePattern));
		}

		internal Position Position {
			get {
				return _position;
			}
		}
	}
}