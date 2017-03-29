using System.Collections.Generic;

public class Node
{
    #region Private Member Variables
    private readonly Hex _myValue;
    public readonly Dictionary<Node, int> Neighbors = new Dictionary<Node, int>();
    #endregion

    #region Constructors
    public Node(Hex value)
    {
        _myValue = value;
    }
    #endregion

    #region Public Properties

    public Hex Value
    {
        get { return _myValue; }
    }

    public int this[Node index]
    {
        get { return Neighbors[index]; }
        set { Neighbors[index] = value; }
    }
    #endregion

    protected internal virtual void AddDirected(Node n)
    {
        Neighbors.Add(n, 0);
    }

    protected internal virtual void AddDirected(Node n, int cost)
    {
        Neighbors.Add(n, cost);
    }
}
