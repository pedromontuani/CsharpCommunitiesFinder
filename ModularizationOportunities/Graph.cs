namespace ModularizationOportunities;

using System;
using System.Collections.Generic;
using System.Linq;

public class Graph
{
    public List<int> Nodes { get; private set; }
    public List<Tuple<int, int>> Edges { get; private set; }

    public Graph()
    {
        Nodes = new List<int>();
        Edges = new List<Tuple<int, int>>();
    }

    public void AddNode(int node)
    {
        if (!Nodes.Contains(node))
        {
            Nodes.Add(node);
        }
    }

    public void AddEdge(int node1, int node2)
    {
        if (!Edges.Contains(Tuple.Create(node1, node2)) && !Edges.Contains(Tuple.Create(node2, node1)))
        {
            Edges.Add(Tuple.Create(node1, node2));
        }
    }

    public List<int> GetNeighbors(int node)
    {
        return Edges.Where(e => e.Item1 == node).Select(e => e.Item2)
            .Concat(Edges.Where(e => e.Item2 == node).Select(e => e.Item1)).ToList();
    }
}