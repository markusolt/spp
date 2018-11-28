using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Spp.IO;
using Spp;

namespace Spp {
	internal class Text : Value {
		private string _payload;

		internal static readonly Parser<ValueRecipe> Parser = new ParseToken<ValueRecipe>("string", "\"", _parse);

		internal Text (Position position, string payload) : base(position) {
			_payload = payload;
		}

		internal override bool IsString { get { return true; } }

		internal override string AsString () { return _payload; }

		internal override TextWriter Stringify (TextWriter buffer, bool root) {
			if (root) {
				buffer.Write(_payload);
				return buffer;
			}

			buffer.Write('"');
			foreach (char c in _payload) {
				switch (c) {
					case '"': {
						buffer.Write("\\\"");
						break;
					}
					case '$': {
						buffer.Write(@"\$");
						break;
					}
					case '\\': {
						buffer.Write(@"\\");
						break;
					}
					case '\r': {
						buffer.Write(@"\r");
						break;
					}
					case '\n': {
						buffer.Write(@"\n");
						break;
					}
					case '\t': {
						buffer.Write(@"\t");
						break;
					}
					case '\f': {
						buffer.Write(@"\f");
						break;
					}
					default: {
						buffer.Write(c);
						break;
					}
				}
			}
			buffer.Write('"');
			return buffer;
		}

		private static ValueRecipe _parse (Reader reader) {
			StringBuilder buffer;
			char c;
			List<ValueRecipe> list;
			Position rootPos;
			Position pos;

			rootPos = reader.Position;
			reader.Read();

			buffer = new StringBuilder();
			c = ' ';
			list = new List<ValueRecipe>();

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
						reader.Skip(" \t");
						list.Add(ValueRecipe.ValueRecipeParser.Parse(reader));
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

			if (list.Count == 1) {
				return list[0];
			}
			return new Concat(rootPos, list);
		}
	}
}
