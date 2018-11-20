using System;
using System.IO;
using System.Text;
using Spp;
using Spp.IO;
using Spp.Values;

namespace Spp.Values {
	internal class Text : Value {
		private string _content;

		internal new const string StartPattern = "\"";

		internal Text (Position position, string content) : base(position) {
			_content = content;
		}

		internal new static Text Parse (Reader reader) {
			StringBuilder buffer;
			char c;
			Position rootPos;
			Position pos;

			rootPos = reader.Position;
			reader.Assert('"');

			buffer = new StringBuilder();
			c = ' ';

			while (c != '"') {
				pos = reader.Position;
				c = reader.Read();
				switch (c) {
					case '\\': {
						switch (reader.Read()) {
							case '"': {
								buffer.Append('"');
								break;
							}
							case '\\': {
								buffer.Append('\\');
								break;
							}
							case '/': {
								buffer.Append('/');
								break;
							}
							case 'r': {
								buffer.Append('\r');
								break;
							}
							case 'n': {
								buffer.Append('\n');
								break;
							}
							case 't': {
								buffer.Append('\t');
								break;
							}
							case 'f': {
								buffer.Append('\f');
								break;
							}
							default: {
								throw new CompileException("Unkown escape sequence.", pos);
							}
						}
						break;
					}
					case '"': {
						break;
					}
					case '\n': {
						throw new CompileException("Unclosed quoted string.", pos);
					}
					default: {
						buffer.Append(c);
						break;
					}
				}
			}

			return new Text(rootPos, buffer.ToString());
		}

		internal override void Stringify (TextWriter writer) {
			writer.Write('"');
			foreach (char c in _content) {
				switch (c) {
					case '"': {
						writer.Write("\\\"");
						break;
					}
					case '\\': {
						writer.Write(@"\\");
						break;
					}
					case '\r': {
						writer.Write(@"\r");
						break;
					}
					case '\n': {
						writer.Write(@"\n");
						break;
					}
					case '\t': {
						writer.Write(@"\t");
						break;
					}
					case '\f': {
						writer.Write(@"\f");
						break;
					}
					default: {
						writer.Write(c);
						break;
					}
				}
			}
			writer.Write('"');
		}
	}
}