using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace IdSetConv.Tests
{
  [TestFixture]
  public class GraphTests
  {
    public static readonly ((string v1, string v2, string v3)[] tris, string[][] expected)[]
      Possible =
      {
        (
          new[]
          {
            ("a", "b", "c"),
            ("d", "e", "f")
          },
          new[]
          {
            new[] {"a", "d"},
            new[] {"a", "e"},
            new[] {"a", "f"},
            new[] {"b", "d"},
            new[] {"b", "e"},
            new[] {"b", "f"},
            new[] {"c", "d"},
            new[] {"c", "e"},
            new[] {"c", "f"},
          }
        ),
        (
          new[]
          {
            ("a", "b", "c"),
            ("d", "e", "f"),
            ("g", "h", "i")
          },
          new[]
          {
            new[] {"a", "d", "g"},
            new[] {"a", "d", "h"},
            new[] {"a", "d", "i"},
            new[] {"a", "e", "g"},
            new[] {"a", "e", "h"},
            new[] {"a", "e", "i"},
            new[] {"a", "f", "g"},
            new[] {"a", "f", "h"},
            new[] {"a", "f", "i"},
            new[] {"b", "d", "g"},
            new[] {"b", "d", "h"},
            new[] {"b", "d", "i"},
            new[] {"b", "e", "g"},
            new[] {"b", "e", "h"},
            new[] {"b", "e", "i"},
            new[] {"b", "f", "g"},
            new[] {"b", "f", "h"},
            new[] {"b", "f", "i"},
            new[] {"c", "d", "g"},
            new[] {"c", "d", "h"},
            new[] {"c", "d", "i"},
            new[] {"c", "e", "g"},
            new[] {"c", "e", "h"},
            new[] {"c", "e", "i"},
            new[] {"c", "f", "g"},
            new[] {"c", "f", "h"},
            new[] {"c", "f", "i"}
          }
        )
      };

    [Test]
    public void PossibleTest([ValueSource(nameof(Possible))]((string v1, string v2, string v3)[] tris, string[][] expected) var)
    {
      var graph = new Graph();
      foreach (var (s1, s2, s3) in var.tris)
      {
        graph.Add((new Variable(true, s1), new Variable(true, s2), new Variable(true, s3)));
      }

      var expected = graph.Possible();
      var comparand = expected.Select(exp => exp.Select(ite => ite.Name).ToArray()).ToArray();

      var compareLogic = new CompareLogic();
      var results = compareLogic.Compare(var.expected, comparand);

      Assert.True(results.AreEqual, results.DifferencesString);
    }

    public static readonly ((string, string, string)[], (string, string)[], string[][])[]
      CheckCase =
      {
        (
          new[]
          {
            ("a", "b", "c"),
            ("d", "e", "f")
          },
          new[]
          {
            ("a", "d"),
            ("b", "e"),
            ("c", "f")
          },
          new[]
          {
            new[] {"a", "e"},
            new[] {"a", "f"},
            new[] {"b", "d"},
            new[] {"b", "f"},
            new[] {"c", "d"},
            new[] {"c", "e"},
          }
        ),
        (
          new[]
          {
            ("a", "b", "c"),
            ("d", "e", "f")
          },
          new[]
          {
            ("a", "d"),
            ("b", "e"),
            ("c", "f"),
            ("a", "e")
          },
          new[]
          {
            new[] {"a", "f"},
            new[] {"b", "d"},
            new[] {"b", "f"},
            new[] {"c", "d"},
            new[] {"c", "e"},
          }
        )
      };

    [Test]
    public void IdSetTest([ValueSource(nameof(CheckCase))]((string v1, string v2, string v3)[] tris, (string e1, string e2)[] edges, string[][] expected) var)
    {
      Dictionary<string, Variable> vars = new Dictionary<string, Variable>();
      var graph = new Graph();
      foreach (var (s1, s2, s3) in var.tris)
      {
        graph.Add((
          vars[s1] = new Variable(true, s1),
          vars[s2] = new Variable(true, s2),
          vars[s3] = new Variable(true, s3)));
      }

      foreach (var (e1, e2) in var.edges)
      {
        graph.AddEdge(vars[e1], vars[e2]);
      }

      var expected = graph.IdSet();
      var comparand = expected.Select(exp => exp.Select(ite => ite.Name).ToArray()).ToArray();

      var compareLogic = new CompareLogic
      {
        Config =
        {
          MaxDifferences = 20
        }
      };
      var results = compareLogic.Compare(var.expected, comparand);

      Assert.True(results.AreEqual, results.DifferencesString);
    }
  }
}
