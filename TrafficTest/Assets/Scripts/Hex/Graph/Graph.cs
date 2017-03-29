using System;
using System.Collections.Generic;

public class Graph
{
    // private member variables
    public readonly Dictionary<Hex, Node> Nodes = new Dictionary<Hex, Node>();

    public Node this[Hex hex]
    {
        get { return Nodes[hex]; }
        set { Nodes[hex] = value; }
    }

    public virtual void AddNode(Node n)
    {
        // Make sure this node is unique
        if (!Nodes.ContainsKey(n.Value))
            Nodes.Add(n.Value, n);
        else
            throw new ArgumentException("There already exists a node in the graph with key " + n.Value);
    }


    public virtual void AddDirectedEdge(Hex uKey, Hex vKey)
    {
        AddDirectedEdge(uKey, vKey, 0);
    }

    public virtual void AddDirectedEdge(Hex uKey, Hex vKey, int cost)
    {
        if (!Nodes.ContainsKey(uKey))
            AddNode(new Node(uKey));
        if (!Nodes.ContainsKey(vKey))
            AddNode(new Node(vKey));
        AddDirectedEdge(Nodes[uKey], Nodes[vKey], cost);
    }

    public virtual void AddDirectedEdge(Node u, Node v)
    {
        AddDirectedEdge(u, v, 0);
    }

    public virtual void AddDirectedEdge(Node u, Node v, int cost)
    {
        if (!Nodes.ContainsKey(u.Value))
            AddNode(u);
        // get references to uKey and vKey
        if (!Nodes.ContainsKey(v.Value))
            AddNode(v);
        // add an edge from u -> v
        u.AddDirected(v, cost);
    }


    //public virtual void AddUndirectedEdge(Hex uKey, Hex vKey)
    //{
    //    AddUndirectedEdge(uKey, vKey, 0);
    //}

    public virtual void AddUndirectedEdge(Hex uKey, Hex vKey, int cost)
    {
        if (!Nodes.ContainsKey(uKey))
            AddNode(new Node(uKey));
        if (!Nodes.ContainsKey(vKey))
            AddNode(new Node(vKey));
        AddUndirectedEdge(Nodes[uKey], Nodes[vKey], cost);
    }

    //public virtual void AddUndirectedEdge(Node u, Node v)
    //{
    //    AddUndirectedEdge(u, v, 0);
    //}

    public virtual void AddUndirectedEdge(Node u, Node v, int cost)
    {
        // Make sure u and v are Nodes in this graph
        if (Nodes.ContainsKey(u.Value) && Nodes.ContainsKey(v.Value))
        {
            // Add an edge from u -> v and from v -> u
            u.AddDirected(v, cost);
            v.AddDirected(u, cost);
        }
        else
            // one or both of the nodes were not found in the graph
            throw new ArgumentException("One or both of the nodes supplied were not members of the graph.");
    }


    public virtual bool Contains(Node n)
    {
        return Contains(n.Value);
    }

    public virtual bool Contains(Hex key)
    {
        return Nodes.ContainsKey(key);
    }
}