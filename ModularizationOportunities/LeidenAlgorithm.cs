namespace ModularizationOportunities;

public class LeidenAlgorithm(Graph graph)
{
    private readonly Dictionary<int, int> _nodeToCommunity = new();
    private readonly Dictionary<int, List<int>> _communityToNodes = new();

    public NodeList FindCommunities()
    {
        InitializeCommunities();
        bool improvement = true;

        while (improvement)
        {
            improvement = false;

            foreach (var node in graph.Nodes)
            {
                int currentCommunity = _nodeToCommunity[node];
                var bestCommunity = currentCommunity;
                double bestGain = 0;

                foreach (var neighbor in graph.GetNeighbors(node))
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
        foreach (var node in graph.Nodes)
        {
            _nodeToCommunity[node] = node;
            _communityToNodes[node] = new List<int> { node };
        }
    }

    private double CalculateModularityGain(int node, int community)
    {
        double m = graph.Edges.Count;
        double k_i_in = graph.GetNeighbors(node).Count(neighbor => _nodeToCommunity[neighbor] == community);
        double k_i = graph.GetNeighbors(node).Count;
        double sum_tot = _communityToNodes[community].Sum(n => graph.GetNeighbors(n).Count);

        double deltaQ = (k_i_in / m) - (sum_tot * k_i) / (2 * m * m);
        return deltaQ;
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