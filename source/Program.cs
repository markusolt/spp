using System;
using Spp.IO;
using Spp;

namespace Spp {
  public class Program {
    static int Main (string[] args) {
      if (args.Length == 0) {
        Console.WriteLine("Insufficient arguments.");
        return 1;
      }

      if (args.Length == 1) {
        Compiler c = new Compiler(args[0]);

        try {
          c.Compile();
        } catch (CompileException e) {
          Console.WriteLine(e.ToString());
          return 1;
        }

        return 0;
      }

      Console.WriteLine("Too many arguments.");
      return 1;
    }
  }
}