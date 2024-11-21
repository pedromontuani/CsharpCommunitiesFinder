namespace ModularizationOportunities;

public class LeidenAlgorithm
{
    private Graph _graph;
    private Dictionary<int, int> _nodeToCommunity;
    private Dictionary<int, List<int>> _communityToNodes;

    public LeidenAlgorithm(Graph graph)
    {
        _graph = graph;
        _nodeToCommunity = new Dictionary<int, int>();
        _communityToNodes = new Dictionary<int, List<int>>();
    }

    public List<List<int>> FindCommunities()
    {
        InitializeCommunities();
        bool improvement = true;

        while (improvement)
        {
            improvement = false;

            foreach (var node in _graph.Nodes)
            {
                int currentCommunity = _nodeToCommunity[node];
                var bestCommunity = currentCommunity;
                double bestGain = 0;

                foreach (var neighbor in _graph.GetNeighbors(node))
                {
                    int neighborCommunity = _nodeToCommunity[neighbor];
                    if (neighborCommunity != currentCommunity)
                    {
                        double gain = CalculateModularityGain(node, neighborCommunity);
                        if (gain > bestGain)
                        {
                            bestGain = gain;
                            bestCommunity = neighborCommunity;
                        }
                    }
                }

                if (bestCommunity != currentCommunity)
                {
                    MoveNodeToCommunity(node, bestCommunity);
                    improvement = true;
                }
            }
        }

        return _communityToNodes.Values.ToList();
    }

    private void InitializeCommunities()
    {
        foreach (var node in _graph.Nodes)
        {
            _nodeToCommunity[node] = node;
            _communityToNodes[node] = new List<int> { node };
        }
    }

    private double CalculateModularityGain(int node, int community)
    {
        // Implement modularity gain calculation
        // This is a placeholder implementation
        return new Random().NextDouble();
    }

    private void MoveNodeToCommunity(int node, int newCommunity)
    {
        int oldCommunity = _nodeToCommunity[node];
        _communityToNodes[oldCommunity].Remove(node);

        if (_communityToNodes[oldCommunity].Count == 0)
        {
            _communityToNodes.Remove(oldCommunity);
        }

        _nodeToCommunity[node] = newCommunity;
        if (!_communityToNodes.ContainsKey(newCommunity))
        {
            _communityToNodes[newCommunity] = new List<int>();
        }
        _communityToNodes[newCommunity].Add(node);
    }
}