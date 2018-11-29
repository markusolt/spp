using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Spp.IO;
using Spp;

namespace Spp {
  internal class Instruction {
    private Func<Compiler, Expression[], Value> _advancedFunction;
    private Func<Compiler, Value, Value, Value> _simpleFunction;
    private int _argumentCount;
    private bool _isAdvanced;

    internal static readonly Dictionary<Signature, Instruction> Instructions = new Dictionary<Signature, Instruction>() {
      {new Signature("basename",  1), new Instruction(_basename,  1)},
      {new Signature("cdinput",   1), new Instruction(_cdinput,   1)},
      {new Signature("cdoutput",  1), new Instruction(_cdoutput,  1)},
      {new Signature("close",     0), new Instruction(_close,     0)},
      {new Signature("error",     1), new Instruction(_error,     1)},
      {new Signature("files",     1), new Instruction(_files,     1)},
      {new Signature("for",       3), new Instruction(_for,       3)},
      {new Signature("get",       2), new Instruction(_get,       2)},
      {new Signature("if",        2), new Instruction(_if,        2)},
      {new Signature("input",     1), new Instruction(_input,     1)},
      {new Signature("let",       2), new Instruction(_let,       2)},
      {new Signature("loadtext",  1), new Instruction(_loadText,  1)},
      {new Signature("loadvalue", 1), new Instruction(_loadValue, 1)},
      {new Signature("not",       1), new Instruction(_not,       1)},
      {new Signature("output",    1), new Instruction(_output,    1)},
      {new Signature("push",      2), new Instruction(_push,      2)},
      {new Signature("using",     2), new Instruction(_using,     2)},
      {new Signature("warn",      1), new Instruction(_warn,      1)}
    };

    private Instruction (Func<Compiler, Value, Value, Value> simpleFunction, int argumentCount) {
      _simpleFunction = simpleFunction;
      _argumentCount = argumentCount;
      _isAdvanced = false;
    }

    private Instruction (Func<Compiler, Expression[], Value> advancedFunction, int argumentCount) {
      _advancedFunction = advancedFunction;
      _argumentCount = argumentCount;
      _isAdvanced = true;
    }

    internal Value Invoke (Compiler compiler, Expression[] nodes) {
      Value v1;
      Value v2;

      if (_isAdvanced) {
        return _advancedFunction(compiler, nodes);
      }

      if (nodes.Length != _argumentCount) {
        throw new ArgumentException("Wrong number of arguments!");
      }

      v1 = null;
      v2 = null;
      if (_argumentCount > 0) {
        v1 = nodes[0].Evaluate(compiler);
      }
      if (_argumentCount > 1) {
        v2 = nodes[1].Evaluate(compiler);
      }

      return _simpleFunction(compiler, v1, v2);
    }

    private static Value _basename (Compiler compiler, Value v1, Value v2) {
      return Value.New(Path.GetFileNameWithoutExtension(v1.AsString()));
    }

    private static Value _cdinput (Compiler compiler, Value v1, Value v2) {
      compiler.CdInput = _resolveDirectory(compiler.CdInput, v1.AsString(), v1.Position, false);
      return Value.Empty;
    }

    private static Value _cdoutput (Compiler compiler, Value v1, Value v2) {
      compiler.CdOutput = _resolveDirectory(compiler.CdOutput, v1.AsString(), v1.Position, false);
      return Value.Empty;
    }

    private static Value _close (Compiler compiler, Value v1, Value v2) {
      compiler.Writer = null;
      return Value.Empty;
    }

    private static Value _error (Compiler compiler, Value v1, Value v2) {
      throw new CompileException(v1.ToString(), v1.Position);
    }

    private static Value _files (Compiler compiler, Value v1, Value v2) {
      List<Value> files;

      files = new List<Value>();
      foreach (string s in Directory.GetFiles(compiler.CdInput, v1.AsString())) {
        files.Add(Value.New(s));
      }

      return Value.New(files);
    }

