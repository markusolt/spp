using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Spp;
using Spp.IO;
using Spp.Types;

namespace Spp.Types {
	internal class Text : Value {
		private string _content;

		internal Text (Position position, string content) : base(position) {
			_content = content;
		}

		internal static Value Parse (Reader reader) {
			StringBuilder buffer;
			char c;
			List<Value> list;
			Position rootPos;
			Position pos;

			if (!reader.Match("\"")) {
				throw new CompileException("Expected a string.", reader.Position);
			}

			rootPos = reader.Position;
			reader.Assert('"');

			buffer = new StringBuilder();
			c = ' ';
			list = new List<Value>();

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
							case '$': {
								buffer.Append('$');
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
					case '$': {
						if (reader.Match("$")) {
							reader.Read();
							buffer.Append('$');
							break;
						}

						if (buffer.Length > 0) {
							list.Add(new Text(rootPos, buffer.ToString()));
							buffer.Clear();
						}
						list.Add(Value.ParseValue(reader));
						reader.Assert('$');
						break;
					}
					default: {
						buffer.Append(c);
						break;
					}
				}
			}

			if (buffer.Length > 0) {
				list.Add(new Text(rootPos, buffer.ToString()));
				buffer.Clear();
			}

			if (list.Count == 1 && list[0] is Text) {
				return list[0];
			}
			return new Concat(rootPos, list);
		}

		internal override void Stringify (TextWriter writer, bool root) {
			if (root) {
				writer.Write(_content);
				return;
			}

			writer.Write('"');
			foreach (char c in _content) {
				switch (c) {
					case '"': {
						writer.Write("\\\"");
						break;
					}
					case '$': {
						writer.Write(@"\$");
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

		internal override string AsString () {
			return _content;
		}
	}
}
