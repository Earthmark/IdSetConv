using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace IdSetConv
{
  public class Graph
  {
    private readonly Dictionary<(string name, bool positive), List<Variable>> _tokenLookups =
      new Dictionary<(string name, bool positive), List<Variable>>();

    private readonly Dictionary<Variable, HashSet<Variable>> _edges = new Dictionary<Variable, HashSet<Variable>>();

    private readonly List<(Variable v1, Variable v2, Variable v3)> _triangles =
      new List<(Variable v1, Variable v2, Variable v3)>();

    public void Add((Variable v1, Variable v2, Variable v3) var)
    {
      _triangles.Add(var);
      _edges.Add(var.v1, new HashSet<Variable>());
      _edges.Add(var.v2, new HashSet<Variable>());
      _edges.Add(var.v3, new HashSet<Variable>());
      AddEdge(var.v1, var.v2);
      AddEdge(var.v2, var.v3);
      AddEdge(var.v1, var.v3);
      RegisterLookup(var.v1);
      RegisterLookup(var.v2);
      RegisterLookup(var.v3);
    }

    public void AddEdge(Variable v1, Variable v2)
    {
      _edges[v1].Add(v2);
      _edges[v2].Add(v1);
    }

    private void RegisterLookup(Variable v)
    {
      if (!_tokenLookups.TryGetValue((v.Name, v.Positive), out var lst))
      {
        _tokenLookups.Add((v.Name, v.Positive), lst = new List<Variable>());
      }

      lst.Add(v);
      if (_tokenLookups.TryGetValue((v.Name, !v.Positive), out var targets))
      {
        foreach (var target in targets)
        {
          AddEdge(v, target);
        }
      }
    }

    public IEnumerable<IEnumerable<Variable>> IdSet()
    {
      return Possible().Where(IsDisconnected);
    }

    public bool IsDisconnected(IEnumerable<Variable> selected)
    {
      var immutSelected = selected.ToImmutableList();
      return immutSelected.All(sel => !_edges[sel].Overlaps(immutSelected));
    }

    public IEnumerable<IEnumerable<Variable>> Possible()
    {
      IEnumerable<Variable> EnumerateSingle((Variable v1, Variable v2, Variable v3) val)
      {
        yield return val.v1;
        yield return val.v2;
        yield return val.v3;
      }

      IEnumerable<ImmutableList<Variable>> Initial()
      {
        yield return ImmutableList<Variable>.Empty;
      }

      return _triangles.Select(EnumerateSingle).Aggregate(Initial(),
        (current, next) => from cur in current from variable in next select cur.Add(variable));
    }
  }
}
