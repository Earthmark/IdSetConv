using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IdSetConv
{
  class Program
  {
    static void Main(string[] args)
    {
      var content = File.ReadAllText(args[0]);
      var vars = Set3Loader.Parse(content);
      var graph = new Graph();
      foreach (var ite in vars)
      {
        graph.Add(ite);
      }

      HashSet<string> printed = new HashSet<string>();
      foreach (var accepted in graph.IdSet())
      {
        var str = string.Join(",", accepted.OrderBy(ite => ite.Name).Select(var => var.Positive ? var.Name : $"!{var.Name}").Distinct());
        if (printed.Add(str))
        {
          Console.WriteLine(str);
        }
      }
    }
  }
}
