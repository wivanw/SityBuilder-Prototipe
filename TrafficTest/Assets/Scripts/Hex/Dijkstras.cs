using System;
using System.Collections.Generic;
/// <summary>
/// Path finding class
/// </summary>
public class Dijkstras
{
    public readonly Dictionary<Hex, IEnumerable<Hex>> _vertices = new Dictionary<Hex, IEnumerable<Hex>>();

    public void add_vertex(Hex name, IEnumerable<Hex> neighbors)
    {
        _vertices[name] = neighbors;
    }

    public IEnumerable<Hex> shortest_path(Hex start, Hex finish, Func<Hex, int> costStep = null)
    {
        costStep = costStep ?? (hex => 1);
        var previous = new Dictionary<Hex, Hex>();
        var distances = new Dictionary<Hex, int>();
        var nodes = new List<Hex>();
        var path = new List<Hex>();
        foreach (var vertex in _vertices)
        {
            if (vertex.Key == start)
                distances[vertex.Key] = 0;
            else
                distances[vertex.Key] = int.MaxValue;

            nodes.Add(vertex.Key);
        }
        while (nodes.Count != 0)
        {
            nodes.Sort((x, y) => distances[x] - distances[y]);
            var smallest = nodes[0];

            nodes.Remove(smallest);
            if (smallest == finish)
            {
                while (previous.ContainsKey(smallest))
                {
                    path.Add(smallest);
                    smallest = previous[smallest];
                }
                break;
            }

            if (distances[smallest] == int.MaxValue)
                break;
            foreach (var neighbor in _vertices[smallest])
            {
                var alt = distances[smallest] + costStep(neighbor);
                if (alt >= distances[neighbor])
                    continue;
                distances[neighbor] = alt;
                previous[neighbor] = smallest;
            }
        }
        path.Reverse();
        return path;
    }

    public void Clear()
    {
        _vertices.Clear();
    }
}