    private static Value _for (Compiler compiler, Expression[] nodes) {
      foreach (Value step in nodes[1].Evaluate(compiler).AsEnumerable()) {
        nodes[0].AsVariable().Set(compiler, step);
        nodes[2].Evaluate(compiler);
      }
      return Value.Empty;
    }

    private static Value _get (Compiler compiler, Value v1, Value v2) {
      return v1[v2];
    }

    private static Value _if (Compiler compiler, Expression[] nodes) {
      if (nodes[0].Evaluate(compiler).AsBool()) {
        return nodes[1].Evaluate(compiler);
      }
      return Value.Empty;
    }

    private static Value _input (Compiler compiler, Value v1, Value v2) {
      string filePath;
      string oldCdInput;

      filePath = _resolveFile(compiler.CdInput, v1.AsString(), v1.Position, false);
      oldCdInput = compiler.CdInput;

      compiler.CdInput = Path.GetDirectoryName(filePath);
      compiler.CompileInsert(filePath);

      compiler.CdInput = oldCdInput;
      return Value.Empty;
    }

    private static Value _loadValue (Compiler compiler, Value v1, Value v2) {
      string filePath;
      Reader reader;
      Value res;

      filePath = _resolveFile(compiler.CdInput, v1.AsString(), v1.Position, false);

      reader = new Reader(new StreamReader(filePath), filePath);

      Expression.SkipWhitespace(reader);
      reader.Assert('#');
      Expression.SkipWhitespace(reader);
      res = Expression.ExpressionParser.Parse(reader).Evaluate(compiler);

      reader.Dispose();
      return res;
    }

    private static Value _loadText (Compiler compiler, Value v1, Value v2) {
      StreamReader reader;
      string contents;

      reader = new StreamReader(_resolveFile(compiler.CdInput, v1.AsString(), v1.Position, false));
      contents = reader.ReadToEnd().Trim();
      reader.Dispose();

      return Value.New(contents);
    }

    private static Value _not (Compiler compiler, Value v1, Value v2) {
      return Value.New(!(v1.AsBool()));
    }

    private static Value _output (Compiler compiler, Value v1, Value v2) {
      compiler.Writer = new StreamWriter(_resolveFile(compiler.CdOutput, v1.AsString(), v1.Position, true), false, new UTF8Encoding(true));
      return Value.Empty;
    }

    private static Value _let (Compiler compiler, Expression[] nodes) {
      nodes[0].AsVariable().Set(compiler, nodes[1].Evaluate(compiler));
      return Value.Empty;
    }

    private static Value _push (Compiler compiler, Value v1, Value v2) {
      v1.Push(v2);
      return Value.Empty;
    }

    private static Value _using (Compiler compiler, Expression[] nodes) {
      Value oldUsing;
      Value result;

      oldUsing = compiler.Using;
      compiler.Using = nodes[0].Evaluate(compiler);

      result = nodes[1].Evaluate(compiler);

      compiler.Using = oldUsing;
      return result;
    }

    private static Value _warn (Compiler compiler, Value v1, Value v2) {
      Console.WriteLine(v1.Position.ToString() + ": Warning: " + v1.ToString());
      return Value.Empty;
    }

    private static string _resolveDirectory (string basePath, string path, Position position, bool canCreate) {
      try {
        path = Path.Combine(basePath, path);
      } catch (ArgumentException e) {
        throw new CompileException("Illegal characters in path.", position, e);
      }

      if (!canCreate && !Directory.Exists(path)) {
        throw new CompileException("Directory does not exist.", position);
      }

      return path;
    }

    private static string _resolveFile (string basePath, string path, Position position, bool canCreate) {
      try {
        path = Path.Combine(basePath, path);
      } catch (ArgumentException e) {
        throw new CompileException("Illegal characters in path.", position, e);
      }

      if (!canCreate && !File.Exists(path)) {
        throw new CompileException("File does not exist.", position);
      }

      if (canCreate && !Directory.Exists(Path.GetDirectoryName(path))) {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
      }

      return path;
    }
  }
}
